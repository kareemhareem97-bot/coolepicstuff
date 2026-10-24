using System;
using System.Collections.Generic;
using InventorySystem.Items;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace CameraShaking
{
	// Token: 0x02000888 RID: 2184
	public class CameraShakeController : MonoBehaviour
	{
		// Token: 0x060031DC RID: 12764 RVA: 0x000A769C File Offset: 0x000A589C
		public static bool CheckEffect(IShakeEffect effect)
		{
			return CameraShakeController.Singleton._effects.Contains(effect);
		}

		// Token: 0x060031DD RID: 12765 RVA: 0x000A76AE File Offset: 0x000A58AE
		public static void AddEffect(IShakeEffect effect)
		{
			CameraShakeController.Singleton._effects.Add(effect);
		}

		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x060031DE RID: 12766 RVA: 0x000A76C0 File Offset: 0x000A58C0
		// (set) Token: 0x060031DF RID: 12767 RVA: 0x000A76CD File Offset: 0x000A58CD
		public float CamerasFOV
		{
			get
			{
				return this._camera.fieldOfView;
			}
			private set
			{
				this._camera.fieldOfView = value;
			}
		}

		// Token: 0x060031E0 RID: 12768 RVA: 0x000A76DB File Offset: 0x000A58DB
		private void Start()
		{
			this._startPos = base.transform.localPosition;
			CameraShakeController.Singleton = this;
			this._viewmodelRoot = base.GetComponentInChildren<SharedHandsController>().transform;
		}

		// Token: 0x060031E1 RID: 12769 RVA: 0x000A7705 File Offset: 0x000A5905
		private void OnDisable()
		{
			this._effects.Clear();
		}

		// Token: 0x060031E2 RID: 12770 RVA: 0x000A7714 File Offset: 0x000A5914
		private void LateUpdate()
		{
			ReferenceHub referenceHub;
			if (!ReferenceHub.TryGetLocalHub(out referenceHub))
			{
				return;
			}
			if (this._effects.Count == 0)
			{
				IZoomModifyingItem zoomModifyingItem = referenceHub.inventory.CurInstance as IZoomModifyingItem;
				if (zoomModifyingItem != null)
				{
					this.CamerasFOV = 70f / zoomModifyingItem.ZoomAmount;
				}
				return;
			}
			HashSet<float> hashSet = HashSetPool<float>.Shared.Rent();
			Quaternion quaternion = Quaternion.identity;
			Quaternion quaternion2 = Quaternion.identity;
			Vector3 vector = this._startPos;
			for (int i = this._effects.Count - 1; i >= 0; i--)
			{
				ShakeEffectValues shakeEffectValues;
				if (!this._effects[i].GetEffect(referenceHub, out shakeEffectValues))
				{
					this._effects.RemoveAt(i);
				}
				else
				{
					if (shakeEffectValues.FovPercent != 1f)
					{
						hashSet.Add(shakeEffectValues.FovPercent);
					}
					FpcMouseLook fpcMouseLook;
					if (FpcMouseLook.TryGetLocalMouseLook(out fpcMouseLook))
					{
						if (shakeEffectValues.HorizontalMouseLook != 0f)
						{
							fpcMouseLook.CurrentHorizontal += shakeEffectValues.HorizontalMouseLook;
						}
						if (shakeEffectValues.VerticalMouseLook != 0f)
						{
							fpcMouseLook.CurrentVertical += shakeEffectValues.VerticalMouseLook;
						}
					}
					quaternion *= shakeEffectValues.RootCameraRotation;
					vector += shakeEffectValues.RootCameraPositionOffset;
					quaternion2 *= shakeEffectValues.ViewmodelCameraRotation;
				}
			}
			if (hashSet.Count > 0)
			{
				float num = 0f;
				foreach (float num2 in hashSet)
				{
					num += num2 - 1f;
				}
				this.CamerasFOV = 70f * (num + 1f);
			}
			else
			{
				this.CamerasFOV = 70f;
			}
			IZoomModifyingItem zoomModifyingItem2 = referenceHub.inventory.CurInstance as IZoomModifyingItem;
			if (zoomModifyingItem2 != null)
			{
				this.CamerasFOV /= zoomModifyingItem2.ZoomAmount;
			}
			HashSetPool<float>.Shared.Return(hashSet);
			base.transform.SetLocalPositionAndRotation(Vector3.zero, quaternion);
			base.transform.position += vector;
			this._viewmodelRoot.localRotation = quaternion2;
		}

		// Token: 0x040026EF RID: 9967
		[SerializeField]
		private Camera _camera;

		// Token: 0x040026F0 RID: 9968
		private Vector3 _startPos;

		// Token: 0x040026F1 RID: 9969
		private Transform _viewmodelRoot;

		// Token: 0x040026F2 RID: 9970
		private readonly List<IShakeEffect> _effects = new List<IShakeEffect>();

		// Token: 0x040026F3 RID: 9971
		public static CameraShakeController Singleton;
	}
}
