using System;
using System.Buffers.Binary;

// Token: 0x020001B0 RID: 432
public readonly struct RemoteAdminResponse : IEquatable<RemoteAdminResponse>
{
	// Token: 0x06000874 RID: 2164 RVA: 0x000216F4 File Offset: 0x0001F8F4
	public RemoteAdminResponse(string content, RemoteAdminResponse.RemoteAdminResponseFlags flags, string overrideDisplay)
	{
		this.Content = (string.IsNullOrEmpty(content) ? null : content);
		this.Flags = flags;
		this.OverrideDisplay = (string.IsNullOrEmpty(overrideDisplay) ? null : overrideDisplay);
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x00021721 File Offset: 0x0001F921
	public RemoteAdminResponse(string content, bool isSuccess, bool logInConsole, string overrideDisplay)
	{
		this = new RemoteAdminResponse(content, (isSuccess ? RemoteAdminResponse.RemoteAdminResponseFlags.Successful : ((RemoteAdminResponse.RemoteAdminResponseFlags)0)) | (logInConsole ? RemoteAdminResponse.RemoteAdminResponseFlags.LogInConsole : ((RemoteAdminResponse.RemoteAdminResponseFlags)0)), overrideDisplay);
	}

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06000876 RID: 2166 RVA: 0x0002173B File Offset: 0x0001F93B
	public int GetLength
	{
		get
		{
			return ((this.Content == null) ? 0 : Utf8.GetLength(this.Content)) + ((this.OverrideDisplay == null) ? 0 : Utf8.GetLength(this.OverrideDisplay)) + 5;
		}
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x0002176C File Offset: 0x0001F96C
	public byte[] Serialize()
	{
		byte[] array = new byte[this.GetLength];
		this.Serialize(array);
		return array;
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x00021790 File Offset: 0x0001F990
	public void Serialize(byte[] array)
	{
		array[0] = (byte)this.Flags;
		int num = (this.Content == null) ? 0 : Utf8.GetBytes(this.Content, array, 5);
		BinaryPrimitives.WriteInt32BigEndian(new Span<byte>(array, 1, 4), num);
		if (this.OverrideDisplay != null)
		{
			Utf8.GetBytes(this.OverrideDisplay, array, num + 5);
		}
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x000217E8 File Offset: 0x0001F9E8
	public static RemoteAdminResponse Deserialize(byte[] array, int length)
	{
		int num = BinaryPrimitives.ReadInt32BigEndian(new Span<byte>(array, 1, 4));
		string content = (num > 0) ? Utf8.GetString(array, 5, num) : null;
		string overrideDisplay = (num + 5 < length) ? Utf8.GetString(array, num + 5, length - num - 5) : null;
		return new RemoteAdminResponse(content, (RemoteAdminResponse.RemoteAdminResponseFlags)array[0], overrideDisplay);
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x00021838 File Offset: 0x0001FA38
	public bool Equals(RemoteAdminResponse other)
	{
		return string.Equals(this.Content, other.Content) && this.Flags == other.Flags && string.Equals(this.OverrideDisplay, other.OverrideDisplay);
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x00021870 File Offset: 0x0001FA70
	public override bool Equals(object obj)
	{
		if (obj is RemoteAdminResponse)
		{
			RemoteAdminResponse other = (RemoteAdminResponse)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x00021898 File Offset: 0x0001FA98
	public override int GetHashCode()
	{
		return (((this.Content != null) ? this.Content.GetHashCode() : 0) * 397 ^ this.Flags.GetHashCode()) * 397 ^ ((this.OverrideDisplay != null) ? this.OverrideDisplay.GetHashCode() : 0);
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x000218F0 File Offset: 0x0001FAF0
	public static bool operator ==(RemoteAdminResponse left, RemoteAdminResponse right)
	{
		return left.Equals(right);
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x000218FA File Offset: 0x0001FAFA
	public static bool operator !=(RemoteAdminResponse left, RemoteAdminResponse right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x00021907 File Offset: 0x0001FB07
	public bool HasFlag(RemoteAdminResponse.RemoteAdminResponseFlags flag)
	{
		return (this.Flags & flag) == flag;
	}

	// Token: 0x040007A3 RID: 1955
	public readonly string Content;

	// Token: 0x040007A4 RID: 1956
	public readonly RemoteAdminResponse.RemoteAdminResponseFlags Flags;

	// Token: 0x040007A5 RID: 1957
	public readonly string OverrideDisplay;

	// Token: 0x040007A6 RID: 1958
	private const int HeaderSize = 5;

	// Token: 0x020001B1 RID: 433
	[Flags]
	public enum RemoteAdminResponseFlags : byte
	{
		// Token: 0x040007A8 RID: 1960
		Successful = 1,
		// Token: 0x040007A9 RID: 1961
		LogInConsole = 2
	}
}
