using System;
using Mirror;
using UnityEngine;
using Utils.Networking;

namespace DrawableLine
{
	// Token: 0x02000987 RID: 2439
	public static class DrawableLines
	{
		// Token: 0x17000AF1 RID: 2801
		// (get) Token: 0x06003754 RID: 14164 RVA: 0x000B3AB6 File Offset: 0x000B1CB6
		// (set) Token: 0x06003755 RID: 14165 RVA: 0x000B3ABD File Offset: 0x000B1CBD
		public static float ClientDefaultDuration { get; set; } = 0.25f;

		// Token: 0x17000AF2 RID: 2802
		// (get) Token: 0x06003756 RID: 14166 RVA: 0x000B3AC5 File Offset: 0x000B1CC5
		// (set) Token: 0x06003757 RID: 14167 RVA: 0x000B3ACC File Offset: 0x000B1CCC
		public static Color ClientDefaultColor { get; set; } = Color.green;

		// Token: 0x06003758 RID: 14168 RVA: 0x000B3AD4 File Offset: 0x000B1CD4
		public static void ClientGenerateLine(params Vector3[] positions)
		{
			DrawableLines.ClientGenerateLine(new float?(DrawableLines.ClientDefaultDuration), new Color?(DrawableLines.ClientDefaultColor), positions);
		}

		// Token: 0x06003759 RID: 14169 RVA: 0x000B3AF0 File Offset: 0x000B1CF0
		public static void ClientGenerateLine(float? duration, params Vector3[] positions)
		{
			DrawableLines.ClientGenerateLine(duration, new Color?(DrawableLines.ClientDefaultColor), positions);
		}

		// Token: 0x0600375A RID: 14170 RVA: 0x000B3B03 File Offset: 0x000B1D03
		public static void ClientGenerateLine(Color? color, params Vector3[] positions)
		{
			DrawableLines.ClientGenerateLine(new float?(DrawableLines.ClientDefaultDuration), color, positions);
		}

		// Token: 0x0600375B RID: 14171 RVA: 0x000B3B18 File Offset: 0x000B1D18
		public static void ClientGenerateLine(float? duration, Color? color, params Vector3[] positions)
		{
			if (color == null)
			{
				color = new Color?(DrawableLines.ClientDefaultColor);
			}
			if (DrawableLines.DurationOverride != null)
			{
				duration = new float?(DrawableLines.DurationOverride.Value);
			}
			else if (duration == null)
			{
				duration = new float?(DrawableLines.ClientDefaultDuration);
			}
			for (int i = 0; i < positions.Length - 1; i++)
			{
				Vector3 start = positions[i];
				Vector3 end = positions[i + 1];
				DrawableLinesManager.DrawLine(start, end, color.Value, duration.Value, 0.05f);
			}
		}

		// Token: 0x17000AF3 RID: 2803
		// (get) Token: 0x0600375C RID: 14172 RVA: 0x000B3BAE File Offset: 0x000B1DAE
		// (set) Token: 0x0600375D RID: 14173 RVA: 0x000B3BB5 File Offset: 0x000B1DB5
		public static bool IsDebugModeEnabled { get; set; } = false;

		// Token: 0x17000AF4 RID: 2804
		// (get) Token: 0x0600375E RID: 14174 RVA: 0x000B3BBD File Offset: 0x000B1DBD
		// (set) Token: 0x0600375F RID: 14175 RVA: 0x000B3BC4 File Offset: 0x000B1DC4
		public static float? DurationOverride { get; set; }

		// Token: 0x17000AF5 RID: 2805
		// (get) Token: 0x06003760 RID: 14176 RVA: 0x000B3BCC File Offset: 0x000B1DCC
		public static float DefaultDuration
		{
			get
			{
				if (!NetworkServer.active)
				{
					return DrawableLines.ClientDefaultDuration;
				}
				return DrawableLines.ServerDefaultDuration;
			}
		}

		// Token: 0x17000AF6 RID: 2806
		// (get) Token: 0x06003761 RID: 14177 RVA: 0x000B3BE0 File Offset: 0x000B1DE0
		public static Color DefaultColor
		{
			get
			{
				if (!NetworkServer.active)
				{
					return DrawableLines.ClientDefaultColor;
				}
				return DrawableLines.ServerDefaultColor;
			}
		}

		// Token: 0x06003762 RID: 14178 RVA: 0x000B3BF4 File Offset: 0x000B1DF4
		public static void GenerateLine(params Vector3[] positions)
		{
			DrawableLines.GenerateLine(new float?(DrawableLines.DefaultDuration), new Color?(DrawableLines.DefaultColor), positions);
		}

		// Token: 0x06003763 RID: 14179 RVA: 0x000B3C10 File Offset: 0x000B1E10
		public static void GenerateLine(Color? color, params Vector3[] positions)
		{
			DrawableLines.GenerateLine(new float?(DrawableLines.DefaultDuration), color, positions);
		}

