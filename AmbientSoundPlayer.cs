using System;
using System.Collections.Generic;
using Mirror;
using Mirror.RemoteCalls;
using Security;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class AmbientSoundPlayer : NetworkBehaviour
{
	// Token: 0x060004D9 RID: 1241 RVA: 0x00012218 File Offset: 0x00010418
	private void Start()
	{
		if (!base.isLocalPlayer || !base.isServer)
		{
			return;
		}
		for (int i = 0; i < this.clips.Length; i++)
		{
			this.clips[i].index = i;
		}
		base.Invoke("GenerateRandom", 10f);
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x00012268 File Offset: 0x00010468
	private void GenerateRandom()
	{
		this.list.Clear();
		foreach (AmbientSoundPlayer.AmbientClip ambientClip in this.clips)
		{
			if (!ambientClip.played)
			{
				this.list.Add(ambientClip);
			}
		}
		int index = UnityEngine.Random.Range(0, this.list.Count);
		int index2 = this.list[index].index;
		if (!this.clips[index2].repeatable)
		{
			this.clips[index2].played = true;
		}
		this.RpcPlaySound(index2);
		base.Invoke("GenerateRandom", (float)UnityEngine.Random.Range(this.minTime, this.maxTime));
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x00012318 File Offset: 0x00010518
	[ClientRpc]
	private void RpcPlaySound(int id)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteInt(id);
		this.SendRPCInternal("System.Void AmbientSoundPlayer::RpcPlaySound(System.Int32)", -1494438428, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_RpcPlaySound__Int32(int id)
	{
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x00012387 File Offset: 0x00010587
	protected static void InvokeUserCode_RpcPlaySound__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPlaySound called on server.");
			return;
		}
		((AmbientSoundPlayer)obj).UserCode_RpcPlaySound__Int32(reader.ReadInt());
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x000123B0 File Offset: 0x000105B0
	static AmbientSoundPlayer()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(AmbientSoundPlayer), "System.Void AmbientSoundPlayer::RpcPlaySound(System.Int32)", new RemoteCallDelegate(AmbientSoundPlayer.InvokeUserCode_RpcPlaySound__Int32));
	}

	// Token: 0x0400042E RID: 1070
	public GameObject audioPrefab;

	// Token: 0x0400042F RID: 1071
	public int minTime = 30;

	// Token: 0x04000430 RID: 1072
	public int maxTime = 60;

	// Token: 0x04000431 RID: 1073
	public AmbientSoundPlayer.AmbientClip[] clips;

	// Token: 0x04000432 RID: 1074
	private List<AmbientSoundPlayer.AmbientClip> list = new List<AmbientSoundPlayer.AmbientClip>();

	// Token: 0x04000433 RID: 1075
	private RateLimit _ambientSoundRateLimit = new RateLimit(4, 3f, null);

	// Token: 0x02000103 RID: 259
	[Serializable]
	public class AmbientClip
	{
		// Token: 0x04000434 RID: 1076
		public AudioClip clip;

		// Token: 0x04000435 RID: 1077
		public bool repeatable = true;

		// Token: 0x04000436 RID: 1078
		public bool is3D = true;

		// Token: 0x04000437 RID: 1079
		public bool played;

		// Token: 0x04000438 RID: 1080
		public int index;
	}
}
