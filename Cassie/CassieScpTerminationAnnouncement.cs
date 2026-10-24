using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Footprinting;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerStatsSystem;
using Subtitles;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace Cassie
{
	// Token: 0x02000ABA RID: 2746
	public class CassieScpTerminationAnnouncement : CassieAnnouncement
	{
		// Token: 0x140000B2 RID: 178
		// (add) Token: 0x06003DF8 RID: 15864 RVA: 0x000CC974 File Offset: 0x000CAB74
		// (remove) Token: 0x06003DF9 RID: 15865 RVA: 0x000CC9A8 File Offset: 0x000CABA8
		public static event Action<CassieScpTerminationAnnouncement> OnAnnouncementFinishedPlaying;

		// Token: 0x17000E18 RID: 3608
		// (get) Token: 0x06003DFA RID: 15866 RVA: 0x000CC9DB File Offset: 0x000CABDB
		public IReadOnlyList<Footprint> Victims
		{
			get
			{
				return this._victims;
			}
		}

		// Token: 0x17000E19 RID: 3609
		// (get) Token: 0x06003DFB RID: 15867 RVA: 0x000CC9E3 File Offset: 0x000CABE3
		public override bool ReadyToPlay
		{
			get
			{
				return this._remainingWait <= 0f;
			}
		}

		// Token: 0x06003DFC RID: 15868 RVA: 0x000CC9F8 File Offset: 0x000CABF8
		public CassieScpTerminationAnnouncement(Footprint victim, string tts, SubtitlePart[] subtitles) : base(default(CassieTtsPayload), -1f, 1f)
		{
			this._victims = new List<Footprint>
			{
				victim
			};
			this._announcementTts = tts;
			this._subtitles = (subtitles ?? Array.Empty<SubtitlePart>());
			this._remainingWait = 1f;
		}

		// Token: 0x06003DFD RID: 15869 RVA: 0x000CCA52 File Offset: 0x000CAC52
		public override void UpdateQueued()
		{
			base.UpdateQueued();
			if (this._remainingWait > 0f)
			{
				this._remainingWait -= Time.deltaTime;
			}
		}

		// Token: 0x06003DFE RID: 15870 RVA: 0x000CCA7C File Offset: 0x000CAC7C
		public override void OnStartedPlaying()
		{
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			List<SubtitlePart> list = new List<SubtitlePart>();
			for (int i = 0; i < this.Victims.Count; i++)
			{
				string text;
				string value;
				CassieScpTerminationAnnouncement.ConvertSCP(this.Victims[i].Role, out text, out value);
				if (i == 0)
				{
					stringBuilder.Append("SCP ");
					stringBuilder.Append(value);
				}
				else
				{
					stringBuilder.Append(". SCP ");
					stringBuilder.Append(value);
				}
				list.Add(new SubtitlePart(SubtitleType.SCP, new string[]
				{
					text
				}));
			}
			stringBuilder.Append(this._announcementTts);
			list.AddRange(this._subtitles);
			this.Payload = new CassieTtsPayload(StringBuilderPool.Shared.ToStringReturn(stringBuilder), list.ToArray());
			base.OnStartedPlaying();
		}

		// Token: 0x06003DFF RID: 15871 RVA: 0x000CCB4B File Offset: 0x000CAD4B
		public override void OnFinishedPlaying()
		{
			base.OnFinishedPlaying();
			Action<CassieScpTerminationAnnouncement> onAnnouncementFinishedPlaying = CassieScpTerminationAnnouncement.OnAnnouncementFinishedPlaying;
			if (onAnnouncementFinishedPlaying == null)
			{
				return;
			}
			onAnnouncementFinishedPlaying(this);
		}

		// Token: 0x06003E00 RID: 15872 RVA: 0x000CCB64 File Offset: 0x000CAD64
		public static void ConvertSCP(RoleTypeId role, out string withoutSpace, out string withSpace)
		{
			PlayerRoleBase playerRoleBase;
			if (!role.TryGetRoleTemplate(out playerRoleBase))
			{
				withoutSpace = string.Empty;
				withSpace = string.Empty;
				return;
			}
			CassieScpTerminationAnnouncement.ConvertSCP(playerRoleBase.RoleName, out withoutSpace, out withSpace);
		}

		// Token: 0x06003E01 RID: 15873 RVA: 0x000CCB98 File Offset: 0x000CAD98
		public static void ConvertSCP(string roleName, out string withoutSpace, out string withSpace)
		{
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			string[] array = roleName.Split('-', StringSplitOptions.None);
			if (array.Length < 2)
			{
				Debug.LogError("Cassie role cannot be split by '-'. Possibly missing translation.");
				withoutSpace = "404";
				withSpace = "4 0 4";
				return;
			}
			withoutSpace = array[1];
			foreach (char value in withoutSpace)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(' ');
			}
			withSpace = StringBuilderPool.Shared.ToStringReturn(stringBuilder);
		}

		// Token: 0x06003E02 RID: 15874 RVA: 0x000CCC1C File Offset: 0x000CAE1C
		public static void AnnounceScpTermination(ReferenceHub scp, DamageHandlerBase hit)
		{
			if (!scp.IsSCP(false))
			{
				if (!scp.IsFlamingo(true))
				{
					return;
				}
				if (ReferenceHub.AllHubs.Count((ReferenceHub hub) => hub.IsFlamingo(true)) > 1)
				{
					return;
				}
			}
			string announcement = hit.CassieDeathAnnouncement.Announcement;
			SubtitlePart[] subtitleParts = hit.CassieDeathAnnouncement.SubtitleParts;
			if (string.IsNullOrEmpty(announcement))
			{
				return;
			}
			foreach (CassieAnnouncement cassieAnnouncement in CassieAnnouncementDispatcher.AllAnnouncementsPreview)
			{
				CassieScpTerminationAnnouncement cassieScpTerminationAnnouncement = cassieAnnouncement as CassieScpTerminationAnnouncement;
				if (cassieScpTerminationAnnouncement != null && !(cassieScpTerminationAnnouncement._announcementTts != announcement) && SubtitlePart.CheckEqualValues(cassieScpTerminationAnnouncement._subtitles, subtitleParts))
				{
					cassieScpTerminationAnnouncement._victims.Add(new Footprint(scp));
					cassieScpTerminationAnnouncement._remainingWait = 1f;
					return;
				}
			}
			CassieQueuingScpTerminationEventArgs cassieQueuingScpTerminationEventArgs = new CassieQueuingScpTerminationEventArgs(scp, announcement, subtitleParts, hit);
			ServerEvents.OnCassieQueuingScpTermination(cassieQueuingScpTerminationEventArgs);
			if (!cassieQueuingScpTerminationEventArgs.IsAllowed)
			{
				return;
			}
			announcement = cassieQueuingScpTerminationEventArgs.Announcement;
			subtitleParts = cassieQueuingScpTerminationEventArgs.SubtitleParts;
			new CassieScpTerminationAnnouncement(new Footprint(scp), announcement, subtitleParts).AddToQueue();
			ServerEvents.OnCassieQueuedScpTermination(new CassieQueuedScpTerminationEventArgs(scp, announcement, subtitleParts, hit));
		}

		// Token: 0x06003E03 RID: 15875 RVA: 0x000CCD54 File Offset: 0x000CAF54
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			Action<ReferenceHub, DamageHandlerBase> value;
			if ((value = CassieScpTerminationAnnouncement.<>O.<0>__AnnounceScpTermination) == null)
			{
				value = (CassieScpTerminationAnnouncement.<>O.<0>__AnnounceScpTermination = new Action<ReferenceHub, DamageHandlerBase>(CassieScpTerminationAnnouncement.AnnounceScpTermination));
			}
			PlayerStats.OnAnyPlayerDied += value;
		}

		// Token: 0x04002D58 RID: 11608
		private const float WaitForOtherScpsDuration = 1f;

		// Token: 0x04002D59 RID: 11609
		private const float AnnouncementPriority = -1f;

		// Token: 0x04002D5A RID: 11610
		private readonly List<Footprint> _victims;

		// Token: 0x04002D5B RID: 11611
		private readonly string _announcementTts;

		// Token: 0x04002D5C RID: 11612
		private readonly SubtitlePart[] _subtitles;

		// Token: 0x04002D5D RID: 11613
		private float _remainingWait;

		// Token: 0x02000ABB RID: 2747
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002D5E RID: 11614
			public static Action<ReferenceHub, DamageHandlerBase> <0>__AnnounceScpTermination;
		}
	}
}
