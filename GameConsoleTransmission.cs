using System;
using System.Runtime.CompilerServices;
using GameCore;
using Mirror;
using Security;
using UnityEngine;

// Token: 0x02000127 RID: 295
public class GameConsoleTransmission : NetworkBehaviour
{
	// Token: 0x06000588 RID: 1416 RVA: 0x00014BAC File Offset: 0x00012DAC
	private void Start()
	{
		this._hub = ReferenceHub.GetHub(this);
		this._cmdRateLimit = this._hub.playerRateLimitHandler.RateLimits[1];
		if (base.isLocalPlayer)
		{
			EncryptedChannelManager.EncryptedChannel channel = EncryptedChannelManager.EncryptedChannel.GameConsole;
			EncryptedChannelManager.EncryptedMessageClientHandler clientHandler;
			if ((clientHandler = GameConsoleTransmission.<>O.<0>__ClientHandleMessage) == null)
			{
				clientHandler = (GameConsoleTransmission.<>O.<0>__ClientHandleMessage = new EncryptedChannelManager.EncryptedMessageClientHandler(GameConsoleTransmission.ClientHandleMessage));
			}
			EncryptedChannelManager.ReplaceClientHandler(channel, clientHandler);
			if (NetworkServer.active)
			{
				EncryptedChannelManager.EncryptedChannel channel2 = EncryptedChannelManager.EncryptedChannel.GameConsole;
				EncryptedChannelManager.EncryptedMessageServerHandler handler;
				if ((handler = GameConsoleTransmission.<>O.<1>__ServerHandleCommand) == null)
				{
					handler = (GameConsoleTransmission.<>O.<1>__ServerHandleCommand = new EncryptedChannelManager.EncryptedMessageServerHandler(GameConsoleTransmission.ServerHandleCommand));
				}
				EncryptedChannelManager.ReplaceServerHandler(channel2, handler);
			}
		}
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x00014C2E File Offset: 0x00012E2E
	[Server]
	public void SendToClient(string text, string color)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GameConsoleTransmission::SendToClient(System.String,System.String)' called when server was not active");
			return;
		}
		ReferenceHub hub = this._hub;
		if (hub == null)
		{
			return;
		}
		hub.encryptedChannelManager.TrySendMessageToClient(color + "#" + text, EncryptedChannelManager.EncryptedChannel.GameConsole);
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x00014C68 File Offset: 0x00012E68
	private static void ClientHandleMessage(EncryptedChannelManager.EncryptedMessage content, EncryptedChannelManager.SecurityLevel securityLevel)
	{
		string text = content.ToString();
		int num = text.IndexOf("#", StringComparison.Ordinal);
		string name = text.Remove(num);
		text = text.Remove(0, num + 1);
		GameCore.Console.AddLog(((securityLevel == EncryptedChannelManager.SecurityLevel.EncryptedAndAuthenticated) ? "[FROM SERVER] " : "[UNENCRYPTED FROM SERVER] ") + text, GameConsoleTransmission.ProcessColor(name), false, ConsoleLogType.Log);
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x00014CC6 File Offset: 0x00012EC6
	[Client]
	internal void SendToServer(string command)
	{
		if (!NetworkClient.active)
		{
			Debug.LogWarning("[Client] function 'System.Void GameConsoleTransmission::SendToServer(System.String)' called when client was not active");
			return;
		}
		this._hub.encryptedChannelManager.TrySendMessageToServer(command, EncryptedChannelManager.EncryptedChannel.GameConsole);
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x00014CF0 File Offset: 0x00012EF0
	private static void ServerHandleCommand(ReferenceHub hub, EncryptedChannelManager.EncryptedMessage content, EncryptedChannelManager.SecurityLevel securityLevel)
	{
		if (hub.gameConsoleTransmission._cmdRateLimit.CanExecute(true))
		{
			hub.queryProcessor.ProcessGameConsoleQuery(content.ToString());
		}
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x00014D20 File Offset: 0x00012F20
	private static Color ProcessColor(string name)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
		if (num <= 1231115066U)
		{
			if (num <= 96429129U)
			{
				if (num != 18738364U)
				{
					if (num == 96429129U)
					{
						if (name == "yellow")
						{
							return Color.yellow;
						}
					}
				}
				else if (name == "green")
				{
					return Color.green;
				}
			}
			else if (num != 1089765596U)
			{
				if (num == 1231115066U)
				{
					if (name == "cyan")
					{
						return Color.cyan;
					}
				}
			}
			else if (name == "red")
			{
				return Color.red;
			}
		}
		else if (num <= 1676028392U)
		{
			if (num != 1452231588U)
			{
				if (num == 1676028392U)
				{
					if (name == "magenta")
					{
						return Color.magenta;
					}
				}
			}
			else if (name == "black")
			{
				return Color.black;
			}
		}
		else if (num != 2197550541U)
		{
			if (num == 3724674918U)
			{
				if (name == "white")
				{
					return Color.white;
				}
			}
		}
		else if (name == "blue")
		{
			return Color.blue;
		}
		return Color.grey;
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x040004DE RID: 1246
	private ReferenceHub _hub;

	// Token: 0x040004DF RID: 1247
	private RateLimit _cmdRateLimit;

	// Token: 0x02000128 RID: 296
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x040004E0 RID: 1248
		public static EncryptedChannelManager.EncryptedMessageClientHandler <0>__ClientHandleMessage;

		// Token: 0x040004E1 RID: 1249
		public static EncryptedChannelManager.EncryptedMessageServerHandler <1>__ServerHandleCommand;
	}
}
