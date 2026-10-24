using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class SkyboxHubert : NetworkBehaviour
{
	// Token: 0x0600012A RID: 298 RVA: 0x00005D91 File Offset: 0x00003F91
	private void Awake()
	{
		SkyboxHubert._singleton = this;
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00005D99 File Offset: 0x00003F99
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		Action value;
		if ((value = SkyboxHubert.<>O.<0>__CharacterClassManager_OnRoundStarted) == null)
		{
			value = (SkyboxHubert.<>O.<0>__CharacterClassManager_OnRoundStarted = new Action(SkyboxHubert.CharacterClassManager_OnRoundStarted));
		}
		CharacterClassManager.OnRoundStarted += value;
	}

	// Token: 0x0600012C RID: 300 RVA: 0x00005DBB File Offset: 0x00003FBB
	private static void CharacterClassManager_OnRoundStarted()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (UnityEngine.Random.Range(0, 250) != 249)
		{
			return;
		}
		if (SkyboxHubert._singleton == null)
		{
			return;
		}
		SkyboxHubert._singleton.NetworkHubert = true;
	}

	// Token: 0x0600012D RID: 301 RVA: 0x00005DF1 File Offset: 0x00003FF1
	private void HubertHook(bool old, bool newValue)
	{
		if (!newValue)
		{
			return;
		}
		if (this._skyboxMeshRenderer != null)
		{
			this._skyboxMeshRenderer.sharedMaterial = this._hubertSkybox;
		}
	}

	// Token: 0x0600012F RID: 303 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x06000130 RID: 304 RVA: 0x00005E18 File Offset: 0x00004018
	// (set) Token: 0x06000131 RID: 305 RVA: 0x00005E2B File Offset: 0x0000402B
	public bool NetworkHubert
	{
		get
		{
			return this.Hubert;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this.Hubert, 1UL, new Action<bool, bool>(this.HubertHook));
		}
	}

	// Token: 0x06000132 RID: 306 RVA: 0x00005E50 File Offset: 0x00004050
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(this.Hubert);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteBool(this.Hubert);
		}
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00005EA8 File Offset: 0x000040A8
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this.Hubert, new Action<bool, bool>(this.HubertHook), reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this.Hubert, new Action<bool, bool>(this.HubertHook), reader.ReadBool());
		}
	}

	// Token: 0x04000111 RID: 273
	private const int Chance = 250;

	// Token: 0x04000112 RID: 274
	[SyncVar(hook = "HubertHook")]
	public bool Hubert;

	// Token: 0x04000113 RID: 275
	[SerializeField]
	private MeshRenderer _skyboxMeshRenderer;

	// Token: 0x04000114 RID: 276
	[SerializeField]
	private Material _hubertSkybox;

	// Token: 0x04000115 RID: 277
	private static SkyboxHubert _singleton;

	// Token: 0x02000035 RID: 53
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x04000116 RID: 278
		public static Action <0>__CharacterClassManager_OnRoundStarted;
	}
}
