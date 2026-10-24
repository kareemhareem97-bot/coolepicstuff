using System;
using System.Collections.Generic;
using MapGeneration;
using Mirror;
using UnityEngine;

// Token: 0x02000029 RID: 41
public class Scp559Spawnpoint : MonoBehaviour
{
	// Token: 0x1700002D RID: 45
	// (get) Token: 0x060000C0 RID: 192 RVA: 0x00004265 File Offset: 0x00002465
	// (set) Token: 0x060000C1 RID: 193 RVA: 0x0000426D File Offset: 0x0000246D
	public RoomIdentifier Room { get; private set; }

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x060000C2 RID: 194 RVA: 0x00004276 File Offset: 0x00002476
	// (set) Token: 0x060000C3 RID: 195 RVA: 0x0000427E File Offset: 0x0000247E
	public bool HidePedestal { get; private set; }

	// Token: 0x060000C4 RID: 196 RVA: 0x00004287 File Offset: 0x00002487
	private void Start()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		this.Room = base.GetComponentInParent<RoomIdentifier>();
		Scp559Spawnpoint.InstancesPerRoom.Add(this.Room, this);
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x000042AE File Offset: 0x000024AE
	private void OnDestroy()
	{
		if (this.Room == null)
		{
			return;
		}
		Scp559Spawnpoint.InstancesPerRoom.Remove(this.Room);
	}

	// Token: 0x040000B7 RID: 183
	public const float PedestalHeight = 1.042f;

	// Token: 0x040000B8 RID: 184
	public static readonly Dictionary<RoomIdentifier, Scp559Spawnpoint> InstancesPerRoom = new Dictionary<RoomIdentifier, Scp559Spawnpoint>();
}
