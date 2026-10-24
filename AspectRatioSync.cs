using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class AspectRatioSync : NetworkBehaviour
{
	// Token: 0x14000016 RID: 22
	// (add) Token: 0x06000619 RID: 1561 RVA: 0x00017508 File Offset: 0x00015708
	// (remove) Token: 0x0600061A RID: 1562 RVA: 0x0001753C File Offset: 0x0001573C
	public static event Action OnAspectRatioChanged;

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x0600061B RID: 1563 RVA: 0x0001756F File Offset: 0x0001576F
	// (set) Token: 0x0600061C RID: 1564 RVA: 0x00017576 File Offset: 0x00015776
	public static float YScreenEdge { get; private set; } = 35f;

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x0600061D RID: 1565 RVA: 0x0001757E File Offset: 0x0001577E
	// (set) Token: 0x0600061E RID: 1566 RVA: 0x00017586 File Offset: 0x00015786
	public float XScreenEdge
	{
		[CompilerGenerated]
		get
		{
			return this.<XScreenEdge>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.Network<XScreenEdge>k__BackingField = value;
		}
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x0600061F RID: 1567 RVA: 0x0001758F File Offset: 0x0001578F
	// (set) Token: 0x06000620 RID: 1568 RVA: 0x00017597 File Offset: 0x00015797
	public float XplusY { get; private set; }

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000621 RID: 1569 RVA: 0x000175A0 File Offset: 0x000157A0
	// (set) Token: 0x06000622 RID: 1570 RVA: 0x000175A8 File Offset: 0x000157A8
	public float AspectRatio { get; private set; }

	// Token: 0x06000623 RID: 1571 RVA: 0x000175B4 File Offset: 0x000157B4
	private void Start()
	{
		if (!base.isLocalPlayer)
		{
			return;
		}
		Camera component = base.GetComponent<ReferenceHub>().PlayerCameraReference.GetComponent<Camera>();
		AspectRatioSync._defaultCameraFieldOfView = ((component == null) ? 70f : component.fieldOfView);
		AspectRatioSync.YScreenEdge = AspectRatioSync._defaultCameraFieldOfView / 2f;
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x00017608 File Offset: 0x00015808
	private void UpdateAspectRatio()
	{
		this._savedWidth = Screen.width;
		this._savedHeight = Screen.height;
		float aspectRatio = (float)Screen.width / (float)Screen.height;
		this.CmdSetAspectRatio(aspectRatio);
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x00002E9A File Offset: 0x0000109A
	private void FixedUpdate()
	{
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x00017640 File Offset: 0x00015840
	[Command(channel = 4)]
	private void CmdSetAspectRatio(float aspectRatio)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(aspectRatio);
		base.SendCommandInternal("System.Void AspectRatioSync::CmdSetAspectRatio(System.Single)", -837572567, writer, 4, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x0001767A File Offset: 0x0001587A
	public AspectRatioSync()
	{
		this.<XScreenEdge>k__BackingField = 35f;
		this.XplusY = 70f;
		this.AspectRatio = 1f;
		base..ctor();
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x000176A3 File Offset: 0x000158A3
	static AspectRatioSync()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(AspectRatioSync), "System.Void AspectRatioSync::CmdSetAspectRatio(System.Single)", new RemoteCallDelegate(AspectRatioSync.InvokeUserCode_CmdSetAspectRatio__Single), true);
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x0600062A RID: 1578 RVA: 0x000176D0 File Offset: 0x000158D0
	// (set) Token: 0x0600062B RID: 1579 RVA: 0x000176E3 File Offset: 0x000158E3
	public float Network<XScreenEdge>k__BackingField
	{
		get
		{
			return this.<XScreenEdge>k__BackingField;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<float>(value, ref this.<XScreenEdge>k__BackingField, 1UL, null);
		}
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x00017700 File Offset: 0x00015900
	protected void UserCode_CmdSetAspectRatio__Single(float aspectRatio)
	{
		if (aspectRatio < 1f)
		{
			aspectRatio = 1f;
		}
		this.AspectRatio = aspectRatio;
		float num = Mathf.Tan(AspectRatioSync._defaultCameraFieldOfView * 0.017453292f * 0.5f);
		this.XScreenEdge = Mathf.Atan(num * aspectRatio) * 57.29578f;
		this.XplusY = this.XScreenEdge + AspectRatioSync.YScreenEdge;
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x00017760 File Offset: 0x00015960
	protected static void InvokeUserCode_CmdSetAspectRatio__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetAspectRatio called on client.");
			return;
		}
		((AspectRatioSync)obj).UserCode_CmdSetAspectRatio__Single(reader.ReadFloat());
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x0001778C File Offset: 0x0001598C
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteFloat(this.<XScreenEdge>k__BackingField);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteFloat(this.<XScreenEdge>k__BackingField);
		}
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x000177E4 File Offset: 0x000159E4
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<float>(ref this.<XScreenEdge>k__BackingField, null, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<float>(ref this.<XScreenEdge>k__BackingField, null, reader.ReadFloat());
		}
	}

	// Token: 0x04000588 RID: 1416
	private static float _defaultCameraFieldOfView;

	// Token: 0x0400058D RID: 1421
	private int _savedWidth;

	// Token: 0x0400058E RID: 1422
	private int _savedHeight;
}
