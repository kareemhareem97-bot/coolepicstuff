using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000F5 RID: 245
public class UiCircle : MaskableGraphic
{
	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x06000497 RID: 1175 RVA: 0x00011748 File Offset: 0x0000F948
	// (set) Token: 0x06000498 RID: 1176 RVA: 0x00011750 File Offset: 0x0000F950
	public float Radius
	{
		get
		{
			return this._radius;
		}
		set
		{
			this._radius = value;
			this.SetVerticesDirty();
		}
	}

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x06000499 RID: 1177 RVA: 0x0001175F File Offset: 0x0000F95F
	// (set) Token: 0x0600049A RID: 1178 RVA: 0x00011767 File Offset: 0x0000F967
	public float Width
	{
		get
		{
			return this._width;
		}
		set
		{
			this._width = value;
			this.SetVerticesDirty();
		}
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x00011778 File Offset: 0x0000F978
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.color = this.color;
		float radius = this.Radius;
		float num = this.Radius - this.Width;
		this.DrawVertCircle(vh, simpleVert, radius);
		if (num <= 0f)
		{
			this.RenderFullCircle(vh, simpleVert);
			return;
		}
		this.DrawVertCircle(vh, simpleVert, num);
		for (int i = 0; i < this._vertsCount; i++)
		{
			vh.AddTriangle(i, (i + 1) % this._vertsCount, i + this._vertsCount);
			vh.AddTriangle(i, i + this._vertsCount, (i - 1 + this._vertsCount) % this._vertsCount + this._vertsCount);
		}
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x0001182C File Offset: 0x0000FA2C
	private void DrawVertCircle(VertexHelper vh, UIVertex vert, float radius)
	{
		float num = 6.2831855f / (float)this._vertsCount;
		float num2 = 0f;
		for (int i = 0; i < this._vertsCount; i++)
		{
			float num3 = Mathf.Sin(num2);
			float num4 = Mathf.Sin(num2 + 1.5707964f);
			vert.position = new Vector2(num3 * radius, num4 * radius);
			vh.AddVert(vert);
			num2 += num;
		}
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x00011898 File Offset: 0x0000FA98
	private void RenderFullCircle(VertexHelper vh, UIVertex vert)
	{
		vert.position = Vector2.zero;
		vh.AddVert(vert);
		for (int i = 0; i < this._vertsCount; i++)
		{
			vh.AddTriangle(i, (i + 1) % this._vertsCount, this._vertsCount);
		}
	}

	// Token: 0x04000408 RID: 1032
	[Range(3f, 180f)]
	[SerializeField]
	private int _vertsCount = 64;

	// Token: 0x04000409 RID: 1033
	[SerializeField]
	private float _radius;

	// Token: 0x0400040A RID: 1034
	[SerializeField]
	private float _width;
}
