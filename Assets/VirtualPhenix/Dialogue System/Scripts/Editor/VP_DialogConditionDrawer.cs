﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VirtualPhenix;
using Object = UnityEngine.Object;

namespace VirtualPhenix.Dialog
{
    [CustomPropertyDrawer(typeof(TargetConstraintAttribute))]
    [CustomPropertyDrawer(typeof(VP_DialogCondition), true)]
    public class VP_DialogConditionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Indent label
            label.text = " " + label.text;

            //GUI.Box(position, "", (GUIStyle)"flow overlay box");

            position.y += 4;
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            // Draw label
            Rect pos = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            Rect targetRect = new Rect(pos.x, pos.y, pos.width, EditorGUIUtility.singleLineHeight);

            // Get target
            SerializedProperty targetProp = property.FindPropertyRelative("_target");
            object target = targetProp.objectReferenceValue;

            Rect buttonRect = position;
            buttonRect.width = 80;
            buttonRect.height = 15;
            Rect labelRect = buttonRect;
            labelRect.x += 66;
            
            if (targetProp.objectReferenceValue is GameObject)
            {
                Debug.Log("Target property is game object");
            }
            GUILayout.Space(20);

            if (attribute != null && attribute is TargetConstraintAttribute)
            {
                Type targetType = (attribute as TargetConstraintAttribute).targetType;
                EditorGUI.ObjectField(targetRect, targetProp, targetType, GUIContent.none);

            }
            else
            {
                EditorGUI.PropertyField(targetRect, targetProp, GUIContent.none);
            }

