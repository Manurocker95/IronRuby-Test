using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix;

namespace VirtualPhenix.Formatter
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.SO_DATABASE_MANAGER)]
    public class VP_ScriptableObjectDataBaseManager : VP_SingletonMonobehaviour<VP_ScriptableObjectDataBaseManager>
    {
#if USE_SCRIPTABLE_OBJECT_DATABASE
	    [SerializeField, DatabaseAssetProperty(typeof(ScriptableObject))] int m_completeDatabase;

	    public virtual ScriptableObject GetScriptableObjectFromFullDatabase(string _strkey)
        {
            short _key = short.Parse(_strkey);

            return GetScriptableObjectFromFullDatabase(_key);
        }

        public virtual ScriptableObject GetScriptableObjectFromFullDatabase(short _key)
        {
            MyLib.Shared.Database.Database db = MainDatabaseCollection.GetDatabase(m_completeDatabase).DatabaseData;

            return db.ContainsKey(_key) ? db.GetAsset(_key).AssetObject as ScriptableObject : null;
        }

        public virtual short GetScriptableObjectIDFromFullDatabase(ScriptableObject _so)
        {
            MyLib.Shared.Database.Database db = MainDatabaseCollection.GetDatabase(m_completeDatabase).DatabaseData;

            for (int i = 0; i < db.NumOfEntries; i++)
            {
                ScriptableObject item = db.GetAssetAtIndex(i).AssetObject as ScriptableObject;

                if (item == _so)
                {
                    return db.GetAssetAtIndex(i).AssetKey16;
                }
            }

            return 0;
        }
#else
        public virtual ScriptableObject GetScriptableObjectFromFullDatabase(string _strkey)
        {
            short _key = short.Parse(_strkey);

            return GetScriptableObjectFromFullDatabase(_key);
        }

        public virtual ScriptableObject GetScriptableObjectFromFullDatabase(short _key)
        {
            return null;
        }

        public virtual short GetScriptableObjectIDFromFullDatabase(ScriptableObject _so)
        {
            return 0;
        }
#endif
    }
}
