using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

// Token: 0x020001DE RID: 478
public static class GpuDriver
{
	// Token: 0x06000A21 RID: 2593
	[DllImport("GpuDriver.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "get_gpu_driver")]
	private static extern IntPtr GetDriverVersion([MarshalAs(UnmanagedType.LPWStr)] string name);

	// Token: 0x06000A22 RID: 2594
	[DllImport("GpuDriver.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "free_driver")]
	private static extern void Free(IntPtr version);

	// Token: 0x14000026 RID: 38
	// (add) Token: 0x06000A23 RID: 2595 RVA: 0x00028094 File Offset: 0x00026294
	// (remove) Token: 0x06000A24 RID: 2596 RVA: 0x000280C8 File Offset: 0x000262C8
	public static event Action<string> DriverLoaded;

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06000A25 RID: 2597 RVA: 0x000280FC File Offset: 0x000262FC
	public static string DriverVersion
	{
		get
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return SystemInfo.graphicsDeviceVersion;
			}
			object dataLock = GpuDriver._dataLock;
			string result;
			lock (dataLock)
			{
				if (!string.IsNullOrWhiteSpace(GpuDriver._driverVersion))
				{
					result = GpuDriver._driverVersion;
				}
				else
				{
					try
					{
						string gpuName = SystemInfo.graphicsDeviceName;
						Thread thread = new Thread(delegate()
						{
							object dataLock2 = GpuDriver._dataLock;
							lock (dataLock2)
							{
								IntPtr driverVersion = GpuDriver.GetDriverVersion(gpuName);
								if (driverVersion == IntPtr.Zero)
								{
									Debug.LogWarning("GPU Driver version for " + gpuName + " not found!");
									driverVersion = GpuDriver.GetDriverVersion(null);
								}
								GpuDriver._driverVersion = (Marshal.PtrToStringUni(driverVersion) ?? "Loading failed");
								GpuDriver.Free(driverVersion);
								Debug.Log("GPU Driver version: " + GpuDriver._driverVersion);
								MainThreadDispatcher.Dispatch(delegate
								{
									Action<string> driverLoaded = GpuDriver.DriverLoaded;
									if (driverLoaded == null)
									{
										return;
									}
									driverLoaded(GpuDriver._driverVersion);
								}, MainThreadDispatcher.DispatchTime.Update);
							}
						});
						thread.IsBackground = true;
						thread.SetApartmentState(ApartmentState.MTA);
						thread.Start();
						result = "Loading...";
					}
					catch (Exception message)
					{
						Debug.Log(message);
						result = SystemInfo.graphicsDeviceVersion;
					}
				}
			}
			return result;
		}
	}

	// Token: 0x040008B1 RID: 2225
	private const string Library = "GpuDriver.dll";

	// Token: 0x040008B2 RID: 2226
	private const CallingConvention CallingConv = CallingConvention.StdCall;

	// Token: 0x040008B3 RID: 2227
	private static string _driverVersion;

	// Token: 0x040008B4 RID: 2228
	private static readonly object _dataLock = new object();
}