		// Token: 0x06003764 RID: 14180 RVA: 0x000B3C23 File Offset: 0x000B1E23
		public static void GenerateLine(float? duration, params Vector3[] positions)
		{
			DrawableLines.GenerateLine(duration, new Color?(DrawableLines.DefaultColor), positions);
		}

		// Token: 0x06003765 RID: 14181 RVA: 0x000B3C36 File Offset: 0x000B1E36
		public static void GenerateLine(float? duration, Color? color, params Vector3[] positions)
		{
			if (!DrawableLines.IsDebugModeEnabled)
			{
				return;
			}
			if (NetworkServer.active)
			{
				DrawableLines.ServerGenerateLine(duration, color, positions);
				return;
			}
			DrawableLines.ClientGenerateLine(duration, color, positions);
		}

		// Token: 0x06003766 RID: 14182 RVA: 0x000B3C58 File Offset: 0x000B1E58
		public static void GenerateSphere(Vector3 origin, float radius, int segments = 8)
		{
			DrawableLines.GenerateSphere(origin, radius, new float?(DrawableLines.DefaultDuration), segments);
		}

		// Token: 0x06003767 RID: 14183 RVA: 0x000B3C6C File Offset: 0x000B1E6C
		public static void GenerateSphere(Vector3 origin, float radius, float? duration, int segments = 8)
		{
			DrawableLines.GenerateSphere(origin, radius, duration, new Color?(DrawableLines.DefaultColor), segments);
		}

		// Token: 0x06003768 RID: 14184 RVA: 0x000B3C81 File Offset: 0x000B1E81
		public static void GenerateSphere(Vector3 origin, float radius, Color? color, int segments = 8)
		{
			DrawableLines.GenerateSphere(origin, radius, new float?(DrawableLines.DefaultDuration), color, segments);
		}

		// Token: 0x06003769 RID: 14185 RVA: 0x000B3C96 File Offset: 0x000B1E96
		public static void GenerateSphere(Vector3 origin, float radius, float? duration, Color? color, int segments = 8)
		{
			DrawableLines.GenerateLine(duration, color, DrawableLines.GetCircle(origin, radius, true, segments));
			DrawableLines.GenerateLine(duration, color, DrawableLines.GetCircle(origin, radius, false, segments));
		}

		// Token: 0x0600376A RID: 14186 RVA: 0x000B3CBA File Offset: 0x000B1EBA
		public static void GenerateBounds(Bounds bounds, float? duration)
		{
			DrawableLines.GenerateBounds(bounds, duration, new Color?(DrawableLines.DefaultColor));
		}

		// Token: 0x0600376B RID: 14187 RVA: 0x000B3CCD File Offset: 0x000B1ECD
		public static void GenerateBounds(Bounds bounds, Color? color)
		{
			DrawableLines.GenerateBounds(bounds, new float?(DrawableLines.DefaultDuration), color);
		}

		// Token: 0x0600376C RID: 14188 RVA: 0x000B3CE0 File Offset: 0x000B1EE0
		public static void GenerateBounds(Bounds bounds)
		{
			DrawableLines.GenerateBounds(bounds, new float?(DrawableLines.DefaultDuration), new Color?(DrawableLines.DefaultColor));
		}

		// Token: 0x0600376D RID: 14189 RVA: 0x000B3CFC File Offset: 0x000B1EFC
		public static void GenerateBounds(Bounds bounds, float? duration, Color? color)
		{
			Vector3 center = bounds.center;
			Vector3 extents = bounds.extents;
			Vector3[] array = new Vector3[5];
			Vector3[] array2 = new Vector3[5];
			array[0] = center + new Vector3(-extents.x, -extents.y, -extents.z);
			array[1] = center + new Vector3(extents.x, -extents.y, -extents.z);
			array[2] = center + new Vector3(extents.x, -extents.y, extents.z);
			array[3] = center + new Vector3(-extents.x, -extents.y, extents.z);
			array[4] = array[0];
			array2[0] = center + new Vector3(-extents.x, extents.y, -extents.z);
			array2[1] = center + new Vector3(extents.x, extents.y, -extents.z);
			array2[2] = center + new Vector3(extents.x, extents.y, extents.z);
			array2[3] = center + new Vector3(-extents.x, extents.y, extents.z);
			array2[4] = array2[0];
			DrawableLines.GenerateLine(duration, color, array);
			DrawableLines.GenerateLine(duration, color, array2);
			for (int i = 0; i < 4; i++)
			{
				DrawableLines.GenerateLine(duration, color, new Vector3[]
				{
					array[i],
					array2[i]
				});
			}
		}

