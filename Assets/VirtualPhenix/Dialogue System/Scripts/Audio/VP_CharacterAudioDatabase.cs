using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    /// <summary>
    /// Alphabet
    /// </summary>
    public enum ALPHABET
    {
        SPECIAL_CHARACTER,
        A, B, C, D, E, F, G, H,
        I, J, K, L, M, N, O, P,
        Q, R, S, T, U, V, W, X,
        Y, Z, NONE

    }
    
    [CreateAssetMenu(fileName = "CharacterAudioDB", menuName = "Virtual Phenix/Dialogue System/Character Audios", order = 1)]
    public class VP_CharacterAudioDatabase : VP_ScriptableObject
    {
        [SerializeField] protected VP_CharacterAudioDictionary m_maleLetterSounds = new VP_CharacterAudioDictionary();
        [SerializeField] protected VP_CharacterAudioDictionary m_femaleLetterSounds = new VP_CharacterAudioDictionary();
        [SerializeField] protected VP_CharacterAudioDictionary m_otherLetterSounds = new VP_CharacterAudioDictionary();
        [SerializeField] protected AudioClip m_defaultCharacterSound = null;
        [SerializeField] protected AudioClip m_continueSound = null;
        [SerializeField] protected string m_defaultPath = "Dialogue\\Audio\\Letters\\";

        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_defaultCharacterSound == null)
            {
                m_defaultCharacterSound = Resources.Load<AudioClip>(m_defaultPath + "UITextDisplay");
            }

            if (m_continueSound == null)
            {
                m_continueSound = Resources.Load<AudioClip>(m_defaultPath + "UITextDisplay");
            }

            if (m_maleLetterSounds == null)
            {
                m_maleLetterSounds = new VP_CharacterAudioDictionary();
                ParseLetters(CHARACTER_GENDER.MALE);
            }

            if (m_femaleLetterSounds == null)
            {
                m_femaleLetterSounds = new VP_CharacterAudioDictionary();
                ParseLetters(CHARACTER_GENDER.FEMALE);
            }

            if (m_otherLetterSounds == null)
            {
                m_otherLetterSounds = new VP_CharacterAudioDictionary();
                ParseLetters(CHARACTER_GENDER.OTHER);
            }

        }

        public virtual AudioClip SpecialCharacter(CHARACTER_GENDER _gender) 
        {
            AudioClip ret = null;

            switch (_gender)
            {
                case CHARACTER_GENDER.MALE:
                    ret = m_maleLetterSounds.ContainsKey(ALPHABET.SPECIAL_CHARACTER) ? m_maleLetterSounds[ALPHABET.SPECIAL_CHARACTER] : null;
                    break;
                case CHARACTER_GENDER.FEMALE:
                    ret = m_femaleLetterSounds.ContainsKey(ALPHABET.SPECIAL_CHARACTER) ? m_femaleLetterSounds[ALPHABET.SPECIAL_CHARACTER] : null;
                    break;
                default:
                    ret = m_otherLetterSounds.ContainsKey(ALPHABET.SPECIAL_CHARACTER) ? m_otherLetterSounds[ALPHABET.SPECIAL_CHARACTER] : null;
                    break;
            }

            return (ret == null) ? m_defaultCharacterSound : ret;
        }

        public virtual AudioClip GetContinueSound()
        {
            return m_continueSound;
        }

        public virtual AudioClip GetDefaultCharacter()
        {
            return m_defaultCharacterSound;
        }

        public virtual AudioClip GetLetterSound(char _character, CHARACTER_GENDER _gender)
        {
            AudioClip ret = null;
            ALPHABET letter = GetAlphabetFromChar(_character);

            switch (_gender)
            {
                case CHARACTER_GENDER.MALE:
                    ret = m_maleLetterSounds.Contains(letter) ? m_maleLetterSounds[letter] : null;
                    break;
                case CHARACTER_GENDER.FEMALE:
                    ret = m_femaleLetterSounds.Contains(letter) ? m_femaleLetterSounds[letter] : null;
                    break;
                default:
                    ret = m_otherLetterSounds.Contains(letter) ? m_otherLetterSounds[letter] : null;
                    break;
            }

            return (ret == null) ? m_defaultCharacterSound : ret;
        }

        public virtual void ParseLetters(CHARACTER_GENDER _gender)
        {
            string _path = m_defaultPath + _gender.ToString() + "\\";
            AudioClip[] clips = Resources.LoadAll<AudioClip>(_path);
            for (int i = 0; i < clips.Length; i++)
            {
                string charaStr = clips[i].name.ToUpper();

                ALPHABET letterAlphabet = ALPHABET.NONE;

                System.Enum.TryParse<ALPHABET>(charaStr, out letterAlphabet);

                if (letterAlphabet != ALPHABET.NONE)
                {
                    switch (_gender)
                    {
                        case CHARACTER_GENDER.MALE:
                            m_maleLetterSounds.Add(letterAlphabet, clips[i]);
                            break;
                        case CHARACTER_GENDER.FEMALE:
                            m_femaleLetterSounds.Add(letterAlphabet, clips[i]);
                            break;
                        default:
                            m_otherLetterSounds.Add(letterAlphabet, clips[i]);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Get index of a vocal in the list
        /// </summary>
        /// <param name="_chara"></param>
        /// <returns></returns>
        protected virtual ALPHABET GetAlphabetFromChar(char _chara)
        {
            string charaStr = _chara.ToString().ToUpper();

            ALPHABET letterAlphabet = ALPHABET.A;

            System.Enum.TryParse<ALPHABET>(charaStr, out letterAlphabet);
            return letterAlphabet;
        }

        /// <summary>
        /// Get index of a vocal in the list
        /// </summary>
        /// <param name="_chara"></param>
        /// <returns></returns>
        protected virtual int GetIndexOfLetterInAlphabet(char _chara)
        {
            string charaStr = _chara.ToString().ToUpper();

            ALPHABET letterAlphabet = ALPHABET.NONE;

            System.Enum.TryParse<ALPHABET>(charaStr, out letterAlphabet);
            int index = (int)(letterAlphabet);
            //VP_Debug.Log("Index = " + index);
            return index;
        }       
    }
}
