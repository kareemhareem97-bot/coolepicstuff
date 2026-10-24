using System;
using System.Linq;

// Token: 0x02000226 RID: 550
public static class StartupArguments
{
	// Token: 0x06000C41 RID: 3137 RVA: 0x000302C0 File Offset: 0x0002E4C0
	public static bool IsSetShort(string param)
	{
		return Environment.GetCommandLineArgs().Any((string x) => x.StartsWith("-") && !x.StartsWith("--") && x.Contains(param));
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x000302F0 File Offset: 0x0002E4F0
	public static bool IsSetBool(string param, string alias = "")
	{
		return Environment.GetCommandLineArgs().Contains("--" + param) || (!string.IsNullOrEmpty(alias) && StartupArguments.IsSetShort(alias));
	}

	// Token: 0x06000C43 RID: 3139 RVA: 0x0003031C File Offset: 0x0002E51C
	public static string GetArgument(string param, string alias = "", string def = "")
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		bool flag = false;
		foreach (string text in commandLineArgs)
		{
			if (flag && !text.StartsWith("-"))
			{
				return text;
			}
			flag = (text == "--" + param || (!string.IsNullOrEmpty(alias) && text.StartsWith("-") && !text.StartsWith("--") && text.EndsWith(alias)));
		}
		return def;
	}
}
