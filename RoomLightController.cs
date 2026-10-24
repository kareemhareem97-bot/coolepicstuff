using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using MapGeneration;
using Mirror;
using UnityEngine;
using Utils.NonAllocLINQ;

// Token: 0x020001BC RID: 444
public class RoomLightController : NetworkBehaviour
{
	// Token: 0x14000022 RID: 34
	// (add) Token: 0x060008A9 RID: 2217 RVA: 0x00022760 File Offset: 0x00020960
	// (remove) Token: 0x060008AA RID: 2218 RVA: 0x00022794 File Offset: 0x00020994
	public static event Action<RoomLightController> OnAdded;

	// Token: 0x14000023 RID: 35
	// (add) Token: 0x060008AB RID: 2219 RVA: 0x000227C8 File Offset: 0x000209C8
	// (remove) Token: 0x060008AC RID: 2220 RVA: 0x000227FC File Offset: 0x000209FC
	public static event Action<RoomLightController> OnRemoved;

	// Token: 0x17000155 RID: 341
	// (get) Token: 0x060008AD RID: 2221 RVA: 0x00022830 File Offset: 0x00020A30
	public RoomIdentifier Room
	{
		get
		{
			if (!this._roomCacheSet)
			{
				if (!base.transform.TryGetComponentInParent(out this._cachedRoom))
				{
					throw new NullReferenceException("Null room for Light Controller: " + base.transform.GetHierarchyPath());
				}
				this._roomCacheSet = true;
			}
			return this._cachedRoom;
		}
	}

	// Token: 0x14000024 RID: 36
	// (add) Token: 0x060008AE RID: 2222 RVA: 0x00022880 File Offset: 0x00020A80
	// (remove) Token: 0x060008AF RID: 2223 RVA: 0x000228B8 File Offset: 0x00020AB8
	public event Action<bool> OnLightsSet;

	// Token: 0x060008B0 RID: 2224 RVA: 0x000228F0 File Offset: 0x00020AF0
	private void Start()
	{
		if (NetworkServer.active)
		{
			this.NetworkLightsEnabled = true;
		}
		else
		{
			this.SetLights(this.LightsEnabled);
		}
		RoomLightController.Instances.Add(this);
		Action<RoomLightController> onAdded = RoomLightController.OnAdded;
		if (onAdded != null)
		{
			onAdded(this);
		}
		this.Room.LightControllers.AddIfNotContains(this);
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x00022947 File Offset: 0x00020B47
	private void OnDestroy()
	{
		RoomLightController.Instances.Remove(this);
		Action<RoomLightController> onRemoved = RoomLightController.OnRemoved;
		if (onRemoved == null)
		{
			return;
		}
		onRemoved(this);
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x00022965 File Offset: 0x00020B65
	private void Update()
	{
		if (!NetworkServer.active || this._flickerDuration <= 0f)
		{
			return;
		}
		this._flickerDuration -= Time.deltaTime;
		if (this._flickerDuration > 0f)
		{
			return;
		}
		this.SetLights(true);
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x000229A3 File Offset: 0x00020BA3
	private void SetLights(bool state)
	{
		if (NetworkServer.active)
		{
			this.NetworkLightsEnabled = state;
		}
		Action<bool> onLightsSet = this.OnLightsSet;
		if (onLightsSet == null)
		{
			return;
		}
		onLightsSet(state);
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x000229C4 File Offset: 0x00020BC4
	private void LightsEnabledHook(bool oldValue, bool newValue)
	{
		ServerEvents.OnRoomLightChanged(new RoomLightChangedEventArgs(this.Room, newValue));
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x000229D7 File Offset: 0x00020BD7
	private void OverrideColorHook(Color oldValue, Color newValue)
	{
		ServerEvents.OnRoomColorChanged(new RoomColorChangedEventArgs(this.Room, newValue));
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x000229EA File Offset: 0x00020BEA
	[Server]
	public void ServerFlickerLights(float dur)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void RoomLightController::ServerFlickerLights(System.Single)' called when server was not active");
			return;
		}
		if (dur <= 0f)
		{
			this._flickerDuration = 0f;
			this.SetLights(true);
			return;
		}
		this._flickerDuration = dur;
		this.SetLights(false);
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x00022A2C File Offset: 0x00020C2C
	public static bool IsInDarkenedRoom(Vector3 positionToCheck)
	{
		RoomIdentifier rid;
		return positionToCheck.TryGetRoom(out rid) && RoomLightController.IsInDarkenedRoom(rid, positionToCheck);
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x00022A4C File Offset: 0x00020C4C
	public static bool IsInDarkenedRoom(RoomIdentifier rid, Vector3 positionToCheck)
	{
		foreach (RoomLightController roomLightController in rid.LightControllers)
		{
			float num = Mathf.Abs(roomLightController.transform.position.y - positionToCheck.y);
			if (!roomLightController.LightsEnabled && num <= 50f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x060008BC RID: 2236 RVA: 0x00022AD8 File Offset: 0x00020CD8
	// (set) Token: 0x060008BD RID: 2237 RVA: 0x00022AEB File Offset: 0x00020CEB
	public bool NetworkLightsEnabled
	{
		get
		{
			return this.LightsEnabled;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this.LightsEnabled, 1UL, new Action<bool, bool>(this.LightsEnabledHook));
		}
	}

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x060008BE RID: 2238 RVA: 0x00022B10 File Offset: 0x00020D10
	// (set) Token: 0x060008BF RID: 2239 RVA: 0x00022B23 File Offset: 0x00020D23
	public Color NetworkOverrideColor
	{
		get
		{
			return this.OverrideColor;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<Color>(value, ref this.OverrideColor, 2UL, new Action<Color, Color>(this.OverrideColorHook));
		}
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x00022B48 File Offset: 0x00020D48
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(this.LightsEnabled);
			writer.WriteColor(this.OverrideColor);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteBool(this.LightsEnabled);
		}
		if ((base.syncVarDirtyBits & 2UL) != 0UL)
		{
			writer.WriteColor(this.OverrideColor);
		}
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x00022BD0 File Offset: 0x00020DD0
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this.LightsEnabled, new Action<bool, bool>(this.LightsEnabledHook), reader.ReadBool());
			base.GeneratedSyncVarDeserialize<Color>(ref this.OverrideColor, new Action<Color, Color>(this.OverrideColorHook), reader.ReadColor());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this.LightsEnabled, new Action<bool, bool>(this.LightsEnabledHook), reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<Color>(ref this.OverrideColor, new Action<Color, Color>(this.OverrideColorHook), reader.ReadColor());
		}
	}

	// Token: 0x040007F7 RID: 2039
	public const float HeightRadius = 50f;

	// Token: 0x040007F8 RID: 2040
	private float _flickerDuration;

	// Token: 0x040007F9 RID: 2041
	private RoomIdentifier _cachedRoom;

	// Token: 0x040007FA RID: 2042
	private bool _roomCacheSet;

	// Token: 0x040007FB RID: 2043
	[SyncVar(hook = "LightsEnabledHook")]
	public bool LightsEnabled;

	// Token: 0x040007FC RID: 2044
	[SyncVar(hook = "OverrideColorHook")]
	public Color OverrideColor;

	// Token: 0x040007FE RID: 2046
	public static readonly List<RoomLightController> Instances = new List<RoomLightController>();
}
