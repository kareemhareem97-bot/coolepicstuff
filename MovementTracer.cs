using System;
using RelativePositioning;
using UnityEngine;

// Token: 0x02000132 RID: 306
public class MovementTracer
{
	// Token: 0x060005BB RID: 1467 RVA: 0x000153BC File Offset: 0x000135BC
	public MovementTracer(byte size, byte cooldown, float teleportDistance)
	{
		this._size = size;
		this.Positions = new RelativePosition[(int)size];
		this._cooldown = cooldown;
		this._tpDis = teleportDistance;
		this._cooldownTimer = 0;
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x000153EC File Offset: 0x000135EC
	public void Record(Vector3 plyPosition)
	{
		if (this._cooldownTimer > 0)
		{
			this._cooldownTimer -= 1;
			return;
		}
		byte b = this.Clock + 1;
		this.Clock = b;
		if (b >= this._size)
		{
			this.Clock = 0;
		}
		this.Positions[(int)this.Clock] = new RelativePosition(plyPosition);
		this._cooldownTimer = this._cooldown;
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x00015458 File Offset: 0x00013658
	public Bounds GenerateBounds(float time, bool ignoreTeleports)
	{
		int num = Mathf.FloorToInt(time / Time.fixedDeltaTime / (float)(this._cooldown + 1));
		if (num <= 0)
		{
			Debug.LogError(string.Format("MovementTracer was requested to generate Bounds for the last {0} seconds, but it's too short. Please access PlayerMovementSync.RealModelPosition directly.", time));
			num = 1;
		}
		else if (num > (int)this._size)
		{
			Debug.LogError(string.Format("MovementTracer was requested to generate Bounds for the last {0} seconds, but it can't keep track of positions after {1}", time, ((float)this._cooldown + 1f) * (float)this._size * Time.fixedDeltaTime));
			num = (int)this._size;
		}
		Bounds result = new Bounds(this.Positions[(int)this.Clock].Position, Vector3.zero);
		for (int i = 1; i < num; i++)
		{
			int num2 = (int)this.Clock - i;
			if (num2 < 0)
			{
				num2 += (int)(this._size - 1);
			}
			RelativePosition relativePosition = this.Positions[num2];
			using (new WaypointBacktracker(relativePosition.WaypointId, (double)((float)(i * (int)(this._cooldown + 1)) * Time.fixedDeltaTime)))
			{
				Vector3 position = relativePosition.Position;
				if (!ignoreTeleports && Vector3.Distance(result.ClosestPoint(position), position) > this._tpDis)
				{
					break;
				}
				result.Encapsulate(position);
			}
		}
		return result;
	}

	// Token: 0x04000553 RID: 1363
	public byte Clock;

	// Token: 0x04000554 RID: 1364
	public readonly RelativePosition[] Positions;

	// Token: 0x04000555 RID: 1365
	private readonly byte _size;

	// Token: 0x04000556 RID: 1366
	private readonly byte _cooldown;

	// Token: 0x04000557 RID: 1367
	private readonly float _tpDis;

	// Token: 0x04000558 RID: 1368
	private byte _cooldownTimer;
}
