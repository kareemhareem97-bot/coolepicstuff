using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cassie;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009D6 RID: 2518
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CassieWordsCommand : ICommand
	{
		// Token: 0x17000B97 RID: 2967
		// (get) Token: 0x060038E0 RID: 14560 RVA: 0x000B9621 File Offset: 0x000B7821
		public string Command { get; } = "cassiewords";

		// Token: 0x17000B98 RID: 2968
		// (get) Token: 0x060038E1 RID: 14561 RVA: 0x000B9629 File Offset: 0x000B7829
		public string[] Aliases { get; }

		// Token: 0x17000B99 RID: 2969
		// (get) Token: 0x060038E2 RID: 14562 RVA: 0x000B9631 File Offset: 0x000B7831
		public string Description { get; } = "Lists CASSIE words.";

		// Token: 0x060038E3 RID: 14563 RVA: 0x000B963C File Offset: 0x000B783C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			CassieLineDatabase cassieLineDatabase;
			if (!CassieTtsAnnouncer.TryGetDatabase(out cassieLineDatabase))
			{
				response = "Unable to fetch line database.";
				return false;
			}
			IEnumerable<string> enumerable = from line in cassieLineDatabase.AllLines
			select line.ApiName;
			if (arguments.Count > 0)
			{
				Regex regex = new Regex(arguments.FirstElement<string>(), RegexOptions.IgnoreCase);
				enumerable = from line in enumerable
				where regex.IsMatch(line)
				select line;
			}
			response = "CASSIE words: " + string.Join(" ", enumerable);
			return true;
		}
	}
}