            if (targetRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.DragPerform)
                {
                    Debug.Log("Drag Perform in serializeCallback! " + this);
                    Debug.Log(DragAndDrop.objectReferences.Length);

                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {
                        GameObject go = DragAndDrop.objectReferences[i] as GameObject;
                        property.objectReferenceValue = go;
                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.Update();
                    }
                    Event.current.Use();
                }
            }


            if (target != null)
            {
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                // Get method name
                SerializedProperty methodProp = property.FindPropertyRelative("_methodName");
                string methodName = methodProp.stringValue;

                // Get args
                SerializedProperty argProps = property.FindPropertyRelative("_args");
                Type[] argTypes = GetArgTypes(argProps);

                // Get dynamic
                SerializedProperty dynamicProp = property.FindPropertyRelative("_dynamic");
                bool dynamic = dynamicProp.boolValue;

                // Get active method
                MethodInfo activeMethod = GetMethod(target, methodName, argTypes);


                EditorGUI.LabelField(labelRect, GetVarName(property));

                GUIContent methodlabel = new GUIContent("n/a");
                if (activeMethod != null) methodlabel = new GUIContent(PrettifyMethod(activeMethod));
                else if (!string.IsNullOrEmpty(methodName)) methodlabel = new GUIContent("Missing (" + PrettifyMethod(methodName, argTypes) + ")");

                Rect methodRect = new Rect(position.x, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

                // Method select button
                pos = EditorGUI.PrefixLabel(methodRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(dynamic ? "Method (dynamic)" : "Method"));

                if (EditorGUI.DropdownButton(pos, methodlabel, FocusType.Keyboard))
                {
                    MethodSelector(property);
                }

                if (activeMethod != null && !dynamic)
                {
                    // Args
                    ParameterInfo[] activeParameters = activeMethod.GetParameters();
                    Rect argRect = new Rect(position.x, methodRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
                    string[] types = new string[argProps.arraySize];
                    for (int i = 0; i < types.Length; i++)
                    {
                        SerializedProperty argProp = argProps.FindPropertyRelative("Array.data[" + i + "]");
                        GUIContent argLabel = new GUIContent(ObjectNames.NicifyVariableName(activeParameters[i].Name));

                        EditorGUI.BeginChangeCheck();
                        switch ((Arg.ArgType)argProp.FindPropertyRelative("argType").enumValueIndex)
                        {
                            case Arg.ArgType.Bool:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("boolValue"), argLabel);
                                break;
                            case Arg.ArgType.Int:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("intValue"), argLabel);
                                break;
                            case Arg.ArgType.Float:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("floatValue"), argLabel);
                                break;
                            case Arg.ArgType.String:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("stringValue"), argLabel);
                                break;
                            case Arg.ArgType.Object:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("objectValue"), argLabel);
                                break;
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.FindPropertyRelative("dirty").boolValue = true;
                        }
                        argRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                EditorGUI.indentLevel = indent;
            }
            else
            {

                Rect helpBoxRect = new Rect(position.x + 8, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width - 16, EditorGUIUtility.singleLineHeight);
                string msg = "Call not set. Execution will be slower.";
                EditorGUI.LabelField(helpBoxRect, new GUIContent(msg, msg), "helpBox");
            }

            // Set indent back to what it was
            EditorGUI.EndProperty();
        }

        private class MenuItem
        {
            public GenericMenu.MenuFunction action;
            public string path;
            public GUIContent label;

            public MenuItem(string path, string name, GenericMenu.MenuFunction action)
            {
                this.action = action;
                this.label = new GUIContent(path + '/' + name);
                this.path = path;
            }
        }
        void MethodSelector(SerializedProperty property)
        {
            // Return type constraint
            Type returnType = null;
            // Arg type constraint
            Type[] argTypes = new Type[0];

            // Get return type and argument constraints
            VP_DialogCondition dummy = GetDummyFunction(property);
            Type[] genericTypes = dummy.GetType().BaseType.GetGenericArguments();

            // SerializableEventBase is always void return type
            if (genericTypes != null && genericTypes.Length > 0)
            {
                // The last generic argument is the return type
                returnType = genericTypes[genericTypes.Length - 1];
                if (genericTypes.Length > 1)
                {
                    argTypes = new Type[genericTypes.Length - 1];
                    Array.Copy(genericTypes, argTypes, genericTypes.Length - 1);
                }
            }

            SerializedProperty targetProp = property.FindPropertyRelative("_target");

            List<MenuItem> dynamicItems = new List<MenuItem>();
            List<MenuItem> staticItems = new List<MenuItem>();

            List<Object> targets = new List<Object>() { targetProp.objectReferenceValue };
            if (targets[0] is Component)
            {
                targets = (targets[0] as Component).gameObject.GetComponents<Component>().ToList<Object>();
                targets.Add((targetProp.objectReferenceValue as Component).gameObject);

            }
            else if (targets[0] is GameObject)
            {
                targets = (targets[0] as GameObject).GetComponents<Component>().ToList<Object>();
                targets.Add(targetProp.objectReferenceValue as GameObject);
            }
            for (int c = 0; c < targets.Count; c++)
            {
                Object t = targets[c];

                MethodInfo[] methods = targets[c].GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];

                    // Skip methods with wrong return type
                    if (returnType != null && method.ReturnType != returnType) continue;
                    // Skip methods with null return type
                    // if (method.ReturnType == typeof(void)) continue;
                    // Skip generic methods
                    if (method.IsGenericMethod) continue;

                    Type[] parms = method.GetParameters().Select(x => x.ParameterType).ToArray();

                    // Skip methods with more than 4 args
                    if (parms.Length > 4) continue;
                    // Skip methods with unsupported args
                    if (parms.Any(x => !Arg.IsSupported(x))) continue;

                    string methodPrettyName = PrettifyMethod(methods[i]);
                    staticItems.Add(new MenuItem(targets[c].GetType().Name + "/" + methods[i].DeclaringType.Name, methodPrettyName, () => SetMethod(property, t, method, false)));

                    // Skip methods with wrong constrained args
                    if (argTypes.Length == 0 || !Enumerable.SequenceEqual(argTypes, parms)) continue;

                    dynamicItems.Add(new MenuItem(targets[c].GetType().Name + "/" + methods[i].DeclaringType.Name, methods[i].Name, () => SetMethod(property, t, method, true)));
                }
            }

            // Construct and display context menu
            GenericMenu menu = new GenericMenu();
            if (dynamicItems.Count > 0)
            {
                string[] paths = dynamicItems.GroupBy(x => x.path).Select(x => x.First().path).ToArray();
                foreach (string path in paths)
                {
                    menu.AddItem(new GUIContent(path + "/Dynamic " + PrettifyTypes(argTypes)), false, null);
                }
                for (int i = 0; i < dynamicItems.Count; i++)
                {
                    menu.AddItem(dynamicItems[i].label, false, dynamicItems[i].action);
                }
                foreach (string path in paths)
                {
                    menu.AddItem(new GUIContent(path + "/  "), false, null);
                    menu.AddItem(new GUIContent(path + "/Static parameters"), false, null);
                }
            }
            for (int i = 0; i < staticItems.Count; i++)
            {
                menu.AddItem(staticItems[i].label, false, staticItems[i].action);
            }
            if (menu.GetItemCount() == 0) menu.AddDisabledItem(new GUIContent("No methods with return type '" + GetTypeName(returnType) + "'"));
            menu.ShowAsContext();
        }

        string PrettifyMethod(string methodName, Type[] parmTypes)
        {
            string parmnames = PrettifyTypes(parmTypes);
            return methodName + "(" + parmnames + ")";
        }

        string PrettifyMethod(MethodInfo methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");
            ParameterInfo[] parms = methodInfo.GetParameters();
            string parmnames = PrettifyTypes(parms.Select(x => x.ParameterType).ToArray());
            return GetTypeName(methodInfo.ReturnParameter.ParameterType) + " " + methodInfo.Name + "(" + parmnames + ")";
        }

        string PrettifyTypes(Type[] types)
        {
            if (types == null) throw new ArgumentNullException("types");
            return string.Join(", ", types.Select(x => GetTypeName(x)).ToArray());
        }

        MethodInfo GetMethod(object target, string methodName, Type[] types)
        {
            MethodInfo activeMethod = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, null, CallingConventions.Any, types, null);
            return activeMethod;
        }

        private Type[] GetArgTypes(SerializedProperty argProp)
        {
            Type[] types = new Type[argProp.arraySize];
            for (int i = 0; i < argProp.arraySize; i++)
            {
                types[i] = Arg.RealType((Arg.ArgType)argProp.FindPropertyRelative("Array.data[" + i + "].argType").enumValueIndex);
            }
            return types;
        }

        private void SetNewTarget(SerializedProperty property, UnityEngine.Object target)
        {
            SerializedProperty targetProp = property.FindPropertyRelative("_target");
            targetProp.objectReferenceValue = target;
            property.FindPropertyRelative("dirty").boolValue = true;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        private string GetVarName(SerializedProperty property)
        {
            string varName = "";
            SerializedProperty targetProp = property.FindPropertyRelative("_target");
            List<Object> targets = new List<Object>() { targetProp.objectReferenceValue };
            if (targets[0] is Component)
            {
                targets = (targets[0] as Component).gameObject.GetComponents<Component>().ToList<Object>();
                targets.Add((targetProp.objectReferenceValue as Component).gameObject);

            }
            else if (targets[0] is GameObject)
            {
                targets = (targets[0] as GameObject).GetComponents<Component>().ToList<Object>();
                targets.Add(targetProp.objectReferenceValue as GameObject);
            }
            for (int c = 0; c < targets.Count; c++)
            {
                Object t = targets[c];


                if (targets[c] != null && (targets[c] is VP_DialogBoolConditionData))
                {
                    varName = (targets[c] as VP_DialogBoolConditionData).varName;
                }
                else if (targets[c] != null && (targets[c] is VP_DialogIntConditionData))
                {
                    varName = (targets[c] as VP_DialogIntConditionData).varName;
                }
                else if (targets[c] != null && (targets[c] is VP_DialogFloatConditionData))
                {
                    varName = (targets[c] as VP_DialogFloatConditionData).varName;
                }
                else if (targets[c] != null && (targets[c] is VP_DialogStringConditionData))
                {
                    varName = (targets[0] as VP_DialogStringConditionData).varName;
                }
                else if (targets[c] != null && (targets[c] is VP_DialogGameObjectConditionData))
                {
                    varName = (targets[c] as VP_DialogGameObjectConditionData).varName;
                }
                else
                {
                    varName = "Unknown";
                }

            }

            return varName;
        }

        private void SetMethod(SerializedProperty property, UnityEngine.Object target, MethodInfo methodInfo, bool dynamic)
        {
            SerializedProperty targetProp = property.FindPropertyRelative("_target");
            targetProp.objectReferenceValue = target;
            SerializedProperty methodProp = property.FindPropertyRelative("_methodName");
            methodProp.stringValue = methodInfo.Name;
            SerializedProperty dynamicProp = property.FindPropertyRelative("_dynamic");
            dynamicProp.boolValue = dynamic;
            SerializedProperty argProp = property.FindPropertyRelative("_args");
            ParameterInfo[] parameters = methodInfo.GetParameters();
            argProp.arraySize = parameters.Length;
            for (int i = 0; i < parameters.Length; i++)
            {
                argProp.FindPropertyRelative("Array.data[" + i + "].argType").enumValueIndex = (int)Arg.FromRealType(parameters[i].ParameterType);
            }
            property.FindPropertyRelative("dirty").boolValue = true;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        private static string GetTypeName(Type t)
        {
            if (t == typeof(int)) return "int";
            else if (t == typeof(float)) return "float";
            else if (t == typeof(string)) return "string";
            else if (t == typeof(bool)) return "bool";
            else if (t == typeof(void)) return "void";
            else return t.Name;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineheight = EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
            SerializedProperty targetProp = property.FindPropertyRelative("_target");
            SerializedProperty argProps = property.FindPropertyRelative("_args");
            SerializedProperty dynamicProp = property.FindPropertyRelative("_dynamic");
            float height = lineheight + lineheight;
            if (targetProp.objectReferenceValue != null && !dynamicProp.boolValue) height += argProps.arraySize * lineheight;
            height += 8;
            return height;
        }

        private static VP_DialogCondition GetDummyFunction(SerializedProperty prop)
        {
            string stringValue = prop.FindPropertyRelative("_typeName").stringValue;
            Type type = Type.GetType(stringValue, false);
            VP_DialogCondition result;
            if (type == null)
            {
                return null;
            }
            else
            {
                result = (Activator.CreateInstance(type) as VP_DialogCondition);
            }
            return result;
        }

        private void SelectGOInfo(SerializedProperty property, UnityEngine.Object reference)
        {
            VP_DialogCondition newCondition = new VP_DialogCondition();
            newCondition.target = reference;
            
           
            property.FindPropertyRelative("dirty").boolValue = true;
            property.FindPropertyRelative("_target").objectReferenceValue = reference;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();

            GameObject currentObj = property.FindPropertyRelative("_target").objectReferenceValue as GameObject;

            Debug.Log("Current Object: " + currentObj);    
        }

        private void SelectVarInfo(SerializedProperty property, string reference)
        {
          
            property.FindPropertyRelative("dirty").boolValue = true;
            property.FindPropertyRelative("m_varName").stringValue = reference;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();

            
        }


    }
}

