using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Formatter;
using VirtualPhenix.Settings;

namespace VirtualPhenix
{
	public enum SaveLocation
	{
		DataPath,
		PersistentDatapath,
		Documents,
		Console,
		PlayerPrefs,
		Custom
	}

}

namespace VirtualPhenix.Save
{
	[DefaultExecutionOrder(VP_ExecutingOrderSetup.SAVE_MANAGER), AddComponentMenu("")]
	public class VP_SaveManagerBase : VP_SingletonMonobehaviour<VP_SaveManagerBase>
	{
		[Header("Save Manager"), Space]
		[SerializeField] protected FORMATTER_METHOD m_formatter = FORMATTER_METHOD.NO_CONTEXT;
		[SerializeField] protected ENCRYPTION m_encryption = ENCRYPTION.AES;
		[SerializeField] protected SaveLocation m_location = SaveLocation.PersistentDatapath;
#if ODIN_INSPECTOR
		[SerializeField] protected Sirenix.Serialization.DataFormat m_dataFormat = Sirenix.Serialization.DataFormat.Binary;
#endif
		[SerializeField] protected string m_savePath;
		[SerializeField] protected string m_saveName = "MySave";
		[SerializeField] protected string m_saveExtension = ".VPData";
		[SerializeField] protected string m_mountName = "MountSave";
		[SerializeField] protected string m_playerPrefKey = "SaveGamePP";
		[SerializeField] protected int m_saveDataVersion = 1;
		[SerializeField] protected bool m_convertSaveData = true;
		[SerializeField] protected bool m_loadGameOnInit = true;

#if UNITY_SWITCH
		[Header("Nintendo Switch"), Space]
		protected nn.account.Uid m_userId;
		protected nn.fs.FileHandle m_fileHandle = new nn.fs.FileHandle();
#endif

		public virtual string SavePath { get { return m_savePath; } }
		public virtual FORMATTER_METHOD Formatter { get { return m_formatter; } }
		public virtual ENCRYPTION Encryption { get { return m_encryption; } }
#if ODIN_INSPECTOR
		public virtual Sirenix.Serialization.DataFormat DataFormat { get { return m_dataFormat; } }
#endif
		public virtual string DataFolder { get { return m_location != SaveLocation.Console && m_location != SaveLocation.PlayerPrefs ? GetSaveLocation() : ""; } }
		public virtual string SaveName { get { return m_saveName; } }
		public virtual string FullSaveName { get { return m_saveName + m_saveExtension; } }
		public virtual string SaveExtension { get { return m_saveExtension; } }
		public virtual string MountName { get { return m_mountName; } }
		public virtual T0 GetSettings<T0>(System.Action<bool> _loadCallback) where T0 : VP_Settings
		{
			if (_loadCallback != null)
				_loadCallback.Invoke(true);

			return default(T0);
		}

		public virtual void SetSettings<T0>(T0 _value, System.Action<bool> _saveCallback) where T0 : VP_Settings
		{
			if (_saveCallback != null)
				_saveCallback.Invoke(true);
		}

		protected override void Reset()
		{
			base.Reset();

#if !UNITY_SWITCH && !UNITY_PS4 && !UNITY_XBOXONE && !UNITY_WEBGL
			m_location = SaveLocation.PersistentDatapath;
#elif UNITY_WEBGL
   m_location = SaveLocation.PlayerPrefs;
#else
			m_location = SaveLocation.Console;
#endif
		}

		public virtual void InitSave()
		{

		}

		public virtual bool GameDataExists(string _path = "")
		{
			if (_path.IsNullOrEmpty())
			{
				_path = m_savePath;
			}
#if (UNITY_PS4 || UNITY_SWITCH || UNITY_XBOXONE) && !UNITY_EDITOR
#if UNITY_SWITCH
			nn.fs.EntryType entryType = nn.fs.EntryType.Directory;
			nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, _path);

			if (!nn.fs.FileSystem.ResultPathNotFound.Includes(result))
			{
				return true;
			}
		    
