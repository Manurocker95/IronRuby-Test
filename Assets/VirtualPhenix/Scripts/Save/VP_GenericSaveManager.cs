using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Formatter;

namespace VirtualPhenix.Save
{

	[DefaultExecutionOrder(VP_ExecutingOrderSetup.SAVE_MANAGER), AddComponentMenu("")]
	public class VP_GenericSaveManager<T> : VP_SaveManagerBase where T : VP_Save
	{
		[Header("Save Data"), Space]
#if ODIN_INSPECTOR
		[Sirenix.Serialization.OdinSerialize] protected T m_save;
#else
		[SerializeField] protected T m_save;
#endif

		[SerializeField] protected bool m_saveOnInitSave = true;

		public virtual T Save { get { return m_save; } }

		protected override void Initialize()
		{
			base.Initialize();
			InitFileSystem();

			if (m_loadGameOnInit)
				InitSave();
			else if (m_save == null)
				m_save = DefaultValue();
		}

		protected override void Reset()
		{
			base.Reset();

			m_save = DefaultValue();

			InitFileSystem();

			VP_Debug.Log("Save Manager Reset");
		}

		public override void InitSave()
		{
#if !UNITY_SWITCH || UNITY_EDITOR
			if (m_location != SaveLocation.PlayerPrefs && GameDataExists())
			{
				LoadGame((bool _loaded) =>
				{
					if (!_loaded || m_save == null)
					{
						m_save = DefaultValue();

						if (m_saveOnInitSave)
						{
							SaveGame(m_save, (bool _saved) =>
							{
								VP_Debug.Log(_saved ? "Saved " : " not saved");

								if (m_save == null)
								{
									m_save = DefaultValue();
									VP_Debug.Log("You can play but only with a new save file as it couldn't be loaded.");
								}
							});
						}
					}
				});
			}
			else
			{
				if (m_location == SaveLocation.PlayerPrefs)
				{
					string s = LoadFileStringFromPathRegular("", out string str, (bool _load) =>
					{
						VP_Debug.Log(_load ? "Saved " : " not saved");
					});

					if (string.IsNullOrEmpty(s))
					{
						m_save = DefaultValue();
						if (m_saveOnInitSave)
						{
							SaveGame(m_save, (bool _saved) =>
							{
								VP_Debug.Log(_saved ? "First Saved " : "First not saved");

								if (m_save == null)
								{
									m_save = DefaultValue();
									VP_Debug.Log("You can play but cannot be saved.");
								}
							});
						}
					}
					else
					{
						m_save = LoadObjectFromJSONString<T>(s, (bool _loaded) =>
						{
							VP_Debug.Log(_loaded ? "Saved " : " not saved");
						});
						
						if (m_saveOnInitSave)
						{
							SaveGame(m_save, (bool _saved) =>
							{
								VP_Debug.Log(_saved ? "First Saved " : "First not saved");

								if (m_save == null)
								{
									m_save = DefaultValue();
									VP_Debug.Log("You can play but cannot be saved.");
								}
							});
						}
					}
				}
				else
				{
					m_save = DefaultValue();
					if (m_saveOnInitSave)
					{
						SaveGame(m_save, (bool _saved) =>
						{
							VP_Debug.Log(_saved ? "First Saved " : "First not saved");

							if (m_save == null)
							{
								m_save = DefaultValue();
								VP_Debug.Log("You can play but cannot be saved.");
							}

						});
					}
				}

			}
#else
			NintendoSwitchLoadGame<T>(out m_save, (bool _canLoad) =>
			{
				string str = _canLoad ? "Load Successfully" : "Load aborted";
				VP_Debug.Log(str);

				if (!_canLoad)
				{
					m_save = DefaultValue();
					if (m_saveOnInitSave)
					{
						NintendoSwitchSaveGame(m_save);
					}
				}
			});
#endif

		}


#if ODIN_INSPECTOR
		public virtual void LoadGameFormatted(System.Action<bool> _loadCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT, Sirenix.Serialization.DataFormat _dataFormat = Sirenix.Serialization.DataFormat.Binary)
		{
#if UNITY_SWITCH && !UNITY_EDITOR
			NintendoSwitchLoadGame<T>(out m_save, (bool _canLoad) =>
			{
			string str = _canLoad ? "Load Successfully" : "Load aborted";
			VP_Debug.Log(str);

			if (_loadCallback != null)
			_loadCallback.Invoke(_canLoad);

			});
#else
			VP_Formatter.LoadObjectFromDataFile<T>(m_savePath, m_location == SaveLocation.PlayerPrefs, out m_save, _formatter, _dataFormat, (bool _canLoad) =>
			{
				string str = _canLoad ? "Load Successfully" : "Load aborted";
				VP_Debug.Log(str);

				if (_loadCallback != null)
					_loadCallback.Invoke(_canLoad);

			}, m_convertSaveData, m_encryption);
#endif
		}
#endif


		public virtual T DefaultValue()
		{
			return default(T);
		}

		public override void ResetSave()
		{
			m_save = DefaultValue();
		}


		public virtual void SaveGame(T _save = null, System.Action<bool> _saveCallback = null)
		{
			T save = (_save != null) ? _save : m_save;


#if UNITY_SWITCH && !UNITY_EDITOR
			NintendoSwitchSaveGame(save, _saveCallback);
#else

			VP_Formatter.SaveObjectoToDataFile(save, m_savePath, m_location == SaveLocation.PlayerPrefs, m_formatter, (bool _canSave) =>
			{
				string str = _canSave ? "Saved Successfully" : "Saved aborted";
				VP_Debug.Log(str);

				if (_saveCallback != null)
					_saveCallback.Invoke(_canSave);
			}, m_convertSaveData, m_encryption);
#endif
		}


		public override void LoadGame(System.Action<bool> _loadCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT)
		{
#if UNITY_SWITCH && !UNITY_EDITOR
			NintendoSwitchLoadGame<T>(out m_save, (bool _canLoad) =>
			{
			string str = _canLoad ? "Load Successfully" : "Load aborted";
			VP_Debug.Log(str);

			if (_loadCallback != null)
			_loadCallback.Invoke(_canLoad);

			});
#else
			VP_Formatter.LoadObjectFromDataFile<T>(m_savePath, m_location == SaveLocation.PlayerPrefs, out m_save, _formatter, (bool _canLoad) =>
			{
				string str = _canLoad ? "Load Successfully" : "Load aborted";
				VP_Debug.Log(str);
				if (_loadCallback != null)
					_loadCallback.Invoke(_canLoad);
			}, m_convertSaveData, m_encryption);
#endif
		}


#if ODIN_INSPECTOR
		public virtual void SaveGameFormatted(T _save = null, System.Action<bool> _saveCallback = null, Sirenix.Serialization.DataFormat _dataFormat = Sirenix.Serialization.DataFormat.Binary)
		{
			T save = (_save != null) ? _save : m_save;
#if UNITY_SWITCH && !UNITY_EDITOR
			NintendoSwitchSaveGame(save, _saveCallback);
#else
			VP_Formatter.SaveObjectoToDataFile(save, m_savePath, m_location == SaveLocation.PlayerPrefs, m_formatter, _dataFormat, (bool _canSave) =>
			{
				string str = _canSave ? "Saved Successfully" : "Saved aborted";
				VP_Debug.Log(str);

				if (_saveCallback != null)
					_saveCallback.Invoke(_canSave);
			}, m_convertSaveData, m_encryption);
#endif
		}
#endif

	}

}