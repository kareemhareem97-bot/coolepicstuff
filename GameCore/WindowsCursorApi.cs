using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GameCore
{
	// Token: 0x02000864 RID: 2148
	public static class WindowsCursorApi
	{
		// Token: 0x06003120 RID: 12576 RVA: 0x000A4EC4 File Offset: 0x000A30C4
		public static void SetCursor(SystemCursorID id)
		{
			WindowsCursorApi.CachedCursor cachedCursor;
			if (!WindowsCursorApi.CachedCursors.TryGetValue(id, out cachedCursor))
			{
				Vector2 hotspot;
				Texture2D texture = WindowsCursorApi.LoadCursorAsTexture(id, out hotspot);
				cachedCursor = new WindowsCursorApi.CachedCursor(texture, hotspot);
				WindowsCursorApi.CachedCursors.Add(id, cachedCursor);
			}
			if (cachedCursor.Texture != null)
			{
				Cursor.SetCursor(cachedCursor.Texture, cachedCursor.Hotspot, CursorMode.Auto);
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
		}

		// Token: 0x06003121 RID: 12577 RVA: 0x000A4F2C File Offset: 0x000A312C
		private unsafe static Texture2D LoadCursorAsTexture(SystemCursorID id, out Vector2 hotspot)
		{
			IntPtr intPtr = WindowsCursorApi.LoadCursor(IntPtr.Zero, (int)id);
			hotspot = Vector2.zero;
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			intPtr = WindowsCursorApi.CopyIcon(intPtr);
			WindowsCursorApi.ICONINFO iconinfo;
			if (!WindowsCursorApi.GetIconInfo(intPtr, out iconinfo))
			{
				return null;
			}
			int systemMetrics = WindowsCursorApi.GetSystemMetrics(13);
			int systemMetrics2 = WindowsCursorApi.GetSystemMetrics(14);
			hotspot = new Vector2(iconinfo.XHotspot, iconinfo.YHotspot);
			IntPtr hdc = WindowsCursorApi.CreateCompatibleDC(IntPtr.Zero);
			WindowsCursorApi.BITMAPINFO bitmapinfo = default(WindowsCursorApi.BITMAPINFO);
			bitmapinfo.BmiHeader.BiSize = (uint)Marshal.SizeOf<WindowsCursorApi.BITMAPINFOHEADER>();
			bitmapinfo.BmiHeader.BiWidth = systemMetrics;
			bitmapinfo.BmiHeader.BiHeight = -systemMetrics2;
			bitmapinfo.BmiHeader.BiPlanes = 1;
			bitmapinfo.BmiHeader.BitBitCount = 32;
			bitmapinfo.BmiHeader.BiCompression = 0U;
			IntPtr intPtr3;
			IntPtr intPtr2 = WindowsCursorApi.CreateDIBSection(hdc, ref bitmapinfo, 0U, out intPtr3, IntPtr.Zero, 0U);
			IntPtr h = WindowsCursorApi.SelectObject(hdc, intPtr2);
			WindowsCursorApi.DrawIconEx(hdc, 0, 0, intPtr, systemMetrics, systemMetrics2, 0, IntPtr.Zero, 3U);
			Texture2D texture2D = new Texture2D(systemMetrics, systemMetrics2, TextureFormat.RGBA32, false);
			uint* ptr = (uint*)intPtr3.ToPointer();
			Color32[] array = new Color32[systemMetrics * systemMetrics2];
			for (int i = 0; i < systemMetrics2; i++)
			{
				int num = i * systemMetrics;
				for (int j = 0; j < systemMetrics; j++)
				{
					uint num2 = ptr[num + j];
					byte b = (byte)(num2 & 255U);
					byte g = (byte)(num2 >> 8 & 255U);
					byte r = (byte)(num2 >> 16 & 255U);
					byte a = (byte)(num2 >> 24 & 255U);
					int num3 = systemMetrics2 - 1 - i;
					array[num3 * systemMetrics + j] = new Color32(r, g, b, a);
				}
			}
			texture2D.SetPixels32(array);
			texture2D.Apply();
			WindowsCursorApi.SelectObject(hdc, h);
			WindowsCursorApi.DeleteObject(intPtr2);
			if (iconinfo.HbmColor != IntPtr.Zero)
			{
				WindowsCursorApi.DeleteObject(iconinfo.HbmColor);
			}
			if (iconinfo.HbmMask != IntPtr.Zero)
			{
				WindowsCursorApi.DeleteObject(iconinfo.HbmMask);
			}
			return texture2D;
		}

		// Token: 0x06003122 RID: 12578
		[DllImport("user32.dll")]
		private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

		// Token: 0x06003123 RID: 12579
		[DllImport("user32.dll")]
		private static extern bool GetIconInfo(IntPtr hCursor, out WindowsCursorApi.ICONINFO iconInfo);

		// Token: 0x06003124 RID: 12580
		[DllImport("user32.dll")]
		private static extern IntPtr CopyIcon(IntPtr hIcon);

		// Token: 0x06003125 RID: 12581
		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		// Token: 0x06003126 RID: 12582
		[DllImport("gdi32.dll")]
		private static extern IntPtr CreateDIBSection(IntPtr hdc, ref WindowsCursorApi.BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

		// Token: 0x06003127 RID: 12583
		[DllImport("gdi32.dll")]
		private static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

		// Token: 0x06003128 RID: 12584
		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		// Token: 0x06003129 RID: 12585
		[DllImport("user32.dll")]
		private static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyHeight, int istepIfAniCur, IntPtr hbrFlickerFreeDraw, uint diFlags);

		// Token: 0x0600312A RID: 12586
		[DllImport("user32.dll")]
		private static extern int GetSystemMetrics(int nIndex);

		// Token: 0x0400262A RID: 9770
		private const int SM_CXCURSOR = 13;

		// Token: 0x0400262B RID: 9771
		private const int SM_CYCURSOR = 14;

		// Token: 0x0400262C RID: 9772
		private const uint DI_NORMAL = 3U;

		// Token: 0x0400262D RID: 9773
		private const uint DIB_RGB_COLORS = 0U;

		// Token: 0x0400262E RID: 9774
		private static readonly Dictionary<SystemCursorID, WindowsCursorApi.CachedCursor> CachedCursors = new Dictionary<SystemCursorID, WindowsCursorApi.CachedCursor>();

		// Token: 0x02000865 RID: 2149
		private struct ICONINFO
		{
			// Token: 0x0400262F RID: 9775
			public bool FIcon;

			// Token: 0x04002630 RID: 9776
			public uint XHotspot;

			// Token: 0x04002631 RID: 9777
			public uint YHotspot;

			// Token: 0x04002632 RID: 9778
			public IntPtr HbmMask;

			// Token: 0x04002633 RID: 9779
			public IntPtr HbmColor;
		}

		// Token: 0x02000866 RID: 2150
		private struct BITMAPINFOHEADER
		{
			// Token: 0x04002634 RID: 9780
			public uint BiSize;

			// Token: 0x04002635 RID: 9781
			public int BiWidth;

			// Token: 0x04002636 RID: 9782
			public int BiHeight;

			// Token: 0x04002637 RID: 9783
			public ushort BiPlanes;

			// Token: 0x04002638 RID: 9784
			public ushort BitBitCount;

			// Token: 0x04002639 RID: 9785
			public uint BiCompression;

			// Token: 0x0400263A RID: 9786
			public uint BiSizeImage;

			// Token: 0x0400263B RID: 9787
			public int BiXPelsPerMeter;

			// Token: 0x0400263C RID: 9788
			public int BiYPelsPerMeter;

			// Token: 0x0400263D RID: 9789
			public uint BiClrUsed;

			// Token: 0x0400263E RID: 9790
			public uint BiClrImportant;
		}

		// Token: 0x02000867 RID: 2151
		private struct BITMAPINFO
		{
			// Token: 0x0400263F RID: 9791
			public WindowsCursorApi.BITMAPINFOHEADER BmiHeader;

			// Token: 0x04002640 RID: 9792
			public uint BmiColors;
		}

		// Token: 0x02000868 RID: 2152
		private readonly struct CachedCursor
		{
			// Token: 0x0600312C RID: 12588 RVA: 0x000A5146 File Offset: 0x000A3346
			public CachedCursor(Texture2D texture, Vector2 hotspot)
			{
				this.Texture = texture;
				this.Hotspot = hotspot;
			}

			// Token: 0x04002641 RID: 9793
			public readonly Texture2D Texture;

			// Token: 0x04002642 RID: 9794
			public readonly Vector2 Hotspot;
		}
	}
}
