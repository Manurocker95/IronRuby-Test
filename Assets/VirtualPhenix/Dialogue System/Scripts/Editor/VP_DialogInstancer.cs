using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    public class VP_DialogInstancer : MonoBehaviour
    {
        [MenuItem("Virtual Phenix/Create/Dialogue System/Full Init Setup")]
        static void CreateFullSetup()
        {

            CreateChart();
            CreateManagers();
     
        }

        [MenuItem("Virtual Phenix/Create/Dialogue System/Add Default Dialog")]
        static void AddDefaultDialog()
        {
            if (Selection.gameObjects.Length > 0)
            {
                foreach (GameObject go in Selection.gameObjects)
                {
                    GameObject d = GameObject.Instantiate(Resources.Load<GameObject>("Dialogue/Prefabs/Dialog/Dialog"), go.transform);
                    d.name = d.name.Replace("(Clone)", "");
                    d.SetActive(true);
                }
            }
            else
            {
                Transform t = GameObject.Find("Canvas").transform;
                GameObject d = GameObject.Instantiate(Resources.Load<GameObject>("Dialogue/Prefabs/Dialog/Dialog"), t);
                d.name = d.name.Replace("(Clone)", "");
                d.SetActive(true);
            }
        }

        [MenuItem("Virtual Phenix/Create/Dialogue System/New Chart")]
        public static void CreateChart()
        {
            GameObject canvas = Instantiate(Resources.Load<GameObject>("Dialogue/Prefabs/Dialog/Canvas"), Vector3.zero, Quaternion.identity);
            canvas.name = canvas.name.Replace("(Clone)", "");

            GameObject chart = Instantiate(Resources.Load<GameObject>("Dialogue/Prefabs/Dialog/Chart"), Vector3.zero, Quaternion.identity);
            chart.name = chart.name.Replace("(Clone)", "");
            VP_DialogChart dChart = chart.GetComponent<VP_DialogChart>();
            VP_DialogMessage msg = canvas.transform.GetChild(0).GetComponent<VP_DialogMessage>();
            dChart.SetIntanceData(canvas.transform, msg);

            if (EditorUtility.DisplayDialog("Chart Warning", "You need to create a new graph and set it in the Chart", "OK"))
            {
                VP_DialogManager DialogManager = GameObject.FindObjectOfType<VP_DialogManager>();
                if (!DialogManager)
                {
                    GameObject DialogManagerGO = Instantiate(Resources.Load<GameObject>("Dialogue/Prefabs/Managers/DialogManager"), Vector3.zero, Quaternion.identity);
                    DialogManagerGO.name = DialogManagerGO.name.Replace("(Clone)", "");
                    DialogManager = DialogManagerGO.GetComponent<VP_DialogManager>();
                    DialogManager.SetInitialData(msg, canvas.transform, dChart);

                }
                else
                {
                    DialogManager.SetInitialData(msg, canvas.transform, dChart);
                }
            }
        }

        [MenuItem("Virtual Phenix/Create/Dialogue System/Instance Managers")]
        public static void CreateManagers()
        {
            VP_DialogManager DialogManager = GameObject.FindObjectOfType<VP_DialogManager>();
            if (!DialogManager)
            {
                GameObject DialogManagerGO = Instantiate(Resources.Load<GameObject>("Dialogue/Prefabs/Managers/DialogManager"), Vector3.zero, Quaternion.identity);
                DialogManagerGO.name = DialogManagerGO.name.Replace("(Clone)", "");
            }

            Localization.VP_LocalizationManager localizationManager = GameObject.FindObjectOfType<Localization.VP_LocalizationManager>();

            if (!localizationManager)
            {
                GameObject localizationGO = Instantiate(Resources.Load<GameObject>("Dialogue/Prefabs/Managers/LocalizationManager"), Vector3.zero, Quaternion.identity);
                localizationGO.name = localizationGO.name.Replace("(Clone)", "");
            }

            VP_AudioManager audioManager = GameObject.FindObjectOfType<VP_AudioManager>();

            if (!audioManager)
            {
                GameObject audioManagerGO = Instantiate(Resources.Load<GameObject>("Dialogue/Prefabs/Managers/AudioManager"), Vector3.zero, Quaternion.identity);
                audioManagerGO.name = audioManagerGO.name.Replace("(Clone)", "");
            }

            VP_EventManager eventManager = GameObject.FindObjectOfType<VP_EventManager>();

            if (!eventManager)
            {
                GameObject eventManagerGO = Instantiate(Resources.Load<GameObject>("Dialogue/Prefabs/Managers/EventManager"), Vector3.zero, Quaternion.identity);
                eventManagerGO.name = eventManagerGO.name.Replace("(Clone)", "");
            }

        }
    }

}
