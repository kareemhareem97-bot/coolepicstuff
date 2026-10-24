using System;
using DrawableLine;
using RemoteAdmin;
using UnityEngine;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A91 RID: 2705
	[CommandHandler(typeof(LineCommand))]
	public class LineTestCommand : ICommand
	{
		// Token: 0x17000DCA RID: 3530
		// (get) Token: 0x06003D0A RID: 15626 RVA: 0x000C8B93 File Offset: 0x000C6D93
		public string Command { get; } = "test";

		// Token: 0x17000DCB RID: 3531
		// (get) Token: 0x06003D0B RID: 15627 RVA: 0x000C8B9B File Offset: 0x000C6D9B
		public string[] Aliases { get; } = new string[]
		{
			"t"
		};

		// Token: 0x17000DCC RID: 3532
		// (get) Token: 0x06003D0C RID: 15628 RVA: 0x000C8BA3 File Offset: 0x000C6DA3
		public string Description { get; } = "Creates a new line directly in front of the player.";

		// Token: 0x06003D0D RID: 15629 RVA: 0x000C8BAC File Offset: 0x000C6DAC
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			ReferenceHub referenceHub;
			if (playerCommandSender != null)
			{
				referenceHub = playerCommandSender.ReferenceHub;
			}
			else if (!ReferenceHub.TryGetLocalHub(out referenceHub))
			{
				response = "You must be a player in order to execute this command.";
				return false;
			}
			int num = UnityEngine.Random.Range(3, 7);
			Transform playerCameraReference = referenceHub.PlayerCameraReference;
			Vector3[] array = new Vector3[num];
			Vector3 vector = playerCameraReference.position;
			for (int i = 0; i < num; i++)
			{
				if (i != 0)
				{
					if (i == 1)
					{
						vector += playerCameraReference.forward * LineTestCommand.GetRandomDistance(false);
					}
					else
					{
						Vector3 a = playerCameraReference.forward * LineTestCommand.GetRandomDistance(false);
						Vector3 b = playerCameraReference.up * LineTestCommand.GetRandomDistance(true);
						Vector3 b2 = playerCameraReference.right * LineTestCommand.GetRandomDistance(true);
						vector += a + b + b2;
					}
				}
				array[i] = vector;
			}
			DrawableLines.GenerateLine(new float?(6.5f), array);
			string format = "Drawn a line between {0} and {1}, with {2} segments.";
			object arg = array[0];
			Vector3[] array2 = array;
			response = string.Format(format, arg, array2[array2.Length - 1], num - 1);
			return true;
		}

		// Token: 0x06003D0E RID: 15630 RVA: 0x000C8CDC File Offset: 0x000C6EDC
		private static float GetRandomDistance(bool allowNegativeValues = true)
		{
			bool flag = allowNegativeValues && UnityEngine.Random.Range(0, 2) == 0;
			float num = UnityEngine.Random.Range(0.85f, 2f);
			if (!flag)
			{
				return num;
			}
			return -num;
		}

		// Token: 0x04002CAB RID: 11435
		private const float MinDistanceBetweenPoints = 0.85f;

		// Token: 0x04002CAC RID: 11436
		private const float MaxDistanceBetweenPoints = 2f;

		// Token: 0x04002CAD RID: 11437
		private const float DefaultDuration = 6.5f;

		// Token: 0x04002CAE RID: 11438
		private const int MaxSegments = 7;
	}
}
