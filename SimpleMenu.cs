using System;
using System.Collections.Generic;
using CentralAuth;
using MEC;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000EF RID: 239
public class SimpleMenu : MonoBehaviour
{
	// Token: 0x06000482 RID: 1154 RVA: 0x00011350 File Offset: 0x0000F550
	private void Awake()
	{
		if (this.isPreloader)
		{
			return;
		}
		CentralAuthManager.InitAuth();
		foreach (string a in StartupArgs.Args)
		{
			if (!(a == "-fastmenu"))
			{
				if (!(a == "-newmenu"))
				{
					if (!(a == "-nographics"))
					{
						if (a == "-forcemenu")
						{
							SimpleMenu._forceSettings = true;
						}
					}
					else
					{
						SimpleMenu._server = true;
					}
				}
				else
				{
					PlayerPrefsSl.Set("menumode", 1);
				}
			}
			else
			{
				PlayerPrefsSl.Set("fastmenu", true);
				PlayerPrefsSl.Set("menumode", 2);
			}
		}
		SimpleMenu.Refresh();
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x000113F1 File Offset: 0x0000F5F1
	private void Start()
	{
		Timing.RunCoroutine(this.StartLoad());
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x000113FF File Offset: 0x0000F5FF
	private IEnumerator<float> StartLoad()
	{
		yield return float.NegativeInfinity;
		if (!this.isPreloader)
		{
			yield break;
		}
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Loader", LoadSceneMode.Single);
		asyncOperation.allowSceneActivation = true;
		LauncherAssetScanProgressBar.Text = "LOADING";
		float sceneProgress = 0f;
		while (!asyncOperation.isDone)
		{
			sceneProgress = Math.Max(sceneProgress, asyncOperation.progress);
			LauncherAssetScanProgressBar.Progress = sceneProgress;
			yield return float.NegativeInfinity;
		}
		yield break;
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x0001140E File Offset: 0x0000F60E
	public void ChangeMode()
	{
		PlayerPrefsSl.Set("fastmenu", false);
		PlayerPrefsSl.Set("menumode", 1);
		SimpleMenu.Refresh();
		SimpleMenu.LoadCorrectScene();
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x00011430 File Offset: 0x0000F630
	public static void ChangeMode(int id)
	{
		PlayerPrefsSl.Set("menumode", id);
		SimpleMenu.Refresh();
		SimpleMenu.LoadCorrectScene();
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x00011448 File Offset: 0x0000F648
	private static void Refresh()
	{
		SimpleMenu._targetSceneName = (SimpleMenu._server ? "FastMenu" : SimpleMenu.MenuSceneNames[SimpleMenu._forceSettings ? Mathf.Clamp(PlayerPrefsSl.Get("menumode", 1), 0, 2) : 1]);
		UnityEngine.Object.FindObjectOfType<CustomNetworkManager>().offlineScene = SimpleMenu._targetSceneName;
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x00011499 File Offset: 0x0000F699
	public static void LoadCorrectScene()
	{
		SceneManager.LoadScene(SimpleMenu._targetSceneName);
	}

	// Token: 0x040003FA RID: 1018
	public bool isPreloader;

	// Token: 0x040003FB RID: 1019
	private static bool _server;

	// Token: 0x040003FC RID: 1020
	private static bool _forceSettings;

	// Token: 0x040003FD RID: 1021
	private static string _targetSceneName;

	// Token: 0x040003FE RID: 1022
	private const float minLoadingTime = 3f;

	// Token: 0x040003FF RID: 1023
	internal static readonly string[] MenuSceneNames = new string[]
	{
		"MainMenuRemastered",
		"NewMainMenu",
		"FastMenu"
	};
}