		// Token: 0x0600376E RID: 14190 RVA: 0x000B3EBC File Offset: 0x000B20BC
		public static Vector3[] GetCircle(Vector3 origin, float radius, bool horizontalAxis = true, int segments = 8)
		{
			if (segments <= 0)
			{
				segments = 8;
			}
			if (segments % 2 != 0)
			{
				segments++;
			}
			Vector3[] array = new Vector3[segments + 1];
			float num = 6.2831855f / (float)segments;
			for (int i = 0; i < segments; i++)
			{
				float f = (float)i * num;
				float num2 = Mathf.Cos(f) * radius;
				float num3 = Mathf.Sin(f) * radius;
				array[i] = (horizontalAxis ? new Vector3(origin.x + num2, origin.y + num3, origin.z) : new Vector3(origin.x + num2, origin.y, origin.z + num3));
			}
			Vector3[] array2 = array;
			array2[array2.Length - 1] = array[0];
			return array;
		}

		// Token: 0x17000AF7 RID: 2807
		// (get) Token: 0x0600376F RID: 14191 RVA: 0x000B3F65 File Offset: 0x000B2165
		// (set) Token: 0x06003770 RID: 14192 RVA: 0x000B3F6C File Offset: 0x000B216C
		public static float ServerDefaultDuration { get; set; } = 0.3f;

		// Token: 0x17000AF8 RID: 2808
		// (get) Token: 0x06003771 RID: 14193 RVA: 0x000B3F74 File Offset: 0x000B2174
		// (set) Token: 0x06003772 RID: 14194 RVA: 0x000B3F7B File Offset: 0x000B217B
		public static Color ServerDefaultColor { get; set; } = Color.red;

		// Token: 0x06003773 RID: 14195 RVA: 0x000B3F83 File Offset: 0x000B2183
		public static void ServerGenerateLine(params Vector3[] positions)
		{
			DrawableLines.ServerGenerateLine(new float?(DrawableLines.ServerDefaultDuration), new Color?(DrawableLines.ServerDefaultColor), positions);
		}

		// Token: 0x06003774 RID: 14196 RVA: 0x000B3F9F File Offset: 0x000B219F
		public static void ServerGenerateLine(float? duration, params Vector3[] positions)
		{
			DrawableLines.ServerGenerateLine(duration, new Color?(DrawableLines.ServerDefaultColor), positions);
		}

		// Token: 0x06003775 RID: 14197 RVA: 0x000B3FB2 File Offset: 0x000B21B2
		public static void ServerGenerateLine(Color? color, params Vector3[] positions)
		{
			DrawableLines.ServerGenerateLine(new float?(DrawableLines.ServerDefaultDuration), color, positions);
		}

		// Token: 0x06003776 RID: 14198 RVA: 0x000B3FC5 File Offset: 0x000B21C5
		public static void ServerGenerateLine(float? duration, Color? color, params Vector3[] positions)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (!DrawableLines.IsDebugModeEnabled)
			{
				return;
			}
			DrawableLines.ServerGenerateMessage(duration, color, positions).SendToAuthenticated(0);
		}

		// Token: 0x06003777 RID: 14199 RVA: 0x000B3FE5 File Offset: 0x000B21E5
		public static void ServerGenerateLine(ReferenceHub hub, params Vector3[] positions)
		{
			DrawableLines.ServerGenerateLine(hub, new float?(DrawableLines.ServerDefaultDuration), new Color?(DrawableLines.ServerDefaultColor), positions);
		}

		// Token: 0x06003778 RID: 14200 RVA: 0x000B4002 File Offset: 0x000B2202
		public static void ServerGenerateLine(ReferenceHub hub, float? duration, params Vector3[] positions)
		{
			DrawableLines.ServerGenerateLine(hub, duration, new Color?(DrawableLines.ServerDefaultColor), positions);
		}

		// Token: 0x06003779 RID: 14201 RVA: 0x000B4016 File Offset: 0x000B2216
		public static void ServerGenerateLine(ReferenceHub hub, Color? color, params Vector3[] positions)
		{
			DrawableLines.ServerGenerateLine(hub, new float?(DrawableLines.ServerDefaultDuration), color, positions);
		}

		// Token: 0x0600377A RID: 14202 RVA: 0x000B402A File Offset: 0x000B222A
		public static void ServerGenerateLine(ReferenceHub hub, float? duration, Color? color, params Vector3[] positions)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (!DrawableLines.IsDebugModeEnabled)
			{
				return;
			}
			hub.connectionToServer.Send<DrawableLineMessage>(DrawableLines.ServerGenerateMessage(duration, color, positions), 0);
		}

		// Token: 0x0600377B RID: 14203 RVA: 0x000B4050 File Offset: 0x000B2250
		private static DrawableLineMessage ServerGenerateMessage(float? duration, Color? color, Vector3[] positions)
		{
			if (DrawableLines.DurationOverride != null)
			{
				duration = new float?(DrawableLines.DurationOverride.Value);
			}
			else if (duration == null)
			{
				duration = new float?(DrawableLines.ServerDefaultDuration);
			}
			if (color == null)
			{
				color = new Color?(DrawableLines.ServerDefaultColor);
			}
			return new DrawableLineMessage(duration, color, positions);
		}

		// Token: 0x04002998 RID: 10648
		private const int RecommendedSegments = 8;
	}
}
