using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.Serialization;
#endif
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Reflection;

namespace VirtualPhenix.Formatter
{
    public enum FORMATTER_METHOD
    {
        STRING_REFERENCE_RESOLVER,
        INDEX_REFERENCE_RESOLVER,
        GUID_REFERENCE_RESOLVER,
        NO_CONTEXT,
        COMPLEX_FORMATTER,
        DATABASE_RESOLVER,
        ADDRESSABLE_RESOLVER,
        JSONUTILITY_RESOLVER,
        NEWTONSOFT_RESOLVER
    }

    public enum ENCRYPTION
    {
        NONE,
        DES,
        AES
    }

    public static class VP_Formatter
    {
        #region AES 256 Encryption
        public const string AES_KEY = "*#4$%^.++q~!cfr0(_!#$@$!&#&#*&@(7cy9rn8r265&$@&*E^184t44t+qfdg87f7d";

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }


        private static void EncryptAESFile(string filename, string outfilename, string psw)
        {
            string file = filename;
            string password = psw;

            byte[] bytesToBeEncrypted = File.ReadAllBytes(file); //read bytes to encrypt them 
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password); //read with UTF8 encoding the password.
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes); //hash the psw

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            string fileEncrypted = outfilename;

            File.WriteAllBytes(fileEncrypted, bytesEncrypted);
        }

        private static void DecryptAESFile(string filename, string outfilename, string psw)
        {
            string fileEncrypted = filename;
            string password = psw;

            byte[] bytesToBeDecrypted = File.ReadAllBytes(fileEncrypted);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string file = outfilename;
            File.WriteAllBytes(file, bytesDecrypted);
        }

        private static byte[] EncryptAESBytes(byte[] bytes, string psw)
        {
            string password = psw;

            byte[] bytesToBeEncrypted = bytes; //read bytes to encrypt them 
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password); //read with UTF8 encoding the password.
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes); //hash the psw

            return AES_Encrypt(bytesToBeEncrypted, passwordBytes);
        }

        private static byte[] DecryptAESBytes(byte[] bytes, string psw)
        {
            string password = psw;

            byte[] bytesToBeDecrypted = bytes;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            return AES_Decrypt(bytesToBeDecrypted, passwordBytes);
        }
        #endregion

        #region DES

        private const string DES_KEY = "*#4$%^.++q~!cfr0(_!#$@$!&#&#*&@(7cy9rn8r265&$@&*E^184t44tq2cr9o2w34f6";

        /// <summary>
        /// Binder de serialización
        /// </summary>
        public class VP_VersionDeserializationBinder : SerializationBinder
        {
            public override System.Type BindToType(string assemblyName, string typeName)
            {
                if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
                {
                    System.Type typeToDeserialize = null;

                    assemblyName = Assembly.GetExecutingAssembly().FullName;

                    // The following line of code returns the type. 
                    typeToDeserialize = System.Type.GetType(System.String.Format("{0}, {1}", typeName, assemblyName));

                    return typeToDeserialize;
                }

                return null;
            }
        }


        private static byte[] DecryptDESBytes(byte[] encryptedBytes, string key = DES_KEY)
        {
            try
            {
                Stream stream = new System.IO.MemoryStream(encryptedBytes);
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Binder = new VP_VersionDeserializationBinder();

                string strEncrypt = key;

                byte[] dv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

                byte[] byKey = Encoding.UTF8.GetBytes(strEncrypt.Substring(0, 8));

                byte[] decryptedBytes;

                DESCryptoServiceProvider des = null;

                using (des = new DESCryptoServiceProvider())
                {
                    using (Stream cryptoStream = new CryptoStream(stream, des.CreateDecryptor(byKey, dv), CryptoStreamMode.Read))
                    {
                        decryptedBytes = bformatter.Deserialize(cryptoStream) as byte[];
                    }
                }

                stream.Close();

                return decryptedBytes;
            }
            catch (System.UnauthorizedAccessException)
            {
                Debug.LogError("UnauthorizedAccessException when DES Decryption");
            }
            catch (FileNotFoundException)
            {
                Debug.Log("No save data found");
            }

            return null;
        }

        private static byte[] ReadBytesFromStream(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.Position = 0;
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private static byte[] EncryptDESBytes(byte[] decryptedBytes, string _key = DES_KEY)
        {
            try
            {
                Stream stream = new System.IO.MemoryStream(decryptedBytes);
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Binder = new VP_VersionDeserializationBinder();

                string strEncrypt = _key;

                byte[] dv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

                byte[] byKey = Encoding.UTF8.GetBytes(strEncrypt.Substring(0, 8));


                //DES ENCRYPTION
                DESCryptoServiceProvider des = null;

                using (des = new DESCryptoServiceProvider())
                {
                    using (Stream cryptoStream = new CryptoStream(stream, des.CreateEncryptor(byKey, dv), CryptoStreamMode.Write))
                    {
                        bformatter.Serialize(cryptoStream, decryptedBytes);
                    }
                }

                byte[] encryptedBytes = ReadBytesFromStream(stream);
                stream.Close();

                return encryptedBytes;

            }
            catch (System.UnauthorizedAccessException)
            {
                Debug.LogError("UnauthorizedAccessException");
            }

            return null;
        }

        #endregion


#if ODIN_INSPECTOR
        public static void SaveObjectoToDataFile<T>(T _data, string _path, bool _isPlayerPref, FORMATTER_METHOD _formatter = FORMATTER_METHOD.STRING_REFERENCE_RESOLVER, DataFormat _format = DataFormat.Binary, System.Action<bool> _callback = null, bool _convert = true, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            try
            {
                if (_isPlayerPref)
                {
                    SaveToPlayerPrefs(_data, _path, _formatter, _format, _callback, _convert);
                }
                else
                {
                    switch (_formatter)
                    {
                        case FORMATTER_METHOD.STRING_REFERENCE_RESOLVER:
	                        SaveWithStringContext(_data, _path, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.GUID_REFERENCE_RESOLVER:
                            SaveWithGUIDContext(_data, _path, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.INDEX_REFERENCE_RESOLVER:
                            List<Object> objectList = new List<Object>();
                            SaveWithIndexContext(_data, _path, out objectList, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.COMPLEX_FORMATTER:
                            VirtualPhenix.Serialization.VP_ComplexFormatter.SaveObjectoToJSONFile(_data, _path);
                            break;
                        case FORMATTER_METHOD.DATABASE_RESOLVER:
                            SaveWithDatabaseContext(_data, _path, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.ADDRESSABLE_RESOLVER:
                            SaveWithAddressableContext(_data, _path, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.JSONUTILITY_RESOLVER:
                            SaveWithJSONUtility(_data, _path, _encryption);
                            break;
                        case FORMATTER_METHOD.NO_CONTEXT:
                            SaveWithoutContext(_data, _path, _format, _encryption);
                            break;
                        default:
                            SaveWithJSONUtility(_data, _path, _encryption);
                            break;
                    }
                }
               
                if (_callback != null)
                {
                    _callback.Invoke(true);
                }
            }
	        catch (System.Exception e)
	        {
		        Debug.LogError("Saving Exception: "+e.Message+":"+e.StackTrace);
            	
                if (_callback != null)
                {
                    _callback.Invoke(false);
                }
            }
        }
#endif

        public static void SaveObjectoToDataFile<T>(T _data, string _path, bool _isPlayerPref, FORMATTER_METHOD _formatter = FORMATTER_METHOD.STRING_REFERENCE_RESOLVER, System.Action<bool> _callback = null, bool _convert = true, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            try
            {
                if (_isPlayerPref)
                {
                    SaveToPlayerPrefs(_data, _path, _formatter, _callback, _convert, _encryption);
                }
                else
                {
                    if (_formatter == FORMATTER_METHOD.COMPLEX_FORMATTER)
                    {
                        VirtualPhenix.Serialization.VP_ComplexFormatter.SaveObjectoToJSONFile(_data, _path);
                    }
                    else
                    {
	                    SaveWithJSONUtility(_data, _path, _encryption);
                    }
                }

                if (_callback != null)
                {
                    _callback.Invoke(true);
                }
            }
            catch (System.Exception e)
            {
                VP_Debug.Log("Catched exception: "+e.Message);

                if (_callback != null)
                {
                    _callback.Invoke(false);
                }
            }
        }


        public static void SaveToPlayerPrefs(string _data, string _key, System.Action<bool> _canSave)
        {
            try
            {
                if (string.IsNullOrEmpty(_key))
                    _key = "SaveGamePP";

                PlayerPrefs.SetString(_key, _data);
                PlayerPrefs.Save();

                if (_canSave != null)
                    _canSave.Invoke(true);
            }
            catch
            {
                if (_canSave != null)
                    _canSave.Invoke(false);
            }
           
        }


        public static void SaveToPlayerPrefs<T>(T _data, string _key, FORMATTER_METHOD _formatter, System.Action<bool> _canSave, bool _convert = true, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            try
            {
                string jsonString = !_convert ? _data.ToString() : _formatter == FORMATTER_METHOD.COMPLEX_FORMATTER ? jsonString = VirtualPhenix.Serialization.VP_ComplexFormatter.SerializeToJSON(_data) : JsonUtility.ToJson(_data);

                if (string.IsNullOrEmpty(_key))
                    _key = "SaveGamePP";

                PlayerPrefs.SetString(_key, jsonString);
                PlayerPrefs.Save();

                if (_canSave != null)
                    _canSave.Invoke(true);
            }
            catch
            {
                if (_canSave != null)
                    _canSave.Invoke(false);
            }
        }

#if ODIN_INSPECTOR
        public static void SaveToPlayerPrefs<T>(T _data, string _key, FORMATTER_METHOD _formatter, DataFormat _format, System.Action<bool> _canSave, bool _convert = true, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            try
            {
                string jsonString = "";
                if (_convert)
                {

                    switch (_formatter)
                    {
                        case FORMATTER_METHOD.STRING_REFERENCE_RESOLVER:
	                        var bytes = GetBytesWithStringContext(_data, _format, _encryption);
                            jsonString = Encoding.UTF8.GetString(bytes);
                            break;
                        case FORMATTER_METHOD.GUID_REFERENCE_RESOLVER:
                            bytes = GetBytesWithGUIDContext(_data, _format, _encryption);
                            jsonString = Encoding.UTF8.GetString(bytes);
                            break;
                        case FORMATTER_METHOD.INDEX_REFERENCE_RESOLVER:
                            List<Object> objectList = new List<Object>();
                            bytes = GetBytesWithIndexContext(_data, out objectList, _format, _encryption);
                            jsonString = Encoding.UTF8.GetString(bytes);
                            break;
                        case FORMATTER_METHOD.COMPLEX_FORMATTER:
                            jsonString = VirtualPhenix.Serialization.VP_ComplexFormatter.SerializeToJSON(_data);
                            break;
                        case FORMATTER_METHOD.DATABASE_RESOLVER:
                            bytes = GetBytesWithDatabaseContext(_data, _format, _encryption);
                            jsonString = Encoding.UTF8.GetString(bytes);
                            break;
                        case FORMATTER_METHOD.ADDRESSABLE_RESOLVER:
                            bytes = GetBytesWithAddressableContext(_data, _format, _encryption);
                            jsonString = Encoding.UTF8.GetString(bytes);
                            break;
                        case FORMATTER_METHOD.NO_CONTEXT:
                            bytes = GetBytesWithoutContext(_data, _format, _encryption);
                            jsonString = Encoding.UTF8.GetString(bytes);
                            break;
                        case FORMATTER_METHOD.JSONUTILITY_RESOLVER:
                            jsonString = JsonUtility.ToJson(_data);
                            break;
                        default:
                            jsonString = JsonUtility.ToJson(_data);
                            break;
                    }

                }
                else
                {
                    jsonString = _data.ToString();
                }

                if (string.IsNullOrEmpty(_key))
                    _key = "SaveGamePP";

                PlayerPrefs.SetString(_key, jsonString);
                PlayerPrefs.Save();

                if (_canSave != null)
                    _canSave.Invoke(true);
            }
            catch
            {
                if (_canSave != null)
                    _canSave.Invoke(false);
            }

      
        }
#endif
        public static void SaveWithJSONUtility<T>(T _data, string _path, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            string jsonString = JsonUtility.ToJson(_data);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = EncryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = EncryptDESBytes(bytes, DES_KEY);
            }
            File.WriteAllBytes(_path, bytes);
        }

#if ODIN_INSPECTOR
        public static void SaveWithAddressableContext<T>(T _data, string _path, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
#if USE_ADDRESSABLES
            var context = new SerializationContext()
            {
                StringReferenceResolver = new MIS_ScriptableObjectDatabaseReferenceResolver(),
            };
            byte[] bytes = SerializationUtility.SerializeValue(_data, _format, context);
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = EncryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = EncryptDESBytes(bytes, DES_KEY);
            }
            File.WriteAllBytes(_path, bytes);
#endif
        }


        public static void SaveWithDatabaseContext<T>(T _data, string _path, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            var context = new SerializationContext()
            {
                StringReferenceResolver = new VP_ScriptableObjectDatabaseReferenceResolver(),
            };
	        byte[] bytes = SerializationUtility.SerializeValue((object)_data, _format, context);
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = EncryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = EncryptDESBytes(bytes, DES_KEY);
            }
            File.WriteAllBytes(_path, bytes);
        }

        public static void SaveWithStringContext<T>(T _data, string _path, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            var context = new SerializationContext()
            {
                StringReferenceResolver = new VP_ScriptableObjectStringReferenceResolver(),
            };
            byte[] bytes = SerializationUtility.SerializeValue(_data, _format, context);
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = EncryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = EncryptDESBytes(bytes, DES_KEY);
            }
            File.WriteAllBytes(_path, bytes);
        }

        public static void SaveWithGUIDContext<T>(T _data, string _path, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            var context = new SerializationContext()
            {
                GuidReferenceResolver = new VP_ScriptableObjectGuidReferenceResolver(),
            };
            byte[] bytes = SerializationUtility.SerializeValue(_data, _format, context);
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = EncryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = EncryptDESBytes(bytes, DES_KEY);
            }
            File.WriteAllBytes(_path, bytes);
        }


        public static void SaveWithIndexContext<T>(T _data, string _path, out List<UnityEngine.Object> references, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            var resolver = new VP_IndexResolver();
            var context = new SerializationContext()
            {
                IndexReferenceResolver = resolver,
            };
            var bytes = SerializationUtility.SerializeValue(_data, _format, context);
            references = resolver.referenceList;
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = EncryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = EncryptDESBytes(bytes, DES_KEY);
            }
            File.WriteAllBytes(_path, bytes);
        }

        public static void SaveWithoutContext<T>(T _data, string _path, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            var bytes = SerializationUtility.SerializeValue(_data, _format);
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = EncryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = EncryptDESBytes(bytes, DES_KEY);
            }
            File.WriteAllBytes(_path, bytes);
        }

        public static byte[] GetBytesWithAddressableContext<T>(T _data, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
#if USE_ADDRESSABLES
            var context = new SerializationContext()
            {
                StringReferenceResolver = new MIS_ScriptableObjectDatabaseReferenceResolver(),
            };
            return SerializationUtility.SerializeValue(_data, _format, context);
#endif

            return new byte[0];
        }


        public static byte[] GetBytesWithDatabaseContext<T>(T _data, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            var context = new SerializationContext()
            {
                StringReferenceResolver = new VP_ScriptableObjectDatabaseReferenceResolver(),
            };
            byte[] bytes = SerializationUtility.SerializeValue(_data, _format, context);
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = EncryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = EncryptDESBytes(bytes, DES_KEY);
            }
            return bytes;
        }

        public static byte[] GetBytesWithStringContext<T>(T _data, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            var context = new SerializationContext()
            {
                StringReferenceResolver = new VP_ScriptableObjectStringReferenceResolver(),
            };
            byte[] bytes = SerializationUtility.SerializeValue(_data, _format, context);
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = EncryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = EncryptDESBytes(bytes, DES_KEY);
            }
            return bytes;
        }

        public static byte[] GetBytesWithGUIDContext<T>(T _data, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            var context = new SerializationContext()
            {
                GuidReferenceResolver = new VP_ScriptableObjectGuidReferenceResolver(),
            };

            byte[] bytes = SerializationUtility.SerializeValue(_data, _format, context);
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = EncryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = EncryptDESBytes(bytes, DES_KEY);
            }
            return bytes;
        }


        public static byte[] GetBytesWithIndexContext<T>(T _data, out List<UnityEngine.Object> references, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            var resolver = new VP_IndexResolver();
            var context = new SerializationContext()
            {
                IndexReferenceResolver = resolver,
            };
            var bytes = SerializationUtility.SerializeValue(_data, _format, context);
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = EncryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = EncryptDESBytes(bytes, DES_KEY);
            }
            references = resolver.referenceList;
            return bytes;
        }

        public static byte[] GetBytesWithoutContext<T>(T _data, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            byte[] bytes = SerializationUtility.SerializeValue(_data, _format);
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = EncryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = EncryptDESBytes(bytes, DES_KEY);
            }
            return bytes;
        }

        public static string LoadJSONStringFromDataFile(string _path, bool _isPlayerPref, out string str, FORMATTER_METHOD _formatter = FORMATTER_METHOD.COMPLEX_FORMATTER, DataFormat _format = DataFormat.Binary, System.Action<bool> _callback = null, bool _convert = true, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            str = "";
            if (!_isPlayerPref)
            {
                byte[] bytes = File.ReadAllBytes(_path);
              
                try
                {

                    switch (_formatter)
                    {
                        case FORMATTER_METHOD.STRING_REFERENCE_RESOLVER:
                            str = LoadWithStringContext<string>(bytes, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.GUID_REFERENCE_RESOLVER:
                            str = LoadWithGUIDContext<string>(bytes, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.INDEX_REFERENCE_RESOLVER:
                            str = LoadWithIndexContext<string>(bytes, new List<Object>(), _format, _encryption);
                            break;
                        case FORMATTER_METHOD.COMPLEX_FORMATTER:
                            str = VirtualPhenix.Serialization.VP_ComplexFormatter.LoadJSONStringFromJSONFile(_path, Encoding.UTF8);
                            break;
                        case FORMATTER_METHOD.DATABASE_RESOLVER:
                            str = LoadWithDataBaseContext<string>(bytes, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.ADDRESSABLE_RESOLVER:
                            str = LoadWithAddressableContext<string>(bytes, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.NO_CONTEXT:
                            str = LoadWithNoContext<string>(bytes, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.JSONUTILITY_RESOLVER:
	                        str = LoadWithJSONUtility(bytes, _encryption);
                            break;
                        default:
                            str = LoadWithJSONUtility(bytes, _encryption);
                            break;
                    }
                    if (_callback != null)
                        _callback.Invoke(true);
                    return str;
                }
                catch
                {
                    if (_callback != null)
                        _callback.Invoke(false);
                    return "";
                }
            }
            else
            {
                string jsonString = PlayerPrefs.GetString(_path, "");
                if (string.IsNullOrEmpty(jsonString))
                {
                    if (_callback != null)
                        _callback.Invoke(false);
                    return "";
                }
                else
                {
                    if (_callback != null)
                        _callback.Invoke(true);
                    return jsonString;
                }
            }

        }
#endif

        public static string LoadJSONStringFromDataFile(string _path, bool _isPlayerPref, out string str, FORMATTER_METHOD _formatter = FORMATTER_METHOD.COMPLEX_FORMATTER, System.Action<bool> _callback = null, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            str = "";
            if (!_isPlayerPref)
            {
	            byte[] bytes = File.ReadAllBytes(_path);
                
                try
                {
	                str = LoadWithJSONUtility(bytes, _encryption);

                    if (_callback != null)
                        _callback.Invoke(true);

                    return str;
                }
                catch
                {
                    if (_callback != null)
                        _callback.Invoke(false);
                    return "";
                }
            }
            else
            {
                string jsonString = PlayerPrefs.GetString(_path, "");
                if (string.IsNullOrEmpty(jsonString))
                {
                    if (_callback != null)
                        _callback.Invoke(false);
                    return "";
                }
                else
                {
                    if (_callback != null)
                        _callback.Invoke(true);
                    return jsonString;
                }
            }
        }

        public static string LoadWithJSONUtility(byte[] bytes, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = DecryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = DecryptDESBytes(bytes, DES_KEY);
            }
            return Encoding.UTF8.GetString(bytes);
        }

        public static T LoadObjectWithJSONUtility<T>(string _path, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            byte[] bytes = File.ReadAllBytes(_path);
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = DecryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = DecryptDESBytes(bytes, DES_KEY);
            }
            string jsonString = Encoding.UTF8.GetString(bytes);
            return JsonUtility.FromJson<T>(jsonString);
        }

        public static T LoadObjecFromJSONString<T>(string _jsonString, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            return JsonUtility.FromJson<T>(_jsonString);
        }

#if ODIN_INSPECTOR
        public static T LoadObjectFromDataFile<T>(string _path, bool _isPlayerPref, out T obj, FORMATTER_METHOD _formatter = FORMATTER_METHOD.STRING_REFERENCE_RESOLVER, DataFormat _format = DataFormat.Binary, System.Action<bool> _callback = null, bool _convert = true, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            obj = default(T);

#if !UNITY_SWITCH && !UNITY_PS4 && !UNITY_XBOXONE
            if (!_isPlayerPref)
            {
                if (!File.Exists(_path))
                {
                	Debug.LogError("File not exist at "+_path);
                	
                    if (_callback != null)
                        _callback.Invoke(false);

                    return obj;
                }
            }
#endif
            if (!_isPlayerPref)
            {
                byte[] bytes = File.ReadAllBytes(_path);

                try
                {
                    switch (_formatter)
                    {
                        case FORMATTER_METHOD.STRING_REFERENCE_RESOLVER:
                            obj = LoadWithStringContext<T>(bytes, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.GUID_REFERENCE_RESOLVER:
                            obj = LoadWithGUIDContext<T>(bytes, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.INDEX_REFERENCE_RESOLVER:
                            obj = LoadWithIndexContext<T>(bytes, new List<Object>(), _format, _encryption);
                            break;
                        case FORMATTER_METHOD.COMPLEX_FORMATTER:
	                        obj = VirtualPhenix.Serialization.VP_ComplexFormatter.LoadObjectFromJSONFile<T>(_path);
                            break;
                        case FORMATTER_METHOD.DATABASE_RESOLVER:
                            obj = LoadWithDataBaseContext<T>(bytes, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.ADDRESSABLE_RESOLVER:
                            obj = LoadWithAddressableContext<T>(bytes, _format, _encryption);
                            break;
                        case FORMATTER_METHOD.JSONUTILITY_RESOLVER:
                            obj = LoadObjectWithJSONUtility<T>(_path, _encryption);
                            break;
                        case FORMATTER_METHOD.NO_CONTEXT:
                            obj = LoadWithNoContext<T>(bytes, _format, _encryption);
                            break;
                        default:
                            obj = LoadObjectWithJSONUtility<T>(_path, _encryption);
                            break;
                    }

                    if (_callback != null)
                        _callback.Invoke(true);

                    return obj;
                }
	                catch (System.Exception e)
	                {
                	
		                Debug.LogError("Catched exception when loading: "+e.Message+":"+e.StackTrace);
                    if (_callback != null)
                        _callback.Invoke(false);

                    return obj;
                }
            }
            else
            {
                return LoadFromPlayerPrefs<T>(_path, out obj, _callback, _formatter, _format, _convert, _encryption);
            }
        }
#endif

        public static T LoadObjectFromDataFile<T>(string _path, bool _isPlayerPref, out T obj, FORMATTER_METHOD _formatter = FORMATTER_METHOD.STRING_REFERENCE_RESOLVER, System.Action<bool> _callback = null, bool _convert = true, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            obj = default(T);

            if (!_isPlayerPref)
            {
                if (!File.Exists(_path))
                {
                    if (_callback != null)
                        _callback.Invoke(false);

                    return obj;
                }

                try
                {
                    if (_formatter == FORMATTER_METHOD.COMPLEX_FORMATTER)
                    {
                        obj = VirtualPhenix.Serialization.VP_ComplexFormatter.LoadObjectFromJSONFile<T>(_path);
                    }
                    else
                    {
	                    obj = LoadObjectWithJSONUtility<T>(_path, _encryption);
                    }

                    if (_callback != null)
                        _callback.Invoke(obj != null);

                    return obj;
                }
                catch (System.Exception e)
                {
                	Debug.LogError("Catched error loading: "+e.Message+":"+e.StackTrace);
                	
                    if (_callback != null)
                        _callback.Invoke(false);

                    return obj;
                }
            }
            else
            {
                return LoadFromPlayerPrefs<T>(_path, out obj, _callback, _formatter, _convert);
            }
        }


        public static T LoadFromPlayerPrefs<T>(string _key, out T obj, System.Action<bool> _callback, FORMATTER_METHOD _formatter, bool _convert = true, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            obj = default(T);

            if (!string.IsNullOrEmpty(_key))
            {
                string jsonString = PlayerPrefs.GetString(_key, "");
                if (string.IsNullOrEmpty(jsonString))
                {
                    if (_callback != null)
                        _callback.Invoke(false);

                    return obj;
                }
                else
                {
                    try
                    {
                        obj = _formatter == FORMATTER_METHOD.COMPLEX_FORMATTER ? Serialization.VP_ComplexFormatter.DeserializeFromJSON<T>(jsonString) : JsonUtility.FromJson<T>(jsonString);
                        
                        if (_callback != null)
                            _callback.Invoke(true);

                        return obj;
                    }
                    catch
                    {
                        obj = default(T);

                        if (_callback != null)
                            _callback.Invoke(false);

                        return obj;
                    }
                   
                }
            }
            else
            {
                if (_callback != null)
                    _callback.Invoke(false);

                return obj;
            }
        }


        public static string LoadFromPlayerPrefs(string _key, out string obj, System.Action<bool> _callback, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            obj = "";
            if (!string.IsNullOrEmpty(_key))
            {
                string jsonString = PlayerPrefs.GetString(_key, "");
                if (string.IsNullOrEmpty(jsonString))
                {
                    if (_callback != null)
                        _callback.Invoke(false);

                    return obj;
                }
                else
                {
                    try
                    {
                        obj = jsonString;

                        if (_callback != null)
                            _callback.Invoke(true);

                       
                        return obj;

                    }
                    catch
                    {
                        if (_callback != null)
                            _callback.Invoke(false);

                        return obj;
                    }
                }
            }
            else
            {
                if (_callback != null)
                    _callback.Invoke(false);

                return obj;
            }
        }


#if ODIN_INSPECTOR
        public static T LoadFromPlayerPrefs<T>(string _key, out T obj, System.Action<bool> _callback, FORMATTER_METHOD _formatter, DataFormat _format, bool _convert = true, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            obj = default(T);
            if (!string.IsNullOrEmpty(_key))
            {
                string jsonString = PlayerPrefs.GetString(_key, "");
                if (string.IsNullOrEmpty(jsonString))
                {
                    if (_callback != null)
                        _callback.Invoke(false);

                    return default(T);
                }
                else
                {
                    try
                    {

                        if (_convert)
                        {
                            switch (_formatter)
                            {
                                case FORMATTER_METHOD.STRING_REFERENCE_RESOLVER:
                                    obj = LoadWithStringContext<T>(Encoding.UTF8.GetBytes(jsonString), _format);
                                	break;
                                case FORMATTER_METHOD.GUID_REFERENCE_RESOLVER:
                                    obj = LoadWithGUIDContext<T>(Encoding.UTF8.GetBytes(jsonString), _format);
                                	break;
                                case FORMATTER_METHOD.INDEX_REFERENCE_RESOLVER:
                                    obj = LoadWithIndexContext<T>(Encoding.UTF8.GetBytes(jsonString), new List<Object>(), _format);
                                	break;
                                case FORMATTER_METHOD.COMPLEX_FORMATTER:
                                    obj = Serialization.VP_ComplexFormatter.DeserializeFromJSON<T>(jsonString);
                                	break;
                                case FORMATTER_METHOD.DATABASE_RESOLVER:
                                    obj = LoadWithDataBaseContext<T>(Encoding.UTF8.GetBytes(jsonString), _format);
                                	break;
                                case FORMATTER_METHOD.ADDRESSABLE_RESOLVER:
                                    obj = LoadWithAddressableContext<T>(Encoding.UTF8.GetBytes(jsonString), _format);
                                	break;
                                case FORMATTER_METHOD.NO_CONTEXT:
                                    obj = LoadWithNoContext<T>(Encoding.UTF8.GetBytes(jsonString), _format);
                                	break;
                                case FORMATTER_METHOD.JSONUTILITY_RESOLVER:
                                    obj = JsonUtility.FromJson<T>(jsonString);
                                	break;
                                default:
	                                obj = JsonUtility.FromJson<T>(jsonString);
	                                break;
                            }
                        }
                        else
                        {
                            obj = default(T);
                        }

                        if (_callback != null)
                            _callback.Invoke(true);

                        return obj;
                    }
                    catch
                    {
                        obj = default(T);

                        if (_callback != null)
                            _callback.Invoke(false);

                        return obj;
                    }                   
                }
            }
            else
            {
                obj = default(T);

                if (_callback != null)
                    _callback.Invoke(false);

                return obj;
            }
        }

        public static T LoadWithGUIDContext<T>(byte[] bytes, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            var context = new DeserializationContext()
            {
                GuidReferenceResolver = new VP_ScriptableObjectGuidReferenceResolver(),
            };
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = DecryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = DecryptDESBytes(bytes, DES_KEY);
            }
            return SerializationUtility.DeserializeValue<T>(bytes, _format, context);
        }

        public static T LoadWithIndexContext<T>(byte[] bytes, List<UnityEngine.Object> references, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            var context = new DeserializationContext()
            {
                IndexReferenceResolver = new VP_IndexResolver(references),
            };
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = DecryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = DecryptDESBytes(bytes, DES_KEY);
            }
            return SerializationUtility.DeserializeValue<T>(bytes, _format, context);
        }

        public static T LoadWithStringContext<T>(byte[] bytes, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {

            var context = new DeserializationContext()
            {
                StringReferenceResolver = new VP_ScriptableObjectStringReferenceResolver(),
            };
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = DecryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = DecryptDESBytes(bytes, DES_KEY);
            }
            return SerializationUtility.DeserializeValue<T>(bytes, _format, context);
        }

        public static T LoadWithDataBaseContext<T>(byte[] bytes, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = DecryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = DecryptDESBytes(bytes, DES_KEY);
            }
            
            
	        var context = new DeserializationContext()
	        {
		        StringReferenceResolver = new VP_ScriptableObjectDatabaseReferenceResolver(),
            };
            
	        return (T)SerializationUtility.DeserializeValue<object>(bytes, _format, context);
        }

        public static T LoadWithAddressableContext<T>(byte[] bytes, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
#if USE_ADDRESSABLES
            var context = new DeserializationContext()
            {
                StringReferenceResolver = new VP_ScriptableObjectDatabaseReferenceResolver(),
            };

            return SerializationUtility.DeserializeValue<T>(bytes, _format, context);
#else
            return default(T);
#endif
        }

        public static T LoadWithNoContext<T>(byte[] bytes, DataFormat _format = DataFormat.Binary, ENCRYPTION _encryption = ENCRYPTION.NONE)
        {
            if (_encryption == ENCRYPTION.AES)
            {
                bytes = DecryptAESBytes(bytes, AES_KEY);
            }
            else if (_encryption == ENCRYPTION.DES)
            {
                bytes = DecryptDESBytes(bytes, DES_KEY);
            }
            return SerializationUtility.DeserializeValue<T>(bytes, _format);
        }

#endif

    }
}
