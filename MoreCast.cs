using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200021D RID: 541
public class MoreCast : MonoBehaviour
{
	// Token: 0x06000B76 RID: 2934 RVA: 0x0002D820 File Offset: 0x0002BA20
	public static bool BeamCast(Vector3 start, Vector3 end, Vector3 beamRadius, float beamStep, out List<RaycastHit> hitInfo, int layerMask, bool any)
	{
		hitInfo = new List<RaycastHit>();
		Vector3 vector = start;
		Vector3 vector2 = end;
		vector -= beamRadius;
		vector2 -= beamRadius;
		for (float num = -beamRadius.x; num < beamRadius.x; num += beamStep)
		{
			vector.y = start.y;
			vector2.y = end.y;
			vector.x += beamStep;
			vector2.x += beamStep;
			for (float num2 = -beamRadius.y; num2 < beamRadius.x; num2 += beamStep)
			{
				vector.z = start.z;
				vector2.z = end.z;
				vector.y += beamStep;
				vector2.y += beamStep;
				for (float num3 = -beamRadius.x; num3 < beamRadius.x; num3 += beamStep)
				{
					vector.z += beamStep;
					vector2.z += beamStep;
					RaycastHit item;
					bool flag = Physics.Linecast(vector, vector2, out item, layerMask);
					hitInfo.Add(item);
					if (any && flag)
					{
						return true;
					}
					if (!flag && !any)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x0002D948 File Offset: 0x0002BB48
	public static bool BeamCast(Vector3 start, Vector3 end, Vector3 beamRadius, float beamStep, int layerMask, bool any)
	{
		List<RaycastHit> list;
		return MoreCast.BeamCast(start, end, beamRadius, beamStep, out list, layerMask, any);
	}
}
