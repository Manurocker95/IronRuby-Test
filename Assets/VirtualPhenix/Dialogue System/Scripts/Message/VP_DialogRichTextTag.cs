namespace VirtualPhenix.Dialog
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// RichTextTags help parse text that contains HTML style tags, used by Unity's RichText text components.
    /// </summary>
    public class VP_DialogRichTextTag
    {
        public static readonly VP_DialogRichTextTag ClearColorTag = new VP_DialogRichTextTag("<color=#00000000>");

        private const char OpeningNodeDelimeter = '<';
        private const char CloseNodeDelimeter = '>';
        private const char EndTagDelimeter = '/';
        private const string ParameterDelimeter = "=";


        public string m_tagType = "";
        public string m_fullText = "";
        public string m_middleText = "";
        public string m_parameter = "";


        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextTag"/> class.
        /// </summary>
        /// <param name="tagText">Tag text.</param>
        public VP_DialogRichTextTag(string tagText, string _tagType = "", string middleText = "", string fullText = "")
        {
            this.m_fullText = fullText;
            this.TagText = tagText;
            var parameterDelimeterIndex = 0;

            // -------- TagType ---------
            if (string.IsNullOrEmpty(_tagType))
            {
                int idxStart = tagText.IndexOf(OpeningNodeDelimeter);
                int idxEnd = tagText.IndexOf(EndTagDelimeter);
                // Strip start and end tags
                var tagType = tagText.Substring(1, tagText.Length - 2);
                tagType = tagType.TrimStart(EndTagDelimeter);

                // Strip Parameter
                parameterDelimeterIndex = tagType.IndexOf(ParameterDelimeter);
                if (parameterDelimeterIndex > 0)
                {
                    m_tagType = tagType.Substring(0, parameterDelimeterIndex);
                }
            }
            else
            {
                m_tagType = _tagType;
            }

            // -------- Parameter (after =) -------
            parameterDelimeterIndex = tagText.IndexOf(ParameterDelimeter);
            if (parameterDelimeterIndex < 0)
            {
                m_parameter = string.Empty;
            }
            else
            {
                // Subtract two, one for the delimeter and one for the closing character
                var parameterLength = tagText.Length - parameterDelimeterIndex - 2;
                var parameter = tagText.Substring(parameterDelimeterIndex + 1, parameterLength);

                // Kill optional enclosing quotes
                if (parameter.Length > 0)
                {
                    if (parameter[0] == '\"' && parameter[parameter.Length - 1] == '\"')
                    {
                        parameter = parameter.Substring(1, parameter.Length - 2);
                    }
                }

                m_parameter = parameter;
            }
         

            // -------- Middle Text -------
            m_middleText = middleText;
            
        }

        /// <summary>
        /// Gets the full tag text including markers.
        /// </summary>
        /// <value>The tag full text.</value>
        public string TagText { get; private set; }

        /// <summary>
        /// Gets the text for this tag if it's used as a closing tag. Closing tags are unchanged.
        /// </summary>
        /// <value>The closing tag text.</value>
        public string ClosingTagText
        {
            get
            {
                return this.IsClosingTag ? this.TagText : string.Format("</{0}>", this.TagType);
            }
        }

        /// <summary>
        /// Gets the TagType, the body of the tag as a string
        /// </summary>
        /// <value>The type of the tag.</value>
        public string TagType
        {
            get
            {
                return m_tagType;
            }
        }
       
        /// <summary>
        /// Gets the parameter as a string. Ex: For tag Color=#FF00FFFF the parameter would be #FF00FFFF.
        /// </summary>
        /// <value>The parameter.</value>
        public string Parameter
        {
            get
            {
              
                return m_parameter;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is an opening tag.
        /// </summary>
        /// <value><c>true</c> if this instance is an opening tag; otherwise, <c>false</c>.</value>
        public bool IsOpeningTag
        {
            get
            {
                return !this.IsClosingTag;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a closing tag.
        /// </summary>
        /// <value><c>true</c> if this instance is a closing tag; otherwise, <c>false</c>.</value>
        public bool IsClosingTag
        {
            get
            {
                return this.TagText.Length > 1 && this.TagText[1] == EndTagDelimeter;
            }
        }

        /// <summary>
        /// Gets the length of the tag. Shorcut for the length of the full TagText.
        /// </summary>
        /// <value>The text length.</value>
        public int Length
        {
            get
            {
                return this.TagText.Length;
            }
        }

        /// <summary>
        /// Checks if the specified String starts with a tag.
        /// </summary>
        /// <returns><c>true</c>, if the first character begins a tag <c>false</c> otherwise.</returns>
        /// <param name="text">Text to check for tags.</param>
        public static bool StringStartsWithTag(string text)
        {
            return text.StartsWith(VP_DialogRichTextTag.OpeningNodeDelimeter.ToString());
        }

        /// <summary>
        /// Parses the text for the next RichTextTag.
        /// </summary>
        /// <returns>The next RichTextTag in the sequence. Null if the sequence contains no RichTextTag</returns>
        /// <param name="text">Text to parse.</param>
        public static VP_DialogRichTextTag ParseNext(string text)
        {
            // Trim up to the first delimeter
            var openingDelimeterIndex = text.IndexOf(VP_DialogRichTextTag.OpeningNodeDelimeter);

            // No opening delimeter found. Might want to throw.
            if (openingDelimeterIndex < 0)
            {
                return null;
            }

            var closingDelimeterIndex = text.IndexOf(VP_DialogRichTextTag.CloseNodeDelimeter);

            // No closingDelimeter found. Might want to throw.
            if (closingDelimeterIndex < 0)
            {
                return null;
            }

            var tagText = text.Substring(openingDelimeterIndex, closingDelimeterIndex - openingDelimeterIndex + 1);
            return new VP_DialogRichTextTag(tagText);
        }


        /// <summary>
        /// Parses the text for the next RichTextTag.
        /// </summary>
        /// <returns>The next RichTextTag in the sequence. Null if the sequence contains no RichTextTag</returns>
        /// <param name="text">Text to parse.</param>
        public static VP_DialogRichTextTag ParseNextWithMiddleText(string text)
        {
            // Trim up to the first delimeter
            var openingDelimeterIndex = text.IndexOf(VP_DialogRichTextTag.OpeningNodeDelimeter);

            // No opening delimeter found. Might want to throw.
            if (openingDelimeterIndex < 0 || text[1] == VP_DialogRichTextTag.EndTagDelimeter)
            {
                return null;
            }

            var closingDelimeterIndex = text.IndexOf(VP_DialogRichTextTag.CloseNodeDelimeter);

            // No closingDelimeter found. Might want to throw.
            if (closingDelimeterIndex < 0)
            {
                return null;
            }

            var closingPlusOne = closingDelimeterIndex + 1;

            var tagText = text.Substring(openingDelimeterIndex, closingDelimeterIndex - openingDelimeterIndex + 1);

            // Strip start and end tags
            var tagType = tagText.Substring(1, tagText.Length - 2);
            tagType = tagType.TrimStart(EndTagDelimeter);
            // Strip Parameter
            var parameterDelimeterIndex = tagType.IndexOf(ParameterDelimeter);
            if (parameterDelimeterIndex > 0)
            {
                tagType = tagType.Substring(0, parameterDelimeterIndex);
            }

            var finalTxt = text.Substring(closingPlusOne, text.Length - closingPlusOne);
            var lastOpening = finalTxt.IndexOf($"</{tagType}>", 0);
            var lastEnding = finalTxt.IndexOf($"</{tagType}>", 0) + 3 + tagType.Length;
            var middleText = lastOpening > 0 ? finalTxt.Substring(0, lastOpening) : "";
           
            var fulltxt = text.Substring(openingDelimeterIndex, closingPlusOne + lastEnding);

            return new VP_DialogRichTextTag(tagText, tagType, middleText, fulltxt);
        }


        /// <summary>
        /// Removes all copies of the tag of the specified type from the text string.
        /// </summary>
        /// <returns>The text string without any tag of the specified type.</returns>
        /// <param name="text">Text to remove Tags from.</param>
        /// <param name="tagType">Tag type to remove.</param>
        public static string RemoveTagsFromString(string text, string tagType, bool _dialog = true)
        {
            var dialogManager = VP_DialogManager.Instance;
            var bodyWithoutTags = text;
            for (int i = 0; i < text.Length; ++i)
            {
                var remainingText = text.Substring(i, text.Length - i);
                if (StringStartsWithTag(remainingText))
                {
                    var parsedTag = ParseNextWithMiddleText(remainingText);

                    if (parsedTag != null && parsedTag.TagType == tagType)
                    {
                        // Global Var
                        if (parsedTag.TagType == VP_DialogSetup.Tags.VARIABLE)
                        {
                            FieldData variable = null;
                            string middleText = parsedTag.m_middleText;
                            string varName = parsedTag.Parameter;

                            if (!string.IsNullOrEmpty(middleText) && !string.IsNullOrEmpty(parsedTag.Parameter))
                            {
                                if (middleText == "bool" || middleText == "Bool" || middleText == "boolean" || middleText == "Boolean")
                                {
                                    variable = dialogManager.GetVariable(varName, true);
                                }
                                else if (middleText == "int" || middleText == "Int" || middleText == "integer" || middleText == "Integer")
                                {
                                    variable = dialogManager.GetVariable(varName, 0);
                                }
                                else if (middleText == "float" || middleText == "Float")
                                {
                                    variable = dialogManager.GetVariable(varName, 0f);
                                }
                                else if (middleText == "double" || middleText == "Double")
                                {
                                    variable = dialogManager.GetVariable(varName, 0.0);
                                }
                                else if (middleText == "string" || middleText == "String" || middleText == "str" || middleText == "Str")
                                {
                                    variable = dialogManager.GetVariable(varName, "");
                                }
                                else if (middleText == "object" || middleText == "Object" || middleText == "unityobject" || middleText == "UnityObject")
                                {
                                    variable = dialogManager.GetVariable(varName, new UnityEngine.Object());
                                }
                                else if (middleText == "gameobject" || middleText == "Gameobject" || middleText == "gameObject" || middleText == "GameObject")
                                {
                                    variable = dialogManager.GetVariable(varName, new UnityEngine.GameObject());
                                }
                                string m_variableT;
                                if (variable != null)
                                {
                                    m_variableT = null;

                                    if (variable is Field<string>)
                                    {
                                        m_variableT = (variable as Field<string>).Value;
                                    }
                                    else if (variable is Field<bool>)
                                    {
                                        m_variableT = (variable as Field<bool>).Value.ToString();
                                    }
                                    else if (variable is Field<int>)
                                    {
                                        m_variableT = (variable as Field<int>).Value.ToString();
                                    }
                                    else if (variable is Field<float>)
                                    {
                                        m_variableT = (variable as Field<float>).Value.ToString();
                                    }
                                    else if (variable is Field<double>)
                                    {
                                        m_variableT = (variable as Field<double>).Value.ToString();
                                    }
                                    else if (variable is Field<UnityEngine.Object>)
                                    {
                                        m_variableT = (variable as Field<UnityEngine.Object>).Value.name;
                                    }

                                    if (!string.IsNullOrEmpty(m_variableT))
                                    {
                                        bodyWithoutTags = bodyWithoutTags.Replace(parsedTag.m_fullText, m_variableT);
                                    }
                                    else
                                    {

                                        bodyWithoutTags = bodyWithoutTags.Replace(parsedTag.m_fullText, string.Empty);
                                    }
                                }
                                else
                                {
                                    bodyWithoutTags = bodyWithoutTags.Replace(parsedTag.m_fullText, string.Empty);
                                }
                            }
                            else
                            {
                                bodyWithoutTags = bodyWithoutTags.Replace(parsedTag.m_fullText, string.Empty);
                            }
                        }
                        else if (parsedTag.TagType == VP_DialogSetup.Tags.EMOTION)
                        {
                            bodyWithoutTags = bodyWithoutTags.Replace(parsedTag.m_fullText, string.Empty);
                        }
                        else if (parsedTag.TagType == VP_DialogSetup.Tags.GRAPH_VARIABLE)
                        {
                            //Debug.Log("Graph Variable: " + middleText);
                            FieldData variable = null;
                            string middleText = parsedTag.m_middleText;
                            string varName = parsedTag.Parameter;

                            if (!string.IsNullOrEmpty(parsedTag.Parameter))
                            {
                                if (middleText == "bool" || middleText == "Bool" || middleText == "boolean" || middleText == "Boolean")
                                {
                                    variable = dialogManager.GetGraphVariable(varName, true);
                                }
                                else if (middleText == "int" || middleText == "Int" || middleText == "integer" || middleText == "Integer")
                                {
                                    variable = dialogManager.GetGraphVariable(varName, 0);
                                }
                                else if (middleText == "float" || middleText == "Float")
                                {
                                    variable = dialogManager.GetGraphVariable(varName, 0f);
                                }
                                else if (middleText == "double" || middleText == "Double")
                                {
                                    variable = dialogManager.GetGraphVariable(varName, 0.0);
                                }
                                else if (middleText == "string" || middleText == "String" || middleText == "str" || middleText == "Str")
                                {
                                    variable = dialogManager.GetGraphVariable(varName, "");
                                }
                                else if (middleText == "object" || middleText == "Object" || middleText == "unityobject" || middleText == "UnityObject")
                                {
                                    variable = dialogManager.GetGraphVariable(varName, new UnityEngine.Object());
                                }
                                else if (middleText == "gameobject" || middleText == "Gameobject" || middleText == "gameObject" || middleText == "GameObject")
                                {
                                    variable = dialogManager.GetGraphVariable(varName, new UnityEngine.GameObject());
                                }
                                string m_variableT;
                                if (variable != null)
                                {
                                    m_variableT = null;

                                    if (variable is Field<string>)
                                    {
                                        m_variableT = (variable as Field<string>).Value;
                                    }
                                    else if (variable is Field<bool>)
                                    {
                                        m_variableT = (variable as Field<bool>).Value.ToString();
                                    }
                                    else if (variable is Field<int>)
                                    {
                                        m_variableT = (variable as Field<int>).Value.ToString();
                                    }
                                    else if (variable is Field<float>)
                                    {
                                        m_variableT = (variable as Field<float>).Value.ToString();
                                    }
                                    else if (variable is Field<double>)
                                    {
                                        m_variableT = (variable as Field<double>).Value.ToString();
                                    }
                                    else if (variable is Field<UnityEngine.Object>)
                                    {
                                        m_variableT = (variable as Field<UnityEngine.Object>).Value.name;
                                    }

                                    if (!string.IsNullOrEmpty(m_variableT))
                                    {
                                        bodyWithoutTags = bodyWithoutTags.Replace(parsedTag.m_fullText, m_variableT);
                                    }
                                    else
                                    {
                                        bodyWithoutTags = bodyWithoutTags.Replace(parsedTag.m_fullText, "");
                                    }
                                }
                                else
                                {
                                    bodyWithoutTags = bodyWithoutTags.Replace(parsedTag.m_fullText, "");
                                }
                            }
                            else
                            {
                                bodyWithoutTags = bodyWithoutTags.Replace(parsedTag.m_fullText, "");
                            }

                        }
                        else
                        {
                            
                            string middleText = parsedTag.m_middleText;
                            bodyWithoutTags = bodyWithoutTags.Replace(parsedTag.m_fullText, middleText);
                        }

                        i += parsedTag.m_middleText.Length;
                    }
                }
            }

            return bodyWithoutTags;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="RichTextTag"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="RichTextTag"/>.</returns>
        public override string ToString()
        {
            return this.TagText;
        }
    }
}