using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

#if PHOENIX_ACTIONS
using VirtualPhenix.Actions;
#endif

#if USE_SERIALIZABLE_DICTIONARIES_LITE
using RotaryHeart.Lib.SerializableDictionary;
#endif

using VirtualPhenix.Dialog;
using VirtualPhenix.Serialization;

namespace VirtualPhenix
{
#if !USE_SERIALIZABLE_DICTIONARIES_LITE
#if !ODIN_INSPECTOR    
	[System.Serializable]
	public class VP_SerializableDictionary<TKey, TValue> : SerializableDictionary<TKey, TValue>
#else
	[System.Serializable]
	public class VP_SerializableDictionary<TKey, TValue> : System.Collections.Generic.Dictionary<TKey, TValue>
#endif
	{

#if ODIN_INSPECTOR
		public virtual void CopyFrom(IDictionary<TKey, TValue> src)
		{
			if (src != null)
			{

				Clear();
				foreach (TKey k in src.Keys)
				{
					Add(k, src[k]);
				}
		
			}
		}
#endif

		public VP_SerializableDictionary() : base()
        {

        }

        public VP_SerializableDictionary(IDictionary<TKey, TValue> dict) : base(dict)
        {

        }

        public virtual bool Contains(TKey _key)
		{
			return Keys.Contains(_key);
		}

        public virtual bool ContainsValueInDictionary(TValue _value)
        {
            return Values.Contains(_value);
        }

        public virtual void RemoveValue(TValue _value)
        {

            bool _found = false;
            int idx = 0;
            if (ContainsValueInDictionary(_value))
            {
                foreach (TValue v in Values)
                {
                    if (v.Equals(_value))
                    {
                        _found = true;
                        break;
                    }
                    idx++;
                }
            }

            if (_found)
            {
                TKey k = Keys.ElementAt(idx);
                Remove(k);
            }
        }
    }
#else
        [System.Serializable]
    public class VP_SerializableDictionary<TKey, TValue> : SerializableDictionaryBase<TKey, TValue>
    {
        public virtual bool ContainsValue(TValue _value)
        {
            return Values.Contains(_value);
        }

        public virtual bool Contains(TKey _key)
        {
            return base.ContainsKey(_key);
        }

        public virtual bool ContainsValueInDictionary(TValue _value)
        {
            return Values.Contains(_value);
        }

        public virtual void RemoveValue(TValue _value)
        {

            bool _found = false;
            int idx = 0;
            if (ContainsValue(_value))
            {
                foreach (TValue v in Values)
                {
                    if (v.Equals(_value))
                    {
                        _found = true;
                        break;
                    }
                    idx++;
                }
            }

            if (_found)
            {
                TKey k = Keys.ElementAt(idx);
                Remove(k);
            }
        }
    }
#endif
    public static class VP_SerializableDictionary
	{
		[System.Serializable]
        public class Storage<T> : SerializableDictionary.Storage<T>
        {

        }
    }

    [System.Serializable]
    public class VP_CharacterAudioDictionary : VP_SerializableDictionary<ALPHABET, AudioClip>
    {

    }

    [System.Serializable]
    public class VP_GenericVariables<T> : VP_SerializableDictionary<string, Field<T>>
    {

    }
	[System.Serializable]
	public class VP_TextAssetDictionary : VP_SerializableDictionary<VirtualPhenix.Localization.LANGUAGE_PARSER, TextAsset>
    {

    }

	[System.Serializable]
	public class VP_TextItemDictionary : VP_SerializableDictionary<string, VirtualPhenix.Localization.VP_TextItem>
	{
		public VP_TextItemDictionary()
		{

		}

		public VP_TextItemDictionary(IDictionary<string, VirtualPhenix.Localization.VP_TextItem> dict) : base(dict)
		{

		}
	}

    [System.Serializable]
    public class VP_BoolVariables : VP_SerializableDictionary<string, bool>
    {

    }

    [System.Serializable]
    public class VP_IntVariables : VP_SerializableDictionary<string, int>
    {

    }

    [System.Serializable]
    public class VP_DoubleVariables : VP_SerializableDictionary<string, double>
    {

    }

    [System.Serializable]
    public class VP_FloatVariables : VP_SerializableDictionary<string, float>
    {

    }

    [System.Serializable]
    public class VP_StringVariables : VP_SerializableDictionary<string, string>
    {

    }

    [System.Serializable]
    public class VP_GameObjectVariables : VP_SerializableDictionary<string, GameObject>
    {

    }    

    [System.Serializable]
    public class VP_UnityObjectVariables : VP_SerializableDictionary<string, UnityEngine.Object>
    {

    }

