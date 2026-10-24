using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Achievements.Handlers;
using InventorySystem;
using Mirror;
using UnityEngine;

namespace Achievements
{
	// Token: 0x02000AE8 RID: 2792
	public static class AchievementManager
	{
		// Token: 0x17000E60 RID: 3680
		// (get) Token: 0x06003F8B RID: 16267 RVA: 0x000D2A2F File Offset: 0x000D0C2F
		// (set) Token: 0x06003F8C RID: 16268 RVA: 0x000D2A36 File Offset: 0x000D0C36
		public static bool AchievementsDisabled { get; set; } = false;

		// Token: 0x06003F8D RID: 16269 RVA: 0x000D2A40 File Offset: 0x000D0C40
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			Action value;
			if ((value = AchievementManager.<>O.<0>__LocalPlayerStarted) == null)
			{
				value = (AchievementManager.<>O.<0>__LocalPlayerStarted = new Action(AchievementManager.LocalPlayerStarted));
			}
			Inventory.OnLocalClientStarted += value;
			AchievementHandlerBase[] handlers = AchievementManager.Handlers;
			for (int i = 0; i < handlers.Length; i++)
			{
				handlers[i].OnInitialize();
			}
		}

		// Token: 0x06003F8E RID: 16270 RVA: 0x000D2A8C File Offset: 0x000D0C8C
		private static void LocalPlayerStarted()
		{
			Action<AchievementManager.AchievementMessage> handler;
			if ((handler = AchievementManager.<>O.<1>__ClientMessageReceived) == null)
			{
				handler = (AchievementManager.<>O.<1>__ClientMessageReceived = new Action<AchievementManager.AchievementMessage>(AchievementManager.ClientMessageReceived));
			}
			NetworkClient.ReplaceHandler<AchievementManager.AchievementMessage>(handler, true);
			AchievementHandlerBase[] handlers = AchievementManager.Handlers;
			for (int i = 0; i < handlers.Length; i++)
			{
				handlers[i].OnRoundStarted();
			}
		}

		// Token: 0x06003F8F RID: 16271 RVA: 0x000D2AD8 File Offset: 0x000D0CD8
		private static void ClientMessageReceived(AchievementManager.AchievementMessage msg)
		{
			Achievement achievement;
			if (!AchievementManager.Achievements.TryGetValue((AchievementName)msg.AchievementId, out achievement))
			{
				return;
			}
			if (!achievement.ActivatedByServer)
			{
				return;
			}
			achievement.Achieve();
		}

