using System;
using System.Runtime.InteropServices;
using GameCore;
using Mirror;
using UnityEngine;

// Token: 0x02000165 RID: 357
public class ServerTime : NetworkBehaviour
{
	// Token: 0x0600070F RID: 1807 RVA: 0x0001B347 File Offset: 0x00019547
	public static bool CheckSynchronization(int myTime)
	{
		int num = Mathf.Abs(myTime - ServerTime.time);
		if (num > 2)
		{
			GameCore.Console.AddLog("Damage sync error.", new Color32(byte.MaxValue, 200, 0, byte.MaxValue), false, ConsoleLogType.Log);
		}
		return num <= 2;
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x0001B385 File Offset: 0x00019585
	private void Update()
	{
		this._rateLimit = false;
		if (this.timeFromStartup != 0)
		{
			ServerTime.time = this.timeFromStartup;
		}
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x0001B3A1 File Offset: 0x000195A1
	private void Start()
	{
		if (base.isLocalPlayer && NetworkServer.active)
		{
			base.InvokeRepeating("IncreaseTime", 1f, 1f);
		}
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x0001B3C7 File Offset: 0x000195C7
	private void IncreaseTime()
	{
		this.NetworktimeFromStartup = this.timeFromStartup + 1;
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06000715 RID: 1813 RVA: 0x0001B3D8 File Offset: 0x000195D8
	// (set) Token: 0x06000716 RID: 1814 RVA: 0x0001B3EB File Offset: 0x000195EB
	public int NetworktimeFromStartup
	{
		get
		{
			return this.timeFromStartup;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<int>(value, ref this.timeFromStartup, 1UL, null);
		}
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x0001B408 File Offset: 0x00019608
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteInt(this.timeFromStartup);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteInt(this.timeFromStartup);
		}
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x0001B460 File Offset: 0x00019660
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<int>(ref this.timeFromStartup, null, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<int>(ref this.timeFromStartup, null, reader.ReadInt());
		}
	}

	// Token: 0x04000667 RID: 1639
	[SyncVar]
	public int timeFromStartup;

	// Token: 0x04000668 RID: 1640
	public static int time;

	// Token: 0x04000669 RID: 1641
	private const int AllowedDeviation = 2;

	// Token: 0x0400066A RID: 1642
	private bool _rateLimit;
}