			return false;
#endif
#else
			return System.IO.File.Exists(_path);
#endif
		}


		public virtual void DeleteSaveFile(string _path = "")
		{
			if (_path.IsNullOrEmpty())
			{
				_path = m_savePath;
			}

#if UNITY_PS4 || UNITY_SWITCH || UNITY_XBOXONE
#if UNITY_SWITCH
			nn.fs.EntryType entryType = nn.fs.EntryType.Directory;
			nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, _path);

			if (nn.fs.FileSystem.ResultPathNotFound.Includes(result))
			{
				return;
			}
#endif
#else
			if (GameDataExists())
			{
				System.IO.File.Delete(_path);
			}
#endif
		}


		public virtual void InitFileSystem()
		{

#if UNITY_PS4 || UNITY_SWITCH || UNITY_XBOXONE
			if (m_location == SaveLocation.PlayerPrefs)
				m_location = SaveLocation.Console;
#endif

#if UNITY_SWITCH && !UNITY_EDITOR
			m_fileHandle = new nn.fs.FileHandle();
			nn.account.Account.Initialize();
			nn.account.UserHandle userHandle = new nn.account.UserHandle();
			nn.account.Account.TryOpenPreselectedUser(ref userHandle);
			nn.account.Account.GetUserId(ref m_userId, userHandle);
			nn.Result result = nn.fs.SaveData.Mount(m_mountName, m_userId);
			result.abortUnlessSuccess();
			m_savePath = string.Format("{0}:/{1}", m_mountName, FullSaveName);
#elif UNITY_WEBGL
			m_location = SaveLocation.PlayerPrefs;
#endif

			m_savePath = GetSaveFile();
		}

		public virtual string GetSaveLocation(SaveLocation m_location)
		{
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
			if (m_location == SaveLocation.Console)
				m_location = SaveLocation.PersistentDatapath;
#elif UNITY_ANDROID || UNITY_IOS
			if (m_location == SaveLocation.Documents || m_location == SaveLocation.Console)
			m_location = SaveLocation.PersistentDatapath;
			
#if UNITY_IOS
			if (m_location == SaveLocation.DataPath)
			m_location = SaveLocation.PersistentDatapath;
#endif

#elif UNITY_PS4 || UNITY_SWITCH || UNITY_XBOXONE
			if (m_location != SaveLocation.Console)
			m_location = SaveLocation.Console;

#endif

			switch (m_location)
			{
			case SaveLocation.DataPath:
				string dp = Application.dataPath;
				if (System.IO.Directory.Exists(dp))
					return dp;

				VP_Debug.Log("Trying Persistent DataPath");

				return Application.persistentDataPath;
			case SaveLocation.PersistentDatapath:
				return Application.persistentDataPath;
			case SaveLocation.Documents:
				return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			case SaveLocation.Console:
#if UNITY_EDITOR
				return Application.persistentDataPath;
#else
#if UNITY_SWITCH
				return m_mountName;
#else
				return ""; // TODO ADD other Console
#endif
#endif
			case SaveLocation.PlayerPrefs:
				return m_playerPrefKey;
			case SaveLocation.Custom:
				return GetCustomLocation();
			}

			return Application.persistentDataPath;
		}


		public virtual string GetSaveLocation()
		{
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
			if (m_location == SaveLocation.Console)
				m_location = SaveLocation.PersistentDatapath;
#elif UNITY_ANDROID || UNITY_IOS
			if (m_location == SaveLocation.Documents || m_location == SaveLocation.Console)
				m_location = SaveLocation.PersistentDatapath;
			
#if UNITY_IOS
			if (m_location == SaveLocation.DataPath)
				m_location = SaveLocation.PersistentDatapath;
#endif

#elif UNITY_PS4 || UNITY_SWITCH || UNITY_XBOXONE
			if (m_location != SaveLocation.Console)
				m_location = SaveLocation.Console;

#endif

			switch (m_location)
			{
				case SaveLocation.DataPath:
					string dp = Application.dataPath;
					if (System.IO.Directory.Exists(dp))
						return dp;

					VP_Debug.Log("Trying Persistent DataPath");

					return Application.persistentDataPath;
				case SaveLocation.PersistentDatapath:
					return Application.persistentDataPath;
				case SaveLocation.Documents:
					return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
				case SaveLocation.Console:
#if UNITY_EDITOR
					return Application.persistentDataPath;
#else
#if UNITY_SWITCH
					return m_mountName;
#else
				return ""; // TODO ADD other Console
#endif
#endif
				case SaveLocation.PlayerPrefs:
					return m_playerPrefKey;
				case SaveLocation.Custom:
					return GetCustomLocation();
			}

			return Application.persistentDataPath;
		}

		public virtual string GetCustomLocation()
		{
			return "";
		}

		public virtual string GetSaveFile(string saveName)
		{
#if UNITY_SWITCH && !UNITY_EDITOR
			return string.Format("{0}:/{1}", GetSaveLocation(), saveName);
#else
			return m_location != SaveLocation.PlayerPrefs ? string.Format("{0}/{1}", GetSaveLocation(), saveName) : GetSaveLocation();
#endif
		}

		public virtual string GetSaveFile()
		{
#if UNITY_SWITCH && !UNITY_EDITOR
			return string.Format("{0}:/{1}", GetSaveLocation(), FullSaveName);
#else
			return m_location != SaveLocation.PlayerPrefs ? string.Format("{0}/{1}", GetSaveLocation(), FullSaveName) : GetSaveLocation();
#endif
		}

