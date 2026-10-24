using System;
using GameCore;
using UnityEngine;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009A9 RID: 2473
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class BuildInfoCommand : ICommand
	{
		// Token: 0x17000B0D RID: 2829
		// (get) Token: 0x060037F1 RID: 14321 RVA: 0x000B5DD5 File Offset: 0x000B3FD5
		public string Command { get; } = "buildinfo";

		// Token: 0x17000B0E RID: 2830
		// (get) Token: 0x060037F2 RID: 14322 RVA: 0x000B5DDD File Offset: 0x000B3FDD
		public string[] Aliases { get; } = new string[]
		{
			"v",
			"ver",
			"version"
		};

		// Token: 0x17000B0F RID: 2831
		// (get) Token: 0x060037F3 RID: 14323 RVA: 0x000B5DE5 File Offset: 0x000B3FE5
		public string Description { get; } = "Displays information about the current build.";

		// Token: 0x060037F4 RID: 14324 RVA: 0x000B5DED File Offset: 0x000B3FED
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = BuildInfoCommand.BuildInfoString;
			return true;
		}

		// Token: 0x17000B10 RID: 2832
		// (get) Token: 0x060037F5 RID: 14325 RVA: 0x000B5DF8 File Offset: 0x000B3FF8
		internal static string BuildInfoString
		{
			get
			{
				return string.Concat(new string[]
				{
					"Build info:\nGame version: ",
					GameCore.Version.VersionString,
					string.Format("\nPreauth version: {0}.{1}.{2}", GameCore.Version.Major, GameCore.Version.Minor, GameCore.Version.Revision),
					"\nBackward compatibility: ",
					(!GameCore.Version.BackwardCompatibility || GameCore.Version.ExtendedVersionCheckNeeded) ? "False" : string.Format("{0}.{1}.{2} and newer", GameCore.Version.Major, GameCore.Version.Minor, GameCore.Version.BackwardRevision),
					"\nBuild timestamp: 2026-10-12 18:32:38Z",
					string.Format("\nBuild type: {0}", GameCore.Version.BuildType),
					string.Format("\nHoliday type: {0}", GameCore.Version.ActiveHoliday),
					string.Format("\nAlways accept release builds: {0}", GameCore.Version.AlwaysAcceptReleaseBuilds),
					"\nBuild GUID: ",
					PlatformInfo.singleton.BuildGuid,
					"\nUnity version: ",
					Application.unityVersion,
					string.Format("\n\nPrivate beta: {0}", GameCore.Version.PrivateBeta),
					string.Format("\nPublic beta: {0}", GameCore.Version.PublicBeta),
					string.Format("\nRelease candidate: {0}", GameCore.Version.ReleaseCandidate),
					string.Format("\nStreaming allowed: {0}", GameCore.Version.StreamingAllowed),
					string.Format("\nHeadless: {0}", PlatformInfo.singleton.IsHeadless),
					"\nModded: ",
					CustomNetworkManager.Modded ? string.Format("{0}\nMod Description:\n{1}", true, BuildInfoCommand.ModDescription) : false.ToString()
				});
			}
		}

		// Token: 0x040029CE RID: 10702
		public static string ModDescription;
	}
}
