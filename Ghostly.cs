using System;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp106;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008BB RID: 2235
	public class Ghostly : StatusEffectBase, IFpcCollisionModifier
	{
		// Token: 0x17000997 RID: 2455
		// (get) Token: 0x060032CA RID: 13002 RVA: 0x0006CB8D File Offset: 0x0006AD8D
		public LayerMask DetectionMask
		{
			get
			{
				return Scp106MovementModule.PassableDetectionMask;
			}
		}

		// Token: 0x17000998 RID: 2456
		// (get) Token: 0x060032CB RID: 13003 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x060032CC RID: 13004 RVA: 0x000A93BC File Offset: 0x000A75BC
		public void ProcessColliders(ArraySegment<Collider> detections)
		{
			foreach (Collider collider in detections)
			{
				bool flag;
				Scp106MovementModule.GetSlowdownFromCollider(collider, out flag);
				collider.enabled = !flag;
			}
		}

		// Token: 0x060032CD RID: 13005 RVA: 0x000A9418 File Offset: 0x000A7618
		protected override void Enabled()
		{
			base.Enabled();
			IFpcRole fpcRole = base.Hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole == null)
			{
				return;
			}
			FpcCollisionProcessor.AddModifier(this, fpcRole);
		}

		// Token: 0x060032CE RID: 13006 RVA: 0x000A944C File Offset: 0x000A764C
		protected override void Disabled()
		{
			base.Disabled();
			FpcCollisionProcessor.RemoveModifier(this);
		}
	}
}
