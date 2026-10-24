using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mirror;
using RoundRestarting;
using UnityEngine;
using Utils.Networking;
using Utils.NonAllocLINQ;

namespace Cassie
{
	// Token: 0x02000AAF RID: 2735
	public static class CassieAnnouncementDispatcher
	{
		// Token: 0x17000E0E RID: 3598
		// (get) Token: 0x06003DBB RID: 15803 RVA: 0x000CBABF File Offset: 0x000C9CBF
		public static IReadOnlyList<CassieAnnouncement> AllAnnouncementsPreview
		{
			get
			{
				return CassieAnnouncementDispatcher.AllAnnouncements;
			}
		}

		// Token: 0x17000E0F RID: 3599
		// (get) Token: 0x06003DBC RID: 15804 RVA: 0x000CBAC6 File Offset: 0x000C9CC6
		// (set) Token: 0x06003DBD RID: 15805 RVA: 0x000CBACD File Offset: 0x000C9CCD
		public static CassieAnnouncement CurrentAnnouncement { get; private set; }

		// Token: 0x17000E10 RID: 3600
		// (get) Token: 0x06003DBE RID: 15806 RVA: 0x000CBAD5 File Offset: 0x000C9CD5
		// (set) Token: 0x06003DBF RID: 15807 RVA: 0x000CBADC File Offset: 0x000C9CDC
		public static double AnnouncementFinishTime { get; private set; }

		// Token: 0x17000E11 RID: 3601
		// (get) Token: 0x06003DC0 RID: 15808 RVA: 0x000CBAE4 File Offset: 0x000C9CE4
		// (set) Token: 0x06003DC1 RID: 15809 RVA: 0x000CBAEB File Offset: 0x000C9CEB
		public static double NextAnnouncementTime { get; private set; }

		// Token: 0x06003DC2 RID: 15810 RVA: 0x000CBAF3 File Offset: 0x000C9CF3
		public static bool AddToQueue(this CassieAnnouncement announcement)
		{
			return CassieAnnouncementDispatcher.AllAnnouncements.AddIfNotContains(announcement);
		}

		// Token: 0x06003DC3 RID: 15811 RVA: 0x000CBB00 File Offset: 0x000C9D00
		public static bool Cancel(this CassieAnnouncement announcement)
		{
			int num = CassieAnnouncementDispatcher.AllAnnouncements.IndexOf(announcement);
			if (num < 0)
			{
				return false;
			}
			CassieAnnouncementDispatcher.AllAnnouncements[num].OnCancelled();
			CassieAnnouncementDispatcher.AllAnnouncements.RemoveAt(num);
			return true;
		}

		// Token: 0x06003DC4 RID: 15812 RVA: 0x000CBB3C File Offset: 0x000C9D3C
		public static void ClearAll()
		{
			while (CassieAnnouncementDispatcher.AllAnnouncements.Count > 0)
			{
				List<CassieAnnouncement> allAnnouncements = CassieAnnouncementDispatcher.AllAnnouncements;
				allAnnouncements[allAnnouncements.Count - 1].Cancel();
			}
			if (CassieAnnouncementDispatcher.CurrentAnnouncement != null)
			{
				CassieAnnouncementDispatcher.CurrentAnnouncement = null;
				CassieAnnouncementDispatcher.AnnouncementFinishTime = 0.0;
				CassieAnnouncementDispatcher.NextAnnouncementTime = 0.0;
			}
			new CassieTtsPayload(string.Empty, string.Empty, false).SendToAuthenticated(0);
		}

