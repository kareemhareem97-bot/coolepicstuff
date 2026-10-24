using System;
using System.IO;
using Utf8Json;
using Utf8Json.Resolvers;
using Utf8Json.Unity;

// Token: 0x02000214 RID: 532
public static class JsonSerialize
{
	// Token: 0x06000B1F RID: 2847 RVA: 0x0002B463 File Offset: 0x00029663
	static JsonSerialize()
	{
		CompositeResolver.RegisterAndSetAsDefault(new IJsonFormatterResolver[]
		{
			GeneratedResolver.Instance,
			BuiltinResolver.Instance,
			EnumResolver.Default,
			UnityResolver.Instance,
			StandardResolver.Default
		});
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0002B498 File Offset: 0x00029698
	public static string ToJson<T>(T value) where T : IJsonSerializable
	{
		return JsonSerializer.ToJsonString<T>(value);
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x0002B4A0 File Offset: 0x000296A0
	public static T FromJson<T>(Stream value) where T : IJsonSerializable
	{
		return JsonSerializer.Deserialize<T>(value);
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x0002B4A8 File Offset: 0x000296A8
	public static T FromJson<T>(byte[] value) where T : IJsonSerializable
	{
		return JsonSerializer.Deserialize<T>(value);
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x0002B4B0 File Offset: 0x000296B0
	public static T FromJson<T>(byte[] value, int offset) where T : IJsonSerializable
	{
		return JsonSerializer.Deserialize<T>(value, offset);
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x0002B4B9 File Offset: 0x000296B9
	public static T FromJson<T>(string value) where T : IJsonSerializable
	{
		return JsonSerializer.Deserialize<T>(value);
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x0002B4C4 File Offset: 0x000296C4
	public static T FromFile<T>(string path) where T : IJsonSerializable
	{
		T result;
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
		{
			result = JsonSerializer.Deserialize<T>(fileStream);
		}
		return result;
	}
}
