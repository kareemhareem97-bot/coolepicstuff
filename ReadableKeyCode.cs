using System;
using System.Collections.Generic;
using Hints;
using UnityEngine;

// Token: 0x020000EA RID: 234
public readonly struct ReadableKeyCode
{
	// Token: 0x0600046F RID: 1135 RVA: 0x00010C98 File Offset: 0x0000EE98
	public ReadableKeyCode(KeyCode keycode)
	{
		ReadableKeyCode readableKeyCode;
		if (ReadableKeyCode.AlreadyDefinedKeycodes.TryGetValue(keycode, out readableKeyCode))
		{
			this.NormalVersion = readableKeyCode.NormalVersion;
			this.ShortVersion = readableKeyCode.ShortVersion;
			this._normalVersionLength = readableKeyCode._normalVersionLength;
			return;
		}
		ReadableKeyCode.GetReadableForm(keycode, out this.NormalVersion, out this.ShortVersion, true);
		this._normalVersionLength = this.NormalVersion.Length;
		ReadableKeyCode.AlreadyDefinedKeycodes[keycode] = this;
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x00010D0E File Offset: 0x0000EF0E
	[RuntimeInitializeOnLoadMethod]
	private static void SetupForTranslationsReload()
	{
		TranslationReader.OnTranslationsRefreshed += delegate()
		{
			ReadableKeyCode.AlreadyDefinedKeycodes.Clear();
		};
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x00010D34 File Offset: 0x0000EF34
	public ReadableKeyCode(ActionName action)
	{
		this = new ReadableKeyCode(NewInput.GetKey(action, KeyCode.None));
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x00010D43 File Offset: 0x0000EF43
	public string GetBestVersion(int maxCharacters)
	{
		if (this._normalVersionLength <= maxCharacters)
		{
			return this.NormalVersion;
		}
		return this.ShortVersion;
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x00010D5B File Offset: 0x0000EF5B
	public override string ToString()
	{
		return this.NormalVersion;
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x00010D64 File Offset: 0x0000EF64
	private static void GetReadableForm(KeyCode keycode, out string normalVer, out string shortVer, bool translate = true)
	{
		if (keycode <= KeyCode.BackQuote)
		{
			if (keycode <= KeyCode.Escape)
			{
				if (keycode == KeyCode.Backspace)
				{
					normalVer = "Backspace";
					shortVer = "BKSP";
					goto IL_325;
				}
				if (keycode == KeyCode.Escape)
				{
					normalVer = "Escape";
					shortVer = "Esc";
					goto IL_325;
				}
			}
			else
			{
				if (keycode - KeyCode.Alpha0 <= 9)
				{
					normalVer = (keycode - KeyCode.Alpha0).ToString();
					shortVer = normalVer;
					goto IL_325;
				}
				if (keycode == KeyCode.BackQuote)
				{
					normalVer = "~";
					shortVer = "~";
					goto IL_325;
				}
			}
		}
		else if (keycode <= KeyCode.PageDown)
		{
			if (keycode == KeyCode.Delete)
			{
				normalVer = "Delete";
				shortVer = "Del";
				goto IL_325;
			}
			switch (keycode)
			{
			case KeyCode.Keypad0:
			case KeyCode.Keypad1:
			case KeyCode.Keypad2:
			case KeyCode.Keypad3:
			case KeyCode.Keypad4:
			case KeyCode.Keypad5:
			case KeyCode.Keypad6:
			case KeyCode.Keypad7:
			case KeyCode.Keypad8:
			case KeyCode.Keypad9:
			{
				int num = keycode - KeyCode.Keypad0;
				normalVer = "Numpad " + num.ToString();
				shortVer = "Num " + num.ToString();
				goto IL_325;
			}
			case KeyCode.UpArrow:
				normalVer = "Up Arrow";
				shortVer = "▲";
				goto IL_325;
			case KeyCode.DownArrow:
				normalVer = "Down Arrow";
				shortVer = "▼";
				goto IL_325;
			case KeyCode.RightArrow:
				normalVer = "Right Arrow";
				shortVer = "►";
				goto IL_325;
			case KeyCode.LeftArrow:
				normalVer = "Left Arrow";
				shortVer = "◄";
				goto IL_325;
			case KeyCode.Insert:
				normalVer = "Insert";
				shortVer = "Ins";
				goto IL_325;
			case KeyCode.PageUp:
				normalVer = "Page Up";
				shortVer = "PgUp";
				goto IL_325;
			case KeyCode.PageDown:
				normalVer = "Page Down";
				shortVer = "PgDn";
				goto IL_325;
			}
		}
		else
		{
			switch (keycode)
			{
			case KeyCode.RightShift:
				normalVer = "Right Shift";
				shortVer = "Right ⇧";
				goto IL_325;
			case KeyCode.LeftShift:
				normalVer = "Left Shift";
				shortVer = "Left ⇧";
				goto IL_325;
			case KeyCode.RightControl:
				normalVer = "Right Control";
				shortVer = "R Ctrl";
				goto IL_325;
			case KeyCode.LeftControl:
				normalVer = "Left Control";
				shortVer = "L Ctrl";
				goto IL_325;
			case KeyCode.RightAlt:
				normalVer = "Right Alt";
				shortVer = "R Alt";
				goto IL_325;
			case KeyCode.LeftAlt:
				normalVer = "Left Alt";
				shortVer = "L Alt";
				goto IL_325;
			default:
				switch (keycode)
				{
				case KeyCode.Mouse0:
					normalVer = "Left Mouse Button";
					shortVer = "LMB";
					goto IL_325;
				case KeyCode.Mouse1:
					normalVer = "Right Mouse Button";
					shortVer = "RMB";
					goto IL_325;
				case KeyCode.Mouse2:
					normalVer = "Middle Mouse Button";
					shortVer = "MMB";
					goto IL_325;
				case KeyCode.Mouse3:
				case KeyCode.Mouse4:
				case KeyCode.Mouse5:
				case KeyCode.Mouse6:
				{
					normalVer = keycode.ToString().Insert(5, " Button ");
					string str = "MB";
					string text = normalVer;
					shortVer = str + text[text.Length - 1].ToString();
					goto IL_325;
				}
				}
				break;
			}
		}
		normalVer = keycode.ToString();
		shortVer = normalVer;
		IL_325:
		if (translate)
		{
			normalVer = ReadableKeyCode.TranslateKey(keycode, normalVer);
		}
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x000110A4 File Offset: 0x0000F2A4
	private static string TranslateKey(KeyCode keycode, string normalVer)
	{
		KeyCodeTranslations index;
		if (!keycode.TryGetTranslationKey(out index))
		{
			return normalVer;
		}
		string text = TranslationReader.Get("KeyCodes", (int)index, normalVer);
		if (keycode - KeyCode.Mouse3 <= 3)
		{
			return string.Format(text, normalVer[normalVer.Length - 1].ToString());
		}
		return text;
	}

	// Token: 0x040003E9 RID: 1001
	public const string TranslationFile = "KeyCodes";

	// Token: 0x040003EA RID: 1002
	private static readonly Dictionary<KeyCode, ReadableKeyCode> AlreadyDefinedKeycodes = new Dictionary<KeyCode, ReadableKeyCode>();

	// Token: 0x040003EB RID: 1003
	private readonly int _normalVersionLength;

	// Token: 0x040003EC RID: 1004
	public readonly string NormalVersion;

	// Token: 0x040003ED RID: 1005
	public readonly string ShortVersion;
}
