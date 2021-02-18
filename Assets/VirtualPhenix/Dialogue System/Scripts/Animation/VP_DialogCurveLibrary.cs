namespace VirtualPhenix.Dialog
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    [Serializable]
    public class VP_DialogCurvePreset
    {
        [Tooltip("Name identifying this preset. Can also be used as a CurveLibrary indexer key.")]
        public string Name;

        [Tooltip("Time offset between each character when calculating animation transform. 0 makes all characters move together. Other values produce a 'wave' effect.")]
        [Range(0f, 0.5f)]
        public float timeOffsetPerChar = 0f;

        [Tooltip("Curve showing x-position delta over time")]
        public AnimationCurve xPosCurve;
        [Tooltip("x-position curve is multiplied by this value")]
        [Range(0, 20)]
        public float xPosMultiplier = 0f;

        [Tooltip("Curve showing y-position delta over time")]
        public AnimationCurve yPosCurve;
        [Tooltip("y-position curve is multiplied by this value")]
        [Range(0, 20)]
        public float yPosMultiplier = 0f;

        [Tooltip("Curve showing 2D rotation delta over time")]
        public AnimationCurve rotationCurve;
        [Tooltip("2D rotation curve is multiplied by this value")]
        [Range(0, 90)]
        public float rotationMultiplier = 0f;

        [Tooltip("Curve showing uniform scale delta over time")]
        public AnimationCurve scaleCurve;
        [Tooltip("Uniform scale curve is multiplied by this value")]
        [Range(0, 10)]
        public float scaleMultiplier = 0f;
    }

    [CreateAssetMenu(fileName = "CurveLibrary", menuName = "Virtual Phenix/Dialogue System/Text Animation/Curve Preset", order = 1)]
    public class VP_DialogCurveLibrary : ScriptableObject
    {
        public List<VP_DialogCurvePreset> CurvePresets;

        /// <summary>
        /// Get the CurvePreset from this library with the provided key/name
        /// </summary>
        /// <param name="key">Key/name identifying the desired CurvePreset</param>
        /// <returns>Matching CurvePreset</returns>
        public VP_DialogCurvePreset this[string key]
        {
            get
            {
                var preset = this.FindPresetOrNull(key);
                if (preset == null)
                {
                    throw new KeyNotFoundException();
                }
                else
                {
                    return preset;
                }
            }
        }

        public bool ContainsKey(string key)
        {
            return this.FindPresetOrNull(key) != null;
        }

        private VP_DialogCurvePreset FindPresetOrNull(string key)
        {
            foreach (var preset in this.CurvePresets)
            {
                if (preset.Name.ToUpper() == key.ToUpper())
                {
                    return preset;
                }
            }

            return null;
        }
    }
}