using System;
using PlayerRoles.Ragdolls;
using RelativePositioning;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class RagdollAnimationTemplate : MonoBehaviour
{
	// Token: 0x06000658 RID: 1624 RVA: 0x00017DE0 File Offset: 0x00015FE0
	public void ProcessRagdoll(BasicRagdoll rg)
	{
		DynamicRagdoll dynamicRagdoll = rg as DynamicRagdoll;
		if (dynamicRagdoll == null)
		{
			return;
		}
		int num = Mathf.Min(this._bones.Length, dynamicRagdoll.LinkedRigidbodies.Length);
		rg.transform.rotation *= this._overallRotation;
		for (int i = 0; i < num; i++)
		{
			Rigidbody rigidbody = dynamicRagdoll.LinkedRigidbodies[i];
			RagdollAnimationTemplate.RagdollBone ragdollBone = this._bones[i];
			Transform transform = rigidbody.transform;
			transform.localRotation = ragdollBone.RotationOffset;
			Quaternion worldRotation = WaypointBase.GetWorldRotation(rg.Info.StartRelativePosition.WaypointId, rg.Info.StartRelativeRotation);
			transform.position = rg.Info.StartRelativePosition.Position + worldRotation * ragdollBone.PositionOffset;
			rigidbody.linearVelocity = worldRotation * ragdollBone.StartVelocity;
		}
	}

	// Token: 0x040005D7 RID: 1495
	[SerializeField]
	private RagdollAnimationTemplate.RagdollBone[] _bones;

	// Token: 0x040005D8 RID: 1496
	[SerializeField]
	private Quaternion _overallRotation;

	// Token: 0x0200014D RID: 333
	[Serializable]
	private struct RagdollBone
	{
		// Token: 0x040005D9 RID: 1497
		public Vector3 PositionOffset;

		// Token: 0x040005DA RID: 1498
		public Quaternion RotationOffset;

		// Token: 0x040005DB RID: 1499
		public Vector3 StartVelocity;
	}
}
