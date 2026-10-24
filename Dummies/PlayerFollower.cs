using System;
using Mirror;
using PlayerRoles.FirstPersonControl;
using RelativePositioning;
using UnityEngine;

namespace CommandSystem.Commands.RemoteAdmin.Dummies
{
	// Token: 0x02000A5E RID: 2654
	public class PlayerFollower : MonoBehaviour
	{
		// Token: 0x06003C00 RID: 15360 RVA: 0x000C619E File Offset: 0x000C439E
		public void Init(ReferenceHub playerToFollow, float maxDistance = 20f, float minDistance = 1.75f, float speed = 30f)
		{
			this._hub = base.GetComponent<ReferenceHub>();
			this._hubToFollow = playerToFollow;
			this._maxDistance = maxDistance;
			this._minDistance = minDistance;
			this._speed = speed;
		}

		// Token: 0x06003C01 RID: 15361 RVA: 0x000C61CC File Offset: 0x000C43CC
		private void Update()
		{
			if (NetworkServer.active && !(this._hubToFollow == null) && !(this._hub == null) && this._hubToFollow.roleManager.CurrentRole is IFpcRole)
			{
				IFpcRole fpcRole = this._hub.roleManager.CurrentRole as IFpcRole;
				if (fpcRole != null)
				{
					FirstPersonMovementModule fpcModule = fpcRole.FpcModule;
					float num = Vector3.Distance(this._hubToFollow.transform.position, base.transform.position);
					if (num > this._maxDistance)
					{
						fpcModule.ServerOverridePosition(this._hubToFollow.transform.position);
						return;
					}
					if (num < this._minDistance)
					{
						return;
					}
					Vector3 position = base.transform.position;
					Vector3 dir = this._hubToFollow.transform.position - position;
					Vector3 b = Time.deltaTime * this._speed * dir.normalized;
					fpcModule.Motor.ReceivedPosition = new RelativePosition(position + b);
					fpcModule.MouseLook.LookAtDirection(dir, 1f);
					return;
				}
			}
			UnityEngine.Object.Destroy(this);
		}

		// Token: 0x04002C1E RID: 11294
		private const float DefaultMaxDistance = 20f;

		// Token: 0x04002C1F RID: 11295
		private const float DefaultMinDistance = 1.75f;

		// Token: 0x04002C20 RID: 11296
		private const float DefaultSpeed = 30f;

		// Token: 0x04002C21 RID: 11297
		private ReferenceHub _hub;

		// Token: 0x04002C22 RID: 11298
		private ReferenceHub _hubToFollow;

		// Token: 0x04002C23 RID: 11299
		private float _maxDistance;

		// Token: 0x04002C24 RID: 11300
		private float _minDistance;

		// Token: 0x04002C25 RID: 11301
		private float _speed;
	}
}
