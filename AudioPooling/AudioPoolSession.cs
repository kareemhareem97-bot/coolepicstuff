using System;
using UnityEngine;

namespace AudioPooling
{
	// Token: 0x02000983 RID: 2435
	public readonly struct AudioPoolSession : IEquatable<AudioPoolSession>
	{
		// Token: 0x17000AE8 RID: 2792
		// (get) Token: 0x06003727 RID: 14119 RVA: 0x000B3150 File Offset: 0x000B1350
		public bool SameSession
		{
			get
			{
				return this.HandledInstance != null && !this.HandledInstance.Pooled && this.HandledInstance.TotalRecycles == this._sessionId;
			}
		}

		// Token: 0x17000AE9 RID: 2793
		// (get) Token: 0x06003728 RID: 14120 RVA: 0x000B3182 File Offset: 0x000B1382
		public bool IsPlaying
		{
			get
			{
				return this.SameSession && this.Source.isPlaying;
			}
		}

		// Token: 0x17000AEA RID: 2794
		// (get) Token: 0x06003729 RID: 14121 RVA: 0x000B3199 File Offset: 0x000B1399
		public AudioSource Source
		{
			get
			{
				return this.HandledInstance.Source;
			}
		}

		// Token: 0x0600372A RID: 14122 RVA: 0x000B31A6 File Offset: 0x000B13A6
		public AudioPoolSession(PooledAudioSource subject)
		{
			this.HandledInstance = subject;
			this._sessionId = subject.TotalRecycles;
		}

		// Token: 0x0600372B RID: 14123 RVA: 0x000B31BB File Offset: 0x000B13BB
		public bool Equals(AudioPoolSession other)
		{
			return this.HandledInstance == other.HandledInstance && this._sessionId == other._sessionId;
		}

		// Token: 0x0600372C RID: 14124 RVA: 0x000B31E0 File Offset: 0x000B13E0
		public override int GetHashCode()
		{
			return HashCode.Combine<ulong, int>(this._sessionId, (this.HandledInstance == null) ? 0 : this.HandledInstance.GetHashCode());
		}

		// Token: 0x0600372D RID: 14125 RVA: 0x000B320C File Offset: 0x000B140C
		public override bool Equals(object obj)
		{
			if (obj is AudioPoolSession)
			{
				AudioPoolSession other = (AudioPoolSession)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x0600372E RID: 14126 RVA: 0x000B3231 File Offset: 0x000B1431
		public override string ToString()
		{
			return base.ToString();
		}

		// Token: 0x0600372F RID: 14127 RVA: 0x000B3243 File Offset: 0x000B1443
		public static bool operator ==(AudioPoolSession left, AudioPoolSession right)
		{
			return left.Equals(right);
		}

		// Token: 0x06003730 RID: 14128 RVA: 0x000B324D File Offset: 0x000B144D
		public static bool operator !=(AudioPoolSession left, AudioPoolSession right)
		{
			return !left.Equals(right);
		}

		// Token: 0x0400297D RID: 10621
		private readonly ulong _sessionId;

		// Token: 0x0400297E RID: 10622
		public readonly PooledAudioSource HandledInstance;
	}
}
