using System;
using GameObjectPools;
using InventorySystem.Items;
using InventorySystem.Items.Thirdperson;
using UnityEngine;

// Token: 0x02000129 RID: 297
public class HandPart : ThirdpersonItemBase
{
	// Token: 0x170000DA RID: 218
	// (get) Token: 0x06000590 RID: 1424 RVA: 0x00014E71 File Offset: 0x00013071
	// (set) Token: 0x06000591 RID: 1425 RVA: 0x00014E79 File Offset: 0x00013079
	protected bool CurrentlyEnabled { get; set; }

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x06000592 RID: 1426 RVA: 0x00014E82 File Offset: 0x00013082
	// (set) Token: 0x06000593 RID: 1427 RVA: 0x00014E8A File Offset: 0x0001308A
	protected GameObject SpawnedObject { get; set; }

	// Token: 0x06000594 RID: 1428 RVA: 0x00014E94 File Offset: 0x00013094
	public void UpdateItem()
	{
		bool flag = base.OwnerHub.inventory.CurItem.TypeId == this.TargetItemId;
		if (flag == this.CurrentlyEnabled)
		{
			return;
		}
		this.CurrentlyEnabled = flag;
		this.TargetPart.SetActive(flag);
		if (flag && this.optionalPrefab != null)
		{
			PoolObject poolObject;
			this.SpawnedObject = (PoolManager.Singleton.TryGetPoolObject(this.optionalPrefab, this.TargetPart.transform, out poolObject, true) ? poolObject.gameObject : UnityEngine.Object.Instantiate<GameObject>(this.optionalPrefab, this.TargetPart.transform));
			this.SpawnedObject.transform.localScale = Vector3.one;
			this.SpawnedObject.transform.localPosition = Vector3.zero;
			this.SpawnedObject.transform.localRotation = Quaternion.identity;
			this.OnActiveStateChange(true);
			return;
		}
		if (this.SpawnedObject != null)
		{
			this.OnActiveStateChange(false);
			if (!PoolManager.Singleton.TryReturnPoolObject(this.SpawnedObject))
			{
				UnityEngine.Object.Destroy(this.SpawnedObject);
			}
		}
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x00014FB0 File Offset: 0x000131B0
	public override void ResetObject()
	{
		base.ResetObject();
		this.TargetPart.SetActive(false);
		this.CurrentlyEnabled = false;
		this.OnActiveStateChange(false);
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x00002E9A File Offset: 0x0000109A
	protected virtual void OnActiveStateChange(bool isEnabled)
	{
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x00014FD2 File Offset: 0x000131D2
	public override float GetTransitionTime(ItemIdentifier iid)
	{
		return 2f;
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x00014FD9 File Offset: 0x000131D9
	public override ThirdpersonLayerWeight GetWeightForLayer(AnimItemLayer3p layer)
	{
		return new ThirdpersonLayerWeight(1f, false);
	}

	// Token: 0x040004E2 RID: 1250
	public GameObject TargetPart;

	// Token: 0x040004E3 RID: 1251
	public ItemType TargetItemId;

	// Token: 0x040004E4 RID: 1252
	public bool UseUniversalAnimations;

	// Token: 0x040004E5 RID: 1253
	[SerializeField]
	private GameObject optionalPrefab;
}
