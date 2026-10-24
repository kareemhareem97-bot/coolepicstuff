using System;
using UnityEngine;

namespace GameCore
{
	// Token: 0x02000859 RID: 2137
	internal class ConsoleCommandSender : CommandSender
	{
		// Token: 0x17000902 RID: 2306
		// (get) Token: 0x060030E6 RID: 12518 RVA: 0x000A441D File Offset: 0x000A261D
		public override string SenderId
		{
			get
			{
				return "GAME CONSOLE";
			}
		}

		// Token: 0x17000903 RID: 2307
		// (get) Token: 0x060030E7 RID: 12519 RVA: 0x000A441D File Offset: 0x000A261D
		public override string Nickname
		{
			get
			{
				return "GAME CONSOLE";
			}
		}

		// Token: 0x17000904 RID: 2308
		// (get) Token: 0x060030E8 RID: 12520 RVA: 0x00025D54 File Offset: 0x00023F54
		public override ulong Permissions
		{
			get
			{
				return ulong.MaxValue;
			}
		}

		// Token: 0x17000905 RID: 2309
		// (get) Token: 0x060030E9 RID: 12521 RVA: 0x00005495 File Offset: 0x00003695
		public override byte KickPower
		{
			get
			{
				return byte.MaxValue;
			}
		}

		// Token: 0x17000906 RID: 2310
		// (get) Token: 0x060030EA RID: 12522 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool FullPermissions
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060030EB RID: 12523 RVA: 0x000A4424 File Offset: 0x000A2624
		public override void RaReply(string text, bool success, bool logToConsole, string overrideDisplay)
		{
			Console.AddLog("[RA Reply] " + text, success ? Color.green : Color.red, false, ConsoleLogType.Log);
		}

		// Token: 0x060030EC RID: 12524 RVA: 0x000A4447 File Offset: 0x000A2647
		public override void Print(string text)
		{
			Console.AddLog(text, Color.green, false, ConsoleLogType.Log);
		}

		// Token: 0x060030ED RID: 12525 RVA: 0x000A4456 File Offset: 0x000A2656
		public override void Print(string text, ConsoleColor c)
		{
			Console.AddLog(text, ServerConsole.ConsoleColorToColor(c), false, ConsoleLogType.Log);
		}

		// Token: 0x060030EE RID: 12526 RVA: 0x000A4466 File Offset: 0x000A2666
		public override void Print(string text, ConsoleColor c, Color rgbColor)
		{
			Console.AddLog(text, rgbColor, false, ConsoleLogType.Log);
		}

		// Token: 0x060030EF RID: 12527 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Available()
		{
			return true;
		}

		// Token: 0x060030F0 RID: 12528 RVA: 0x000A4471 File Offset: 0x000A2671
		public override void Respond(string message, bool success = true)
		{
			Console.AddLog(message, success ? Color.green : Color.red, false, ConsoleLogType.Log);
		}
	}
}
