using System;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using Subtitles;

namespace CustomPlayerEffects
{
	// Token: 0x020008B3 RID: 2227
	public class Decontaminating : TickingEffectBase
	{
		// Token: 0x17000983 RID: 2435
		// (get) Token: 0x060032A2 RID: 12962 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool AllowEnabling
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060032A3 RID: 12963 RVA: 0x000A913C File Offset: 0x000A733C
		protected override void OnTick()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			IHealthbarRole healthbarRole = base.Hub.roleManager.CurrentRole as IHealthbarRole;
			if (healthbarRole == null)
			{
				return;
			}
			float damage = healthbarRole.MaxHealth / 10f + 5f * this.numberOfTicks;
			this.numberOfTicks += 1f;
			DamageHandlerBase.CassieAnnouncement cassieAnnouncement = new DamageHandlerBase.CassieAnnouncement
			{
				Announcement = "LOST IN DECONTAMINATION SEQUENCE",
				SubtitleParts = new SubtitlePart[]
				{
					new SubtitlePart(SubtitleType.LostInDecontamination, null)
				}
			};
			base.Hub.playerStats.DealDamage(new UniversalDamageHandler(damage, DeathTranslations.Decontamination, cassieAnnouncement));
		}

		// Token: 0x060032A4 RID: 12964 RVA: 0x000A91E2 File Offset: 0x000A73E2
		protected override void Enabled()
		{
			base.Enabled();
		}

		// Token: 0x060032A5 RID: 12965 RVA: 0x000A91EA File Offset: 0x000A73EA
		protected override void Disabled()
		{
			base.Disabled();
			this.numberOfTicks = 0f;
		}

		// Token: 0x0400275B RID: 10075
		private const string CassieAnnouncement = "LOST IN DECONTAMINATION SEQUENCE";

		// Token: 0x0400275C RID: 10076
		private float numberOfTicks;

		// Token: 0x0400275D RID: 10077
		public float FogFadeInSpeed;

		// Token: 0x0400275E RID: 10078
		public float FogFadeOutSpeed;
	}
}