#if UNITY_SWITCH
		public virtual void NintendoSwitchSaveGame<T0>(T0 _save = default(T0), System.Action <bool> _saveCallback = null, string _path = "")
		{
			if (string.IsNullOrEmpty(_path))
			{
				_path = m_savePath;
			}
            
			string json = JsonUtility.ToJson(_save);
			if (string.IsNullOrEmpty(json))
			{
				if (_saveCallback != null)
				{
				_saveCallback.Invoke(false);
				}

				return;
			}
			int m_saveDataSize = System.Text.ASCIIEncoding.Unicode.GetByteCount(json) + sizeof(int);

			byte[] data;
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream(m_saveDataSize))
			{
				System.IO.BinaryWriter writer = new System.IO.BinaryWriter(stream);
				writer.Write(m_saveDataVersion);
				writer.Write(json);
				stream.Close();
				data = stream.GetBuffer();
				Debug.Assert(data.Length == m_saveDataSize);
			}

			UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();

			nn.Result result = nn.fs.File.Delete(_path);
			if (!nn.fs.FileSystem.ResultPathNotFound.Includes(result))
			{
				result.abortUnlessSuccess();
			}

			result = nn.fs.File.Create(_path, m_saveDataSize);
			result.abortUnlessSuccess();

			result = nn.fs.File.Open(ref m_fileHandle, _path, nn.fs.OpenFileMode.Write);
			result.abortUnlessSuccess();

			result = nn.fs.File.Write(m_fileHandle, 0, data, data.LongLength, nn.fs.WriteOption.Flush);
			result.abortUnlessSuccess();

			nn.fs.File.Close(m_fileHandle);

			result = nn.fs.FileSystem.Commit(m_mountName);
			result.abortUnlessSuccess();

			UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
            
			if (_saveCallback != null)
			{
				_saveCallback.Invoke(true);
			}
		}

		public virtual T0 NintendoSwitchLoadGame<T0>(out T0 _ret, System.Action<bool> _loadCallback = null, string _path = "")
		{
			if (string.IsNullOrEmpty(_path))
			{
				_path = m_savePath;
			}
            

			nn.fs.EntryType entryType = nn.fs.EntryType.Directory;
			nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, _path);

			if (nn.fs.FileSystem.ResultPathNotFound.Includes(result))
			{
				_ret = default(T0);

			if (_loadCallback != null)
			{
				_loadCallback.Invoke(false);
			}

			return _ret;
			}
			result.abortUnlessSuccess();

			result = nn.fs.File.Open(ref m_fileHandle, _path, nn.fs.OpenFileMode.Read);
			result.abortUnlessSuccess();

			long fileSize = 0;
			result = nn.fs.File.GetSize(ref fileSize, m_fileHandle);
			result.abortUnlessSuccess();

			byte[] data = new byte[fileSize];
			result = nn.fs.File.Read(m_fileHandle, 0, data, fileSize);
			result.abortUnlessSuccess();

			nn.fs.File.Close(m_fileHandle);

			using (System.IO.MemoryStream stream = new System.IO.MemoryStream(data))
			{
				System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);
				int version = reader.ReadInt32();
				Debug.Assert(version == m_saveDataVersion); // Save data version up
				string json = reader.ReadString();
				_ret = (!string.IsNullOrEmpty(json)) ? JsonUtility.FromJson<T0>(json) : default(T0);
				if (_loadCallback != null)
				{
					_loadCallback.Invoke(true);
				}
					return _ret;
			}
		}
        
		public virtual string NintendoSwitchLoadGameString(out string ret, System.Action<bool> _loadCallback = null, string _path = "")
		{
			if (string.IsNullOrEmpty(_path))
			{
				_path = m_savePath;
			}
            

			nn.fs.EntryType entryType = nn.fs.EntryType.Directory;
			nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, _path);

			if (nn.fs.FileSystem.ResultPathNotFound.Includes(result))
			{
				ResetSave();
				ret = "";

				if (_loadCallback != null)
				{
					_loadCallback.Invoke(false);
				}

				return ret;
			}
			result.abortUnlessSuccess();

			result = nn.fs.File.Open(ref m_fileHandle, _path, nn.fs.OpenFileMode.Read);
			result.abortUnlessSuccess();

			long fileSize = 0;
			result = nn.fs.File.GetSize(ref fileSize, m_fileHandle);
			result.abortUnlessSuccess();

			byte[] data = new byte[fileSize];
			result = nn.fs.File.Read(m_fileHandle, 0, data, fileSize);
			result.abortUnlessSuccess();

			nn.fs.File.Close(m_fileHandle);

			using (System.IO.MemoryStream stream = new System.IO.MemoryStream(data))
			{
				System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);
				int version = reader.ReadInt32();
				Debug.Assert(version == m_saveDataVersion); // Save data version up
				ret = reader.ReadString();
                
				if (_loadCallback != null)
				{
					_loadCallback.Invoke(ret.IsNotNullNorEmpty());
				}
                
				return ret;
			}
		}
