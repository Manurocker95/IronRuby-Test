using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VirtualPhenix
{
    [AddComponentMenu(VP_PrefixSetup.MAIN_PREFIX+"/Camera/Camera Shake")]
    /// <summary>
    /// Adds a shaking effect to the camera which can be triggered through code. A random vector is
    /// added to the camera position on OnPreRender() if a shake is occuring.
    /// </summary>
    public class VP_CameraShake : VP_MonoBehaviour
    {

        [System.Serializable]
        public class VP_ShakeConfiguration
        {
            [Tooltip("The maximum distance the camera can move from its normal position while shaking.")]
            public float m_maxShakeDistance = 0.3f;

            [Tooltip("The time (in seconds) it takes for the camera to stop shaking.")]
            public float m_shakeTime = 1;

            [Tooltip("The strength of the camera shake over the course of its cooldown time.")]
            public AnimationCurve m_shakeDistanceOverTime = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        public enum SHAKE_INTENSITY
        {
            TINY,
            GENTLE,
            MODERATE,
            HEAVY,
            EXTREME,
        }

        [Header("Shaking Camera")]
        protected VP_ShakeConfiguration m_currentConfiguration;
        [SerializeField] protected Transform m_shakingCamera;

#if ODIN_INSPECTOR
        [Sirenix.Serialization.OdinSerialize] protected VP_SerializableDictionary<SHAKE_INTENSITY, VP_ShakeConfiguration> m_shakeConfigurations = new VP_SerializableDictionary<SHAKE_INTENSITY, VP_ShakeConfiguration>();
#else
          [SerializeField] protected VP_SerializableDictionary<SHAKE_INTENSITY, VP_ShakeConfiguration> m_shakeConfigurations = new VP_SerializableDictionary<SHAKE_INTENSITY, VP_ShakeConfiguration>();
#endif
        protected float m_shakeCooldown;
        protected float m_shakeCooldownThreshold = 0.05f;
        protected Vector3 m_lastShakeVector;
        protected SHAKE_INTENSITY m_lastIntensity;

        public virtual bool IsShaking { get { return m_shakeCooldown > m_shakeCooldownThreshold; } }


        protected virtual void Reset()
        {

            m_shakeConfigurations = new VP_SerializableDictionary<SHAKE_INTENSITY, VP_ShakeConfiguration>();
            m_shakingCamera = Camera.main.transform;
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (m_shakeConfigurations == null)
                m_shakeConfigurations = new VP_SerializableDictionary<SHAKE_INTENSITY, VP_ShakeConfiguration>();

            if (!m_shakingCamera)
                m_shakingCamera = Camera.main.transform;
        }

        protected override void StartAllListeners()
        {
            base.StartAllListeners();

            VP_EventManager.StartListening<SHAKE_INTENSITY>(VP_EventSetup.Camera.SHAKE_CAMERA, Shake);
        }

        protected override void StopAllListeners()
        {
            base.StopAllListeners();

            VP_EventManager.StopListening<SHAKE_INTENSITY>(VP_EventSetup.Camera.SHAKE_CAMERA, Shake);
        }

        /// <summary>
        /// Shakes the camera.
        /// </summary>
        public virtual void Shake(SHAKE_INTENSITY intensity)
        {
            if (!IsShaking || intensity > m_lastIntensity)
            {
                m_lastIntensity = intensity;
                m_currentConfiguration = m_shakeConfigurations[intensity];
                m_shakeCooldown = m_currentConfiguration.m_shakeTime;
            }
        }

        /// <summary>
        /// Shakes all cameras in the scene.
        /// </summary>
        public static void ShakeAllCameras(SHAKE_INTENSITY intensity)
        {
            VP_CameraShake[] shakes = FindObjectsOfType<VP_CameraShake>();

            foreach (VP_CameraShake shake in shakes)
            {
                shake.Shake(intensity);
            }
        }

        protected virtual void Update()
        {
            if (IsShaking)
            {
                m_shakeCooldown = Mathf.MoveTowards(m_shakeCooldown, 0, Time.unscaledDeltaTime);
            }
#if UNITY_EDITOR
            // Debug shakes by holding R, T, Y, and then pressing a number from 1 to 5.
            if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.T) && Input.GetKey(KeyCode.Y))
            {
                for (KeyCode key = KeyCode.Alpha1; key <= KeyCode.Alpha5; key++)
                {
                    if (Input.GetKeyDown(key))
                    {
                        Shake((SHAKE_INTENSITY)(int)(key - KeyCode.Alpha1));
                    }
                }
            }
#endif
        }

#if !PHOENIX_URP_BLIT_PASS
        protected virtual void OnPreRender()
        {
            if (IsShaking)
            {
                float shakePercent = m_shakeCooldown / m_currentConfiguration.m_shakeTime;
                this.m_lastShakeVector = UnityEngine.Random.insideUnitSphere
                    * m_currentConfiguration.m_shakeDistanceOverTime.Evaluate(shakePercent)
                    * m_currentConfiguration.m_maxShakeDistance;
                m_shakingCamera.position += m_lastShakeVector;
            }
        }
        protected virtual void OnPostRender()
        {
            if (IsShaking)
            {
                m_shakingCamera.position -= m_lastShakeVector;
            }
        }
#endif
    }

}