using System;
using UnityEngine;

namespace Hints
{
	// Token: 0x02000801 RID: 2049
	public static class KeyCodeExtension
	{
		// Token: 0x06002F79 RID: 12153 RVA: 0x000A0100 File Offset: 0x0009E300
		public static bool TryGetTranslationKey(this KeyCode code, out KeyCodeTranslations translation)
		{
			KeyCodeTranslations keyCodeTranslations;
			if (code <= KeyCode.Space)
			{
				if (code == KeyCode.Tab)
				{
					keyCodeTranslations = KeyCodeTranslations.Tab;
					goto IL_EB;
				}
				if (code == KeyCode.Return)
				{
					keyCodeTranslations = KeyCodeTranslations.Enter;
					goto IL_EB;
				}
				if (code == KeyCode.Space)
				{
					keyCodeTranslations = KeyCodeTranslations.Space;
					goto IL_EB;
				}
			}
			else
			{
				switch (code)
				{
				case KeyCode.UpArrow:
					keyCodeTranslations = KeyCodeTranslations.ArrowUp;
					goto IL_EB;
				case KeyCode.DownArrow:
					keyCodeTranslations = KeyCodeTranslations.ArrowDown;
					goto IL_EB;
				case KeyCode.RightArrow:
					keyCodeTranslations = KeyCodeTranslations.ArrowRight;
					goto IL_EB;
				case KeyCode.LeftArrow:
					keyCodeTranslations = KeyCodeTranslations.ArrowLeft;
					goto IL_EB;
				default:
					switch (code)
					{
					case KeyCode.RightShift:
						keyCodeTranslations = KeyCodeTranslations.RightShift;
						goto IL_EB;
					case KeyCode.LeftShift:
						keyCodeTranslations = KeyCodeTranslations.LeftShift;
						goto IL_EB;
					case KeyCode.RightControl:
						keyCodeTranslations = KeyCodeTranslations.RightControl;
						goto IL_EB;
					case KeyCode.LeftControl:
						keyCodeTranslations = KeyCodeTranslations.LeftControl;
						goto IL_EB;
					case KeyCode.RightAlt:
						keyCodeTranslations = KeyCodeTranslations.RightAlt;
						goto IL_EB;
					case KeyCode.LeftAlt:
						keyCodeTranslations = KeyCodeTranslations.LeftAlt;
						goto IL_EB;
					default:
						switch (code)
						{
						case KeyCode.Mouse0:
							keyCodeTranslations = KeyCodeTranslations.MousePrimary;
							goto IL_EB;
						case KeyCode.Mouse1:
							keyCodeTranslations = KeyCodeTranslations.MouseSecondary;
							goto IL_EB;
						case KeyCode.Mouse2:
							keyCodeTranslations = KeyCodeTranslations.MouseMiddle;
							goto IL_EB;
						case KeyCode.Mouse3:
							keyCodeTranslations = KeyCodeTranslations.MouseN;
							goto IL_EB;
						case KeyCode.Mouse4:
							keyCodeTranslations = KeyCodeTranslations.MouseN;
							goto IL_EB;
						case KeyCode.Mouse5:
							keyCodeTranslations = KeyCodeTranslations.MouseN;
							goto IL_EB;
						case KeyCode.Mouse6:
							keyCodeTranslations = KeyCodeTranslations.MouseN;
							goto IL_EB;
						}
						break;
					}
					break;
				}
			}
			keyCodeTranslations = KeyCodeTranslations.KeyNotAssigned;
			IL_EB:
			translation = keyCodeTranslations;
			return translation != KeyCodeTranslations.KeyNotAssigned;
		}
	}
}