/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VirtualPhenix;
using Object = UnityEngine.Object;

namespace VirtualPhenix
{
    [CustomPropertyDrawer(typeof(TargetConstraintAttribute))]
    [CustomPropertyDrawer(typeof(VP_DialogCondition), true)]
    public class VP_ConditionDrawer : PropertyDrawer
    {



        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Indent label
            label.text = " " + label.text;

            //GUI.Box(position, "", (GUIStyle)"flow overlay box");

            position.y += 4;
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            // Draw label
            Rect pos = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            Rect targetRect = new Rect(pos.x, pos.y, pos.width, EditorGUIUtility.singleLineHeight);

            // Get target
            SerializedProperty targetProp = property.FindPropertyRelative("_target");
            object target = targetProp.objectReferenceValue;

            SerializedProperty objPropSer = property.FindPropertyRelative("m_object");
            UnityEngine.Object objProp = objPropSer.objectReferenceValue;
            EditorGUI.ObjectField(targetRect, objPropSer, typeof(GameObject));


            if (attribute != null && attribute is TargetConstraintAttribute)
            {
                Type targetType = (attribute as TargetConstraintAttribute).targetType;
                EditorGUI.ObjectField(targetRect, targetProp, targetType, GUIContent.none);

            }
            else
            {
                EditorGUI.PropertyField(targetRect, targetProp, GUIContent.none);
            }

