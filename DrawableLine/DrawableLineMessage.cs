using System;
using Mirror;
using UnityEngine;

namespace DrawableLine
{
	// Token: 0x02000989 RID: 2441
	public readonly struct DrawableLineMessage : NetworkMessage
	{
		// Token: 0x06003783 RID: 14211 RVA: 0x000B42F3 File Offset: 0x000B24F3
		public DrawableLineMessage(float? duration, Color? color, Vector3[] positions)
		{
			this._duration = duration;
			this._color = color;
			this._positions = positions;
		}

		// Token: 0x06003784 RID: 14212 RVA: 0x000B430A File Offset: 0x000B250A
		public DrawableLineMessage(NetworkReader reader)
		{
			this._duration = reader.ReadFloatNullable();
			this._color = reader.ReadColorNullable();
			this._positions = reader.ReadArray<Vector3>();
		}

		// Token: 0x06003785 RID: 14213 RVA: 0x000B4330 File Offset: 0x000B2530
		public void WriteMessage(NetworkWriter writer)
		{
			writer.WriteFloatNullable(this._duration);
			writer.WriteColorNullable(this._color);
			writer.WriteArray(this._positions);
		}

		// Token: 0x06003786 RID: 14214 RVA: 0x000B4356 File Offset: 0x000B2556
		public void GenerateLine()
		{
			DrawableLines.ClientGenerateLine(this._duration, this._color, this._positions);
		}

		// Token: 0x040029A8 RID: 10664
		private readonly float? _duration;

		// Token: 0x040029A9 RID: 10665
		private readonly Color? _color;

		// Token: 0x040029AA RID: 10666
		private readonly Vector3[] _positions;
	}
}
