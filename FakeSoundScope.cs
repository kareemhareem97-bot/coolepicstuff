using System;
using UnityEngine;

// Token: 0x020000BB RID: 187
public class FakeSoundScope : MonoBehaviour
{
	// Token: 0x060003B2 RID: 946 RVA: 0x0000ECA7 File Offset: 0x0000CEA7
	private void Awake()
	{
		this.line = base.GetComponent<LineRenderer>();
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x0000ECB8 File Offset: 0x0000CEB8
	private void LateUpdate()
	{
		Vector3[] array = new Vector3[this.numOfPos];
		float value = UnityEngine.Random.value;
		float num = 0f;
		for (int i = 0; i < this.numOfPos; i++)
		{
			float num2 = (float)i / (float)this.numOfPos;
			float num3 = Mathf.Abs(1f - Mathf.Abs(num2 - 0.5f) * 2f);
			array[i][0] = num2 * 100f;
			array[i][2] = Mathf.Sin((float)(i * 7) * value) * num3 * this.maxH * (Mathf.Sin((float)i) / 3f) * num;
		}
		this.line.SetPositions(array);
	}

	// Token: 0x0400034B RID: 843
	public AnimationCurve highOverVolume;

	// Token: 0x0400034C RID: 844
	public int numOfPos;

	// Token: 0x0400034D RID: 845
	private LineRenderer line;

	// Token: 0x0400034E RID: 846
	public float maxH;
}
