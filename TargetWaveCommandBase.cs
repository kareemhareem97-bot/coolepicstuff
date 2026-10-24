using System;
using System.Collections.Generic;
using Respawning.Waves;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A08 RID: 2568
	[CommandHandler(typeof(WaveCommand))]
	public abstract class TargetWaveCommandBase : ICommand
	{
		// Token: 0x17000C3C RID: 3132
		// (get) Token: 0x06003A03 RID: 14851
		public abstract string Command { get; }

		// Token: 0x17000C3D RID: 3133
		// (get) Token: 0x06003A04 RID: 14852
		public abstract string[] Aliases { get; }

		// Token: 0x17000C3E RID: 3134
		// (get) Token: 0x06003A05 RID: 14853
		public abstract string Description { get; }

		// Token: 0x06003A06 RID: 14854
		public abstract bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response);

		// Token: 0x06003A07 RID: 14855 RVA: 0x000BF390 File Offset: 0x000BD590
		protected static string TranslateWaveName(string input)
		{
			foreach (KeyValuePair<Type, string[]> keyValuePair in TargetWaveCommandBase.WaveAliases)
			{
				if (keyValuePair.Value.Contains(input, StringComparison.OrdinalIgnoreCase))
				{
					return keyValuePair.Key.Name;
				}
			}
			return input;
		}

		// Token: 0x06003A09 RID: 14857 RVA: 0x000BF400 File Offset: 0x000BD600
		// Note: this type is marked as 'beforefieldinit'.
		static TargetWaveCommandBase()
		{
			Dictionary<Type, string[]> dictionary = new Dictionary<Type, string[]>();
			Type typeFromHandle = typeof(NtfSpawnWave);
			dictionary[typeFromHandle] = new string[]
			{
				"NTF",
				"MTF",
				"MobileTaskForces",
				"NineTailedFox"
			};
			Type typeFromHandle2 = typeof(ChaosSpawnWave);
			dictionary[typeFromHandle2] = new string[]
			{
				"CI",
				"Chaos",
				"ChaosInsurgency"
			};
			Type typeFromHandle3 = typeof(NtfMiniWave);
			dictionary[typeFromHandle3] = new string[]
			{
				"NTFMini",
				"MTFMini",
				"MiniMTF",
				"MiniNTF"
			};
			Type typeFromHandle4 = typeof(ChaosMiniWave);
			dictionary[typeFromHandle4] = new string[]
			{
				"ChaosMini",
				"CIMini",
				"MiniCI",
				"MiniChaos"
			};
			TargetWaveCommandBase.WaveAliases = dictionary;
		}

		// Token: 0x04002B10 RID: 11024
		private static readonly Dictionary<Type, string[]> WaveAliases;
	}
}
