using System;
using System.Buffers;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Cryptography
{
	// Token: 0x0200099A RID: 2458
	public static class AES
	{
		// Token: 0x060037A4 RID: 14244 RVA: 0x000B4569 File Offset: 0x000B2769
		public static void GenerateNonce(byte[] buffer, SecureRandom secureRandom)
		{
			secureRandom.NextBytes(buffer, 0, 32);
		}

		// Token: 0x060037A5 RID: 14245 RVA: 0x000B4575 File Offset: 0x000B2775
		public static void ReadNonce(byte[] buffer, byte[] cipherText, int dataOffset = 0)
		{
			Array.Copy(cipherText, dataOffset, buffer, 0, 32);
		}

		// Token: 0x060037A6 RID: 14246 RVA: 0x000B4584 File Offset: 0x000B2784
		public static GcmBlockCipher AesGcmEncryptInit(int dataLength, byte[] secret, byte[] nonce, out int outputSize)
		{
			GcmBlockCipher gcmBlockCipher = new GcmBlockCipher(new AesEngine());
			gcmBlockCipher.Init(true, new AeadParameters(new KeyParameter(secret), 128, nonce));
			outputSize = gcmBlockCipher.GetOutputSize(dataLength) + 32;
			return gcmBlockCipher;
		}

		// Token: 0x060037A7 RID: 14247 RVA: 0x000B45C4 File Offset: 0x000B27C4
		public static void AesGcmEncrypt(GcmBlockCipher cipher, byte[] nonce, byte[] data, int dataOffset, int dataLength, byte[] cipherText, int cipherTextOffset)
		{
			int num = cipher.ProcessBytes(data, dataOffset, dataLength, cipherText, cipherTextOffset + 32);
			cipher.DoFinal(cipherText, num + cipherTextOffset + 32);
			Array.Copy(nonce, 0, cipherText, cipherTextOffset, 32);
		}

		// Token: 0x060037A8 RID: 14248 RVA: 0x000B4600 File Offset: 0x000B2800
		public static byte[] AesGcmEncrypt(byte[] data, byte[] secret, SecureRandom secureRandom, int dataOffset = 0, int dataLength = 0)
		{
			if (dataLength == 0)
			{
				dataLength = data.Length;
			}
			byte[] array = ArrayPool<byte>.Shared.Rent(32);
			byte[] result;
			try
			{
				secureRandom.NextBytes(array, 0, array.Length);
				GcmBlockCipher gcmBlockCipher = new GcmBlockCipher(new AesEngine());
				gcmBlockCipher.Init(true, new AeadParameters(new KeyParameter(secret), 128, array));
				byte[] array2 = new byte[gcmBlockCipher.GetOutputSize(data.Length) + 32];
				int num = gcmBlockCipher.ProcessBytes(data, dataOffset, dataLength, array2, 32);
				gcmBlockCipher.DoFinal(array2, num + 32);
				Array.Copy(array, array2, 32);
				result = array2;
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(array, false);
			}
			return result;
		}

		// Token: 0x060037A9 RID: 14249 RVA: 0x000B46A8 File Offset: 0x000B28A8
		public static GcmBlockCipher AesGcmDecryptInit(byte[] nonce, byte[] secret, int cipherTextLength, out int outputSize)
		{
			GcmBlockCipher gcmBlockCipher = new GcmBlockCipher(new AesEngine());
			gcmBlockCipher.Init(false, new AeadParameters(new KeyParameter(secret), 128, nonce));
			outputSize = gcmBlockCipher.GetOutputSize(cipherTextLength - 32);
			return gcmBlockCipher;
		}

		// Token: 0x060037AA RID: 14250 RVA: 0x000B46E8 File Offset: 0x000B28E8
		public static void AesGcmDecrypt(GcmBlockCipher cipher, byte[] cipherText, byte[] plainText, int cipherTextOffset = 0, int cipherTextLength = 0, int plainTextOffset = 0)
		{
			if (cipherTextLength == 0)
			{
				cipherTextLength = cipherText.Length;
			}
			int num = cipher.ProcessBytes(cipherText, cipherTextOffset + 32, cipherTextLength - 32, plainText, plainTextOffset);
			cipher.DoFinal(plainText, plainTextOffset + num);
		}

		// Token: 0x060037AB RID: 14251 RVA: 0x000B4720 File Offset: 0x000B2920
		public static byte[] AesGcmDecrypt(byte[] data, byte[] secret, int dataOffset = 0, int dataLength = 0)
		{
			if (dataLength <= 0)
			{
				dataLength = data.Length;
			}
			if (dataLength < 32)
			{
				throw new ArgumentException("Data length can't be smaller than nonce size.", "dataLength");
			}
			byte[] array = ArrayPool<byte>.Shared.Rent(32);
			byte[] result;
			try
			{
				Array.Copy(data, dataOffset, array, 0, 32);
				GcmBlockCipher gcmBlockCipher = new GcmBlockCipher(new AesEngine());
				gcmBlockCipher.Init(false, new AeadParameters(new KeyParameter(secret), 128, array));
				int len = dataLength - 32;
				byte[] array2 = new byte[gcmBlockCipher.GetOutputSize(len)];
				int outOff = gcmBlockCipher.ProcessBytes(data, dataOffset + 32, len, array2, 0);
				gcmBlockCipher.DoFinal(array2, outOff);
				result = array2;
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(array, false);
			}
			return result;
		}

		// Token: 0x040029B8 RID: 10680
		public const int NonceSizeBytes = 32;

		// Token: 0x040029B9 RID: 10681
		private const int MacSizeBits = 128;
	}
}
