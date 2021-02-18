namespace VirtualPhenix.Encryption
{
	using System;
	using System.Security.Cryptography;
	using System.Text;


	// AES 128 Approach
	public static partial class VP_AESEncryption
	{
		public const string KEY = "GYtv6B]ey74htr6jfl;yefsrhetrhj74htr6jfl;";

		/// <summary>
		/// Encrypt the specified toEncrypt.
		/// </summary>
		/// <param name="toEncrypt">To encrypt.</param>
		public static string Encrypt(string toEncrypt, string key="")
		{
			if (key.IsNullOrEmpty())
				key = KEY;

			byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
			// 256-AES key
			byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
			RijndaelManaged rDel = new RijndaelManaged();
			rDel.Key = keyArray;
			rDel.Mode = CipherMode.ECB;
			// http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
			rDel.Padding = PaddingMode.PKCS7;
			// better lang support
			ICryptoTransform cTransform = rDel.CreateEncryptor();
			byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
			return Convert.ToBase64String(resultArray, 0, resultArray.Length);
		}

		/// <summary>
		/// Decrypt the specified toDecrypt.
		/// </summary>
		/// <param name="toDecrypt">To decrypt.</param>
		public static string Decrypt(string toDecrypt, string key = "")
		{
			if (key.IsNullOrEmpty())
				key = KEY;

			byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
			// AES-256 key
			byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
			RijndaelManaged rDel = new RijndaelManaged();
			rDel.Key = keyArray;
			rDel.Mode = CipherMode.ECB;
			// http://msdn.microsoft.com/en-us/library/system.security.cryptography.ciphermode.aspx
			rDel.Padding = PaddingMode.PKCS7;
			// better lang support
			ICryptoTransform cTransform = rDel.CreateDecryptor();
			byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
			return UTF8Encoding.UTF8.GetString(resultArray);
		}
	}
}