            string buttonLabel = "Select";

            if (target != null)
                buttonLabel = (target as Object).name;

            Rect buttonRect = position;
            buttonRect.width = 80;


            if (GUI.Button(buttonRect, buttonLabel))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("None"), target == null, () => SelectGOInfo(property, null));

                foreach (UnityEngine.Object obj in Object.FindObjectsOfType(typeof(GameObject)))
                {
                    GUIContent content = new GUIContent(obj.name);
                    menu.AddItem(content, obj == (target as Object), () => SelectGOInfo(property, obj));
                }

                menu.ShowAsContext();
            }


            if (targetRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.DragPerform)
                {
                    Debug.Log("Drag Perform in serializeCallback! " + this);
                    Debug.Log(DragAndDrop.objectReferences.Length);

                    for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                    {

                        GameObject go = DragAndDrop.objectReferences[i] as GameObject;
                        VP_Debug.Log("Target: " + target);
                        SetNewTarget(property, go.GetComponent<MonoBehaviour>());
                        VP_Debug.Log("Target2: " + target);
                        VP_Debug.Log("go: " + go);
                        //myTarget.m_GameObjectGroups[groupIndex].Add(DragAndDrop.objectReferences[i] as GameObject);
                    }
                    Event.current.Use();

                    //serializedObject.ApplyModifiedProperties();
                }
            }


            if (target != null)
            {
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;

                // Get method name
                SerializedProperty methodProp = property.FindPropertyRelative("_methodName");
                string methodName = methodProp.stringValue;

                // Get args
                SerializedProperty argProps = property.FindPropertyRelative("_args");
                Type[] argTypes = GetArgTypes(argProps);

                // Get dynamic
                SerializedProperty dynamicProp = property.FindPropertyRelative("_dynamic");
                bool dynamic = dynamicProp.boolValue;

                // Get active method
                MethodInfo activeMethod = GetMethod(target, methodName, argTypes);

                GUIContent methodlabel = new GUIContent("n/a");
                if (activeMethod != null) methodlabel = new GUIContent(PrettifyMethod(activeMethod));
                else if (!string.IsNullOrEmpty(methodName)) methodlabel = new GUIContent("Missing (" + PrettifyMethod(methodName, argTypes) + ")");

                Rect methodRect = new Rect(position.x, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

                // Method select button
                pos = EditorGUI.PrefixLabel(methodRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(dynamic ? "Method (dynamic)" : "Method"));
                if (EditorGUI.DropdownButton(pos, methodlabel, FocusType.Keyboard))
                {
                    MethodSelector(property);
                }

                if (activeMethod != null && !dynamic)
                {
                    // Args
                    ParameterInfo[] activeParameters = activeMethod.GetParameters();
                    Rect argRect = new Rect(position.x, methodRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
                    string[] types = new string[argProps.arraySize];
                    for (int i = 0; i < types.Length; i++)
                    {
                        SerializedProperty argProp = argProps.FindPropertyRelative("Array.data[" + i + "]");
                        GUIContent argLabel = new GUIContent(ObjectNames.NicifyVariableName(activeParameters[i].Name));

                        EditorGUI.BeginChangeCheck();
                        switch ((Arg.ArgType)argProp.FindPropertyRelative("argType").enumValueIndex)
                        {
                            case Arg.ArgType.Bool:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("boolValue"), argLabel);
                                break;
                            case Arg.ArgType.Int:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("intValue"), argLabel);
                                break;
                            case Arg.ArgType.Float:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("floatValue"), argLabel);
                                break;
                            case Arg.ArgType.String:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("stringValue"), argLabel);
                                break;
                            case Arg.ArgType.Object:
                                EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("objectValue"), argLabel);
                                break;
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            property.FindPropertyRelative("dirty").boolValue = true;
                        }
                        argRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                EditorGUI.indentLevel = indent;
            }
            else
            {

                Rect helpBoxRect = new Rect(position.x + 8, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width - 16, EditorGUIUtility.singleLineHeight);
                string msg = "Call not set. Execution will be slower.";
                EditorGUI.LabelField(helpBoxRect, new GUIContent(msg, msg), "helpBox");
            }

            // Set indent back to what it was
            EditorGUI.EndProperty();
        }

        private class MenuItem
        {
            public GenericMenu.MenuFunction action;
            public string path;
            public GUIContent label;

            public MenuItem(string path, string name, GenericMenu.MenuFunction action)
            {
                this.action = action;
                this.label = new GUIContent(path + '/' + name);
                this.path = path;
            }
        }
        void MethodSelector(SerializedProperty property)
        {
            // Return type constraint
            Type returnType = null;
            // Arg type constraint
            Type[] argTypes = new Type[0];

            // Get return type and argument constraints
            VP_DialogCondition dummy = GetDummyFunction(property);
            Type[] genericTypes = dummy.GetType().BaseType.GetGenericArguments();
            // SerializableEventBase is always void return type
            if (dummy is SerializableEventBase)
            {
                returnType = typeof(void);
                if (genericTypes.Length > 1)
                {
                    argTypes = new Type[genericTypes.Length];
                    Array.Copy(genericTypes, argTypes, genericTypes.Length);
                }
            }
            else
            {
                if (genericTypes != null && genericTypes.Length > 0)
                {
                    // The last generic argument is the return type
                    returnType = genericTypes[genericTypes.Length - 1];
                    if (genericTypes.Length > 1)
                    {
                        argTypes = new Type[genericTypes.Length - 1];
                        Array.Copy(genericTypes, argTypes, genericTypes.Length - 1);
                    }
                }
            }

            SerializedProperty targetProp = property.FindPropertyRelative("_target");

            List<MenuItem> dynamicItems = new List<MenuItem>();
            List<MenuItem> staticItems = new List<MenuItem>();

            List<Object> targets = new List<Object>() { targetProp.objectReferenceValue };
            if (targets[0] is Component)
            {
                targets = (targets[0] as Component).gameObject.GetComponents<Component>().ToList<Object>();
                targets.Add((targetProp.objectReferenceValue as Component).gameObject);
            }
            else if (targets[0] is GameObject)
            {
                targets = (targets[0] as GameObject).GetComponents<Component>().ToList<Object>();
                targets.Add(targetProp.objectReferenceValue as GameObject);
            }
            for (int c = 0; c < targets.Count; c++)
            {
                Object t = targets[c];
                MethodInfo[] methods = targets[c].GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];

                    // Skip methods with wrong return type
                    if (returnType != null && method.ReturnType != returnType) continue;
                    // Skip methods with null return type
                    // if (method.ReturnType == typeof(void)) continue;
                    // Skip generic methods
                    if (method.IsGenericMethod) continue;

                    Type[] parms = method.GetParameters().Select(x => x.ParameterType).ToArray();

                    // Skip methods with more than 4 args
                    if (parms.Length > 4) continue;
                    // Skip methods with unsupported args
                    if (parms.Any(x => !Arg.IsSupported(x))) continue;

                    string methodPrettyName = PrettifyMethod(methods[i]);
                    staticItems.Add(new MenuItem(targets[c].GetType().Name + "/" + methods[i].DeclaringType.Name, methodPrettyName, () => SetMethod(property, t, method, false)));

                    // Skip methods with wrong constrained args
                    if (argTypes.Length == 0 || !Enumerable.SequenceEqual(argTypes, parms)) continue;

                    dynamicItems.Add(new MenuItem(targets[c].GetType().Name + "/" + methods[i].DeclaringType.Name, methods[i].Name, () => SetMethod(property, t, method, true)));
                }
            }

            // Construct and display context menu
            GenericMenu menu = new GenericMenu();
            if (dynamicItems.Count > 0)
            {
                string[] paths = dynamicItems.GroupBy(x => x.path).Select(x => x.First().path).ToArray();
                foreach (string path in paths)
                {
                    menu.AddItem(new GUIContent(path + "/Dynamic " + PrettifyTypes(argTypes)), false, null);
                }
                for (int i = 0; i < dynamicItems.Count; i++)
                {
                    menu.AddItem(dynamicItems[i].label, false, dynamicItems[i].action);
                }
                foreach (string path in paths)
                {
                    menu.AddItem(new GUIContent(path + "/  "), false, null);
                    menu.AddItem(new GUIContent(path + "/Static parameters"), false, null);
                }
            }
            for (int i = 0; i < staticItems.Count; i++)
            {
                menu.AddItem(staticItems[i].label, false, staticItems[i].action);
            }
            if (menu.GetItemCount() == 0) menu.AddDisabledItem(new GUIContent("No methods with return type '" + GetTypeName(returnType) + "'"));
            menu.ShowAsContext();
        }

        string PrettifyMethod(string methodName, Type[] parmTypes)
        {
            string parmnames = PrettifyTypes(parmTypes);
            return methodName + "(" + parmnames + ")";
        }

        string PrettifyMethod(MethodInfo methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");
            ParameterInfo[] parms = methodInfo.GetParameters();
            string parmnames = PrettifyTypes(parms.Select(x => x.ParameterType).ToArray());
            return GetTypeName(methodInfo.ReturnParameter.ParameterType) + " " + methodInfo.Name + "(" + parmnames + ")";
        }

        string PrettifyTypes(Type[] types)
        {
            if (types == null) throw new ArgumentNullException("types");
            return string.Join(", ", types.Select(x => GetTypeName(x)).ToArray());
        }

        MethodInfo GetMethod(object target, string methodName, Type[] types)
        {
            MethodInfo activeMethod = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, null, CallingConventions.Any, types, null);
            return activeMethod;
        }

        private Type[] GetArgTypes(SerializedProperty argProp)
        {
            Type[] types = new Type[argProp.arraySize];
            for (int i = 0; i < argProp.arraySize; i++)
            {
                types[i] = Arg.RealType((Arg.ArgType)argProp.FindPropertyRelative("Array.data[" + i + "].argType").enumValueIndex);
            }
            return types;
        }

        private void SetNewTarget(SerializedProperty property, UnityEngine.Object target)
        {
            SerializedProperty targetProp = property.FindPropertyRelative("_target");
            targetProp.objectReferenceValue = target;
            property.FindPropertyRelative("dirty").boolValue = true;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        private void SetMethod(SerializedProperty property, UnityEngine.Object target, MethodInfo methodInfo, bool dynamic)
        {
            SerializedProperty targetProp = property.FindPropertyRelative("_target");
            targetProp.objectReferenceValue = target;
            SerializedProperty methodProp = property.FindPropertyRelative("_methodName");
            methodProp.stringValue = methodInfo.Name;
            SerializedProperty dynamicProp = property.FindPropertyRelative("_dynamic");
            dynamicProp.boolValue = dynamic;
            SerializedProperty argProp = property.FindPropertyRelative("_args");
            ParameterInfo[] parameters = methodInfo.GetParameters();
            argProp.arraySize = parameters.Length;
            for (int i = 0; i < parameters.Length; i++)
            {
                argProp.FindPropertyRelative("Array.data[" + i + "].argType").enumValueIndex = (int)Arg.FromRealType(parameters[i].ParameterType);
            }
            property.FindPropertyRelative("dirty").boolValue = true;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }

        private static string GetTypeName(Type t)
        {
            if (t == typeof(int)) return "int";
            else if (t == typeof(float)) return "float";
            else if (t == typeof(string)) return "string";
            else if (t == typeof(bool)) return "bool";
            else if (t == typeof(void)) return "void";
            else return t.Name;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lineheight = EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
            SerializedProperty targetProp = property.FindPropertyRelative("_target");
            SerializedProperty argProps = property.FindPropertyRelative("_args");
            SerializedProperty dynamicProp = property.FindPropertyRelative("_dynamic");
            float height = lineheight + lineheight;
            if (targetProp.objectReferenceValue != null && !dynamicProp.boolValue) height += argProps.arraySize * lineheight;
            height += 8;
            return height;
        }

        private static VP_DialogCondition GetDummyFunction(SerializedProperty prop)
        {
            string stringValue = prop.FindPropertyRelative("_typeName").stringValue;
            Type type = Type.GetType(stringValue, false);
            VP_DialogCondition result;
            if (type == null)
            {
                return null;
            }
            else
            {
                result = (Activator.CreateInstance(type) as VP_DialogCondition);
            }
            return result;
        }

        private void SelectGOInfo(SerializedProperty property, UnityEngine.Object reference)
        {
            SerializedProperty targetProp = property.FindPropertyRelative("m_object");
     
            targetProp.objectReferenceValue = reference;
            targetProp.serializedObject.ApplyModifiedProperties();
            targetProp.serializedObject.Update();

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
    }

}

