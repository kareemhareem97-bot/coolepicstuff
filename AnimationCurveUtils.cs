using System;
using UnityEngine;

// Token: 0x020001D7 RID: 471
public static class AnimationCurveUtils
{
	// Token: 0x060009B4 RID: 2484 RVA: 0x00026FC2 File Offset: 0x000251C2
	public static AnimationCurve MakeLinearCurve(Keyframe[] keyframes, WrapMode preWrapMode = WrapMode.Once, WrapMode postWrapMode = WrapMode.Once)
	{
		return new AnimationCurve(AnimationCurveUtils.MakeLinearKeyframes(keyframes))
		{
			preWrapMode = preWrapMode,
			postWrapMode = postWrapMode
		};
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x00026FE0 File Offset: 0x000251E0
	public static Keyframe[] MakeLinearKeyframes(params Keyframe[] keyframes)
	{
		for (int i = 0; i < keyframes.Length; i++)
		{
			keyframes[i] = AnimationCurveUtils.MakeLinearKeyframe(keyframes[i]);
		}
		return keyframes;
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x0002700F File Offset: 0x0002520F
	public static Keyframe MakeLinearKeyframe(float time, float value)
	{
		return new Keyframe(time, value, 0f, 0f, 0f, 0f);
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x0002702C File Offset: 0x0002522C
	public static Keyframe MakeLinearKeyframe(Keyframe keyframe)
	{
		return AnimationCurveUtils.MakeLinearKeyframe(keyframe.time, keyframe.value);
	}
}
