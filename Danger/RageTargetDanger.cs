using System;
using PlayerRoles.PlayableScps.Scp096;

namespace CustomPlayerEffects.Danger
{
	// Token: 0x02000919 RID: 2329
	public class RageTargetDanger : DangerStackBase
	{
		// Token: 0x17000A77 RID: 2679
		// (get) Token: 0x0600351F RID: 13599 RVA: 0x000ADEE9 File Offset: 0x000AC0E9
		// (set) Token: 0x06003520 RID: 13600 RVA: 0x000ADEF1 File Offset: 0x000AC0F1
		public override float DangerValue { get; set; } = 2f;

		// Token: 0x06003521 RID: 13601 RVA: 0x000ADEFC File Offset: 0x000AC0FC
		public override void Initialize(ReferenceHub target)
		{
			base.Initialize(target);
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				Scp096Role scp096Role = referenceHub.roleManager.CurrentRole as Scp096Role;
				Scp096TargetsTracker scp096TargetsTracker;
				if (scp096Role != null && scp096Role.SubroutineModule.TryGetSubroutine<Scp096TargetsTracker>(out scp096TargetsTracker) && scp096TargetsTracker.Targets.Contains(base.Owner))
				{
					this.IsActive = true;
				}
			}
			Scp096TargetsTracker.OnTargetAdded += this.OnTargetAdded;
			Scp096TargetsTracker.OnTargetRemoved += this.OnTargetRemoved;
		}

		// Token: 0x06003522 RID: 13602 RVA: 0x000ADFAC File Offset: 0x000AC1AC
		public override void Dispose()
		{
			base.Dispose();
			Scp096TargetsTracker.OnTargetAdded -= this.OnTargetAdded;
			Scp096TargetsTracker.OnTargetRemoved -= this.OnTargetRemoved;
		}

		// Token: 0x06003523 RID: 13603 RVA: 0x000ADFD6 File Offset: 0x000AC1D6
		private void OnTargetAdded(ReferenceHub owner, ReferenceHub target)
		{
			this.UpdateState(true, target);
		}

		// Token: 0x06003524 RID: 13604 RVA: 0x000ADFE0 File Offset: 0x000AC1E0
		private void OnTargetRemoved(ReferenceHub owner, ReferenceHub target)
		{
			this.UpdateState(false, target);
		}

		// Token: 0x06003525 RID: 13605 RVA: 0x000ADFEA File Offset: 0x000AC1EA
		private void UpdateState(bool targetAdded, ReferenceHub targetedHub)
		{
			if (targetedHub == null)
			{
				return;
			}
			if (targetedHub != base.Owner)
			{
				return;
			}
			this.IsActive = targetAdded;
		}
	}
}
