using System;
using System.Text;
using System.Text.RegularExpressions;
using NorthwoodLib.Pools;
using RemoteAdmin.Interfaces;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x02000908 RID: 2312
	public static class StatusEffectExtensions
	{
		// Token: 0x17000A5F RID: 2655
		// (get) Token: 0x060034CE RID: 13518 RVA: 0x000AD536 File Offset: 0x000AB736
		public static Color NegativeEffectColor { get; } = new Color(1f, 0.4784314f, 0.4784314f);

		// Token: 0x17000A60 RID: 2656
		// (get) Token: 0x060034CF RID: 13519 RVA: 0x000AD53D File Offset: 0x000AB73D
		public static Color PositiveEffectColor { get; } = new Color(0.7058824f, 1f, 0.7058824f);

		// Token: 0x17000A61 RID: 2657
		// (get) Token: 0x060034D0 RID: 13520 RVA: 0x000AD544 File Offset: 0x000AB744
		public static Color MixedEffectColor { get; } = new Color(1f, 0.7058824f, 1f);

		// Token: 0x17000A62 RID: 2658
		// (get) Token: 0x060034D1 RID: 13521 RVA: 0x000AD54B File Offset: 0x000AB74B
		public static Color TechnicalEffectColor { get; } = new Color(0.9607843f, 0.8705882f, 0.3647059f);

		// Token: 0x060034D2 RID: 13522 RVA: 0x000AD554 File Offset: 0x000AB754
		public static Color GetEffectColor(this StatusEffectBase effect)
		{
			Color result;
			switch (effect.Classification)
			{
			case StatusEffectBase.EffectClassification.Technical:
				result = StatusEffectExtensions.TechnicalEffectColor;
				break;
			case StatusEffectBase.EffectClassification.Negative:
				result = StatusEffectExtensions.NegativeEffectColor;
				break;
			case StatusEffectBase.EffectClassification.Mixed:
				result = StatusEffectExtensions.MixedEffectColor;
				break;
			case StatusEffectBase.EffectClassification.Positive:
				result = StatusEffectExtensions.PositiveEffectColor;
				break;
			default:
				result = StatusEffectExtensions.NegativeEffectColor;
				break;
			}
			return result;
		}

		// Token: 0x060034D3 RID: 13523 RVA: 0x000AD5A8 File Offset: 0x000AB7A8
		public static string GetEffectName(this StatusEffectBase effect)
		{
			ICustomDisplayName customDisplayName = effect as ICustomDisplayName;
			if (customDisplayName != null && !string.IsNullOrEmpty(customDisplayName.DisplayName))
			{
				return customDisplayName.DisplayName;
			}
			return Regex.Replace(effect.name, "(\\B[A-Z])", " $1");
		}

		// Token: 0x060034D4 RID: 13524 RVA: 0x000AD5E8 File Offset: 0x000AB7E8
		public static string GetDisplayNameComplete(this StatusEffectBase effect)
		{
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			stringBuilder.AppendFormat("<color={0}>", effect.GetEffectColor().ToHex());
			stringBuilder.Append(effect.GetEffectName());
			stringBuilder.AppendFormat("[{0}]</color>", effect.Intensity);
			IDisplayInfoEffect displayInfoEffect = effect as IDisplayInfoEffect;
			if (displayInfoEffect != null)
			{
				displayInfoEffect.AppendDisplayInfo(stringBuilder);
			}
			return StringBuilderPool.Shared.ToStringReturn(stringBuilder);
		}
	}
}
