using System;
using System.Collections.Generic;
using System.Linq;
using MapGeneration.Holidays;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace CustomRendering
{
	// Token: 0x020010FF RID: 4351
	public class FogController : MonoBehaviour
	{
		// Token: 0x1700141B RID: 5147
		// (get) Token: 0x060060C8 RID: 24776 RVA: 0x00142EB4 File Offset: 0x001410B4
		// (set) Token: 0x060060C9 RID: 24777 RVA: 0x00142EBB File Offset: 0x001410BB
		public static float FogFarPlaneDistance { get; private set; }

		// Token: 0x1700141C RID: 5148
		// (get) Token: 0x060060CA RID: 24778 RVA: 0x00142EC3 File Offset: 0x001410C3
		// (set) Token: 0x060060CB RID: 24779 RVA: 0x00142ECA File Offset: 0x001410CA
		public static FogController Singleton { get; private set; }

		// Token: 0x1700141D RID: 5149
		// (get) Token: 0x060060CC RID: 24780 RVA: 0x00142ED4 File Offset: 0x001410D4
		public static FogType DefaultFog
		{
			get
			{
				HolidayType activeHoliday = HolidayUtils.GetActiveHoliday(false);
				FogType result;
				if (!FogController.HolidayDefaultFog.TryGetValue(activeHoliday, out result))
				{
					return FogType.Inside;
				}
				return result;
			}
		}

		// Token: 0x1700141E RID: 5150
		// (get) Token: 0x060060CD RID: 24781 RVA: 0x00142EFA File Offset: 0x001410FA
		// (set) Token: 0x060060CE RID: 24782 RVA: 0x00142F02 File Offset: 0x00141102
		public CustomFog FogEffect { get; private set; }

		// Token: 0x1700141F RID: 5151
		// (get) Token: 0x060060CF RID: 24783 RVA: 0x00142F0B File Offset: 0x0014110B
		// (set) Token: 0x060060D0 RID: 24784 RVA: 0x00142F14 File Offset: 0x00141114
		public FogType? ForcedFog
		{
			get
			{
				return this._fogType;
			}
			set
			{
				bool enabled = value == null || value.Value != FogType.None;
				this.FogEffect.enabled = enabled;
				this._fogType = value;
			}
		}

		// Token: 0x060060D1 RID: 24785 RVA: 0x00142F4E File Offset: 0x0014114E
		public static void EnableFogType(FogType fogType, float seconds = 0f)
		{
			FogSetting fogSetting = FogController.Singleton.GetFogSetting(fogType);
			fogSetting.IsEnabled = true;
			fogSetting.BlendTime = seconds;
		}

		// Token: 0x060060D2 RID: 24786 RVA: 0x00142F68 File Offset: 0x00141168
		public static void DisableFogType(FogType fogType, float seconds = 0f)
		{
			FogSetting fogSetting = FogController.Singleton.GetFogSetting(fogType);
			fogSetting.IsEnabled = false;
			fogSetting.BlendTime = seconds;
		}

		// Token: 0x060060D3 RID: 24787 RVA: 0x00142F84 File Offset: 0x00141184
		public FogSetting GetFogSetting(FogType fogType)
		{
			foreach (FogSetting fogSetting in this._fogSettings)
			{
				if (fogSetting.FogType == fogType)
				{
					return fogSetting;
				}
			}
			throw new NotImplementedException(string.Format("The FogSetting component for '{0}' needs to be attached to the FogController GameObject.", fogType));
		}

		// Token: 0x060060D4 RID: 24788 RVA: 0x00142FCC File Offset: 0x001411CC
		private void Awake()
		{
			if (FogController.Singleton == null)
			{
				FogController.Singleton = this;
				this._fogSettings = (from x in base.GetComponents<FogSetting>()
				orderby x.Priority
				select x).ToArray<FogSetting>();
				for (int i = 0; i < this._fogSettings.Length; i++)
				{
					FogSetting fogSetting = this._fogSettings[i];
					if (fogSetting.FogType == FogController.DefaultFog)
					{
						fogSetting.enabled = true;
						this._defaultIndex = i;
					}
				}
				return;
			}
			UnityEngine.Object.Destroy(this);
		}

		// Token: 0x060060D5 RID: 24789 RVA: 0x00143061 File Offset: 0x00141261
		private void Start()
		{
			this.FogEffect = (CustomFog)base.GetComponent<CustomPassVolume>().customPasses[0];
		}

		// Token: 0x060060D6 RID: 24790 RVA: 0x0014307F File Offset: 0x0014127F
		private void OnDisable()
		{
			Shader.SetGlobalVector(FogController.HashGlobalFogDistance, Vector2.one * float.MaxValue);
		}

		// Token: 0x060060D7 RID: 24791 RVA: 0x001430A0 File Offset: 0x001412A0
		private void Update()
		{
			Color color = this._fogSettings[this._defaultIndex].Color;
			float num = this._fogSettings[this._defaultIndex].StartDistance;
			float num2 = this._fogSettings[this._defaultIndex].EndDistance;
			int i = 1;
			while (i < this._fogSettings.Length)
			{
				FogSetting fogSetting = this._fogSettings[i];
				if (this.ForcedFog != null)
				{
					if (this.ForcedFog.Value == fogSetting.FogType)
					{
						goto IL_8C;
					}
				}
				else if (fogSetting.IsEnabled || fogSetting.Weight != 0f)
				{
					goto IL_8C;
				}
				IL_D2:
				i++;
				continue;
				IL_8C:
				fogSetting.UpdateWeight();
				color = Color.Lerp(color, fogSetting.Color, fogSetting.Weight);
				num = Mathf.Lerp(num, fogSetting.StartDistance, fogSetting.Weight);
				num2 = Mathf.Lerp(num2, fogSetting.EndDistance, fogSetting.Weight);
				goto IL_D2;
			}
			this.FogEffect.FogColor = color;
			this.FogEffect.EndDistance = num2;
			this.FogEffect.StartDistance = num;
			Shader.SetGlobalVector(FogController.HashGlobalFogColor, color);
			Shader.SetGlobalVector(FogController.HashGlobalFogDistance, new Vector2(num, num2));
			FogController.FogFarPlaneDistance = num2;
		}

		// Token: 0x060060D9 RID: 24793 RVA: 0x001431E4 File Offset: 0x001413E4
		// Note: this type is marked as 'beforefieldinit'.
		static FogController()
		{
			Dictionary<HolidayType, FogType> dictionary = new Dictionary<HolidayType, FogType>();
			dictionary[HolidayType.None] = FogType.Inside;
			dictionary[HolidayType.Christmas] = FogType.ChristmasInside;
			dictionary[HolidayType.Halloween] = FogType.HalloweenInside;
			FogController.HolidayDefaultFog = dictionary;
		}

		// Token: 0x04004657 RID: 18007
		private static readonly int HashGlobalFogColor = Shader.PropertyToID("_GlobalFogColor");

		// Token: 0x04004658 RID: 18008
		private static readonly int HashGlobalFogDistance = Shader.PropertyToID("_GlobalFogDistance");

		// Token: 0x04004659 RID: 18009
		private static readonly Dictionary<HolidayType, FogType> HolidayDefaultFog;

		// Token: 0x0400465B RID: 18011
		private FogSetting[] _fogSettings;

		// Token: 0x0400465C RID: 18012
		private FogType? _fogType;

		// Token: 0x0400465D RID: 18013
		private int _defaultIndex;
	}
}
