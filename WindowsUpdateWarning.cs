using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using NorthwoodLib;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000FC RID: 252
public class WindowsUpdateWarning : MonoBehaviour
{
	// Token: 0x060004B7 RID: 1207 RVA: 0x00011D48 File Offset: 0x0000FF48
	private void Start()
	{
		this.warning.SetActive(WindowsUpdateWarning.UpdateRequired());
		this.menu.SetActive(SceneManager.GetActiveScene().buildIndex == 3 || !this.warning.activeSelf);
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x00011D94 File Offset: 0x0000FF94
	public static bool UpdateRequired()
	{
		bool result;
		try
		{
			result = (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows && NorthwoodLib.OperatingSystem.Version.Major < 10 && !File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.System) + Path.DirectorySeparatorChar.ToString() + "API-MS-WIN-CRT-MATH-L1-1-0.dll") && !WindowsUpdateWarning.CheckDll("API-MS-WIN-CRT-MATH-L1-1-0.dll"));
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			result = true;
		}
		return result;
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x00011E0C File Offset: 0x0001000C
	private static bool CheckDll(string name)
	{
		IntPtr intPtr = WindowsUpdateWarning.LoadLibrary(name);
		if (intPtr == IntPtr.Zero)
		{
			throw new Win32Exception();
		}
		if (!WindowsUpdateWarning.FreeLibrary(intPtr))
		{
			throw new Win32Exception();
		}
		return true;
	}

	// Token: 0x060004BA RID: 1210
	[DllImport("Kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryW", SetLastError = true)]
	private static extern IntPtr LoadLibrary(string name);

	// Token: 0x060004BB RID: 1211
	[DllImport("Kernel32.dll", SetLastError = true)]
	private static extern bool FreeLibrary(IntPtr library);

	// Token: 0x0400041D RID: 1053
	public GameObject warning;

	// Token: 0x0400041E RID: 1054
	public GameObject menu;
}
