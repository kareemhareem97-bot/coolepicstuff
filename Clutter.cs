using System;
using System.Collections.Generic;
using GameCore;
using UnityEngine;

// Token: 0x020001AA RID: 426
public class Clutter : MonoBehaviour
{
	// Token: 0x06000858 RID: 2136 RVA: 0x000213BC File Offset: 0x0001F5BC
	public void SpawnClutter()
	{
		GameCore.Console.AddDebugLog("MGCLTR", "Spawning clutter component on object of name \"" + base.gameObject.name + "\"", MessageImportance.LeastImportant, true);
		this.spawned = true;
		if (!this.holderObject)
		{
			this.holderObject = base.gameObject;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((this.possiblePrefabs.Count > 0) ? this.possiblePrefabs[UnityEngine.Random.Range(0, this.possiblePrefabs.Count)] : base.gameObject, this.holderObject.transform.position + this.spawnOffset * 0.72745f, this.holderObject.transform.rotation.normalized, this.holderObject.transform.parent);
		if (this.clutterScale != Vector3.zero)
		{
			gameObject.transform.localScale = this.clutterScale;
		}
		else
		{
			gameObject.transform.localScale = this.holderObject.transform.localScale;
		}
		gameObject.SetActive(true);
		Clutter obj;
		if (gameObject.TryGetComponent<Clutter>(out obj))
		{
			UnityEngine.Object.Destroy(obj);
		}
		UnityEngine.Object.Destroy(this.holderObject);
	}

	// Token: 0x04000795 RID: 1941
	[Header("Prefab Data")]
	public GameObject holderObject;

	// Token: 0x04000796 RID: 1942
	public List<GameObject> possiblePrefabs = new List<GameObject>();

	// Token: 0x04000797 RID: 1943
	public Vector3 spawnOffset;

	// Token: 0x04000798 RID: 1944
	public Vector3 clutterScale = Vector3.zero;

	// Token: 0x04000799 RID: 1945
	public bool spawned;

	// Token: 0x0400079A RID: 1946
	private const float OverallScale = 0.72745f;
}
