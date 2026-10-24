using System;
using System.Collections.Generic;
using UnityEngine;

namespace DrawableLine
{
	// Token: 0x02000988 RID: 2440
	public class DrawableLinesManager : MonoBehaviour
	{
		// Token: 0x0600377D RID: 14205 RVA: 0x000B40E4 File Offset: 0x000B22E4
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, float width = 0.05f)
		{
			if (!DrawableLinesManager._singletonSet)
			{
				return;
			}
			Vector3 vector = end - start;
			Vector3 pos = Vector3.Lerp(start, end, 0.5f);
			float item = Time.time + duration;
			float magnitude = vector.magnitude;
			Quaternion q = Quaternion.LookRotation(vector.normalized) * DrawableLinesManager.QuadCorrection;
			Vector3 s = new Vector3(width, magnitude, 1f);
			Matrix4x4 item2 = Matrix4x4.TRS(pos, q, s);
			DrawableLinesManager._singleton._timeStamps.Add(item);
			DrawableLinesManager._singleton._matrices.Add(item2);
			DrawableLinesManager._singleton._colors.Add(color);
		}

		// Token: 0x0600377E RID: 14206 RVA: 0x000B4184 File Offset: 0x000B2384
		public static void ApplyMaxDurationRetroactively(float newMaxDuration)
		{
			if (!DrawableLinesManager._singletonSet)
			{
				return;
			}
			float num = Time.time + newMaxDuration;
			for (int i = 0; i < DrawableLinesManager._singleton._timeStamps.Count; i++)
			{
				if (DrawableLinesManager._singleton._timeStamps[i] >= num)
				{
					DrawableLinesManager._singleton._timeStamps[i] = num;
				}
			}
		}

		// Token: 0x0600377F RID: 14207 RVA: 0x000B41E0 File Offset: 0x000B23E0
		private void DrawMeshes()
		{
			if (this._matrices.Count <= 0)
			{
				return;
			}
			this.CleanupExpiredLines();
			this._props.Clear();
			this._props.SetVectorArray("_Color", this._colors);
			Graphics.DrawMeshInstanced(this._lineMesh, 0, this._lineMaterial, this._matrices, this._props);
		}

		// Token: 0x06003780 RID: 14208 RVA: 0x000B4244 File Offset: 0x000B2444
		private void CleanupExpiredLines()
		{
			for (int i = this._matrices.Count - 1; i >= 0; i--)
			{
				if (this._timeStamps[i] <= Time.time)
				{
					this._colors.RemoveAt(i);
					this._matrices.RemoveAt(i);
					this._timeStamps.RemoveAt(i);
				}
			}
		}

		// Token: 0x0400299D RID: 10653
		private const int MaxExpectedSimultaneousMeshes = 550;

		// Token: 0x0400299E RID: 10654
		private const string ColorKey = "_Color";

		// Token: 0x0400299F RID: 10655
		private static readonly Quaternion QuadCorrection = Quaternion.Euler(90f, 0f, 0f);

		// Token: 0x040029A0 RID: 10656
		private static DrawableLinesManager _singleton;

		// Token: 0x040029A1 RID: 10657
		private static bool _singletonSet;

		// Token: 0x040029A2 RID: 10658
		[SerializeField]
		private Material _lineMaterial;

		// Token: 0x040029A3 RID: 10659
		[SerializeField]
		private Mesh _lineMesh;

		// Token: 0x040029A4 RID: 10660
		private readonly List<Matrix4x4> _matrices = new List<Matrix4x4>(550);

		// Token: 0x040029A5 RID: 10661
		private readonly List<Vector4> _colors = new List<Vector4>(550);

		// Token: 0x040029A6 RID: 10662
		private readonly List<float> _timeStamps = new List<float>(550);

		// Token: 0x040029A7 RID: 10663
		private MaterialPropertyBlock _props;
	}
}
