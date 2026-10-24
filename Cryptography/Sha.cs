using System;
using System.Buffers;
using System.Security.Cryptography;
using System.Text;
using NorthwoodLib.Pools;

namespace Cryptography
{
	// Token: 0x020009A0 RID: 2464
	public static class Sha
	{
		// Token: 0x060037BF RID: 14271 RVA: 0x000B4C94 File Offset: 0x000B2E94
		public static byte[] Sha1(byte[] message)
		{
			byte[] result;
			using (SHA1 sha = SHA1.Create())
			{
				result = sha.ComputeHash(message);
			}
			return result;
		}

		// Token: 0x060037C0 RID: 14272 RVA: 0x000B4CCC File Offset: 0x000B2ECC
		public static byte[] Sha1(byte[] message, int offset, int length)
		{
			byte[] result;
			using (SHA1 sha = SHA1.Create())
			{
				result = sha.ComputeHash(message, offset, length);
			}
			return result;
		}

		// Token: 0x060037C1 RID: 14273 RVA: 0x000B4D08 File Offset: 0x000B2F08
		public static byte[] Sha1(string message)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(message.Length));
			int bytes = Utf8.GetBytes(message, array);
			byte[] result = Sha.Sha1(array, 0, bytes);
			ArrayPool<byte>.Shared.Return(array, false);
			return result;
		}

		// Token: 0x060037C2 RID: 14274 RVA: 0x000B4D4C File Offset: 0x000B2F4C
		public static byte[] Sha256(byte[] message)
		{
			byte[] result;
			using (SHA256 sha = SHA256.Create())
			{
				result = sha.ComputeHash(message);
			}
			return result;
		}

		// Token: 0x060037C3 RID: 14275 RVA: 0x000B4D84 File Offset: 0x000B2F84
		public static byte[] Sha256(byte[] message, int offset, int length)
		{
			byte[] result;
			using (SHA256 sha = SHA256.Create())
			{
				result = sha.ComputeHash(message, offset, length);
			}
			return result;
		}

		// Token: 0x060037C4 RID: 14276 RVA: 0x000B4DC0 File Offset: 0x000B2FC0
		public static byte[] Sha256(string message)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(message.Length));
			int bytes = Utf8.GetBytes(message, array);
			byte[] result = Sha.Sha256(array, 0, bytes);
			ArrayPool<byte>.Shared.Return(array, false);
			return result;
		}

		// Token: 0x060037C5 RID: 14277 RVA: 0x000B4E04 File Offset: 0x000B3004
		public static byte[] Sha256Hmac(byte[] key, byte[] message)
		{
			byte[] result;
			using (HMACSHA256 hmacsha = new HMACSHA256(key))
			{
				result = hmacsha.ComputeHash(message);
			}
			return result;
		}

		// Token: 0x060037C6 RID: 14278 RVA: 0x000B4E40 File Offset: 0x000B3040
		public static byte[] Sha512(string message)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(message.Length));
			int bytes = Utf8.GetBytes(message, array);
			byte[] result = Sha.Sha512(array, 0, bytes);
			ArrayPool<byte>.Shared.Return(array, false);
			return result;
		}

		// Token: 0x060037C7 RID: 14279 RVA: 0x000B4E84 File Offset: 0x000B3084
		public static byte[] Sha512(byte[] message)
		{
			byte[] result;
			using (SHA512 sha = SHA512.Create())
			{
				result = sha.ComputeHash(message);
			}
			return result;
		}

		// Token: 0x060037C8 RID: 14280 RVA: 0x000B4EBC File Offset: 0x000B30BC
		public static byte[] Sha512(byte[] message, int offset, int length)
		{
			byte[] result;
			using (SHA512 sha = SHA512.Create())
			{
				result = sha.ComputeHash(message, offset, length);
			}
			return result;
		}

		// Token: 0x060037C9 RID: 14281 RVA: 0x000B4EF8 File Offset: 0x000B30F8
		public static byte[] Sha512Hmac(byte[] key, byte[] message)
		{
			byte[] result;
			using (HMACSHA512 hmacsha = new HMACSHA512(key))
			{
				result = hmacsha.ComputeHash(message);
			}
			return result;
		}

		// Token: 0x060037CA RID: 14282 RVA: 0x000B4F34 File Offset: 0x000B3134
		public static byte[] Sha512Hmac(byte[] key, string data)
		{
			byte[] array = null;
			byte[] result;
			try
			{
				array = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetByteCount(data));
				int bytes = Utf8.GetBytes(data, array);
				result = Sha.Sha512Hmac(key, 0, bytes, array);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(array, false);
			}
			return result;
		}

		// Token: 0x060037CB RID: 14283 RVA: 0x000B4F8C File Offset: 0x000B318C
		public static byte[] Sha512Hmac(byte[] key, int offset, int length, byte[] message)
		{
			byte[] result;
			using (HMACSHA512 hmacsha = new HMACSHA512(key))
			{
				result = hmacsha.ComputeHash(message, offset, length);
			}
			return result;
		}

		// Token: 0x060037CC RID: 14284 RVA: 0x000B4FC8 File Offset: 0x000B31C8
		public static string HashToString(byte[] hash)
		{
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			foreach (byte b in hash)
			{
				stringBuilder.Append(b.ToString("X2"));
			}
			string result = stringBuilder.ToString();
			StringBuilderPool.Shared.Return(stringBuilder);
			return result;
		}
	}
}
