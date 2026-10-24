using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020000C9 RID: 201
public class KeycodeField : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x1400000D RID: 13
	// (add) Token: 0x060003EA RID: 1002 RVA: 0x0000F7D4 File Offset: 0x0000D9D4
	// (remove) Token: 0x060003EB RID: 1003 RVA: 0x0000F80C File Offset: 0x0000DA0C
	public event Action<KeyCode> OnKeySet;

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x060003EC RID: 1004 RVA: 0x0000F841 File Offset: 0x0000DA41
	// (set) Token: 0x060003ED RID: 1005 RVA: 0x0000F849 File Offset: 0x0000DA49
	public KeyCode CurDisplayedKey { get; private set; }

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x060003EE RID: 1006 RVA: 0x0000F852 File Offset: 0x0000DA52
	public virtual string NoneSymbol
	{
		get
		{
			return "";
		}
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x060003EF RID: 1007 RVA: 0x0000F859 File Offset: 0x0000DA59
	public virtual string ReadSymbol
	{
		get
		{
			return "•  •  •";
		}
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x0000F860 File Offset: 0x0000DA60
	public void SetDisplayedKey(KeyCode key)
	{
		this.CurDisplayedKey = key;
		this._keyText.text = ((key == KeyCode.None) ? this.NoneSymbol : new ReadableKeyCode(key).NormalVersion);
		this._keyText.alignment = this._setAlignment;
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x0000F89C File Offset: 0x0000DA9C
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		KeycodeField._lastEntry = this;
		this._keyText.text = this.ReadSymbol;
		this._keyText.alignment = this._readAlignment;
		if (this._currentlyEditing)
		{
			return;
		}
		this._currentlyEditing = true;
		this._requestedChange = KeyCode.None;
		this._inputCooldown = 3;
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x0000F8F8 File Offset: 0x0000DAF8
	public void ExitEditMode()
	{
		this._requestedChange = KeyCode.None;
		if (!this._currentlyEditing)
		{
			return;
		}
		this.SetDisplayedKey(this.CurDisplayedKey);
		this._currentlyEditing = false;
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x00002DA5 File Offset: 0x00000FA5
	protected virtual bool ValidatePressedKey(KeyCode key)
	{
		return true;
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x0000F91D File Offset: 0x0000DB1D
	protected virtual void ApplyPressedKey(KeyCode key)
	{
		this.SetDisplayedKey(key);
		Action<KeyCode> onKeySet = this.OnKeySet;
		if (onKeySet != null)
		{
			onKeySet(key);
		}
		this._currentlyEditing = false;
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x0000F93F File Offset: 0x0000DB3F
	protected virtual void OnDisable()
	{
		this.ExitEditMode();
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x0000F948 File Offset: 0x0000DB48
	protected virtual void Update()
	{
		if (!this._currentlyEditing)
		{
			return;
		}
		if (KeycodeField._lastEntry != this || this._cancelKeys.Contains(this._requestedChange))
		{
			this.ExitEditMode();
			return;
		}
		if (this._requestedChange != KeyCode.None)
		{
			if (this.ValidatePressedKey(this._requestedChange))
			{
				this.ApplyPressedKey(this._requestedChange);
				return;
			}
			this._requestedChange = KeyCode.None;
		}
		if (this._inputCooldown > 0)
		{
			this._inputCooldown--;
			return;
		}
		foreach (KeyCode keyCode in EnumUtils<KeyCode>.Values)
		{
			if (Input.GetKeyUp(keyCode))
			{
				this._requestedChange = keyCode;
			}
		}
	}

	// Token: 0x04000376 RID: 886
	[SerializeField]
	private TMP_Text _keyText;

	// Token: 0x04000377 RID: 887
	[SerializeField]
	private KeyCode[] _cancelKeys = new KeyCode[]
	{
		KeyCode.Escape
	};

	// Token: 0x04000378 RID: 888
	[SerializeField]
	private TextAlignmentOptions _readAlignment = TextAlignmentOptions.Center;

	// Token: 0x04000379 RID: 889
	[SerializeField]
	private TextAlignmentOptions _setAlignment = TextAlignmentOptions.Center;

	// Token: 0x0400037A RID: 890
	private const int InputCooldownFrames = 3;

	// Token: 0x0400037B RID: 891
	private bool _currentlyEditing;

	// Token: 0x0400037C RID: 892
	private int _inputCooldown;

	// Token: 0x0400037D RID: 893
	private KeyCode _requestedChange;

	// Token: 0x0400037E RID: 894
	private static KeycodeField _lastEntry;

	// Token: 0x0400037F RID: 895
	private static KeyCode[] _allKeyCodes;
}
