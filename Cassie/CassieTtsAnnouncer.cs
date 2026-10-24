using System;
using System.Collections.Generic;
using System.Text;
using Cassie.Interpreters;
using NorthwoodLib.Pools;
using UnityEngine;

namespace Cassie
{
	// Token: 0x02000ABE RID: 2750
	public class CassieTtsAnnouncer : MonoBehaviour
	{
		// Token: 0x140000B3 RID: 179
		// (add) Token: 0x06003E09 RID: 15881 RVA: 0x000CCDC8 File Offset: 0x000CAFC8
		// (remove) Token: 0x06003E0A RID: 15882 RVA: 0x000CCDFC File Offset: 0x000CAFFC
		public static event Action OnSingletonSet;

		// Token: 0x17000E1B RID: 3611
		// (get) Token: 0x06003E0B RID: 15883 RVA: 0x000CCE2F File Offset: 0x000CB02F
		// (set) Token: 0x06003E0C RID: 15884 RVA: 0x000CCE36 File Offset: 0x000CB036
		public static CassieTtsPayload LastAnnouncement { get; private set; }

		// Token: 0x06003E0D RID: 15885 RVA: 0x000CCE40 File Offset: 0x000CB040
		public static bool TryPlay(CassieTtsPayload tts, out float totalDuration)
		{
			CassieTtsAnnouncer.Clear();
			if (!CassieTtsAnnouncer._singletonSet)
			{
				totalDuration = 0f;
				return false;
			}
			double dspTime = AudioSettings.dspTime;
			CassieTtsAnnouncer._nextWordDsp = dspTime;
			if (tts.PlayBackground)
			{
				CassieTtsAnnouncer._nextWordDsp += 2.5;
			}
			StringBuilder stringBuilder = (tts.SubtitleSource == CassieTtsPayload.SubtitleMode.Automatic) ? StringBuilderPool.Shared.Rent() : null;
			CassieTtsAnnouncer._singleton.ScheduleClips(tts.Content.AsSpan(), default(CassiePlaybackModifiers), stringBuilder);
			totalDuration = (float)(CassieTtsAnnouncer._nextWordDsp - dspTime);
			if (tts.PlayBackground)
			{
				double num = dspTime + (double)CassieTtsAnnouncer._singleton._schedulerOffset;
				double endingTime = num + (double)totalDuration;
				CassieTtsAnnouncer._singleton._background.Play(num, endingTime);
			}
			switch (tts.SubtitleSource)
			{
			case CassieTtsPayload.SubtitleMode.Automatic:
				tts.PlayOtherSubtitle((double)totalDuration, stringBuilder.ToString());
				StringBuilderPool.Shared.Return(stringBuilder);
				break;
			case CassieTtsPayload.SubtitleMode.Custom:
				tts.PlayOtherSubtitle((double)totalDuration, null);
				break;
			case CassieTtsPayload.SubtitleMode.FromTranslation:
				tts.PlaySubtitleMessage();
				break;
			}
			CassieTtsAnnouncer.LastAnnouncement = tts;
			return true;
		}

		// Token: 0x06003E0E RID: 15886 RVA: 0x000CCF50 File Offset: 0x000CB150
		public static void Clear()
		{
			if (!CassieTtsAnnouncer._singletonSet)
			{
				return;
			}
			CassieTtsAnnouncer._singleton._background.Clear();
			CassieTtsAnnouncer._singleton._scheduledClips.Clear();
			foreach (CassieLineSource cassieLineSource in CassieTtsAnnouncer._singleton._sourcePool)
			{
				if (cassieLineSource.IsBusy)
				{
					cassieLineSource.Clear();
				}
			}
		}

		// Token: 0x06003E0F RID: 15887 RVA: 0x000CCFD4 File Offset: 0x000CB1D4
		public static bool TryGetDatabase(out CassieLineDatabase db)
		{
			if (CassieTtsAnnouncer._singletonSet)
			{
				db = CassieTtsAnnouncer._singleton._clipDatabase;
				return true;
			}
			db = null;
			return false;
		}

		// Token: 0x06003E10 RID: 15888 RVA: 0x000CCFF0 File Offset: 0x000CB1F0
		private void ScheduleClips(ReadOnlySpan<char> remaining, CassiePlaybackModifiers modifiers, StringBuilder autoSubtitleBuilder)
		{
			int num = remaining.IndexOf(' ');
			ReadOnlySpan<char> readOnlySpan2;
			if (num >= 0)
			{
				ReadOnlySpan<char> readOnlySpan = remaining;
				readOnlySpan2 = readOnlySpan.Slice(0, num);
			}
			else
			{
				readOnlySpan2 = remaining;
			}
			ReadOnlySpan<char> word = readOnlySpan2;
			CassieInterpreter[] interpreters = CassieTtsAnnouncer.Interpreters;
			for (int i = 0; i < interpreters.Length; i++)
			{
				bool flag;
				foreach (CassieInterpreter.Result result in interpreters[i].GetResults(this._clipDatabase, ref modifiers, word, autoSubtitleBuilder, out flag))
				{
					this._scheduledClips.Enqueue(new CassieTtsAnnouncer.ScheduledClip(result.Line, CassieTtsAnnouncer._nextWordDsp, result.Modifiers));
					CassieTtsAnnouncer._nextWordDsp += result.Modifiers.GetTimeUntilNextWord(result.Line);
				}
				if (flag)
				{
					break;
				}
			}
			if (num >= 0)
			{
				ReadOnlySpan<char> readOnlySpan = remaining;
				int i = num + 1;
				remaining = readOnlySpan.Slice(i, readOnlySpan.Length - i);
				this.ScheduleClips(remaining, modifiers, autoSubtitleBuilder);
			}
		}

