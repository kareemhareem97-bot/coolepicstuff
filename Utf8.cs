using System;
using System.Text;

// Token: 0x0200022B RID: 555
public static class Utf8
{
	// Token: 0x06000C54 RID: 3156 RVA: 0x000306D3 File Offset: 0x0002E8D3
	public static int GetLength(string data)
	{
		return Utf8.Encoding.GetByteCount(data);
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x000306E0 File Offset: 0x0002E8E0
	public static byte[] GetBytes(string data)
	{
		return Utf8.Encoding.GetBytes(data);
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x000306ED File Offset: 0x0002E8ED
	public static int GetBytes(string data, byte[] buffer)
	{
		return Utf8.Encoding.GetBytes(data, 0, data.Length, buffer, 0);
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x00030703 File Offset: 0x0002E903
	public static int GetBytes(string data, byte[] buffer, int offset)
	{
		return Utf8.Encoding.GetBytes(data, 0, data.Length, buffer, offset);
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x00030719 File Offset: 0x0002E919
	public static string GetString(byte[] data)
	{
		return Utf8.Encoding.GetString(data);
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x00030726 File Offset: 0x0002E926
	public static string GetString(byte[] data, int offset, int count)
	{
		return Utf8.Encoding.GetString(data, offset, count);
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x00030735 File Offset: 0x0002E935
	public static string GetString(ReadOnlySpan<byte> data)
	{
		return Utf8.Encoding.GetString(data);
	}

	// Token: 0x06000C5B RID: 3163 RVA: 0x00030742 File Offset: 0x0002E942
	public static string GetString(ReadOnlySpan<byte> data, int offset, int count)
	{
		return Utf8.Encoding.GetString(data.Slice(offset, count));
	}

	// Token: 0x04000A27 RID: 2599
	private static readonly UTF8Encoding Encoding = new UTF8Encoding(false);
}
