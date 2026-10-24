using System;
using CommandSystem;
using UnityEngine;

// Token: 0x020001AE RID: 430
public abstract class CommandSender : IOutput, ICommandSender
{
	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000861 RID: 2145
	public abstract string SenderId { get; }

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000862 RID: 2146 RVA: 0x000216C1 File Offset: 0x0001F8C1
	public string OutputId
	{
		get
		{
			return this.SenderId;
		}
	}

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x06000863 RID: 2147
	public abstract string Nickname { get; }

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x06000864 RID: 2148
	public abstract ulong Permissions { get; }

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x06000865 RID: 2149
	public abstract byte KickPower { get; }

	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06000866 RID: 2150
	public abstract bool FullPermissions { get; }

	// Token: 0x06000867 RID: 2151
	public abstract void RaReply(string text, bool success, bool logToConsole, string overrideDisplay);

	// Token: 0x06000868 RID: 2152
	public abstract void Print(string text);

	// Token: 0x06000869 RID: 2153 RVA: 0x000216C9 File Offset: 0x0001F8C9
	public virtual void Print(string text, ConsoleColor c)
	{
		this.Print(text);
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x000216D2 File Offset: 0x0001F8D2
	public virtual void Print(string text, ConsoleColor c, Color rgbColor)
	{
		this.Print(text, c);
	}

	// Token: 0x0600086B RID: 2155
	public abstract bool Available();

	// Token: 0x0600086C RID: 2156 RVA: 0x000216DC File Offset: 0x0001F8DC
	public virtual void Respond(string message, bool success = true)
	{
		this.RaReply(message, success, true, string.Empty);
	}

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x0600086D RID: 2157 RVA: 0x000216EC File Offset: 0x0001F8EC
	public virtual string LogName
	{
		get
		{
			return this.Nickname;
		}
	}
}