#endif



		public virtual void SaveFileRegular<T0>(string _name, T0 _save = default(T0), System.Action<bool> _saveCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT)
		{
			if (string.IsNullOrEmpty(_name))
				_name = m_saveName;

			string _path = GetSaveFile(_name);

			T0 save = (_save != null) ? _save : default(T0);

#if UNITY_SWITCH && !UNITY_EDITOR
			NintendoSwitchSaveGame(save, _saveCallback, _path);
#else
			VP_Formatter.SaveObjectoToDataFile(save, _path, m_location == SaveLocation.PlayerPrefs, _formatter, (bool _canSave) =>
			{
				string str = _canSave ? "Saved Successfully" : "Saved aborted";
				VP_Debug.Log(str);

				if (_saveCallback != null)
					_saveCallback.Invoke(_canSave);
			}, m_convertSaveData, m_encryption);
#endif
		}

#if ODIN_INSPECTOR
		public virtual void SaveFile<T0>(string _name, T0 _save = default(T0), System.Action<bool> _saveCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT, Sirenix.Serialization.DataFormat _dataFormat = Sirenix.Serialization.DataFormat.Binary)
		{
			if (string.IsNullOrEmpty(_name))
				_name = m_saveName;

			string _path = GetSaveFile(_name);

			T0 save = (_save != null) ? _save : default(T0);

#if UNITY_SWITCH && !UNITY_EDITOR
			NintendoSwitchSaveGame(save, _saveCallback, _path);
#else
			VP_Formatter.SaveObjectoToDataFile(save, _path, m_location == SaveLocation.PlayerPrefs, _formatter, _dataFormat, (bool _canSave) =>
			{
				string str = _canSave ? "Saved Successfully" : "Saved aborted";
				VP_Debug.Log(str);

				if (_saveCallback != null)
					_saveCallback.Invoke(_canSave);
			}, m_convertSaveData, m_encryption);
#endif
		}
