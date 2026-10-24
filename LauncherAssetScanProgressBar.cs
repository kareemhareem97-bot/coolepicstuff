using System;
using GameCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000CD RID: 205
public class LauncherAssetScanProgressBar : MonoBehaviour
{
	// Token: 0x060003FE RID: 1022 RVA: 0x0000FA5F File Offset: 0x0000DC5F
	private void Awake()
	{
		this._version.text = this.GetVersionName();
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x0000FA74 File Offset: 0x0000DC74
	private void Update()
	{
		this._throbber.fillAmount = LauncherAssetScanProgressBar.Progress;
		this._text.text = string.Format("{0}\n{1}%", LauncherAssetScanProgressBar.Text, Mathf.RoundToInt(LauncherAssetScanProgressBar.Progress * 100f));
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x0000FAC0 File Offset: 0x0000DCC0
	private string GetVersionName()
	{
		string str;
		switch (GameCore.Version.BuildType)
		{
		case GameCore.Version.VersionType.PublicRC:
		case GameCore.Version.VersionType.PrivateRC:
		case GameCore.Version.VersionType.PrivateRCStreamingForbidden:
			str = "RELEASE CANDIDATE";
			break;
		case GameCore.Version.VersionType.PublicBeta:
			str = "PUBLIC BETA";
			break;
		case GameCore.Version.VersionType.PrivateBeta:
		case GameCore.Version.VersionType.PrivateBetaStreamingForbidden:
			str = "CLOSED BETA";
			break;
		case GameCore.Version.VersionType.Development:
			return "INTERNAL BUILD " + GameCore.Version.VersionString;
		default:
			str = GameCore.Version.BuildType.ToString().ToUpperInvariant();
			break;
		}
		string text = GameCore.Version.Major.ToString() + "." + GameCore.Version.Minor.ToString();
		if (GameCore.Version.Revision > 0)
		{
			text = text + "." + GameCore.Version.Revision.ToString();
		}
		return str + " " + text;
	}

	// Token: 0x04000399 RID: 921
	[SerializeField]
	private Image _throbber;

	// Token: 0x0400039A RID: 922
	[SerializeField]
	private SimpleMenu _menu;

	// Token: 0x0400039B RID: 923
	[SerializeField]
	private TMP_Text _text;

	// Token: 0x0400039C RID: 924
	[SerializeField]
	private TMP_Text _version;

	// Token: 0x0400039D RID: 925
	public static float Progress;

	// Token: 0x0400039E RID: 926
	public static string Text;
}
