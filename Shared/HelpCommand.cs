using System;
using System.Text;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009B6 RID: 2486
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(ClientCommandHandler))]
	public class HelpCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000B34 RID: 2868
		// (get) Token: 0x06003834 RID: 14388 RVA: 0x000B6A16 File Offset: 0x000B4C16
		public string Command { get; } = "help";

		// Token: 0x17000B35 RID: 2869
		// (get) Token: 0x06003835 RID: 14389 RVA: 0x000B6A1E File Offset: 0x000B4C1E
		public string[] Aliases { get; }

		// Token: 0x17000B36 RID: 2870
		// (get) Token: 0x06003836 RID: 14390 RVA: 0x000B6A26 File Offset: 0x000B4C26
		public string Description { get; } = "Returns all commands with their descriptions and aliases or displays help for specified command.";

		// Token: 0x17000B37 RID: 2871
		// (get) Token: 0x06003837 RID: 14391 RVA: 0x000B6A2E File Offset: 0x000B4C2E
		public string[] Usage { get; } = new string[]
		{
			"Command (Optional)"
		};

		// Token: 0x06003838 RID: 14392 RVA: 0x000B6A38 File Offset: 0x000B4C38
		public HelpCommand(ICommandHandler commandHandler)
		{
			this._commandHandler = commandHandler;
		}

		// Token: 0x06003839 RID: 14393 RVA: 0x000B6A88 File Offset: 0x000B4C88
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Count == 0)
			{
				response = this.GetCommandList(this._commandHandler, "Command list:");
				return true;
			}
			ICommand command;
			if (this._commandHandler.TryGetCommand(arguments.At(0), out command))
			{
				string text = command.Command;
				ArraySegment<string> arraySegment = arguments.Segment(1);
				while (arraySegment.Count != 0)
				{
					ICommandHandler commandHandler = command as ICommandHandler;
					ICommand command2;
					if (commandHandler == null || !commandHandler.TryGetCommand(arraySegment.At(0), out command2))
					{
						break;
					}
					arraySegment = arraySegment.Segment(1);
					command = command2;
					text = text + " " + command2.Command;
				}
				string str = text;
				string str2 = " - ";
				IHelpProvider helpProvider = command as IHelpProvider;
				response = str + str2 + ((helpProvider != null) ? helpProvider.GetHelp(arraySegment) : command.Description);
				if (command.Aliases != null && command.Aliases.Length != 0)
				{
					response = response + "\nAliases: " + string.Join(", ", command.Aliases);
				}
				ICommandHandler commandHandler2 = command as ICommandHandler;
				if (commandHandler2 != null)
				{
					response += this.GetCommandList(commandHandler2, "\nSubcommand list:");
				}
				try
				{
					Type type = command.GetType();
					if (type != null)
					{
						response = string.Concat(new string[]
						{
							response,
							"\nImplemented in: ",
							type.Assembly.GetName().Name,
							":",
							type.FullName
						});
					}
				}
				catch
				{
				}
				return true;
			}
			response = "Help for " + arguments.At(0) + " isn't available!";
			return false;
		}

		// Token: 0x0600383A RID: 14394 RVA: 0x000B6C1C File Offset: 0x000B4E1C
		private string GetCommandList(ICommandHandler handler, string header)
		{
			this._helpBuilder.Clear();
			this._helpBuilder.Append(header);
			foreach (ICommand command in handler.AllCommands)
			{
				if (!(command is IHiddenCommand))
				{
					this._helpBuilder.AppendLine();
					this._helpBuilder.Append(command.Command);
					this._helpBuilder.Append(" - ");
					this._helpBuilder.Append(command.Description);
					if (command.Aliases != null && command.Aliases.Length != 0)
					{
						this._helpBuilder.Append(" - Aliases: ");
						this._helpBuilder.Append(string.Join(", ", command.Aliases));
					}
				}
			}
			return this._helpBuilder.ToString();
		}

		// Token: 0x040029F3 RID: 10739
		private readonly StringBuilder _helpBuilder = new StringBuilder();

		// Token: 0x040029F4 RID: 10740
		private readonly ICommandHandler _commandHandler;
	}
}
