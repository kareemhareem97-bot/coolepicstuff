using System;
using System.Runtime.CompilerServices;
using LabApi.Events.Handlers;

// Token: 0x02000223 RID: 547
public static class Shutdown
{
	// Token: 0x1400002B RID: 43
	// (add) Token: 0x06000C3A RID: 3130 RVA: 0x000300D4 File Offset: 0x0002E2D4
	// (remove) Token: 0x06000C3B RID: 3131 RVA: 0x00030108 File Offset: 0x0002E308
	public static event Action OnQuit;

	// Token: 0x06000C3C RID: 3132 RVA: 0x0003013B File Offset: 0x0002E33B
	public static void Quit(bool quit = true, bool suppressShutdownBroadcast = false)
	{
		if (Shutdown._quitting)
		{
			return;
		}
		Shutdown._quitting = true;
		Action onQuit = Shutdown.OnQuit;
		if (onQuit != null)
		{
			onQuit();
		}
		ServerEvents.OnShutdown();
		IdleMode.PauseIdleMode = true;
		ServerShutdown.Shutdown(suppressShutdownBroadcast);
		CentralServer.Abort = true;
		Shutdown.InternalShutdown(quit);
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x00030178 File Offset: 0x0002E378
	private static void InternalShutdown(bool quit)
	{
		Shutdown.<InternalShutdown>d__5 <InternalShutdown>d__;
		<InternalShutdown>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<InternalShutdown>d__.quit = quit;
		<InternalShutdown>d__.<>1__state = -1;
		<InternalShutdown>d__.<>t__builder.Start<Shutdown.<InternalShutdown>d__5>(ref <InternalShutdown>d__);
	}

	// Token: 0x04000A17 RID: 2583
	private static bool _quitting;
}
