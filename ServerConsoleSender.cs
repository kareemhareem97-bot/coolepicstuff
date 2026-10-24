using System;

// Token: 0x020001CB RID: 459
public class ServerConsoleSender : CommandSender
{
	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000963 RID: 2403 RVA: 0x00025D4D File Offset: 0x00023F4D
	public override string SenderId
	{
		get
		{
			return "SERVER CONSOLE";
		}
	}

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000964 RID: 2404 RVA: 0x00025D4D File Offset: 0x00023F4D
	public override string Nickname
	{
		get
		{
			return "SERVER CONSOLE";
		}
	}

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x06000965 RID: 2405 RVA: 0x00025D54 File Offset: 0x00023F54
	public override ulong Permissions
	{
		get
		{
			return ulong.MaxValue;
		}
	}

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000966 RID: 2406 RVA: 0x00005495 File Offset: 0x00003695
	public override byte KickPower
	{
		get
		{
			return byte.MaxValue;
		}
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000967 RID: 2407 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool FullPermissions
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x000216C9 File Offset: 0x0001F8C9
	public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay)
	{
		this.Print(text);
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x00025D58 File Offset: 0x00023F58
	public override void Print(string text)
	{
		ServerConsole.AddLog(text, ConsoleColor.Gray, false);
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x00025D62 File Offset: 0x00023F62
	public override void Print(string text, ConsoleColor c)
	{
		ServerConsole.AddLog(text, c, false);
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Available()
	{
		return true;
	}
}
