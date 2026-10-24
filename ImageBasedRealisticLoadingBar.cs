using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000064 RID: 100
public class ImageBasedRealisticLoadingBar : MonoBehaviour
{
	// Token: 0x06000247 RID: 583 RVA: 0x00009070 File Offset: 0x00007270
	private void OnEnable()
	{
		this._targetImage.fillAmount = 0f;
		this._bar = new RealisticLoadingBar(this._targetTime, UnityEngine.Random.Range(this._minNumOfSteps, this._maxNumOfSteps + 1), this._stepVar, this._tickVar);
	}

	// Token: 0x06000248 RID: 584 RVA: 0x000090BD File Offset: 0x000072BD
	private void Update()
	{
		this._targetImage.fillAmount = Mathf.Lerp(this._targetImage.fillAmount, this._bar.Progress, Time.deltaTime * this._smoothing);
	}

	// Token: 0x040001DF RID: 479
	private RealisticLoadingBar _bar;

	// Token: 0x040001E0 RID: 480
	[SerializeField]
	private float _targetTime;

	// Token: 0x040001E1 RID: 481
	[SerializeField]
	private float _stepVar;

	// Token: 0x040001E2 RID: 482
	[SerializeField]
	private float _tickVar;

	// Token: 0x040001E3 RID: 483
	[SerializeField]
	private int _minNumOfSteps;

	// Token: 0x040001E4 RID: 484
	[SerializeField]
	private int _maxNumOfSteps;

	// Token: 0x040001E5 RID: 485
	[SerializeField]
	private Image _targetImage;

	// Token: 0x040001E6 RID: 486
	[SerializeField]
	private float _smoothing;
}
