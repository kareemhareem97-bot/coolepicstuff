using System;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.PlayableScps.Scp096;
using PlayerRoles.PlayableScps.Scp173;
using PlayerRoles.PlayableScps.Scp939;
using PlayerRoles.Subroutines;
using UnityEngine;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010E9 RID: 4329
	public class AbilityCooldownReset : Scp2536AnomalousGiftBase
	{
		// Token: 0x170013F9 RID: 5113
		// (get) Token: 0x06006052 RID: 24658 RVA: 0x0003F21F File Offset: 0x0003D41F
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Five;
			}
		}

		// Token: 0x06006053 RID: 24659 RVA: 0x00141CDC File Offset: 0x0013FEDC
		public override void ServerGrant(ReferenceHub hub)
		{
			base.ServerGrant(hub);
			PlayerRoleBase currentRole = hub.roleManager.CurrentRole;
			Scp173Role scp173Role = currentRole as Scp173Role;
			if (scp173Role != null)
			{
				AbilityCooldownReset.ClearCooldown<Scp173BreakneckSpeedsAbility>(scp173Role, delegate(Scp173BreakneckSpeedsAbility s)
				{
					s.Cooldown.Clear();
				}, true);
				AbilityCooldownReset.ClearCooldown<Scp173TantrumAbility>(scp173Role, delegate(Scp173TantrumAbility s)
				{
					s.Cooldown.Clear();
				}, true);
				return;
			}
			Scp939Role scp939Role = currentRole as Scp939Role;
			if (scp939Role != null)
			{
				AbilityCooldownReset.ClearCooldown<Scp939AmnesticCloudAbility>(scp939Role, delegate(Scp939AmnesticCloudAbility s)
				{
					s.Cooldown.Clear();
				}, true);
				return;
			}
			Scp049Role scp049Role = currentRole as Scp049Role;
			if (scp049Role != null)
			{
				AbilityCooldownReset.ClearCooldown<Scp049SenseAbility>(scp049Role, delegate(Scp049SenseAbility s)
				{
					s.Cooldown.Clear();
				}, true);
				AbilityCooldownReset.ClearCooldown<Scp049CallAbility>(scp049Role, delegate(Scp049CallAbility s)
				{
					s.Cooldown.Clear();
				}, true);
				return;
			}
			Scp096Role scp096Role = currentRole as Scp096Role;
			if (scp096Role == null)
			{
				Debug.LogWarning("Attempted to gift AbilityCooldownReset to " + currentRole.GetType().Name + " with no defined reward.");
				return;
			}
			AbilityCooldownReset.ClearCooldown<Scp096ChargeAbility>(scp096Role, delegate(Scp096ChargeAbility s)
			{
				s.Cooldown.Clear();
			}, true);
		}

		// Token: 0x06006054 RID: 24660 RVA: 0x00141E3C File Offset: 0x0014003C
		public override bool CanBeGranted(ReferenceHub hub)
		{
			if (!base.CanBeGranted(hub))
			{
				return false;
			}
			PlayerRoleBase currentRole = hub.roleManager.CurrentRole;
			Scp173Role scp173Role = currentRole as Scp173Role;
			if (scp173Role != null)
			{
				bool flag = AbilityCooldownReset.IsOnCooldown<Scp173BreakneckSpeedsAbility>(scp173Role, (Scp173BreakneckSpeedsAbility s) => s.Cooldown);
				bool flag2 = AbilityCooldownReset.IsOnCooldown<Scp173TantrumAbility>(scp173Role, (Scp173TantrumAbility s) => s.Cooldown);
				return flag || flag2;
			}
			Scp939Role scp939Role = currentRole as Scp939Role;
			if (scp939Role != null)
			{
				return AbilityCooldownReset.IsOnCooldown<Scp939AmnesticCloudAbility>(scp939Role, (Scp939AmnesticCloudAbility s) => s.Cooldown);
			}
			Scp049Role scp049Role = currentRole as Scp049Role;
			if (scp049Role != null)
			{
				bool flag3 = AbilityCooldownReset.IsOnCooldown<Scp049SenseAbility>(scp049Role, (Scp049SenseAbility s) => s.Cooldown);
				bool flag4 = AbilityCooldownReset.IsOnCooldown<Scp049CallAbility>(scp049Role, (Scp049CallAbility s) => s.Cooldown);
				return flag3 || flag4;
			}
			Scp096Role scp096Role = currentRole as Scp096Role;
			if (scp096Role == null)
			{
				Debug.LogWarning("Attempted to gift AbilityCooldownReset to " + currentRole.GetType().Name + " with no defined reward.");
				return false;
			}
			return AbilityCooldownReset.IsOnCooldown<Scp096ChargeAbility>(scp096Role, (Scp096ChargeAbility s) => s.Cooldown);
		}

		// Token: 0x06006055 RID: 24661 RVA: 0x00141FB0 File Offset: 0x001401B0
		private static bool IsOnCooldown<TSubroutine>(ISubroutinedRole role, Func<TSubroutine, AbilityCooldown> cooldownCheck) where TSubroutine : SubroutineBase
		{
			TSubroutine arg;
			return role.SubroutineModule.TryGetSubroutine<TSubroutine>(out arg) && !((cooldownCheck != null) ? cooldownCheck(arg) : null).IsReady;
		}

		// Token: 0x06006056 RID: 24662 RVA: 0x00141FE4 File Offset: 0x001401E4
		private static void ClearCooldown<TSubroutine>(ISubroutinedRole role, Action<TSubroutine> cooldownResetAction, bool sendResetMessage = true) where TSubroutine : SubroutineBase
		{
			TSubroutine tsubroutine;
			if (!role.SubroutineModule.TryGetSubroutine<TSubroutine>(out tsubroutine))
			{
				return;
			}
			if (cooldownResetAction != null)
			{
				cooldownResetAction(tsubroutine);
			}
			if (sendResetMessage)
			{
				AbilityCooldownReset.UpdateCooldown(tsubroutine);
			}
		}

		// Token: 0x06006057 RID: 24663 RVA: 0x00142019 File Offset: 0x00140219
		private static void UpdateCooldown(SubroutineBase subroutine)
		{
			NetworkServer.SendToReady<SubroutineMessage>(new SubroutineMessage(subroutine, true), 0);
		}
	}
}
