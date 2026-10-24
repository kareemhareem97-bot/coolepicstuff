using System;
using PlayerRoles;
using UnityEngine;

// Token: 0x020000FE RID: 254
public class MainCameraController : MonoBehaviour
{
	// Token: 0x1400000E RID: 14
	// (add) Token: 0x060004BE RID: 1214 RVA: 0x00011E38 File Offset: 0x00010038
	// (remove) Token: 0x060004BF RID: 1215 RVA: 0x00011E6C File Offset: 0x0001006C
	public static event Action OnUpdated;

	// Token: 0x1400000F RID: 15
	// (add) Token: 0x060004C0 RID: 1216 RVA: 0x00011EA0 File Offset: 0x000100A0
	// (remove) Token: 0x060004C1 RID: 1217 RVA: 0x00011ED4 File Offset: 0x000100D4
	public static event Action OnBeforeUpdated;

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x060004C2 RID: 1218 RVA: 0x00011F07 File Offset: 0x00010107
	// (set) Token: 0x060004C3 RID: 1219 RVA: 0x00011F0E File Offset: 0x0001010E
	public static bool InstanceActive { get; private set; }

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x060004C4 RID: 1220 RVA: 0x00011F16 File Offset: 0x00010116
	// (set) Token: 0x060004C5 RID: 1221 RVA: 0x00011F1D File Offset: 0x0001011D
	public static Vector3 LastPosition { get; private set; }

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x060004C6 RID: 1222 RVA: 0x00011F25 File Offset: 0x00010125
	// (set) Token: 0x060004C7 RID: 1223 RVA: 0x00011F2C File Offset: 0x0001012C
	public static Quaternion LastRotation { get; private set; }

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x060004C8 RID: 1224 RVA: 0x00011F34 File Offset: 0x00010134
	public static Ray LastForwardRay
	{
		get
		{
			return new Ray(MainCameraController.LastPosition, MainCameraController.LastRotation * Vector3.forward);
		}
	}

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x060004C9 RID: 1225 RVA: 0x00011F4F File Offset: 0x0001014F
	// (set) Token: 0x060004CA RID: 1226 RVA: 0x00011F58 File Offset: 0x00010158
	public static Transform CurrentCamera
	{
		get
		{
			return MainCameraController._currentCamera;
		}
		set
		{
			if (MainCameraController._currentCamera == value)
			{
				return;
			}
			Transform transform = value;
			if (transform == null)
			{
				transform = MainCameraController._singleton._defaultCamera;
			}
			if (MainCameraController._currentCamera != null)
			{
				MainCameraController._currentCamera.gameObject.SetActive(false);
			}
			MainCameraController._currentCamera = transform;
			if (transform != null)
			{
				transform.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x00011FC1 File Offset: 0x000101C1
	private void Awake()
	{
		MainCameraController.InstanceActive = true;
		MainCameraController._singleton = this;
		MainCameraController._defaultPos = this._defaultCamera.position;
		MainCameraController.CurrentCamera = this._defaultCamera;
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x00011FEA File Offset: 0x000101EA
	private void OnDestroy()
	{
		MainCameraController.InstanceActive = false;
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x00011FF2 File Offset: 0x000101F2
	private void LateUpdate()
	{
		MainCameraController.ForceUpdatePosition();
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x00011FFC File Offset: 0x000101FC
	private static void GetPositionAndRotation(out Vector3 pos, out Quaternion rot)
	{
		ReferenceHub referenceHub;
		if (ReferenceHub.TryGetLocalHub(out referenceHub))
		{
			ICameraController cameraController = referenceHub.roleManager.CurrentRole as ICameraController;
			if (cameraController != null)
			{
				pos = cameraController.CameraPosition;
				IAdvancedCameraController advancedCameraController = cameraController as IAdvancedCameraController;
				float z = (advancedCameraController != null) ? advancedCameraController.RollRotation : 0f;
				rot = Quaternion.Euler(cameraController.VerticalRotation, cameraController.HorizontalRotation, z);
				return;
			}
		}
		pos = MainCameraController._defaultPos;
		rot = Quaternion.identity;
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x0001207C File Offset: 0x0001027C
	public static void ForceUpdatePosition()
	{
		if (!MainCameraController.InstanceActive)
		{
			return;
		}
		Action onBeforeUpdated = MainCameraController.OnBeforeUpdated;
		if (onBeforeUpdated != null)
		{
			onBeforeUpdated();
		}
		Vector3 vector;
		Quaternion quaternion;
		MainCameraController.GetPositionAndRotation(out vector, out quaternion);
		MainCameraController.CurrentCamera.SetPositionAndRotation(vector, quaternion);
		MainCameraController.LastPosition = vector;
		MainCameraController.LastRotation = quaternion;
		Action onUpdated = MainCameraController.OnUpdated;
		if (onUpdated == null)
		{
			return;
		}
		onUpdated();
	}

	// Token: 0x04000421 RID: 1057
	public const float DefaultVerticalFOV = 70f;

	// Token: 0x04000427 RID: 1063
	private static Transform _currentCamera;

	// Token: 0x04000428 RID: 1064
	private static MainCameraController _singleton;

	// Token: 0x04000429 RID: 1065
	private static Vector3 _defaultPos;

	// Token: 0x0400042A RID: 1066
	[SerializeField]
	private Transform _defaultCamera;
}
