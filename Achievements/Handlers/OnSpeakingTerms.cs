using System;
using System.Runtime.CompilerServices;
using PlayerRoles;
using VoiceChat;
using VoiceChat.Networking;

namespace Achievements.Handlers
{
	// Token: 0x02000B08 RID: 2824
	public class OnSpeakingTerms : AchievementHandlerBase
	{
		// Token: 0x06003FE2 RID: 16354 RVA: 0x000D4280 File Offset: 0x000D2480
		internal override void OnInitialize()
		{
			VoiceTransceiver.VoiceMessageReceiving value;
			if ((value = OnSpeakingTerms.<>O.<0>__OnVoiceMessageReceiving) == null)
			{
				value = (OnSpeakingTerms.<>O.<0>__OnVoiceMessageReceiving = new VoiceTransceiver.VoiceMessageReceiving(OnSpeakingTerms.OnVoiceMessageReceiving));
			}
			VoiceTransceiver.OnVoiceMessageReceiving += value;
		}

		// Token: 0x06003FE3 RID: 16355 RVA: 0x000D42A2 File Offset: 0x000D24A2
		private static void OnVoiceMessageReceiving(VoiceMessage message, ReferenceHub hub)
		{
			if (message.Channel != VoiceChatChannel.Radio)
			{
				return;
			}
			if (message.Speaker.GetTeam() != Team.ChaosInsurgency || hub.GetTeam() != Team.FoundationForces)
			{
				return;
			}
			AchievementHandlerBase.ServerAchieve(message.Speaker.connectionToClient, AchievementName.OnSpeakingTerms);
		}

		// Token: 0x02000B09 RID: 2825
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E91 RID: 11921
			public static VoiceTransceiver.VoiceMessageReceiving <0>__OnVoiceMessageReceiving;
		}
	}
}
