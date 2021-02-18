namespace VirtualPhenix.Dialog
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class VP_DialogTextSymbol
    {
        public VP_DialogTextSymbol(string character)
        {
            this.Character = character[0];
        }

        public VP_DialogTextSymbol(VP_DialogRichTextTag tag)
        {
            this.Tag = tag;
        }

        public char Character { get; private set; }

        public VP_DialogRichTextTag Tag { get; private set; }

        public int Length
        {
            get
            {
                return this.Text.Length;
            }
        }

        public string Text
        {
            get
            {
                if (this.IsTag)
                {
                    return this.Tag.TagText;
                }
                else
                {
                    return this.Character.ToString();
                }
            }
        }

        public bool IsTag
        {
            get
            {
                return this.Tag != null;
            }
        }

        public float GetFloatParameter(float defaultValue = 0f)
        {
            if (!this.IsTag)
            {
                Debug.LogWarning("Attempted to retrieve parameter from symbol that is not a tag.");
                return defaultValue;
            }

            float paramValue;
            if (!float.TryParse(this.Tag.Parameter, out paramValue))
            {
                var warning = string.Format(
                                "Found Invalid parameter format in tag [{0}]. " +
                                "Parameter [{1}] does not parse to a float.",
                                this.Tag,
                                this.Tag.Parameter);
                Debug.LogWarning(warning);
                paramValue = defaultValue;
            }

            return paramValue;
        }
    }
    
}