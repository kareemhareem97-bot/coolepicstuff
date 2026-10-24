using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCore
{
	// Token: 0x02000860 RID: 2144
	public class RoundStart : NetworkBehaviour
	{
		// Token: 0x17000909 RID: 2313
		// (get) Token: 0x0600310B RID: 12555 RVA: 0x000A4BD7 File Offset: 0x000A2DD7
		public static bool RoundStarted
		{
			get
			{
				return RoundStart._singletonSet && RoundStart.singleton.Timer == -1;
			}
		}

		// Token: 0x1700090A RID: 2314
		// (get) Token: 0x0600310C RID: 12556 RVA: 0x000A4BEF File Offset: 0x000A2DEF
		public static TimeSpan RoundLength
		{
			get
			{
				return RoundStart.RoundStartTimer.Elapsed;
			}
		}

		// Token: 0x0600310D RID: 12557 RVA: 0x000A4BFB File Offset: 0x000A2DFB
		static RoundStart()
		{
			SceneManager.sceneLoaded += RoundStart.OnSceneLoaded;
		}

		// Token: 0x0600310E RID: 12558 RVA: 0x000A4C18 File Offset: 0x000A2E18
		private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			RoundStart.RoundStartTimer.Reset();
		}

		// Token: 0x0600310F RID: 12559 RVA: 0x000A4C24 File Offset: 0x000A2E24
		private void Start()
		{
			base.GetComponent<RectTransform>().localPosition = Vector3.zero;
		}

		// Token: 0x06003110 RID: 12560 RVA: 0x000A4C36 File Offset: 0x000A2E36
		private void Awake()
		{
			RoundStart.singleton = this;
			RoundStart._singletonSet = true;
		}

		// Token: 0x06003111 RID: 12561 RVA: 0x000A4C44 File Offset: 0x000A2E44
		private void OnDestroy()
		{
			RoundStart._singletonSet = false;
		}

		// Token: 0x06003112 RID: 12562 RVA: 0x00002E9A File Offset: 0x0000109A
		private void Update()
		{
		}

		// Token: 0x06003114 RID: 12564 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x1700090B RID: 2315
		// (get) Token: 0x06003115 RID: 12565 RVA: 0x000A4C64 File Offset: 0x000A2E64
		// (set) Token: 0x06003116 RID: 12566 RVA: 0x000A4C77 File Offset: 0x000A2E77
		public short NetworkTimer
		{
			get
			{
				return this.Timer;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<short>(value, ref this.Timer, 1UL, null);
			}
		}

		// Token: 0x06003117 RID: 12567 RVA: 0x000A4C94 File Offset: 0x000A2E94
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				writer.WriteShort(this.Timer);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 1UL) != 0UL)
			{
				writer.WriteShort(this.Timer);
			}
		}

		// Token: 0x06003118 RID: 12568 RVA: 0x000A4CEC File Offset: 0x000A2EEC
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<short>(ref this.Timer, null, reader.ReadShort());
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 1L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<short>(ref this.Timer, null, reader.ReadShort());
			}
		}

		// Token: 0x040025FF RID: 9727
		public static RoundStart singleton;

		// Token: 0x04002600 RID: 9728
		public static bool LobbyLock;

		// Token: 0x04002601 RID: 9729
		private static bool _singletonSet;

		// Token: 0x04002602 RID: 9730
		[SyncVar]
		public short Timer = -2;

		// Token: 0x04002603 RID: 9731
		private short _lastTimer = -1;

		// Token: 0x04002604 RID: 9732
		internal static readonly Stopwatch RoundStartTimer = new Stopwatch();
	}
}
