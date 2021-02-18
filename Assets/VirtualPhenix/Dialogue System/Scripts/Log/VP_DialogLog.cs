using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    public class VP_DialogLog 
    {
        public VP_DialogCharacterData m_character;
        public string m_saidText;
        public int m_dialogIdx;

        public VP_DialogLog()
        {

        }

        public VP_DialogLog(VP_DialogCharacterData _character, string _text, int _index = 0)
        {
            m_character = _character;
            m_saidText = _text;
            m_dialogIdx = _index;
        }

    }

}
