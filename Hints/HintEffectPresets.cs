using System;
using UnityEngine;

namespace Hints
{
	// Token: 0x02000807 RID: 2055
	public static class HintEffectPresets
	{
		// Token: 0x06002F90 RID: 12176 RVA: 0x000A0354 File Offset: 0x0009E554
		public static Keyframe[] CreateBumpKeyframes(float floorValue, float bumpValue, int count, float duration = 1f)
		{
			Keyframe[] array = new Keyframe[count * 2 + 1];
			float num = duration / (float)array.Length;
			float num2 = 0f;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Keyframe(num2, (i % 2 == 0) ? floorValue : bumpValue);
				num2 += num;
			}
			return array;
		}

		// Token: 0x06002F91 RID: 12177 RVA: 0x000A03A2 File Offset: 0x0009E5A2
		public static AnimationCurve CreateBumpCurve(float floorValue, float bumpValue, int count, float duration = 1f)
		{
			return new AnimationCurve(HintEffectPresets.CreateBumpKeyframes(floorValue, bumpValue, count, duration))
			{
				postWrapMode = WrapMode.Loop
			};
		}

		// Token: 0x06002F92 RID: 12178 RVA: 0x000A03BC File Offset: 0x0009E5BC
		public static AnimationCurve CreateTrailingBumpCurve(float floorValue, float bumpValue, int count, float startTrailPercent, float duration = 1f)
		{
			Keyframe[] array = HintEffectPresets.CreateBumpKeyframes(floorValue, bumpValue, count, duration * startTrailPercent);
			Array.Resize<Keyframe>(ref array, array.Length + 1);
			array[array.Length - 1] = new Keyframe(duration, floorValue);
			return new AnimationCurve(array)
			{
				postWrapMode = WrapMode.Loop
			};
		}

		// Token: 0x06002F93 RID: 12179 RVA: 0x000A0402 File Offset: 0x0009E602
		public static AlphaCurveHintEffect FadeIn(float durationScalar = 1f, float startScalar = 0f, float iterations = 1f)
		{
			AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, iterations, 1f);
			animationCurve.postWrapMode = WrapMode.Loop;
			return new AlphaCurveHintEffect(animationCurve, startScalar, durationScalar);
		}

		// Token: 0x06002F94 RID: 12180 RVA: 0x000A0427 File Offset: 0x0009E627
		public static AlphaCurveHintEffect FadeOut(float durationScalar = 1f, float startScalar = 0f, float iterations = 1f)
		{
			AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 1f, iterations, 0f);
			animationCurve.postWrapMode = WrapMode.Loop;
			return new AlphaCurveHintEffect(animationCurve, startScalar, durationScalar);
		}

		// Token: 0x06002F95 RID: 12181 RVA: 0x000A044C File Offset: 0x0009E64C
		public static HintEffect[] FadeInAndOut(float window, float durationScalar = 1f, float startScalar = 0f)
		{
			float num = (durationScalar - window) / 2f;
			return new HintEffect[]
			{
				HintEffectPresets.FadeIn(num, startScalar, 1f),
				HintEffectPresets.FadeOut(num, startScalar + durationScalar - num, 1f)
			};
		}

		// Token: 0x06002F96 RID: 12182 RVA: 0x000A048B File Offset: 0x0009E68B
		public static AlphaCurveHintEffect PulseAlpha(float floorValue, float peakValue, float iterationScalar = 1f, float startOffset = 0f)
		{
			return new AlphaCurveHintEffect(HintEffectPresets.CreateBumpCurve(floorValue, peakValue, 1, iterationScalar), startOffset, 1f);
		}

		// Token: 0x06002F97 RID: 12183 RVA: 0x000A04A1 File Offset: 0x0009E6A1
		public static AlphaCurveHintEffect TrailingPulseAlpha(float floorValue, float peakValue, float startTrailScalar, float iterationScalar = 1f, float startScalar = 0f, int count = 1)
		{
			return new AlphaCurveHintEffect(HintEffectPresets.CreateTrailingBumpCurve(floorValue, peakValue, count, startTrailScalar, iterationScalar), startScalar, 1f);
		}
	}
}
