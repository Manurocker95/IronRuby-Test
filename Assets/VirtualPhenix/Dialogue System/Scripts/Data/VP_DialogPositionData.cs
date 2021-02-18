using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    [CreateAssetMenu(fileName="New Dialog Position", menuName = "Virtual Phenix/Dialogue System/Data/Dialog position")]
    public class VP_DialogPositionData : VP_ScriptableObject
    {
        [Header("Dialog Position and Rotation")]
        // If false, rect transform is used
        public bool m_useLocalPosition = true;
        public Vector2 m_defaultPosition;
        public Vector2 m_position;
        public Vector2 m_dialogtopBottom;
        public Vector2 m_dialogLeftRight;
        public Vector2 m_offset;
        public bool m_setPosition = true;
        public bool m_useDefaultPosition = false;

        [Header("BG Position")]
        public bool m_useGameObjectPosition;
        public string m_useGameObjectName;


        [Header("Set BG Rotation and Scale")]
        public bool m_changeRotation = false;
        public bool m_changeScale = false;
        public bool m_changeZ = false;
        public Vector3 m_rotation;
        public Vector3 m_scale;
        public float m_posZ;


        [Header("Set BG Position")]
        public bool m_setBGPosition;
        public Vector2 m_bgTopBottom;
        public Vector2 m_bgLeftRight;

        [Header("Arrow Icon")]
        public Vector2 m_arrowIconPosition;
        public Vector2 m_iconOffset;

        [Header("Text Position")]
        public bool m_setTextPosition;

        public Vector2 m_textTopBottomMugshot = new Vector2(197.6f, 84.6f);
        public Vector2 m_textLeftRightMugshot = new Vector2(136.45f, 566.85f);

        public Vector2 m_textTopBottomRegular = new Vector2(197.6f, 84.6f);
        public Vector2 m_textLeftRightRegular = new Vector2(136.45f, 379.1f);
        
        [Header("Option Group Position")]
        public Vector2 m_optionGroupPosition = new Vector2(-117f, 30f);

        public void Trigger()
        {
            if (m_useDefaultPosition)
            {
                SetDefaultPosition();
            }   

            if (m_useGameObjectPosition && !string.IsNullOrEmpty(m_useGameObjectName))
            {
                SetGameObjectPositionByName();
            }
        }

        public void SetGameObjectPositionByName()
        {
            GameObject go = GameObject.Find(m_useGameObjectName);
            if (go)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(go.transform.position);
                m_position.x = screenPos.x + m_offset.x;
                m_position.y = screenPos.y + m_offset.y;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Virtual Phenix/Create/Dialogue System/Position Data based on Dialog &P")]
        static void SetupPositionDataBasedOnDialog()
        {
            string path = UnityEditor.EditorUtility.OpenFolderPanel("Where do you want to save the position data", "", "Position Data");
            string relativePath = Application.dataPath;

            if (path.Contains("Assets"))
            {
                int idx = VP_Utils.GetWordIndexInString(path, "Assets");
                relativePath = "Assets/"+path.Substring(idx, path.Length - idx);
            }
            

            foreach (GameObject selection in UnityEditor.Selection.gameObjects)
            {
                VP_DialogPositionData asset = ScriptableObject.CreateInstance<VP_DialogPositionData>();
                if (asset.InitializeAll(selection))
                {
                    UnityEditor.AssetDatabase.CreateAsset(asset, relativePath + "/"+selection.name+"_positionData.asset");
                    UnityEditor.AssetDatabase.SaveAssets();
                }
            }

            UnityEditor.EditorUtility.DisplayDialog("Position Data Created. ", "Position Data has been created in " + path + ", you may need to modify some values.", "Okay!");
        }

#endif
        public bool InitializeAll(GameObject dialog)
        {
            VP_DialogMessage msg = dialog.GetComponent<VP_DialogMessage>();

            if (!msg)
            {
                Debug.LogError("Dialog object was not selected or " + dialog.name + " has no VP_DialogMessage script attached. Position data can't be parsed.");
                return false;
            }

            RectTransform rct = msg.GetComponent<RectTransform>();
            m_position = rct.localPosition;
            m_defaultPosition = m_position;
            m_posZ = rct.localPosition.z;
            m_rotation = rct.localRotation.eulerAngles;

            RectTransform txtRct = msg.MainTextRect;
	        m_textTopBottomMugshot = VP_Utils.Math.GetRectTopBottom(txtRct);
            m_textLeftRightMugshot = VP_Utils.Math.GetRectLeftRight(txtRct);

            m_textTopBottomRegular = VP_Utils.Math.GetRectTopBottom(txtRct);
            m_textLeftRightRegular = VP_Utils.Math.GetRectLeftRight(txtRct);

            m_dialogtopBottom = VP_Utils.Math.GetRectTopBottom(rct);
            m_dialogLeftRight = VP_Utils.Math.GetRectLeftRight(rct);
            m_useLocalPosition = true;

            Vector3 gp = msg.OptionGroup.transform.localPosition;
            m_optionGroupPosition = new Vector2(gp.x, gp.y);

            RectTransform bgRect = msg.BGRect;
            m_bgTopBottom  = VP_Utils.Math.GetRectTopBottom(bgRect);
            m_bgLeftRight = VP_Utils.Math.GetRectLeftRight(bgRect);

            return true;
        }

        public void SetTargetTransform(Transform trs, Camera cam)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(trs.position);
            m_position.x = screenPos.x + m_offset.x; 
            m_position.y = screenPos.y + m_offset.y;

            m_arrowIconPosition.x = screenPos.x + m_iconOffset.x;
            m_arrowIconPosition.y = screenPos.y + m_iconOffset.y;
        }

        public void SetPosition(float _x, float _y)
        {
            this.m_position.x = _x;
            this.m_position.y = _y;
        }

        public void SetRotation(Vector3 _rotationInAngles)
        {
            m_changeRotation = true;
            this.m_rotation = new Vector3(_rotationInAngles.x, _rotationInAngles.y, _rotationInAngles.z);
        }

        public void SetScale(Vector3 _scale)
        {
            m_changeScale = true;
            this.m_scale = new Vector3(_scale.x, _scale.y, _scale.z);
        }

        public void SetZPos(float _z)
        {
            m_changeZ = true;
            this.m_posZ = _z;
        }

        public void SetDefaultPosition()
        {
            this.m_position.x = this.m_defaultPosition.x;
            this.m_position.y = this.m_defaultPosition.y;

            m_arrowIconPosition.x = m_defaultPosition.x + m_iconOffset.x;
            m_arrowIconPosition.y = m_defaultPosition.y + m_iconOffset.y;
        }
    }

}
