using System;
using UnityEngine;

// Token: 0x02000067 RID: 103
public class LightIntensityAnimation : MonoBehaviour
{
	// Token: 0x0600024D RID: 589 RVA: 0x0000924A File Offset: 0x0000744A
	private void Awake()
	{
		this._roomLight = base.GetComponent<RoomLight>();
		this._animation.postWrapMode = WrapMode.Loop;
	}

	// Token: 0x0600024E RID: 590 RVA: 0x00009264 File Offset: 0x00007464
	private void Reset()
	{
		this._intensityMultiplier = base.GetComponent<Light>().intensity;
		this._animation = AnimationCurve.Constant(0f, 1f, 1f);
	}

	// Token: 0x0600024F RID: 591 RVA: 0x00009294 File Offset: 0x00007494
	private void Update()
	{
		if (this._roomLight.IsCulled)
		{
			return;
		}
		float time = Time.timeSinceLevelLoad + this._timeOffset;
		float intensity = this._intensityMultiplier * this._animation.Evaluate(time);
		this._roomLight.LightSource.intensity = intensity;
	}

	// Token: 0x040001E9 RID: 489
	private RoomLight _roomLight;

	// Token: 0x040001EA RID: 490
	[SerializeField]
	private AnimationCurve _animation;

	// Token: 0x040001EB RID: 491
	[SerializeField]
	private float _intensityMultiplier;

	// Token: 0x040001EC RID: 492
	[SerializeField]
	private float _timeOffset;
}