#endif



		public virtual void SaveFileRegularAtPath<T0>(string _path, T0 _save = default(T0), System.Action<bool> _saveCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT)
		{
			if (string.IsNullOrEmpty(_path))
				_path = GetSaveFile();

			T0 save = (_save != null) ? _save : default(T0);

#if UNITY_SWITCH && !UNITY_EDITOR
			NintendoSwitchSaveGame(save, _saveCallback, _path);
#else
			VP_Formatter.SaveObjectoToDataFile(save, _path, m_location == SaveLocation.PlayerPrefs, _formatter, (bool _canSave) =>
			{
				string str = _canSave ? "Saved Successfully" : "Saved aborted";
				VP_Debug.Log(str);

				if (_saveCallback != null)
					_saveCallback.Invoke(_canSave);
			}, m_convertSaveData, m_encryption);
#endif
		}

#if ODIN_INSPECTOR
		public virtual void SaveFileAtPath<T0>(string _path, T0 _save = default(T0), System.Action<bool> _saveCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT, Sirenix.Serialization.DataFormat _dataFormat = Sirenix.Serialization.DataFormat.Binary)
		{
			if (string.IsNullOrEmpty(_path))
				_path = GetSaveFile();

			T0 save = (_save != null) ? _save : default(T0);

#if UNITY_SWITCH && !UNITY_EDITOR
			NintendoSwitchSaveGame(save, _saveCallback, _path);
#else
			VP_Formatter.SaveObjectoToDataFile(save, _path, m_location == SaveLocation.PlayerPrefs, _formatter, _dataFormat, (bool _canSave) =>
			{
				string str = _canSave ? "Saved Successfully" : "Saved aborted";
				VP_Debug.Log(str);

				if (_saveCallback != null)
					_saveCallback.Invoke(_canSave);
			}, m_convertSaveData, m_encryption);
#endif
		}
#endif


		public virtual void LoadGame(System.Action<bool> _loadCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT)
		{

		}



#if ODIN_INSPECTOR
		public virtual T0 LoadFileFromSavePath<T0>(string _name, out T0 _data, System.Action<bool> _loadCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT, Sirenix.Serialization.DataFormat _dataFormat = Sirenix.Serialization.DataFormat.Binary)
		{
			if (string.IsNullOrEmpty(_name))
				_name = m_saveName;

			string _path = GetSaveFile(_name);
#if UNITY_SWITCH && !UNITY_EDITOR
			return NintendoSwitchLoadGame<T0>(out _data, (bool _canLoad) =>
			{
			string str = _canLoad ? "Load Successfully" : "Load aborted";
			VP_Debug.Log(str);

			if (_loadCallback != null)
			_loadCallback.Invoke(_canLoad);

			});
#else
			return VP_Formatter.LoadObjectFromDataFile<T0>(_path, m_location == SaveLocation.PlayerPrefs, out _data, _formatter, _dataFormat, (bool _canLoad) =>
			{
				string str = _canLoad ? "Load Successfully" : "Load aborted";
				VP_Debug.Log(str);

				if (_loadCallback != null)
					_loadCallback.Invoke(_canLoad);

			}, m_convertSaveData, m_encryption);
#endif
		}
