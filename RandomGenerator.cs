using System;
using System.Security.Cryptography;
using System.Text;
using GameCore;
using NorthwoodLib.Pools;

// Token: 0x02000222 RID: 546
public static class RandomGenerator
{
	// Token: 0x06000BF0 RID: 3056 RVA: 0x0002EF10 File Offset: 0x0002D110
	static RandomGenerator()
	{
		RandomGenerator.CryptoRng = ConfigFile.ServerConfig.GetBool("use_crypto_rng", false);
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x0002EF74 File Offset: 0x0002D174
	public static bool GetBool(bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetBoolUnsecure();
		}
		return RandomGenerator.GetBoolSecure();
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x0002EF8C File Offset: 0x0002D18C
	private static bool GetBoolSecure()
	{
		bool result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.OneByte);
			result = (RandomGenerator.OneByte[0] > 127);
		}
		return result;
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x0002EFD4 File Offset: 0x0002D1D4
	private static bool GetBoolUnsecure()
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.OneByte);
		return RandomGenerator.OneByte[0] > 127;
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x0002EFF0 File Offset: 0x0002D1F0
	public static string GetStringSecure(int length)
	{
		if (length <= 0)
		{
			throw new ArgumentException("Length must be greater than 0.", "length");
		}
		StringBuilder stringBuilder = StringBuilderPool.Shared.Rent(length);
		string result;
		try
		{
			for (int i = 0; i < length; i++)
			{
				stringBuilder.Append("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"[RandomNumberGenerator.GetInt32(0, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".Length)]);
			}
			result = StringBuilderPool.Shared.ToStringReturn(stringBuilder);
		}
		catch
		{
			StringBuilderPool.Shared.Return(stringBuilder);
			throw;
		}
		return result;
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x0002F078 File Offset: 0x0002D278
	public static byte[] GetBytes(int count, bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetBytesUnsecure(count);
		}
		return RandomGenerator.GetBytesSecure(count);
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x0002F094 File Offset: 0x0002D294
	private static byte[] GetBytesSecure(int count)
	{
		byte[] result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			byte[] array = new byte[count];
			rngcryptoServiceProvider.GetBytes(array);
			result = array;
		}
		return result;
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x0002F0D4 File Offset: 0x0002D2D4
	private static byte[] GetBytesUnsecure(int count)
	{
		byte[] array = new byte[count];
		RandomGenerator.Random.NextBytes(array);
		return array;
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x0002F0F4 File Offset: 0x0002D2F4
	public static byte GetByte(bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetByteUnsecure();
		}
		return RandomGenerator.GetByteSecure();
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x0002F10C File Offset: 0x0002D30C
	private static byte GetByteSecure()
	{
		byte result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.OneByte);
			result = RandomGenerator.OneByte[0];
		}
		return result;
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x0002F150 File Offset: 0x0002D350
	private static byte GetByteUnsecure()
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.OneByte);
		return RandomGenerator.OneByte[0];
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x0002F168 File Offset: 0x0002D368
	public static sbyte GetSByte(bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetSByteUnsecure();
		}
		return RandomGenerator.GetSByteSecure();
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x0002F180 File Offset: 0x0002D380
	private static sbyte GetSByteSecure()
	{
		sbyte result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.OneByte);
			result = (sbyte)RandomGenerator.OneByte[0];
		}
		return result;
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x0002F1C4 File Offset: 0x0002D3C4
	private static sbyte GetSByteUnsecure()
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.OneByte);
		return (sbyte)RandomGenerator.OneByte[0];
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x0002F1DD File Offset: 0x0002D3DD
	public static short GetInt16(bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetInt16Unsecure();
		}
		return RandomGenerator.GetInt16Secure();
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x0002F1F4 File Offset: 0x0002D3F4
	private static short GetInt16Secure()
	{
		short result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.TwoBytes);
			result = BitConverter.ToInt16(RandomGenerator.TwoBytes, 0);
		}
		return result;
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x0002F23C File Offset: 0x0002D43C
	private static short GetInt16Unsecure()
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.TwoBytes);
		return BitConverter.ToInt16(RandomGenerator.TwoBytes, 0);
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x0002F258 File Offset: 0x0002D458
	public static ushort GetUInt16(bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetUInt16Unsecure();
		}
		return RandomGenerator.GetUInt16Secure();
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x0002F270 File Offset: 0x0002D470
	private static ushort GetUInt16Secure()
	{
		ushort result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.TwoBytes);
			result = BitConverter.ToUInt16(RandomGenerator.TwoBytes, 0);
		}
		return result;
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x0002F2B8 File Offset: 0x0002D4B8
	private static ushort GetUInt16Unsecure()
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.TwoBytes);
		return BitConverter.ToUInt16(RandomGenerator.TwoBytes, 0);
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x0002F2D4 File Offset: 0x0002D4D4
	public static int GetInt32(bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetInt32Unsecure();
		}
		return RandomGenerator.GetInt32Secure();
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x0002F2EC File Offset: 0x0002D4EC
	private static int GetInt32Secure()
	{
		int result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.FourBytes);
			result = BitConverter.ToInt32(RandomGenerator.FourBytes, 0);
		}
		return result;
	}

	// Token: 0x06000C06 RID: 3078 RVA: 0x0002F334 File Offset: 0x0002D534
	private static int GetInt32Unsecure()
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.FourBytes);
		return BitConverter.ToInt32(RandomGenerator.FourBytes, 0);
	}

	// Token: 0x06000C07 RID: 3079 RVA: 0x0002F350 File Offset: 0x0002D550
	public static uint GetUInt32(bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetUInt32Unsecure();
		}
		return RandomGenerator.GetUInt32Secure();
	}

	// Token: 0x06000C08 RID: 3080 RVA: 0x0002F368 File Offset: 0x0002D568
	private static uint GetUInt32Secure()
	{
		uint result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.FourBytes);
			result = BitConverter.ToUInt32(RandomGenerator.FourBytes, 0);
		}
		return result;
	}

	// Token: 0x06000C09 RID: 3081 RVA: 0x0002F3B0 File Offset: 0x0002D5B0
	private static uint GetUInt32Unsecure()
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.FourBytes);
		return BitConverter.ToUInt32(RandomGenerator.FourBytes, 0);
	}

	// Token: 0x06000C0A RID: 3082 RVA: 0x0002F3CC File Offset: 0x0002D5CC
	public static long GetInt64(bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetInt64Unsecure();
		}
		return RandomGenerator.GetInt64Secure();
	}

	// Token: 0x06000C0B RID: 3083 RVA: 0x0002F3E4 File Offset: 0x0002D5E4
	private static long GetInt64Secure()
	{
		long result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.EightBytes);
			result = BitConverter.ToInt64(RandomGenerator.EightBytes, 0);
		}
		return result;
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x0002F42C File Offset: 0x0002D62C
	private static long GetInt64Unsecure()
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.EightBytes);
		return BitConverter.ToInt64(RandomGenerator.EightBytes, 0);
	}

	// Token: 0x06000C0D RID: 3085 RVA: 0x0002F448 File Offset: 0x0002D648
	public static ulong GetUInt64(bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetUInt64Unsecure();
		}
		return RandomGenerator.GetUInt64Secure();
	}

	// Token: 0x06000C0E RID: 3086 RVA: 0x0002F460 File Offset: 0x0002D660
	private static ulong GetUInt64Secure()
	{
		ulong result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.EightBytes);
			result = BitConverter.ToUInt64(RandomGenerator.EightBytes, 0);
		}
		return result;
	}

	// Token: 0x06000C0F RID: 3087 RVA: 0x0002F4A8 File Offset: 0x0002D6A8
	private static ulong GetUInt64Unsecure()
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.EightBytes);
		return BitConverter.ToUInt64(RandomGenerator.EightBytes, 0);
	}

	// Token: 0x06000C10 RID: 3088 RVA: 0x0002F4C4 File Offset: 0x0002D6C4
	public static float GetFloat(bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetFloatUnsecure();
		}
		return RandomGenerator.GetFloatSecure();
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x0002F4DC File Offset: 0x0002D6DC
	private static float GetFloatSecure()
	{
		float result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.FourBytes);
			result = BitConverter.ToSingle(RandomGenerator.FourBytes, 0);
		}
		return result;
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x0002F524 File Offset: 0x0002D724
	private static float GetFloatUnsecure()
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.FourBytes);
		return BitConverter.ToSingle(RandomGenerator.FourBytes, 0);
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x0002F540 File Offset: 0x0002D740
	public static double GetDouble(bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetDoubleUnsecure();
		}
		return RandomGenerator.GetDoubleSecure();
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x0002F558 File Offset: 0x0002D758
	private static double GetDoubleSecure()
	{
		double result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.EightBytes);
			result = BitConverter.ToDouble(RandomGenerator.EightBytes, 0);
		}
		return result;
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x0002F5A0 File Offset: 0x0002D7A0
	private static double GetDoubleUnsecure()
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.EightBytes);
		return BitConverter.ToDouble(RandomGenerator.EightBytes, 0);
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x0002F5BC File Offset: 0x0002D7BC
	public static decimal GetDecimal(bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetDecimalUnsecure();
		}
		return RandomGenerator.GetDecimalSecure();
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x0002F5D4 File Offset: 0x0002D7D4
	private static decimal GetDecimalSecure()
	{
		decimal result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.SixteenBytes);
			result = new decimal(new int[]
			{
				BitConverter.ToInt32(RandomGenerator.SixteenBytes, 0),
				BitConverter.ToInt32(RandomGenerator.SixteenBytes, 4),
				BitConverter.ToInt32(RandomGenerator.SixteenBytes, 8),
				BitConverter.ToInt32(RandomGenerator.SixteenBytes, 12)
			});
		}
		return result;
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x0002F654 File Offset: 0x0002D854
	private static decimal GetDecimalUnsecure()
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.SixteenBytes);
		return new decimal(new int[]
		{
			BitConverter.ToInt32(RandomGenerator.SixteenBytes, 0),
			BitConverter.ToInt32(RandomGenerator.SixteenBytes, 4),
			BitConverter.ToInt32(RandomGenerator.SixteenBytes, 8),
			BitConverter.ToInt32(RandomGenerator.SixteenBytes, 12)
		});
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x0002F6B4 File Offset: 0x0002D8B4
	public static byte GetByte(byte min, byte max, bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetByteUnsecure(min, max);
		}
		return RandomGenerator.GetByteSecure(min, max);
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x0002F6D0 File Offset: 0x0002D8D0
	private static byte GetByteSecure(byte min, byte max)
	{
		byte result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.OneByte);
			byte b = RandomGenerator.OneByte[0];
			while (b < min || b >= max)
			{
				rngcryptoServiceProvider.GetBytes(RandomGenerator.OneByte);
				b = RandomGenerator.OneByte[0];
			}
			result = b;
		}
		return result;
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x0002F734 File Offset: 0x0002D934
	private static byte GetByteUnsecure(byte min, byte max)
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.OneByte);
		byte b = RandomGenerator.OneByte[0];
		while (b < min || b >= max)
		{
			RandomGenerator.Random.NextBytes(RandomGenerator.OneByte);
			b = RandomGenerator.OneByte[0];
		}
		return b;
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x0002F77A File Offset: 0x0002D97A
	public static sbyte GetSByte(sbyte min, sbyte max, bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetSByteUnsecure(min, max);
		}
		return RandomGenerator.GetSByteSecure(min, max);
	}

	// Token: 0x06000C1D RID: 3101 RVA: 0x0002F798 File Offset: 0x0002D998
	private static sbyte GetSByteSecure(sbyte min, sbyte max)
	{
		sbyte result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.OneByte);
			sbyte b = (sbyte)RandomGenerator.OneByte[0];
			while (b < min || b >= max)
			{
				rngcryptoServiceProvider.GetBytes(RandomGenerator.OneByte);
				b = (sbyte)RandomGenerator.OneByte[0];
			}
			result = b;
		}
		return result;
	}

	// Token: 0x06000C1E RID: 3102 RVA: 0x0002F7FC File Offset: 0x0002D9FC
	private static sbyte GetSByteUnsecure(sbyte min, sbyte max)
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.OneByte);
		sbyte b = (sbyte)RandomGenerator.OneByte[0];
		while (b < min || b >= max)
		{
			RandomGenerator.Random.NextBytes(RandomGenerator.OneByte);
			b = (sbyte)RandomGenerator.OneByte[0];
		}
		return b;
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x0002F844 File Offset: 0x0002DA44
	public static short GetInt16(short min, short max, bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetInt16Unsecure(min, max);
		}
		return RandomGenerator.GetInt16Secure(min, max);
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x0002F860 File Offset: 0x0002DA60
	private static short GetInt16Secure(short min, short max)
	{
		short result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.TwoBytes);
			short num = BitConverter.ToInt16(RandomGenerator.TwoBytes, 0);
			while (num < min || num >= max)
			{
				rngcryptoServiceProvider.GetBytes(RandomGenerator.TwoBytes);
				num = BitConverter.ToInt16(RandomGenerator.TwoBytes, 0);
			}
			result = num;
		}
		return result;
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x0002F8CC File Offset: 0x0002DACC
	private static short GetInt16Unsecure(short min, short max)
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.TwoBytes);
		short num = BitConverter.ToInt16(RandomGenerator.TwoBytes, 0);
		while (num < min || num >= max)
		{
			RandomGenerator.Random.NextBytes(RandomGenerator.TwoBytes);
			num = BitConverter.ToInt16(RandomGenerator.TwoBytes, 0);
		}
		return num;
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x0002F91A File Offset: 0x0002DB1A
	public static ushort GetUInt16(ushort min, ushort max, bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetUInt16Unsecure(min, max);
		}
		return RandomGenerator.GetUInt16Secure(min, max);
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x0002F938 File Offset: 0x0002DB38
	private static ushort GetUInt16Secure(ushort min, ushort max)
	{
		ushort result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.TwoBytes);
			ushort num = BitConverter.ToUInt16(RandomGenerator.TwoBytes, 0);
			while (num < min || num >= max)
			{
				rngcryptoServiceProvider.GetBytes(RandomGenerator.TwoBytes);
				num = BitConverter.ToUInt16(RandomGenerator.TwoBytes, 0);
			}
			result = num;
		}
		return result;
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x0002F9A4 File Offset: 0x0002DBA4
	private static ushort GetUInt16Unsecure(ushort min, ushort max)
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.TwoBytes);
		ushort num = BitConverter.ToUInt16(RandomGenerator.TwoBytes, 0);
		while (num < min || num >= max)
		{
			RandomGenerator.Random.NextBytes(RandomGenerator.TwoBytes);
			num = BitConverter.ToUInt16(RandomGenerator.TwoBytes, 0);
		}
		return num;
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x0002F9F2 File Offset: 0x0002DBF2
	public static int GetInt32(int min, int max, bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetInt32Unsecure(min, max);
		}
		return RandomGenerator.GetInt32Secure(min, max);
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x0002FA10 File Offset: 0x0002DC10
	private static int GetInt32Secure(int min, int max)
	{
		int result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.FourBytes);
			int num = BitConverter.ToInt32(RandomGenerator.FourBytes, 0);
			while (num < min || num >= max)
			{
				rngcryptoServiceProvider.GetBytes(RandomGenerator.FourBytes);
				num = BitConverter.ToInt32(RandomGenerator.FourBytes, 0);
			}
			result = num;
		}
		return result;
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x0002FA7C File Offset: 0x0002DC7C
	private static int GetInt32Unsecure(int min, int max)
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.FourBytes);
		int num = BitConverter.ToInt32(RandomGenerator.FourBytes, 0);
		while (num < min || num >= max)
		{
			RandomGenerator.Random.NextBytes(RandomGenerator.FourBytes);
			num = BitConverter.ToInt32(RandomGenerator.FourBytes, 0);
		}
		return num;
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x0002FACA File Offset: 0x0002DCCA
	public static uint GetUInt32(uint min, uint max, bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetUInt32Unsecure(min, max);
		}
		return RandomGenerator.GetUInt32Secure(min, max);
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x0002FAE8 File Offset: 0x0002DCE8
	private static uint GetUInt32Secure(uint min, uint max)
	{
		uint result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.FourBytes);
			uint num = BitConverter.ToUInt32(RandomGenerator.FourBytes, 0);
			while (num < min || num >= max)
			{
				rngcryptoServiceProvider.GetBytes(RandomGenerator.FourBytes);
				num = BitConverter.ToUInt32(RandomGenerator.FourBytes, 0);
			}
			result = num;
		}
		return result;
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x0002FB54 File Offset: 0x0002DD54
	private static uint GetUInt32Unsecure(uint min, uint max)
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.FourBytes);
		uint num = BitConverter.ToUInt32(RandomGenerator.FourBytes, 0);
		while (num < min || num >= max)
		{
			RandomGenerator.Random.NextBytes(RandomGenerator.FourBytes);
			num = BitConverter.ToUInt32(RandomGenerator.FourBytes, 0);
		}
		return num;
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x0002FBA2 File Offset: 0x0002DDA2
	public static long GetInt64(long min, long max, bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetInt64Unsecure(min, max);
		}
		return RandomGenerator.GetInt64Secure(min, max);
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x0002FBC0 File Offset: 0x0002DDC0
	private static long GetInt64Secure(long min, long max)
	{
		long result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.EightBytes);
			long num = BitConverter.ToInt64(RandomGenerator.EightBytes, 0);
			while (num < min || num >= max)
			{
				rngcryptoServiceProvider.GetBytes(RandomGenerator.EightBytes);
				num = BitConverter.ToInt64(RandomGenerator.EightBytes, 0);
			}
			result = num;
		}
		return result;
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x0002FC2C File Offset: 0x0002DE2C
	private static long GetInt64Unsecure(long min, long max)
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.EightBytes);
		long num = BitConverter.ToInt64(RandomGenerator.EightBytes, 0);
		while (num < min || num >= max)
		{
			RandomGenerator.Random.NextBytes(RandomGenerator.EightBytes);
			num = BitConverter.ToInt64(RandomGenerator.EightBytes, 0);
		}
		return num;
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x0002FC7A File Offset: 0x0002DE7A
	public static ulong GetUInt64(ulong min, ulong max, bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetUInt64Unsecure(min, max);
		}
		return RandomGenerator.GetUInt64Secure(min, max);
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x0002FC98 File Offset: 0x0002DE98
	private static ulong GetUInt64Secure(ulong min, ulong max)
	{
		ulong result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.EightBytes);
			ulong num = BitConverter.ToUInt64(RandomGenerator.EightBytes, 0);
			while (num < min || num >= max)
			{
				rngcryptoServiceProvider.GetBytes(RandomGenerator.EightBytes);
				num = BitConverter.ToUInt64(RandomGenerator.EightBytes, 0);
			}
			result = num;
		}
		return result;
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x0002FD04 File Offset: 0x0002DF04
	private static ulong GetUInt64Unsecure(ulong min, ulong max)
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.EightBytes);
		ulong num = BitConverter.ToUInt64(RandomGenerator.EightBytes, 0);
		while (num < min || num >= max)
		{
			RandomGenerator.Random.NextBytes(RandomGenerator.EightBytes);
			num = BitConverter.ToUInt64(RandomGenerator.EightBytes, 0);
		}
		return num;
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x0002FD52 File Offset: 0x0002DF52
	public static float GetFloat(float min, float max, bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetFloatUnsecure(min, max);
		}
		return RandomGenerator.GetFloatSecure(min, max);
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x0002FD70 File Offset: 0x0002DF70
	private static float GetFloatSecure(float min, float max)
	{
		float result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.FourBytes);
			float num = BitConverter.ToSingle(RandomGenerator.FourBytes, 0);
			while (num < min || num > max)
			{
				rngcryptoServiceProvider.GetBytes(RandomGenerator.FourBytes);
				num = BitConverter.ToSingle(RandomGenerator.FourBytes, 0);
			}
			result = num;
		}
		return result;
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x0002FDDC File Offset: 0x0002DFDC
	private static float GetFloatUnsecure(float min, float max)
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.FourBytes);
		float num = BitConverter.ToSingle(RandomGenerator.FourBytes, 0);
		while (num < min || num > max)
		{
			RandomGenerator.Random.NextBytes(RandomGenerator.FourBytes);
			num = BitConverter.ToSingle(RandomGenerator.FourBytes, 0);
		}
		return num;
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x0002FE2A File Offset: 0x0002E02A
	public static double GetDouble(double min, double max, bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetDoubleUnsecure(min, max);
		}
		return RandomGenerator.GetDoubleSecure(min, max);
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x0002FE48 File Offset: 0x0002E048
	private static double GetDoubleSecure(double min, double max)
	{
		double result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.EightBytes);
			double num = BitConverter.ToDouble(RandomGenerator.EightBytes, 0);
			while (num < min || num > max)
			{
				rngcryptoServiceProvider.GetBytes(RandomGenerator.EightBytes);
				num = BitConverter.ToDouble(RandomGenerator.EightBytes, 0);
			}
			result = num;
		}
		return result;
	}

	// Token: 0x06000C36 RID: 3126 RVA: 0x0002FEB4 File Offset: 0x0002E0B4
	private static double GetDoubleUnsecure(double min, double max)
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.EightBytes);
		double num = BitConverter.ToDouble(RandomGenerator.EightBytes, 0);
		while (num < min || num > max)
		{
			RandomGenerator.Random.NextBytes(RandomGenerator.EightBytes);
			num = BitConverter.ToDouble(RandomGenerator.EightBytes, 0);
		}
		return num;
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x0002FF02 File Offset: 0x0002E102
	public static decimal GetDecimal(decimal min, decimal max, bool secure = false)
	{
		if (!secure && !RandomGenerator.CryptoRng)
		{
			return RandomGenerator.GetDecimalUnsecure(min, max);
		}
		return RandomGenerator.GetDecimalSecure(min, max);
	}

	// Token: 0x06000C38 RID: 3128 RVA: 0x0002FF20 File Offset: 0x0002E120
	private static decimal GetDecimalSecure(decimal min, decimal max)
	{
		decimal result;
		using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
		{
			rngcryptoServiceProvider.GetBytes(RandomGenerator.SixteenBytes);
			decimal num = new decimal(new int[]
			{
				BitConverter.ToInt32(RandomGenerator.SixteenBytes, 0),
				BitConverter.ToInt32(RandomGenerator.SixteenBytes, 4),
				BitConverter.ToInt32(RandomGenerator.SixteenBytes, 8),
				BitConverter.ToInt32(RandomGenerator.SixteenBytes, 12)
			});
			while (num < min || num > max)
			{
				rngcryptoServiceProvider.GetBytes(RandomGenerator.SixteenBytes);
				num = new decimal(new int[]
				{
					BitConverter.ToInt32(RandomGenerator.SixteenBytes, 0),
					BitConverter.ToInt32(RandomGenerator.SixteenBytes, 4),
					BitConverter.ToInt32(RandomGenerator.SixteenBytes, 8),
					BitConverter.ToInt32(RandomGenerator.SixteenBytes, 12)
				});
			}
			result = num;
		}
		return result;
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x00030008 File Offset: 0x0002E208
	private static decimal GetDecimalUnsecure(decimal min, decimal max)
	{
		RandomGenerator.Random.NextBytes(RandomGenerator.SixteenBytes);
		decimal num = new decimal(new int[]
		{
			BitConverter.ToInt32(RandomGenerator.SixteenBytes, 0),
			BitConverter.ToInt32(RandomGenerator.SixteenBytes, 4),
			BitConverter.ToInt32(RandomGenerator.SixteenBytes, 8),
			BitConverter.ToInt32(RandomGenerator.SixteenBytes, 12)
		});
		while (num < min || num > max)
		{
			RandomGenerator.Random.NextBytes(RandomGenerator.SixteenBytes);
			num = new decimal(new int[]
			{
				BitConverter.ToInt32(RandomGenerator.SixteenBytes, 0),
				BitConverter.ToInt32(RandomGenerator.SixteenBytes, 4),
				BitConverter.ToInt32(RandomGenerator.SixteenBytes, 8),
				BitConverter.ToInt32(RandomGenerator.SixteenBytes, 12)
			});
		}
		return num;
	}

	// Token: 0x04000A0F RID: 2575
	private static readonly Random Random = new Random();

	// Token: 0x04000A10 RID: 2576
	private static readonly bool CryptoRng;

	// Token: 0x04000A11 RID: 2577
	private static readonly byte[] OneByte = new byte[1];

	// Token: 0x04000A12 RID: 2578
	private static readonly byte[] TwoBytes = new byte[2];

	// Token: 0x04000A13 RID: 2579
	private static readonly byte[] FourBytes = new byte[4];

	// Token: 0x04000A14 RID: 2580
	private static readonly byte[] EightBytes = new byte[8];

	// Token: 0x04000A15 RID: 2581
	private static readonly byte[] SixteenBytes = new byte[16];

	// Token: 0x04000A16 RID: 2582
	private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
}
