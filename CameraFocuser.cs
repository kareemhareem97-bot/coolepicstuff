using System;
using UnityEngine;

// Token: 0x020000A8 RID: 168
public class CameraFocuser : MonoBehaviour
{
	// Token: 0x06000375 RID: 885 RVA: 0x0000CF88 File Offset: 0x0000B188
	private void OnTriggerStay(Collider other)
	{
		ReferenceHub hub = ReferenceHub.GetHub(other.transform.root.gameObject);
		if (hub != null && hub.characterClassManager.isLocalPlayer)
		{
			base.transform.LookAt(this.lookTarget);
			Mathf.Clamp(Quaternion.Angle(hub.PlayerCameraReference.rotation, base.transform.rotation), this.minimumAngle, 70f);
		}
	}

	// Token: 0x0400030A RID: 778
	public Transform lookTarget;

	// Token: 0x0400030B RID: 779
	public float targetFovScale = 1f;

	// Token: 0x0400030C RID: 780
	public float minimumAngle;
}