#endif

		public virtual T0 LoadFileFromSavePathRegular<T0>(string _name, out T0 _data, System.Action<bool> _loadCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT)
		{
			if (string.IsNullOrEmpty(_name))
				_name = m_saveName;

			string _path = GetSaveFile(_name);
#if UNITY_SWITCH && !UNITY_EDITOR
			return NintendoSwitchLoadGame<T0>(out _data, (bool _canLoad) =>
			{
			string str = _canLoad ? "Load Successfully" : "Load aborted";
			VP_Debug.Log(str);

			if (_loadCallback != null)
			_loadCallback.Invoke(_canLoad);

			});
#else
			return VP_Formatter.LoadObjectFromDataFile<T0>(_path, m_location == SaveLocation.PlayerPrefs, out _data, _formatter, (bool _canLoad) =>
			{
				string str = _canLoad ? "Load Successfully" : "Load aborted";

				VP_Debug.Log(str);

				if (_loadCallback != null)
					_loadCallback.Invoke(_canLoad);
			}, m_convertSaveData, m_encryption);
#endif
		}



#if ODIN_INSPECTOR
		public virtual string LoadFileStringFromSavePath(string _name, out string json, System.Action<bool> _loadCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT, Sirenix.Serialization.DataFormat _dataFormat = Sirenix.Serialization.DataFormat.Binary)
		{
			if (string.IsNullOrEmpty(_name))
				_name = m_saveName;

			string _path = GetSaveFile(_name);
#if UNITY_SWITCH && !UNITY_EDITOR
			return NintendoSwitchLoadGameString(out json, (bool _canLoad) =>
			{
			string str = _canLoad ? "Load Successfully" : "Load aborted";
			VP_Debug.Log(str);
	                
			if (_loadCallback != null)
			_loadCallback.Invoke(_canLoad);
			});
#else
			return VP_Formatter.LoadJSONStringFromDataFile(_path, m_location == SaveLocation.PlayerPrefs, out json, _formatter, _dataFormat, (bool _canLoad) =>
			{
				string str = _canLoad ? "Load Successfully" : "Load aborted";
				VP_Debug.Log(str);

				if (_loadCallback != null)
					_loadCallback.Invoke(_canLoad);

			}, m_convertSaveData, m_encryption);
#endif
		}
#endif

		public virtual string LoadFileStringFromSavePathRegular(string _name, out string json, System.Action<bool> _loadCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT)
		{
			if (string.IsNullOrEmpty(_name))
				_name = m_saveName;

			string _path = GetSaveFile(_name);

#if UNITY_SWITCH && !UNITY_EDITOR
			return NintendoSwitchLoadGameString(out json,(bool _canLoad) =>
			{
			string str = _canLoad ? "Load Successfully" : "Load aborted";
			VP_Debug.Log(str);
	        	
			if (_loadCallback != null)
			_loadCallback.Invoke(_canLoad);
			});
#else
			return VP_Formatter.LoadJSONStringFromDataFile(_path, m_location == SaveLocation.PlayerPrefs, out json, _formatter, (bool _canLoad) =>
			{
				string str = _canLoad ? "Load Successfully" : "Load aborted";

				VP_Debug.Log(str);

				if (_loadCallback != null)
					_loadCallback.Invoke(_canLoad);
			}, m_encryption);
#endif
		}

#if ODIN_INSPECTOR
		public virtual T0 LoadFileFromPath<T0>(string _path, out T0 _data, System.Action<bool> _loadCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT, Sirenix.Serialization.DataFormat _dataFormat = Sirenix.Serialization.DataFormat.Binary)
		{
			if (string.IsNullOrEmpty(_path))
				_path = GetSaveFile();

#if UNITY_SWITCH && !UNITY_EDITOR
			return NintendoSwitchLoadGame<T0>(out _data, (bool _canLoad) =>
			{
			string str = _canLoad ? "Load Successfully" : "Load aborted";
			VP_Debug.Log(str);

			if (_loadCallback != null)
			_loadCallback.Invoke(_canLoad);

			});
#else
			return VP_Formatter.LoadObjectFromDataFile<T0>(_path, m_location == SaveLocation.PlayerPrefs, out _data, _formatter, _dataFormat, (bool _canLoad) =>
			{
				string str = _canLoad ? "Load Successfully" : "Load aborted";
				VP_Debug.Log(str);

				if (_loadCallback != null)
					_loadCallback.Invoke(_canLoad);

			}, m_convertSaveData, m_encryption);
#endif
		}
