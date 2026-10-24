using System;
using System.Buffers;
using System.IO;
using System.Text;
using GameCore;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using UnityEngine;

namespace Cryptography
{
	// Token: 0x0200099C RID: 2460
	public static class ECDSA
	{
		// Token: 0x060037AF RID: 14255 RVA: 0x000B485C File Offset: 0x000B2A5C
		public static AsymmetricCipherKeyPair GenerateKeys(int size = 384)
		{
			ECKeyPairGenerator eckeyPairGenerator = new ECKeyPairGenerator("ECDSA");
			KeyGenerationParameters parameters = new KeyGenerationParameters(new SecureRandom(), size);
			eckeyPairGenerator.Init(parameters);
			return eckeyPairGenerator.GenerateKeyPair();
		}

		// Token: 0x060037B0 RID: 14256 RVA: 0x000B488B File Offset: 0x000B2A8B
		public static string Sign(string data, AsymmetricKeyParameter privKey)
		{
			return Convert.ToBase64String(ECDSA.SignBytes(data, privKey));
		}

		// Token: 0x060037B1 RID: 14257 RVA: 0x000B489C File Offset: 0x000B2A9C
		public static byte[] SignBytes(string data, AsymmetricKeyParameter privKey)
		{
			byte[] result;
			try
			{
				byte[] array = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(data.Length));
				int bytes = Utf8.GetBytes(data, array);
				byte[] array2 = ECDSA.SignBytes(array, 0, bytes, privKey);
				ArrayPool<byte>.Shared.Return(array, false);
				result = array2;
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060037B2 RID: 14258 RVA: 0x000B48FC File Offset: 0x000B2AFC
		public static byte[] SignBytes(byte[] data, AsymmetricKeyParameter privKey)
		{
			return ECDSA.SignBytes(data, 0, data.Length, privKey);
		}

		// Token: 0x060037B3 RID: 14259 RVA: 0x000B490C File Offset: 0x000B2B0C
		public static byte[] SignBytes(byte[] data, int offset, int count, AsymmetricKeyParameter privKey)
		{
			byte[] result;
			try
			{
				ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
				signer.Init(true, privKey);
				signer.BlockUpdate(data, offset, count);
				result = signer.GenerateSignature();
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060037B4 RID: 14260 RVA: 0x000B4954 File Offset: 0x000B2B54
		public static bool Verify(string data, string signature, AsymmetricKeyParameter pubKey)
		{
			return ECDSA.VerifyBytes(data, Convert.FromBase64String(signature), pubKey);
		}

		// Token: 0x060037B5 RID: 14261 RVA: 0x000B4964 File Offset: 0x000B2B64
		public static bool VerifyBytes(string data, byte[] signature, AsymmetricKeyParameter pubKey)
		{
			bool result;
			try
			{
				ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
				signer.Init(false, pubKey);
				byte[] array = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(data.Length));
				int bytes = Utf8.GetBytes(data, array);
				signer.BlockUpdate(array, 0, bytes);
				ArrayPool<byte>.Shared.Return(array, false);
				result = signer.VerifySignature(signature);
			}
			catch (Exception ex)
			{
				GameCore.Console.AddLog("ECDSA Verification Error (BouncyCastle): " + ex.Message + ", " + ex.StackTrace, Color.red, false, ConsoleLogType.Log);
				result = false;
			}
			return result;
		}

		// Token: 0x060037B6 RID: 14262 RVA: 0x000B4A04 File Offset: 0x000B2C04
		public static AsymmetricKeyParameter PublicKeyFromString(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			AsymmetricKeyParameter result;
			using (TextReader textReader = new StringReader(key))
			{
				result = (AsymmetricKeyParameter)new PemReader(textReader).ReadObject();
			}
			return result;
		}

		// Token: 0x060037B7 RID: 14263 RVA: 0x000B4A50 File Offset: 0x000B2C50
		public static AsymmetricKeyParameter PrivateKeyFromString(string key)
		{
			AsymmetricKeyParameter @private;
			using (TextReader textReader = new StringReader(key))
			{
				@private = ((AsymmetricCipherKeyPair)new PemReader(textReader).ReadObject()).Private;
			}
			return @private;
		}

		// Token: 0x060037B8 RID: 14264 RVA: 0x000B4A98 File Offset: 0x000B2C98
		public static string KeyToString(AsymmetricKeyParameter key)
		{
			string result;
			using (TextWriter textWriter = new StringWriter())
			{
				PemWriter pemWriter = new PemWriter(textWriter);
				pemWriter.WriteObject(key);
				pemWriter.Writer.Flush();
				result = textWriter.ToString();
			}
			return result;
		}
	}
}
