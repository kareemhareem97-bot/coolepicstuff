using System;
using InventorySystem.Items.Pickups;
using Mirror;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class PitItemDestroyer : MonoBehaviour
{
	// Token: 0x06000613 RID: 1555 RVA: 0x000173F8 File Offset: 0x000155F8
	private void OnTriggerEnter(Collider other)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		ItemPickupBase componentInParent = other.transform.GetComponentInParent<ItemPickupBase>();
		if (componentInParent == null)
		{
			return;
		}
		NetworkServer.Destroy(componentInParent.gameObject);
	}
}
