using System;
using System.Buffers;
using System.Security.Cryptography;
using System.Text;

namespace Cryptography
{
	// Token: 0x0200099D RID: 2461
	public static class Md
	{
		// Token: 0x060037B9 RID: 14265 RVA: 0x000B4AE8 File Offset: 0x000B2CE8
		public static byte[] Md5(byte[] message)
		{
			byte[] result;
			using (MD5 md = MD5.Create())
			{
				result = md.ComputeHash(message);
			}
			return result;
		}

		// Token: 0x060037BA RID: 14266 RVA: 0x000B4B20 File Offset: 0x000B2D20
		public static byte[] Md5(byte[] message, int offset, int length)
		{
			byte[] result;
			using (MD5 md = MD5.Create())
			{
				result = md.ComputeHash(message, offset, length);
			}
			return result;
		}

		// Token: 0x060037BB RID: 14267 RVA: 0x000B4B5C File Offset: 0x000B2D5C
		public static byte[] Md5(string message)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(message.Length));
			int bytes = Utf8.GetBytes(message, array);
			byte[] result = Md.Md5(array, 0, bytes);
			ArrayPool<byte>.Shared.Return(array, false);
			return result;
		}
	}
}