		// Token: 0x06003E11 RID: 15889 RVA: 0x000CD0F8 File Offset: 0x000CB2F8
		private void Awake()
		{
			CassieTtsAnnouncer._singleton = this;
			CassieTtsAnnouncer._singletonSet = true;
			this._clipDatabase.ValidateCache(false);
		}

		// Token: 0x06003E12 RID: 15890 RVA: 0x000CD112 File Offset: 0x000CB312
		private void OnDestroy()
		{
			CassieTtsAnnouncer._singletonSet = false;
		}

		// Token: 0x06003E13 RID: 15891 RVA: 0x000CD11C File Offset: 0x000CB31C
		private void Update()
		{
			double dspTime = AudioSettings.dspTime;
			foreach (CassieLineSource cassieLineSource in CassieTtsAnnouncer._singleton._sourcePool)
			{
				cassieLineSource.UpdateSpatialization(this._clipSourceTemplate);
			}
			CassieTtsAnnouncer.ScheduledClip scheduledClip;
			if (!this._scheduledClips.TryPeek(out scheduledClip))
			{
				if (dspTime > this._unloadingDspTime)
				{
					CassieLineSource.UpdateUnusedClips();
				}
				return;
			}
			if (scheduledClip.DspTime - dspTime > (double)this._preloadTime)
			{
				return;
			}
			CassieLineSource cassieLineSource2;
			CassieLineSource cassieLineSource3;
			if (!this._sourcePool.TryPeek(out cassieLineSource2) || cassieLineSource2.IsBusy)
			{
				if (this._sourcePool.Count > 25)
				{
					return;
				}
				cassieLineSource3 = UnityEngine.Object.Instantiate<CassieLineSource>(this._clipSourceTemplate, base.transform);
			}
			else
			{
				cassieLineSource3 = this._sourcePool.Dequeue();
			}
			cassieLineSource3.Schedule(scheduledClip.Value, scheduledClip.DspTime + (double)this._schedulerOffset, scheduledClip.Modifiers);
			cassieLineSource3.UpdateSpatialization(this._clipSourceTemplate);
			this._unloadingDspTime = Math.Max(this._unloadingDspTime, cassieLineSource3.ScheduledEndTime);
			this._scheduledClips.Dequeue();
			this._sourcePool.Enqueue(cassieLineSource3);
		}

		// Token: 0x04002D64 RID: 11620
		public static readonly CassieInterpreter[] Interpreters = new CassieInterpreter[]
		{
			new LookupWordInterpreter(),
			new ModifierInterpreter(),
			new NumberInterpreter(),
			new PastTenseInterpreter(),
			new ContinuousInterpreter(),
			new PluralInterpreter(),
			new DegreeInterpreter()
		};

		// Token: 0x04002D65 RID: 11621
		private const int MaxInstantiatedSources = 25;

		// Token: 0x04002D66 RID: 11622
		private static CassieTtsAnnouncer _singleton;

		// Token: 0x04002D67 RID: 11623
		private static bool _singletonSet;

		// Token: 0x04002D68 RID: 11624
		private static double _nextWordDsp;

		// Token: 0x04002D69 RID: 11625
		private double _unloadingDspTime;

		// Token: 0x04002D6A RID: 11626
		private readonly Queue<CassieTtsAnnouncer.ScheduledClip> _scheduledClips = new Queue<CassieTtsAnnouncer.ScheduledClip>();

		// Token: 0x04002D6B RID: 11627
		private readonly Queue<CassieLineSource> _sourcePool = new Queue<CassieLineSource>();

		// Token: 0x04002D6C RID: 11628
		[SerializeField]
		private CassieLineDatabase _clipDatabase;

		// Token: 0x04002D6D RID: 11629
		[SerializeField]
		private CassieLineSource _clipSourceTemplate;

		// Token: 0x04002D6E RID: 11630
		[SerializeField]
		private CassieBackgroundPlayer _background;

		// Token: 0x04002D6F RID: 11631
		[SerializeField]
		private float _preloadTime;

		// Token: 0x04002D70 RID: 11632
		[SerializeField]
		private float _schedulerOffset;

		// Token: 0x02000ABF RID: 2751
		private readonly struct ScheduledClip
		{
			// Token: 0x06003E16 RID: 15894 RVA: 0x000CD2C0 File Offset: 0x000CB4C0
			public ScheduledClip(CassieLine value, double dspTime, CassiePlaybackModifiers modifiers)
			{
				this.Value = value;
				this.DspTime = dspTime;
				this.Modifiers = modifiers;
			}

			// Token: 0x04002D73 RID: 11635
			public readonly CassieLine Value;

			// Token: 0x04002D74 RID: 11636
			public readonly double DspTime;

			// Token: 0x04002D75 RID: 11637
			public readonly CassiePlaybackModifiers Modifiers;
		}
	}
}
