using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000153 RID: 339
public class RoundPlayerHistory : MonoBehaviour
{
	// Token: 0x06000668 RID: 1640 RVA: 0x0001818A File Offset: 0x0001638A
	private void Awake()
	{
		RoundPlayerHistory.singleton = this;
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x00018194 File Offset: 0x00016394
	public RoundPlayerHistory.PlayerHistoryLog GetData(int playerId)
	{
		foreach (RoundPlayerHistory.PlayerHistoryLog playerHistoryLog in this.historyLogs)
		{
			if (playerHistoryLog.PlayerId == playerId)
			{
				return playerHistoryLog;
			}
		}
		return null;
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x000181F0 File Offset: 0x000163F0
	public void SetData(int playerId, string newNick, int newPlayerId, string newUserId, int newConnectionStatus, int newAliveClass, int newCurrentClass, DateTime newStartTime, DateTime newStopTime)
	{
		int num = -1;
		if (playerId == -1)
		{
			this.historyLogs.Add(new RoundPlayerHistory.PlayerHistoryLog
			{
				Nickname = "Player",
				PlayerId = 0,
				UserId = string.Empty,
				ConnectionStatus = 0,
				LastAliveClass = -1,
				CurrentClass = -1,
				ConnectionStart = DateTime.Now,
				ConnectionStop = new DateTime(0, 0, 0)
			});
			num = this.historyLogs.Count - 1;
		}
		else
		{
			for (int i = 0; i < this.historyLogs.Count; i++)
			{
				if (this.historyLogs[i].PlayerId == playerId)
				{
					num = i;
				}
			}
		}
		if (num < 0)
		{
			return;
		}
		if (newNick != string.Empty)
		{
			this.historyLogs[num].Nickname = newNick;
		}
		if (newPlayerId != 0)
		{
			this.historyLogs[num].PlayerId = newPlayerId;
		}
		if (newUserId != string.Empty)
		{
			this.historyLogs[num].UserId = newUserId;
		}
		if (newConnectionStatus != 0)
		{
			this.historyLogs[num].ConnectionStatus = newConnectionStatus;
		}
		if (newAliveClass != 0)
		{
			this.historyLogs[num].LastAliveClass = newAliveClass;
		}
		if (newCurrentClass != 0)
		{
			this.historyLogs[num].CurrentClass = newCurrentClass;
		}
		if (newStartTime.Year != 0)
		{
			this.historyLogs[num].ConnectionStart = newStartTime;
		}
		if (newStopTime.Year != 0)
		{
			this.historyLogs[num].ConnectionStop = newStopTime;
		}
	}

	// Token: 0x040005E8 RID: 1512
	public static RoundPlayerHistory singleton;

	// Token: 0x040005E9 RID: 1513
	public List<RoundPlayerHistory.PlayerHistoryLog> historyLogs = new List<RoundPlayerHistory.PlayerHistoryLog>();

	// Token: 0x02000154 RID: 340
	[Serializable]
	public class PlayerHistoryLog
	{
		// Token: 0x040005EA RID: 1514
		public string Nickname;

		// Token: 0x040005EB RID: 1515
		public int PlayerId;

		// Token: 0x040005EC RID: 1516
		public string UserId;

		// Token: 0x040005ED RID: 1517
		public int ConnectionStatus;

		// Token: 0x040005EE RID: 1518
		public int LastAliveClass;

		// Token: 0x040005EF RID: 1519
		public int CurrentClass;

		// Token: 0x040005F0 RID: 1520
		public DateTime ConnectionStart;

		// Token: 0x040005F1 RID: 1521
		public DateTime ConnectionStop;
	}
}