*/

/*
using UnityEditor;
using UnityEngine;

namespace VirtualPhenix
{
    // prefab override logic works on the entire property.
    [CustomPropertyDrawer(typeof(VP_DialogCondition))]
    public class VP_ConditionDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            // Store old indent level and set it to 0, the PrefixLabel takes care of it

            position = EditorGUI.PrefixLabel(position, label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect buttonRect = position;
            buttonRect.width = 80;

            string buttonLabel = "Select";
      
            SerializedProperty targetProp = property.FindPropertyRelative("_target");
            UnityEngine.Object target = targetProp.objectReferenceValue as UnityEngine.Object;

            if (target != null)
                buttonLabel = target.name;

            if (GUI.Button(buttonRect, buttonLabel))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("None"), target == null, () => SelectGOInfo(property, null));

                foreach (UnityEngine.Object obj in Object.FindObjectsOfType(typeof(GameObject)))
                {
                    GUIContent content = new GUIContent(obj.name);
                    menu.AddItem(content, obj == target, () => SelectGOInfo(property, obj));
                }

                menu.ShowAsContext();
            }

            position.x += buttonRect.width + 4;
            position.width -= buttonRect.width + 4;
            EditorGUI.ObjectField(position, property, typeof(UnityEngine.Object), GUIContent.none);

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

       
    }
}
*/