		// Token: 0x06003F90 RID: 16272 RVA: 0x000D2B0C File Offset: 0x000D0D0C
		// Note: this type is marked as 'beforefieldinit'.
		static AchievementManager()
		{
			Dictionary<AchievementName, Achievement> dictionary = new Dictionary<AchievementName, Achievement>();
			dictionary[AchievementName.TurnThemAll] = new Achievement("turnthemall", 559416662751707317L, true);
			dictionary[AchievementName.Pacified] = new Achievement("unvoluntaryragequit", 559416663020142679L, true);
			dictionary[AchievementName.MelancholyOfDecay] = new Achievement("newb", 559417282514780170L, true);
			dictionary[AchievementName.DontBlink] = new Achievement("firsttime", 559417282594471936L, true);
			dictionary[AchievementName.LightsOut] = new Achievement("arescue", 559417357718781955L, true);
			dictionary[AchievementName.ItsAlwaysLeft] = new Achievement("awayout", 559417906258247700L, true);
			dictionary[AchievementName.AccessGranted] = new Achievement("betrayal", 559417906199265281L, true);
			dictionary[AchievementName.DeltaCommand] = new Achievement("chaos", 559418104510152720L, true);
			dictionary[AchievementName.ProceedWithCaution] = new Achievement("electrocuted", 559418104552095768L, true);
			dictionary[AchievementName.Friendship] = new Achievement("friendship", 564138635457331343L, true);
			dictionary[AchievementName.ForScience] = new Achievement("forscience", 564138635566514176L, true);
			dictionary[AchievementName.IsThisThingOn] = new Achievement("isthisthingon", 564138635448942717L, true);
			dictionary[AchievementName.FireInTheHole] = new Achievement("iwanttobearocket", 564138635402936330L, true);
			dictionary[AchievementName.HeWillBeBack] = new Achievement("larryisyourfriend", 564138674611290119L, true);
			dictionary[AchievementName.BePoliteBeEfficient] = new Achievement("pewpew", 564138635105140767L, true);
			dictionary[AchievementName.ExecutiveAccess] = new Achievement("power", 564139505062379520L, true);
			dictionary[AchievementName.SecureContainProtect] = new Achievement("securecontainprotect", 564139504848601118L, true);
			dictionary[AchievementName.TMinus] = new Achievement("tminus", 564139505112580131L, false);
			dictionary[AchievementName.ChangeInCommand] = new Achievement("tableshaveturned", 564139505834000508L, true);
			dictionary[AchievementName.ThatCanBeUseful] = new Achievement("thatcanbeusefull", 564139505045733386L, true);
			dictionary[AchievementName.JustResources] = new Achievement("justresources", "dboys_killed", 909163354143211570L, 50, true);
			dictionary[AchievementName.ThatWasClose] = new Achievement("thatwasclose", 564140195134439474L, true);
			dictionary[AchievementName.SomethingDoneRight] = new Achievement("timetodoitmyself", 564140195465658378L, true);
			dictionary[AchievementName.WalkItOff] = new Achievement("gravity", 564140195017129984L, true);
			dictionary[AchievementName.AnomalouslyEfficient] = new Achievement("wowreally", 564140195382034434L, true);
			dictionary[AchievementName.MicrowaveMeal] = new Achievement("zap", 564140195000090635L, true);
			dictionary[AchievementName.EscapeArtist] = new Achievement("escapeartist", 564140574047731713L, true);
			dictionary[AchievementName.Escape207] = new Achievement("escape207", 638780450600386580L, true);
			dictionary[AchievementName.CrisisAverted] = new Achievement("crisisaverted", 638780450612969483L, true);
			dictionary[AchievementName.DidntEvenFeelThat] = new Achievement("didntevenfeelthat", 638780488433139752L, true);
			dictionary[AchievementName.IllPassThanks] = new Achievement("illpassthanks", 638780450311110658L, true);
			dictionary[AchievementName.Overcurrent] = new Achievement("attemptedrecharge", 638780450776809472L, true);
			dictionary[AchievementName.PropertyOfChaos] = new Achievement("propertyofchaos", 638780450667626506L, true);
			dictionary[AchievementName.Overtime] = new Achievement("overtime", 0L, true);
			dictionary[AchievementName.RuleBreaker] = new Achievement("rulebreaker", 0L, true);
			dictionary[AchievementName.CompleteTheMission] = new Achievement("completethemission", 0L, true);
			dictionary[AchievementName.ArmyOfOne] = new Achievement("armyofone", 0L, true);
			dictionary[AchievementName.LMGG] = new Achievement("lmgg", 0L, true);
			dictionary[AchievementName.OnSpeakingTerms] = new Achievement("onspeakingterms", 0L, true);
			dictionary[AchievementName.HatsOffToYou] = new Achievement("hatsofftoyou", 0L, true);
			dictionary[AchievementName.AmnesticAmbush] = new Achievement("amnesticambush", 0L, true);
			dictionary[AchievementName.AfterlifeCommunicator] = new Achievement("afterlifecommunicator", 0L, true);
			dictionary[AchievementName.SignalLost] = new Achievement("signallost", 0L, true);
			dictionary[AchievementName.Hawkeye] = new Achievement("hawkeye", 0L, true);
			dictionary[AchievementName.ThinkFast] = new Achievement("thinkfast", 0L, true);
			dictionary[AchievementName.TrilateralTermination] = new Achievement("trilateraltermination", 0L, true);
			dictionary[AchievementName.MutuallyAssuredDestruction] = new Achievement("mutuallyassureddestruction", 0L, true);
			dictionary[AchievementName.UndeadSpaceProgram] = new Achievement("undeadspaceprogram", 0L, true);
			dictionary[AchievementName.ArizonaRanger] = new Achievement("arizonaranger", 0L, true);
			dictionary[AchievementName.Matador] = new Achievement("matador", 0L, true);
			dictionary[AchievementName.ToothAndNail] = new Achievement("toothandnail", 0L, true);
			AchievementManager.Achievements = dictionary;
		}

		// Token: 0x04002E38 RID: 11832
		public static readonly AchievementHandlerBase[] Handlers = new AchievementHandlerBase[]
		{
			new GeneralDamageHandler(),
			new EscapeHandler(),
			new ItemPickupHandler(),
			new BePoliteBeEfficientHandler(),
			new RespawnHandler(),
			new Scp914UpgradeHandler(),
			new TurnThemAllHandler(),
			new GeneralKillsHandler(),
			new IntercomHandler(),
			new ChangeInCommandHandler(),
			new HeWillBeBackHandler(),
			new IllPassThanksHandler(),
			new MelancholyOfDecayHandler(),
			new ProceedWithCautionHandler(),
			new SecureContainProtectHandler(),
			new DontBlinkHandler(),
			new OvertimeHandler(),
			new RuleBreakerHandler(),
			new CompleteTheMissionHandler(),
			new ArmyOfOneHandler(),
			new LMGGHandler(),
			new OnSpeakingTerms(),
			new HatsOffToYouHandler(),
			new GeneralUseItemHandler(),
			new SignalLostHandler(),
			new JustResourcesHandler(),
			new HawkeyeHandler(),
			new ThinkFastHandler(),
			new TrilateralTerminationHandler(),
			new MutuallyAssuredDestructionHandler(),
			new ToothAndNailHandler()
		};

		// Token: 0x04002E39 RID: 11833
		public static readonly Dictionary<AchievementName, Achievement> Achievements;

		// Token: 0x02000AE9 RID: 2793
		public struct AchievementMessage : NetworkMessage
		{
			// Token: 0x04002E3A RID: 11834
			public byte AchievementId;
		}

		// Token: 0x02000AEA RID: 2794
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E3B RID: 11835
			public static Action <0>__LocalPlayerStarted;

			// Token: 0x04002E3C RID: 11836
			public static Action<AchievementManager.AchievementMessage> <1>__ClientMessageReceived;
		}
	}
}
