using System;
using InventorySystem;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Usables.Scp330;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class Scp956DamageHandler : StandardDamageHandler
{
	// Token: 0x1700002F RID: 47
	// (get) Token: 0x060000C8 RID: 200 RVA: 0x000042D6 File Offset: 0x000024D6
	public override string RagdollInspectText
	{
		get
		{
			return "Blunt force trauma and a ruptured torso suggest a death by SCP-956.";
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x060000C9 RID: 201 RVA: 0x000042DD File Offset: 0x000024DD
	public override string DeathScreenText
	{
		get
		{
			return "Bludgeoned by SCP-956.";
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x060000CA RID: 202 RVA: 0x000042E4 File Offset: 0x000024E4
	public override string ServerLogsText
	{
		get
		{
			return "Died to SCP-956";
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x060000CB RID: 203 RVA: 0x000042EB File Offset: 0x000024EB
	public override DamageHandlerBase.CassieAnnouncement CassieDeathAnnouncement
	{
		get
		{
			return null;
		}
	}

	// Token: 0x060000CC RID: 204 RVA: 0x000042EE File Offset: 0x000024EE
	public Scp956DamageHandler(Vector3 direction)
	{
		this._velocity = (direction * 3f + Vector3.up) * 9f;
		this.Damage = -1f;
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x060000CD RID: 205 RVA: 0x00004326 File Offset: 0x00002526
	// (set) Token: 0x060000CE RID: 206 RVA: 0x0000432E File Offset: 0x0000252E
	public override float Damage { get; set; }

	// Token: 0x060000CF RID: 207 RVA: 0x00004338 File Offset: 0x00002538
	public override DamageHandlerBase.HandlerOutput ApplyDamage(ReferenceHub ply)
	{
		DamageHandlerBase.HandlerOutput handlerOutput = base.ApplyDamage(ply);
		if (handlerOutput != DamageHandlerBase.HandlerOutput.Death)
		{
			return handlerOutput;
		}
		Scp330Bag scp330Bag;
		if (!InventoryItemLoader.TryGetItem<Scp330Bag>(ItemType.SCP330, out scp330Bag))
		{
			return handlerOutput;
		}
		int num = (UnityEngine.Random.value < 0.1f) ? 1 : 0;
		for (int i = 0; i < 20; i++)
		{
			PickupSyncInfo value = new PickupSyncInfo(ItemType.SCP330, scp330Bag.Weight, 0, false);
			Scp330Pickup scp330Pickup = ply.inventory.ServerCreatePickup(scp330Bag, new PickupSyncInfo?(value), true, null) as Scp330Pickup;
			if (!(scp330Pickup == null))
			{
				CandyKindID candyKindID = (num-- > 0) ? CandyKindID.Pink : ((CandyKindID)UnityEngine.Random.Range(1, 7));
				scp330Pickup.StoredCandies.Add(candyKindID);
				scp330Pickup.NetworkExposedCandy = candyKindID;
				Rigidbody rigidbody;
				if (scp330Pickup.TryGetComponent<Rigidbody>(out rigidbody))
				{
					rigidbody.linearVelocity = this._velocity;
				}
			}
		}
		return handlerOutput;
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x000043FD File Offset: 0x000025FD
	public override void WriteAdditionalData(NetworkWriter writer)
	{
		this.StartVelocity = this._velocity;
		base.WriteAdditionalData(writer);
	}

	// Token: 0x040000BB RID: 187
	private readonly Vector3 _velocity;
}