    [System.Serializable]
    public class SetVariableList : VP_SerializableDictionary<VariableScope, VP_VariableDataBase>
    {

    }

    [System.Serializable]
    public class VP_GraphicFadingDictionary : VP_SerializableDictionary<int, FadeUITime>
    {

    }

    [System.Serializable]
    public class VP_SerializedEmotions : VP_SerializableDictionary<VP_DialogMessage.EMOTION, VP_DialogEmotionData>
    {

    }

    [System.Serializable]
    public class VP_SerializedDialogTypeSprites : VP_SerializableDictionary<DIALOG_TYPE, Sprite>
    {

    }

	[System.Serializable]
	public class VP_SerializedInputActions : VP_SerializableDictionary<string, VirtualPhenix.Inputs.VP_InputActions>
	{

	}

	[System.Serializable]
	public class VP_DialogAudioKeyDictionary : VP_SerializableDictionary<string, string>
	{

	}

    [System.Serializable]
    public class VP_SerializedCallbackListenDictionary : VP_SerializableDictionary<DIALOG_INPUT_CALLBACK, bool>
    {

    }


    [System.Serializable]
    public class VP_DialogMessageInputAxisDic : VP_SerializableDictionary<string, VP_DialogMessageAxisInputData>
    {

    }

    [System.Serializable]
    public class VP_DialogMessageInputButtonDic : VP_SerializableDictionary<string, VP_DialogMessageInput.PRESS_TYPE>
    {

    }

    [System.Serializable]
    public class VP_DialogMessageInputKeyCodeDic : VP_SerializableDictionary<KeyCode, VP_DialogMessageInput.PRESS_TYPE>
    {

    }

    [System.Serializable]
    public class VP_LocalizationDataDictionary : VP_SerializableDictionary<SystemLanguage, VirtualPhenix.Localization.VP_LocalizationData>
    {

    }

#if USE_ANIMANCER

	[System.Serializable]
	public class VP_RegularStringClipStateTransitionDictionary : VP_SerializableDictionary<int, VP_SerializableDictionary.Storage<VP_SerializableDictionary<string, Animancer.ClipState.Transition>>>
	{
		
	}

	[System.Serializable]
	public class VP_StringClipStateTransitionDictionary: VP_SerializableDictionary<string, Animancer.ClipState.Transition>
	{
		
	}


	[System.Serializable]
	public class VP_ClipStateTransitionDictionary<T1> : VP_SerializableDictionary<T1, Animancer.ClipState.Transition>
	{
		
	}

#endif

#if USE_INCONTROL
	[System.Serializable]
    public class VP_InControlInputDictionary : VP_SerializableDictionary<InControl.IInputControl, VP_InControlActionTriggerEvent>
    {

    }
#endif

    [System.Serializable]
    public class VP_MappedInputDictionary<T> : VP_SerializableDictionary<T, VP_MappedInputTriggerEvent<T>>
    {

    }

    [System.Serializable]
    public class VP_AnimatorControllerParameters : VP_SerializableDictionary<Animator, List<VirtualPhenix.Animations.VP_BasicAnimatorParameter>> 
    {

    }


#if PHOENIX_ACTIONS

    [System.Serializable]
    public class VP_ActionDictionary : VP_SerializableDictionary<int, VP_ActionContainer>
    {

    }

    [System.Serializable]
    public class VP_BranchConditionActionListOfListDictionary : VP_SerializableDictionary<List<VP_CustomBranchCondition>, List<List<VP_CustomActions>>>
    {

    }

    [System.Serializable]
    public class VP_CutomBranchConditionListStorage : VP_SerializableDictionary.Storage<List<VP_CustomBranchCondition>>
    {

    }

    [System.Serializable]
    public class VP_CutomActionListOfListStorage : VP_SerializableDictionary.Storage<List<List<VP_CustomActions>>>
    {

    }

    [System.Serializable]
    public class VP_CutomActionListStorage : VP_SerializableDictionary.Storage<List<VP_CustomActions>>
    {

    }
#endif
[System.Serializable]
    public class VP_GameConditionEventListDictionary : VP_SerializableDictionary<List<VP_GameEventCondition>, List<UnityEvent>>
    {

    }

    [System.Serializable]
    public class VP_UnityEventListStorage : VP_SerializableDictionary.Storage<List<UnityEvent>>
    {

    }

    [System.Serializable]
    public class VP_GameConditionListStorage : VP_SerializableDictionary.Storage<List<VP_GameEventCondition>>
    {

    }
}