#endif

		public virtual T0 LoadFileFromPathRegular<T0>(string _path, out T0 _data, System.Action<bool> _loadCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT)
		{
			if (string.IsNullOrEmpty(_path))
				_path = GetSaveFile();

#if UNITY_SWITCH && !UNITY_EDITOR
			return NintendoSwitchLoadGame<T0>(out _data, (bool _canLoad) =>
			{
			string str = _canLoad ? "Load Successfully" : "Load aborted";
			VP_Debug.Log(str);

			if (_loadCallback != null)
			_loadCallback.Invoke(_canLoad);

			});
#else
			return VP_Formatter.LoadObjectFromDataFile<T0>(_path, m_location == SaveLocation.PlayerPrefs, out _data, _formatter, (bool _canLoad) =>
			{
				string str = _canLoad ? "Load Successfully" : "Load aborted";

				VP_Debug.Log(str);

				if (_loadCallback != null)
					_loadCallback.Invoke(_canLoad);
			}, m_convertSaveData, m_encryption);
#endif
		}



#if ODIN_INSPECTOR
		public virtual string LoadFileStringFromPath(string _path, out string json, System.Action<bool> _loadCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT, Sirenix.Serialization.DataFormat _dataFormat = Sirenix.Serialization.DataFormat.Binary)
		{
			if (string.IsNullOrEmpty(_path))
				_path = GetSaveFile();

#if UNITY_SWITCH && !UNITY_EDITOR
			return NintendoSwitchLoadGameString(out json, (bool _canLoad) =>
			{
			string str = _canLoad ? "Load Successfully" : "Load aborted";
			VP_Debug.Log(str);
	                
			if (_loadCallback != null)
			_loadCallback.Invoke(_canLoad);
			});
#else
			return VP_Formatter.LoadJSONStringFromDataFile(_path, m_location == SaveLocation.PlayerPrefs, out json, _formatter, _dataFormat, (bool _canLoad) =>
			{
				string str = _canLoad ? "Load Successfully" : "Load aborted";
				VP_Debug.Log(str);

				if (_loadCallback != null)
					_loadCallback.Invoke(_canLoad);

			}, m_convertSaveData, m_encryption);
#endif
		}
#endif

		public virtual string LoadFileStringFromPathRegular(string _path, out string json, System.Action<bool> _loadCallback = null, FORMATTER_METHOD _formatter = FORMATTER_METHOD.NO_CONTEXT)
		{
			if (string.IsNullOrEmpty(_path))
				_path = GetSaveFile();


#if UNITY_SWITCH && !UNITY_EDITOR
			return NintendoSwitchLoadGameString(out json,(bool _canLoad) =>
			{
			string str = _canLoad ? "Load Successfully" : "Load aborted";
			VP_Debug.Log(str);
	        	
			if (_loadCallback != null)
			_loadCallback.Invoke(_canLoad);
			});
#else
			return VP_Formatter.LoadJSONStringFromDataFile(_path, m_location == SaveLocation.PlayerPrefs, out json, _formatter, (bool _canLoad) =>
			{
				string str = _canLoad ? "Load Successfully" : "Load aborted";

				VP_Debug.Log(str);

				if (_loadCallback != null)
					_loadCallback.Invoke(_canLoad);
			}, m_encryption);
#endif
		}

		public virtual T0 LoadObjectFromJSONString<T0>(string jsonString, System.Action<bool> _loadCallback)
		{
			bool _canLoad = false;
			T0 obj = default(T0);
			try
			{
				obj = VP_Formatter.LoadObjecFromJSONString<T0>(jsonString, m_encryption);
				_canLoad = true;
			}
			catch
			{
				_canLoad = false;
			}

			string str = _canLoad ? "Load Successfully" : "Load aborted";

			VP_Debug.Log(str);

			if (_loadCallback != null)
				_loadCallback.Invoke(_canLoad);

			return obj;
		}

		public virtual void ResetSave()
		{

		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
#if !UNITY_EDITOR && UNITY_SWITCH
			nn.fs.FileSystem.Unmount(m_mountName);
#endif
		}
	}
}