		// Token: 0x06003DC5 RID: 15813 RVA: 0x000CBBB0 File Offset: 0x000C9DB0
		public static int GetAbsolutePositionInQueue(this CassieAnnouncement announcement)
		{
			int num = 0;
			bool flag = false;
			foreach (CassieAnnouncement cassieAnnouncement in CassieAnnouncementDispatcher.AllAnnouncements)
			{
				if (cassieAnnouncement.Priority > announcement.Priority)
				{
					num++;
				}
				if (Mathf.Approximately(cassieAnnouncement.Priority, announcement.Priority) && !flag)
				{
					if (announcement == cassieAnnouncement)
					{
						flag = true;
					}
					else
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06003DC6 RID: 15814 RVA: 0x000CBC34 File Offset: 0x000C9E34
		public static int GetPositionWithinPriority(this CassieAnnouncement announcement)
		{
			int num = 0;
			foreach (CassieAnnouncement cassieAnnouncement in CassieAnnouncementDispatcher.AllAnnouncements)
			{
				if (Mathf.Approximately(cassieAnnouncement.Priority, announcement.Priority))
				{
					if (announcement == cassieAnnouncement)
					{
						return num;
					}
					num++;
				}
			}
			return num;
		}

		// Token: 0x06003DC7 RID: 15815 RVA: 0x000CBCA4 File Offset: 0x000C9EA4
		public static void SetPositionWithinPriority(this CassieAnnouncement announcement, int position)
		{
			CassieAnnouncementDispatcher.ReorganizerNonAlloc.Clear();
			for (int i = 0; i < CassieAnnouncementDispatcher.AllAnnouncements.Count; i++)
			{
				CassieAnnouncement cassieAnnouncement = CassieAnnouncementDispatcher.AllAnnouncements[i];
				if (Mathf.Approximately(cassieAnnouncement.Priority, announcement.Priority))
				{
					CassieAnnouncementDispatcher.AllAnnouncements.RemoveAt(i);
					if (cassieAnnouncement != announcement)
					{
						CassieAnnouncementDispatcher.ReorganizerNonAlloc.Add(cassieAnnouncement);
					}
					i--;
				}
			}
			int index = Mathf.Clamp(position, 0, CassieAnnouncementDispatcher.ReorganizerNonAlloc.Count);
			CassieAnnouncementDispatcher.ReorganizerNonAlloc.Insert(index, announcement);
			CassieAnnouncementDispatcher.AllAnnouncements.AddRange(CassieAnnouncementDispatcher.ReorganizerNonAlloc);
		}

		// Token: 0x06003DC8 RID: 15816 RVA: 0x000CBD3C File Offset: 0x000C9F3C
		private static void UpdateQueue()
		{
			if (!NetworkServer.active || !StaticUnityMethods.IsPlaying)
			{
				return;
			}
			CassieAnnouncement cassieAnnouncement = null;
			foreach (CassieAnnouncement cassieAnnouncement2 in CassieAnnouncementDispatcher.AllAnnouncements)
			{
				cassieAnnouncement2.UpdateQueued();
				if (cassieAnnouncement == null || cassieAnnouncement2.Priority > cassieAnnouncement.Priority)
				{
					cassieAnnouncement = cassieAnnouncement2;
				}
			}
			if (NetworkTime.time < CassieAnnouncementDispatcher.NextAnnouncementTime)
			{
				CassieAnnouncementDispatcher.UpdateCurrentAnnouncement();
				return;
			}
			if (cassieAnnouncement != null)
			{
				CassieAnnouncementDispatcher.AllAnnouncements.Remove(cassieAnnouncement);
				CassieAnnouncementDispatcher.PlayNewAnnouncement(cassieAnnouncement);
			}
		}

		// Token: 0x06003DC9 RID: 15817 RVA: 0x000CBDD8 File Offset: 0x000C9FD8
		private static void UpdateCurrentAnnouncement()
		{
			if (NetworkTime.time >= CassieAnnouncementDispatcher.AnnouncementFinishTime)
			{
				if (CassieAnnouncementDispatcher.CurrentAnnouncement != null)
				{
					CassieAnnouncementDispatcher.CurrentAnnouncement.OnFinishedPlaying();
					CassieAnnouncementDispatcher.CurrentAnnouncement = null;
				}
				return;
			}
			CassieAnnouncement currentAnnouncement = CassieAnnouncementDispatcher.CurrentAnnouncement;
			if (currentAnnouncement == null)
			{
				return;
			}
			currentAnnouncement.UpdatePlayed();
		}

		// Token: 0x06003DCA RID: 15818 RVA: 0x000CBE10 File Offset: 0x000CA010
		private static void PlayNewAnnouncement(CassieAnnouncement annc)
		{
			CassieAnnouncementDispatcher.CurrentAnnouncement = annc;
			CassieAnnouncementDispatcher.CurrentAnnouncement.OnStartedPlaying();
			CassieTtsPayload payload = CassieAnnouncementDispatcher.CurrentAnnouncement.Payload;
			float postAnnouncementCooldown = CassieAnnouncementDispatcher.CurrentAnnouncement.PostAnnouncementCooldown;
			float num;
			if (!CassieTtsAnnouncer.TryPlay(payload, out num))
			{
				return;
			}
			CassieAnnouncementDispatcher.AnnouncementFinishTime = NetworkTime.time + (double)num;
			CassieAnnouncementDispatcher.NextAnnouncementTime = CassieAnnouncementDispatcher.AnnouncementFinishTime + (double)postAnnouncementCooldown;
			payload.SendToAuthenticated(0);
		}

		// Token: 0x06003DCB RID: 15819 RVA: 0x000CBE70 File Offset: 0x000CA070
		private static void ClientHandleMessage(CassieTtsPayload tts)
		{
			if (!StaticUnityMethods.IsPlaying || NetworkServer.active)
			{
				return;
			}
			float num;
			CassieTtsAnnouncer.TryPlay(tts, out num);
		}

		// Token: 0x06003DCC RID: 15820 RVA: 0x000CBE95 File Offset: 0x000CA095
		private static void Reset()
		{
			CassieAnnouncementDispatcher.AllAnnouncements.Clear();
			CassieAnnouncementDispatcher.CurrentAnnouncement = null;
			CassieAnnouncementDispatcher.NextAnnouncementTime = 0.0;
		}

		// Token: 0x06003DCD RID: 15821 RVA: 0x000CBEB8 File Offset: 0x000CA0B8
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			Action value;
			if ((value = CassieAnnouncementDispatcher.<>O.<0>__UpdateQueue) == null)
			{
				value = (CassieAnnouncementDispatcher.<>O.<0>__UpdateQueue = new Action(CassieAnnouncementDispatcher.UpdateQueue));
			}
			StaticUnityMethods.OnUpdate += value;
			Action value2;
			if ((value2 = CassieAnnouncementDispatcher.<>O.<1>__Reset) == null)
			{
				value2 = (CassieAnnouncementDispatcher.<>O.<1>__Reset = new Action(CassieAnnouncementDispatcher.Reset));
			}
			CassieTtsAnnouncer.OnSingletonSet += value2;
			CustomNetworkManager.OnClientReady += delegate()
			{
				Action<CassieTtsPayload> handler;
				if ((handler = CassieAnnouncementDispatcher.<>O.<2>__ClientHandleMessage) == null)
				{
					handler = (CassieAnnouncementDispatcher.<>O.<2>__ClientHandleMessage = new Action<CassieTtsPayload>(CassieAnnouncementDispatcher.ClientHandleMessage));
				}
				NetworkClient.ReplaceHandler<CassieTtsPayload>(handler, true);
			};
			Action value3;
			if ((value3 = CassieAnnouncementDispatcher.<>O.<3>__ClearAll) == null)
			{
				value3 = (CassieAnnouncementDispatcher.<>O.<3>__ClearAll = new Action(CassieAnnouncementDispatcher.ClearAll));
			}
			RoundRestart.OnRestartTriggered += value3;
		}

		// Token: 0x04002D23 RID: 11555
		private static readonly List<CassieAnnouncement> ReorganizerNonAlloc = new List<CassieAnnouncement>();

		// Token: 0x04002D24 RID: 11556
		private static readonly List<CassieAnnouncement> AllAnnouncements = new List<CassieAnnouncement>();

		// Token: 0x02000AB0 RID: 2736
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002D28 RID: 11560
			public static Action <0>__UpdateQueue;

			// Token: 0x04002D29 RID: 11561
			public static Action <1>__Reset;

			// Token: 0x04002D2A RID: 11562
			public static Action<CassieTtsPayload> <2>__ClientHandleMessage;

			// Token: 0x04002D2B RID: 11563
			public static Action <3>__ClearAll;
		}
	}
}
