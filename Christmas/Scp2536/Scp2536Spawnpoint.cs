using System;
using System.Collections.Generic;
using MapGeneration;
using Mirror;
using UnityEngine;

namespace Christmas.Scp2536
{
	// Token: 0x020010D6 RID: 4310
	public class Scp2536Spawnpoint : MonoBehaviour
	{
		// Token: 0x170013E3 RID: 5091
		// (get) Token: 0x06005FF9 RID: 24569 RVA: 0x00140FD8 File Offset: 0x0013F1D8
		// (set) Token: 0x06005FFA RID: 24570 RVA: 0x00140FE0 File Offset: 0x0013F1E0
		public Vector3 Position { get; private set; }

		// Token: 0x170013E4 RID: 5092
		// (get) Token: 0x06005FFB RID: 24571 RVA: 0x00140FE9 File Offset: 0x0013F1E9
		// (set) Token: 0x06005FFC RID: 24572 RVA: 0x00140FF1 File Offset: 0x0013F1F1
		public Quaternion Rotation { get; private set; }

		// Token: 0x06005FFD RID: 24573 RVA: 0x00140FFA File Offset: 0x0013F1FA
		private void Awake()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this._eventAssigned = true;
			Scp2536Spawnpoint.Spawnpoints.Add(this);
			SeedSynchronizer.OnGenerationFinished += this.OnGen;
		}

		// Token: 0x06005FFE RID: 24574 RVA: 0x00141028 File Offset: 0x0013F228
		private void OnDestroy()
		{
			if (!this._eventAssigned)
			{
				return;
			}
			Scp2536Spawnpoint.Spawnpoints.Remove(this);
			this._eventAssigned = false;
			SeedSynchronizer.OnGenerationFinished -= this.OnGen;
		}

		// Token: 0x06005FFF RID: 24575 RVA: 0x00141058 File Offset: 0x0013F258
		private void OnGen()
		{
			Transform transform = base.transform;
			this.Position = transform.position;
			this.Rotation = transform.rotation;
		}

		// Token: 0x04004601 RID: 17921
		public static readonly HashSet<Scp2536Spawnpoint> Spawnpoints = new HashSet<Scp2536Spawnpoint>();

		// Token: 0x04004604 RID: 17924
		private bool _eventAssigned;
	}
}
