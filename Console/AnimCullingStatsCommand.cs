using System;
using System.Runtime.CompilerServices;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;
using UnityEngine;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A89 RID: 2697
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class AnimCullingStatsCommand : ICommand
	{
		// Token: 0x17000DB8 RID: 3512
		// (get) Token: 0x06003CE1 RID: 15585 RVA: 0x000C83CD File Offset: 0x000C65CD
		public string Command { get; } = "animcullingstats";

		// Token: 0x17000DB9 RID: 3513
		// (get) Token: 0x06003CE2 RID: 15586 RVA: 0x000C83D5 File Offset: 0x000C65D5
		public string[] Aliases { get; } = new string[]
		{
			"animcullingdebug",
			"animdebug"
		};

		// Token: 0x17000DBA RID: 3514
		// (get) Token: 0x06003CE3 RID: 15587 RVA: 0x000C83DD File Offset: 0x000C65DD
		public string Description { get; } = "Displays current animation culling stats.";

		// Token: 0x17000DBB RID: 3515
		// (get) Token: 0x06003CE4 RID: 15588 RVA: 0x000C83E5 File Offset: 0x000C65E5
		// (set) Token: 0x06003CE5 RID: 15589 RVA: 0x000C83EC File Offset: 0x000C65EC
		private static bool Enabled
		{
			get
			{
				return AnimCullingStatsCommand._isEnabled;
			}
			set
			{
				if (value == AnimCullingStatsCommand.Enabled)
				{
					return;
				}
				AnimCullingStatsCommand._isEnabled = value;
				if (value)
				{
					AnimCullingStatsCommand._animUpdates = 0;
					Action value2;
					if ((value2 = AnimCullingStatsCommand.<>O.<0>__OnGUI) == null)
					{
						value2 = (AnimCullingStatsCommand.<>O.<0>__OnGUI = new Action(AnimCullingStatsCommand.OnGUI));
					}
					StaticUnityMethods.OnGui += value2;
					Action value3;
					if ((value3 = AnimCullingStatsCommand.<>O.<1>__OnUpdate) == null)
					{
						value3 = (AnimCullingStatsCommand.<>O.<1>__OnUpdate = new Action(AnimCullingStatsCommand.OnUpdate));
					}
					StaticUnityMethods.OnUpdate += value3;
					Action value4;
					if ((value4 = AnimCullingStatsCommand.<>O.<2>__OnAnyAnimatorUpdated) == null)
					{
						value4 = (AnimCullingStatsCommand.<>O.<2>__OnAnyAnimatorUpdated = new Action(AnimCullingStatsCommand.OnAnyAnimatorUpdated));
					}
					CullingSubcontroller.OnAnyAnimatorUpdated += value4;
					return;
				}
				AnimCullingStatsCommand._totalMeasurements = 0;
				Action value5;
				if ((value5 = AnimCullingStatsCommand.<>O.<0>__OnGUI) == null)
				{
					value5 = (AnimCullingStatsCommand.<>O.<0>__OnGUI = new Action(AnimCullingStatsCommand.OnGUI));
				}
				StaticUnityMethods.OnGui -= value5;
				Action value6;
				if ((value6 = AnimCullingStatsCommand.<>O.<1>__OnUpdate) == null)
				{
					value6 = (AnimCullingStatsCommand.<>O.<1>__OnUpdate = new Action(AnimCullingStatsCommand.OnUpdate));
				}
				StaticUnityMethods.OnUpdate -= value6;
				Action value7;
				if ((value7 = AnimCullingStatsCommand.<>O.<2>__OnAnyAnimatorUpdated) == null)
				{
					value7 = (AnimCullingStatsCommand.<>O.<2>__OnAnyAnimatorUpdated = new Action(AnimCullingStatsCommand.OnAnyAnimatorUpdated));
				}
				CullingSubcontroller.OnAnyAnimatorUpdated -= value7;
			}
		}

		// Token: 0x06003CE6 RID: 15590 RVA: 0x000C84D8 File Offset: 0x000C66D8
		private static void OnGUI()
		{
			int num = 0;
			int num2 = 0;
			int num3 = Mathf.Min(AnimCullingStatsCommand._totalMeasurements, AnimCullingStatsCommand.Samples.Length);
			for (int i = 0; i < num3; i++)
			{
				AnimCullingStatsCommand.AnimSample animSample = AnimCullingStatsCommand.Samples[i];
				num += animSample.Updates;
				num2 += animSample.TotalModels;
			}
			AnimCullingStatsCommand.AnimSample animSample2 = new AnimCullingStatsCommand.AnimSample(num, num2);
			GUILayout.BeginArea(new Rect(new Vector2(10f, 10f), new Vector2(250f, 999f)));
			GUILayout.Label("Animation Culling Stats:", Array.Empty<GUILayoutOption>());
			GUILayout.Label("Last frame: " + AnimCullingStatsCommand._lastSample.ToString(), Array.Empty<GUILayoutOption>());
			GUILayout.Label(string.Format("Last {0} frames: ", num3) + animSample2.ToString(), Array.Empty<GUILayoutOption>());
			GUILayout.EndArea();
		}

		// Token: 0x06003CE7 RID: 15591 RVA: 0x000C85C2 File Offset: 0x000C67C2
		private static void OnAnyAnimatorUpdated()
		{
			AnimCullingStatsCommand._animUpdates++;
		}

		// Token: 0x06003CE8 RID: 15592 RVA: 0x000C85D0 File Offset: 0x000C67D0
		private static void OnUpdate()
		{
			if (!StaticUnityMethods.IsPlaying)
			{
				AnimCullingStatsCommand.Enabled = false;
				return;
			}
			int num = 0;
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				IFpcRole fpcRole = referenceHub.roleManager.CurrentRole as IFpcRole;
				if (fpcRole != null)
				{
					AnimatedCharacterModel animatedCharacterModel = fpcRole.FpcModule.CharacterModelInstance as AnimatedCharacterModel;
					CullingSubcontroller cullingSubcontroller;
					if (animatedCharacterModel != null && animatedCharacterModel.TryGetSubcontroller<CullingSubcontroller>(out cullingSubcontroller))
					{
						num++;
					}
				}
			}
			AnimCullingStatsCommand.AnimSample animSample = new AnimCullingStatsCommand.AnimSample(AnimCullingStatsCommand._animUpdates, num);
			int num2 = AnimCullingStatsCommand._totalMeasurements % AnimCullingStatsCommand.Samples.Length;
			AnimCullingStatsCommand.Samples[num2] = animSample;
			AnimCullingStatsCommand._animUpdates = 0;
			AnimCullingStatsCommand._totalMeasurements++;
			AnimCullingStatsCommand._lastSample = animSample;
		}

		// Token: 0x06003CE9 RID: 15593 RVA: 0x000C86A4 File Offset: 0x000C68A4
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			bool flag = !AnimCullingStatsCommand.Enabled;
			ReferenceHub referenceHub;
			if (flag && (!ReferenceHub.TryGetLocalHub(out referenceHub) || !referenceHub.queryProcessor.GameplayData))
			{
				response = "Gameplay data permission (in Remote Admin) is required to enable animation culling stats.";
				return false;
			}
			AnimCullingStatsCommand.Enabled = flag;
			response = "Animation culling stats " + (AnimCullingStatsCommand.Enabled ? "enabled" : "hidden") + ".";
			return true;
		}

		// Token: 0x04002C92 RID: 11410
		private static readonly AnimCullingStatsCommand.AnimSample[] Samples = new AnimCullingStatsCommand.AnimSample[100];

		// Token: 0x04002C93 RID: 11411
		private static bool _isEnabled;

		// Token: 0x04002C94 RID: 11412
		private static int _animUpdates;

		// Token: 0x04002C95 RID: 11413
		private static int _totalMeasurements;

		// Token: 0x04002C96 RID: 11414
		private static AnimCullingStatsCommand.AnimSample _lastSample;

		// Token: 0x02000A8A RID: 2698
		private readonly struct AnimSample
		{
			// Token: 0x17000DBC RID: 3516
			// (get) Token: 0x06003CEC RID: 15596 RVA: 0x000C8752 File Offset: 0x000C6952
			public int Percent
			{
				get
				{
					return Mathf.RoundToInt((float)(100 * this.Updates) / (float)this.TotalModels);
				}
			}

			// Token: 0x06003CED RID: 15597 RVA: 0x000C876B File Offset: 0x000C696B
			public override string ToString()
			{
				return string.Format("{0}/{1} ({2}%)", this.Updates, this.TotalModels, this.Percent);
			}

			// Token: 0x06003CEE RID: 15598 RVA: 0x000C8798 File Offset: 0x000C6998
			public AnimSample(int updates, int totalModels)
			{
				this.Updates = updates;
				this.TotalModels = totalModels;
			}

			// Token: 0x04002C97 RID: 11415
			public readonly int Updates;

			// Token: 0x04002C98 RID: 11416
			public readonly int TotalModels;
		}

		// Token: 0x02000A8B RID: 2699
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002C99 RID: 11417
			public static Action <0>__OnGUI;

			// Token: 0x04002C9A RID: 11418
			public static Action <1>__OnUpdate;

			// Token: 0x04002C9B RID: 11419
			public static Action <2>__OnAnyAnimatorUpdated;
		}
	}
}
