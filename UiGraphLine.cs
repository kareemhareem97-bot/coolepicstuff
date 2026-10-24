using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000F6 RID: 246
public class UiGraphLine : Graphic
{
	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x0600049F RID: 1183 RVA: 0x000118F5 File Offset: 0x0000FAF5
	// (set) Token: 0x060004A0 RID: 1184 RVA: 0x000118FD File Offset: 0x0000FAFD
	public List<Vector2> RelativePoints
	{
		get
		{
			return this._relativePoints;
		}
		set
		{
			this._relativePoints = value;
			this.SetVerticesDirty();
		}
	}

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x060004A1 RID: 1185 RVA: 0x0001190C File Offset: 0x0000FB0C
	// (set) Token: 0x060004A2 RID: 1186 RVA: 0x00011914 File Offset: 0x0000FB14
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

	// Token: 0x060004A3 RID: 1187 RVA: 0x00011924 File Offset: 0x0000FB24
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.color = this.color;
		int count = this._relativePoints.Count;
		if (count <= 1)
		{
			return;
		}
		this._rectSize = base.rectTransform.rect.size;
		this._rectOffset = base.rectTransform.pivot * this._rectSize;
		for (int i = 0; i < count - 1; i++)
		{
			Vector2 vector = this.ClampPoint(this._relativePoints[i]);
			Vector2 vector2 = this.ClampPoint(this._relativePoints[i + 1]);
			float rotation = this.RealAngle(vector2 - vector);
			this.PlacePoint(vh, vector, rotation);
			this.PlacePoint(vh, vector2, rotation);
			this.FillRectangle(vh, i * 4);
		}
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x00011A00 File Offset: 0x0000FC00
	private void FillRectangle(VertexHelper vh, int startIndex)
	{
		vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
		vh.AddTriangle(startIndex + 1, startIndex + 2, startIndex + 3);
		if (startIndex == 0)
		{
			return;
		}
		vh.AddTriangle(startIndex - 2, startIndex, startIndex + 1);
		vh.AddTriangle(startIndex - 1, startIndex, startIndex + 1);
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x00011A49 File Offset: 0x0000FC49
	private float RealAngle(Vector2 v2)
	{
		return Vector2.Angle(v2, Vector2.up) * -Mathf.Sign(Vector2.Dot(v2, Vector2.right));
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x00011A68 File Offset: 0x0000FC68
	private void PlacePoint(VertexHelper vh, Vector2 p, float rotation)
	{
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.color = this.color;
		float y = Mathf.Sin(rotation * 0.017453292f);
		Vector2 b = new Vector2(Mathf.Cos(rotation * 0.017453292f), y) * this.Width;
		simpleVert.position = p * this._rectSize + b - this._rectOffset;
		vh.AddVert(simpleVert);
		simpleVert.position = p * this._rectSize - b - this._rectOffset;
		vh.AddVert(simpleVert);
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x00011B18 File Offset: 0x0000FD18
	private Vector2 ClampPoint(Vector2 point)
	{
		return new Vector2(Mathf.Clamp01(point.x), Mathf.Clamp01(point.y));
	}

	// Token: 0x0400040B RID: 1035
	[SerializeField]
	private List<Vector2> _relativePoints = new List<Vector2>();

	// Token: 0x0400040C RID: 1036
	[SerializeField]
	private float _width;

	// Token: 0x0400040D RID: 1037
	private Vector2 _rectSize;

	// Token: 0x0400040E RID: 1038
	private Vector2 _rectOffset;
}
