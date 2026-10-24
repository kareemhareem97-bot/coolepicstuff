using System;
using UnityEngine;

// Token: 0x02000073 RID: 115
public class Skybox : MonoBehaviour
{
	// Token: 0x06000289 RID: 649 RVA: 0x00009C62 File Offset: 0x00007E62
	private void Start()
	{
		this._rend = this._cubemapTr.GetComponent<Renderer>();
		this.CalculateWorldspaceBounds();
		MainCameraController.OnUpdated += this.OnCamUpdated;
	}

	// Token: 0x0600028A RID: 650 RVA: 0x00009C8C File Offset: 0x00007E8C
	private void OnDestroy()
	{
		MainCameraController.OnUpdated -= this.OnCamUpdated;
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00009C9F File Offset: 0x00007E9F
	private void OnDrawGizmosSelected()
	{
		this.CalculateWorldspaceBounds();
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(this._worldspaceBounds.center, this._worldspaceBounds.size);
	}

	// Token: 0x0600028C RID: 652 RVA: 0x00009CCC File Offset: 0x00007ECC
	private void CalculateWorldspaceBounds()
	{
		Vector3 center = base.transform.TransformPoint(this._bounds.center);
		Vector3 size = (base.transform.rotation * this._bounds.size).Abs();
		this._worldspaceBounds = new Bounds(center, size);
	}

	// Token: 0x0600028D RID: 653 RVA: 0x00009D20 File Offset: 0x00007F20
	private void OnCamUpdated()
	{
		Vector3 lastPosition = MainCameraController.LastPosition;
		bool flag = this._worldspaceBounds.Contains(lastPosition);
		this._rend.enabled = flag;
		if (!flag)
		{
			return;
		}
		this._cubemapTr.position = lastPosition;
	}

	// Token: 0x0400020E RID: 526
	[SerializeField]
	private Bounds _bounds;

	// Token: 0x0400020F RID: 527
	[SerializeField]
	private Transform _cubemapTr;

	// Token: 0x04000210 RID: 528
	private Renderer _rend;

	// Token: 0x04000211 RID: 529
	private Bounds _worldspaceBounds;
}
