#if ODIN_INSPECTOR
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Formatter
{

    public class VP_IndexResolver : IExternalIndexReferenceResolver
    {
        public List<UnityEngine.Object> referenceList;

        public VP_IndexResolver()
        {
            this.referenceList = new List<UnityEngine.Object>();
        }

        public VP_IndexResolver(List<UnityEngine.Object> references)
        {
            this.referenceList = references;
        }

        public bool CanReference(object value, out int index)
        {
            if (value is UnityEngine.Object)
            {
                index = this.referenceList.Count;
                this.referenceList.Add((UnityEngine.Object)value);
            }

            index = 0;
            return false;
        }

        public bool TryResolveReference(int index, out object value)
        {
            value = this.referenceList[index];
            return true;
        }
    }
}
#endif