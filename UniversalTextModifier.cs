using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000F7 RID: 247
public abstract class UniversalTextModifier : MonoBehaviour
{
	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x060004A9 RID: 1193 RVA: 0x00011B48 File Offset: 0x0000FD48
	// (set) Token: 0x060004AA RID: 1194 RVA: 0x00011B86 File Offset: 0x0000FD86
	public string DisplayText
	{
		get
		{
			if (this._usesTmp)
			{
				return this._tmpText.text;
			}
			if (this._usesUnity)
			{
				return this._unityText.text;
			}
			if (this._text != null)
			{
				return this._text;
			}
			return string.Empty;
		}
		set
		{
			if (this._usesTmp)
			{
				this._tmpText.text = value;
			}
			if (this._usesUnity)
			{
				this._unityText.text = value;
			}
			this._text = value;
		}
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x00011BB7 File Offset: 0x0000FDB7
	protected virtual void Awake()
	{
		this._usesTmp = base.TryGetComponent<TMP_Text>(out this._tmpText);
		this._usesUnity = base.TryGetComponent<Text>(out this._unityText);
	}

	// Token: 0x0400040F RID: 1039
	private TMP_Text _tmpText;

	// Token: 0x04000410 RID: 1040
	private Text _unityText;

	// Token: 0x04000411 RID: 1041
	private bool _usesTmp;

	// Token: 0x04000412 RID: 1042
	private bool _usesUnity;

	// Token: 0x04000413 RID: 1043
	private string _text;
}
