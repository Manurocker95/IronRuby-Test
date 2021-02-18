#if USE_INCONTROL
#if UNITY_EDITOR && (UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER)
namespace VirtualPhenix.Inputs
{
	using UnityEditor;
	using UnityEngine;


	[CustomEditor( typeof( VP_InControlInputModule ) )]
	public class VP_InControlInputModuleEditor : Editor
	{
		SerializedProperty submitButton;
		SerializedProperty cancelButton;

		SerializedProperty analogMoveThreshold;
		SerializedProperty moveRepeatFirstDuration;
		SerializedProperty moveRepeatDelayDuration;
		SerializedProperty forceModuleActive;
		SerializedProperty allowMouseInput;
		SerializedProperty focusOnMouseHover;
		SerializedProperty allowTouchInput;
		SerializedProperty allowKeyboardInput;
		SerializedProperty inputData;
		SerializedProperty destroyActionOnDisable;
		SerializedProperty actionID;


	

		void OnEnable()
		{
			submitButton = serializedObject.FindProperty( "submitButton" );
			cancelButton = serializedObject.FindProperty( "cancelButton" );
			analogMoveThreshold = serializedObject.FindProperty( "analogMoveThreshold" );
			moveRepeatFirstDuration = serializedObject.FindProperty( "moveRepeatFirstDuration" );
			moveRepeatDelayDuration = serializedObject.FindProperty( "moveRepeatDelayDuration" );
			forceModuleActive = serializedObject.FindProperty( "forceModuleActive" );
			allowMouseInput = serializedObject.FindProperty( "allowMouseInput" );
			focusOnMouseHover = serializedObject.FindProperty( "focusOnMouseHover" );
			allowTouchInput = serializedObject.FindProperty( "allowTouchInput" );
			allowKeyboardInput = serializedObject.FindProperty( "m_allowKeyboardInput" );
			inputData = serializedObject.FindProperty( "m_inputData" );
			destroyActionOnDisable = serializedObject.FindProperty( "m_destroyActionOnDisable" );
			actionID = serializedObject.FindProperty( "m_actionSetID" );
		}


		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			GUILayout.Space( 10.0f );
			EditorGUILayout.LabelField( "Navigation", EditorStyles.boldLabel );

			analogMoveThreshold.floatValue = EditorGUILayout.Slider( "Analog Threshold", analogMoveThreshold.floatValue, 0.1f, 0.9f );
			moveRepeatFirstDuration.floatValue = EditorGUILayout.FloatField( "Delay Until Repeat", moveRepeatFirstDuration.floatValue );
			moveRepeatDelayDuration.floatValue = EditorGUILayout.FloatField( "Repeat Interval", moveRepeatDelayDuration.floatValue );

			GUILayout.Space( 10.0f );
		
			EditorGUILayout.LabelField( "Options", EditorStyles.boldLabel );

			forceModuleActive.boolValue = EditorGUILayout.Toggle( "Force Module Active", forceModuleActive.boolValue );
			allowMouseInput.boolValue = EditorGUILayout.Toggle( "Allow Mouse Input", allowMouseInput.boolValue );
			focusOnMouseHover.boolValue = EditorGUILayout.Toggle( "Focus Mouse On Hover", focusOnMouseHover.boolValue );
			allowTouchInput.boolValue = EditorGUILayout.Toggle( "Allow Touch Input", allowTouchInput.boolValue );
			allowKeyboardInput.boolValue = EditorGUILayout.Toggle( "Allow Keyboard & Gamepad Input", allowKeyboardInput.boolValue );
			destroyActionOnDisable.boolValue = EditorGUILayout.Toggle( "Destroy Actions On Disable", destroyActionOnDisable.boolValue );
			GUILayout.Space( 10.0f );
			
			actionID.stringValue = EditorGUILayout.TextField("Action Set ID",actionID.stringValue);
			
			GUILayout.Space( 10.0f );
			
			EditorGUILayout.LabelField( "Input Data", EditorStyles.boldLabel );
			
			if (inputData != null)
				EditorGUILayout.PropertyField(inputData, new GUIContent("Input Data"),true);
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif

#endif