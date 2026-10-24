using System;
using Mirror;
using Subtitles;

namespace Cassie
{
	// Token: 0x02000AC0 RID: 2752
	public readonly struct CassieTtsPayload : NetworkMessage
	{
		// Token: 0x06003E17 RID: 15895 RVA: 0x000CD2D7 File Offset: 0x000CB4D7
		public void PlaySubtitleMessage()
		{
			if (SubtitleController.Singleton == null)
			{
				return;
			}
			SubtitleController.Singleton.ClearSubtitles(CassieAnnouncementType.Normal);
			SubtitleController.Singleton.SetupSubtitle(this._subtitleMessage.SubtitleParts);
		}

		// Token: 0x06003E18 RID: 15896 RVA: 0x000CD308 File Offset: 0x000CB508
		public void PlayOtherSubtitle(double duration, string valueOverride = null)
		{
			if (SubtitleController.Singleton == null)
			{
				return;
			}
			SubtitleController.Singleton.ClearSubtitles(CassieAnnouncementType.Normal);
			string text = valueOverride ?? this._customSubtitle;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			CassieTtsPayload.CustomSubtitleSettings.Delay = (this.PlayBackground ? 2.5f : 0f);
			SubtitleController.Singleton.AddSubtitle(text, (float)duration, CassieTtsPayload.CustomSubtitleSettings);
		}

		// Token: 0x06003E19 RID: 15897 RVA: 0x000CD374 File Offset: 0x000CB574
		public void WriteSelf(NetworkWriter writer)
		{
			writer.WriteString(this.Content);
			int num = (int)this.SubtitleSource;
			if (this.PlayBackground)
			{
				num += 127;
			}
			writer.WriteByte((byte)num);
			CassieTtsPayload.SubtitleMode subtitleSource = this.SubtitleSource;
			if (subtitleSource == CassieTtsPayload.SubtitleMode.Custom)
			{
				writer.WriteString(this._customSubtitle);
				return;
			}
			if (subtitleSource != CassieTtsPayload.SubtitleMode.FromTranslation)
			{
				return;
			}
			writer.WriteSubtitleMessage(this._subtitleMessage);
		}

		// Token: 0x06003E1A RID: 15898 RVA: 0x000CD3D2 File Offset: 0x000CB5D2
		public CassieTtsPayload(string customAnnouncement, bool autoGenerateSubtitles = true, bool playBackground = true)
		{
			this._subtitleMessage = default(SubtitleMessage);
			this._customSubtitle = null;
			this.Content = customAnnouncement;
			this.PlayBackground = playBackground;
			this.SubtitleSource = (autoGenerateSubtitles ? CassieTtsPayload.SubtitleMode.Automatic : CassieTtsPayload.SubtitleMode.None);
		}

		// Token: 0x06003E1B RID: 15899 RVA: 0x000CD402 File Offset: 0x000CB602
		public CassieTtsPayload(string customAnnouncement, string customSubtitles, bool playBackground = true)
		{
			this._subtitleMessage = default(SubtitleMessage);
			this.Content = customAnnouncement;
			this.PlayBackground = playBackground;
			this.SubtitleSource = CassieTtsPayload.SubtitleMode.Custom;
			this._customSubtitle = customSubtitles;
		}

		// Token: 0x06003E1C RID: 15900 RVA: 0x000CD42C File Offset: 0x000CB62C
		public CassieTtsPayload(string announcement, bool playBackground, params SubtitlePart[] subtitleParts)
		{
			this._customSubtitle = null;
			this.Content = announcement;
			this.PlayBackground = playBackground;
			this.SubtitleSource = CassieTtsPayload.SubtitleMode.FromTranslation;
			this._subtitleMessage = new SubtitleMessage(subtitleParts);
		}

		// Token: 0x06003E1D RID: 15901 RVA: 0x000CD456 File Offset: 0x000CB656
		public CassieTtsPayload(string announcement, params SubtitlePart[] subtitleParts)
		{
			this = new CassieTtsPayload(announcement, true, subtitleParts);
		}

		// Token: 0x06003E1E RID: 15902 RVA: 0x000CD464 File Offset: 0x000CB664
		public CassieTtsPayload(NetworkReader reader)
		{
			this._subtitleMessage = default(SubtitleMessage);
			this._customSubtitle = null;
			this.Content = reader.ReadString();
			int num = (int)reader.ReadByte();
			if (num < 127)
			{
				this.PlayBackground = false;
				this.SubtitleSource = (CassieTtsPayload.SubtitleMode)num;
			}
			else
			{
				this.PlayBackground = true;
				this.SubtitleSource = (CassieTtsPayload.SubtitleMode)(num - 127);
			}
			CassieTtsPayload.SubtitleMode subtitleSource = this.SubtitleSource;
			if (subtitleSource == CassieTtsPayload.SubtitleMode.Custom)
			{
				this._customSubtitle = reader.ReadString();
				return;
			}
			if (subtitleSource != CassieTtsPayload.SubtitleMode.FromTranslation)
			{
				return;
			}
			this._subtitleMessage = reader.ReadSubtitleMessage();
		}

		// Token: 0x06003E1F RID: 15903 RVA: 0x000CD4E6 File Offset: 0x000CB6E6
		public CassieTtsPayload(CassieTtsPayload original, string announcement)
		{
			this.Content = announcement;
			this.SubtitleSource = original.SubtitleSource;
			this.PlayBackground = original.PlayBackground;
			this._subtitleMessage = original._subtitleMessage;
			this._customSubtitle = original._customSubtitle;
		}

		// Token: 0x04002D76 RID: 11638
		public readonly string Content;

		// Token: 0x04002D77 RID: 11639
		public readonly CassieTtsPayload.SubtitleMode SubtitleSource;

		// Token: 0x04002D78 RID: 11640
		public readonly bool PlayBackground;

		// Token: 0x04002D79 RID: 11641
		private readonly SubtitleMessage _subtitleMessage;

		// Token: 0x04002D7A RID: 11642
		private readonly string _customSubtitle;

		// Token: 0x04002D7B RID: 11643
		private static readonly Subtitle CustomSubtitleSettings = new Subtitle
		{
			SubtitleCategory = CassieAnnouncementType.Normal
		};

		// Token: 0x02000AC1 RID: 2753
		public enum SubtitleMode
		{
			// Token: 0x04002D7D RID: 11645
			None,
			// Token: 0x04002D7E RID: 11646
			Automatic,
			// Token: 0x04002D7F RID: 11647
			Custom,
			// Token: 0x04002D80 RID: 11648
			FromTranslation
		}
	}
}
