using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cryptography;
using GameCore;
using Mirror;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Security;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class EncryptedChannelManager : NetworkBehaviour
{
	// Token: 0x17000145 RID: 325
	// (get) Token: 0x0600080C RID: 2060 RVA: 0x0002054D File Offset: 0x0001E74D
	// (set) Token: 0x0600080D RID: 2061 RVA: 0x00020555 File Offset: 0x0001E755
	internal AsymmetricCipherKeyPair EcdhKeys { get; private set; }

	// Token: 0x0600080E RID: 2062 RVA: 0x0002055E File Offset: 0x0001E75E
	internal void PrepareExchange()
	{
		if (this._exchange != null)
		{
			return;
		}
		this.EcdhKeys = ECDH.GenerateKeys(384);
		this._exchange = ECDH.Init(this.EcdhKeys);
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x0002058C File Offset: 0x0001E78C
	internal void ServerProcessExchange(string publicKey)
	{
		if (this.EncryptionKey != null)
		{
			return;
		}
		if (EncryptedChannelManager.CryptographyDebug)
		{
			ReferenceHub me;
			ServerConsole.AddLog("Received ECDH parameters from " + (ReferenceHub.TryGetHub(base.gameObject, out me) ? me.LoggedNameFromRefHub() : "(unknown)") + ".", ConsoleColor.Gray, false);
		}
		this.EncryptionKey = ECDH.DeriveKey(this._exchange, ECDSA.PublicKeyFromString(publicKey));
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x000205F2 File Offset: 0x0001E7F2
	public static void ReplaceServerHandler(EncryptedChannelManager.EncryptedChannel channel, EncryptedChannelManager.EncryptedMessageServerHandler handler)
	{
		EncryptedChannelManager.ServerChannelHandlers[channel] = handler;
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x00020600 File Offset: 0x0001E800
	public static void ReplaceClientHandler(EncryptedChannelManager.EncryptedChannel channel, EncryptedChannelManager.EncryptedMessageClientHandler clientHandler)
	{
		EncryptedChannelManager.ClientChannelHandlers[channel] = clientHandler;
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x00020610 File Offset: 0x0001E810
	private void Start()
	{
		if (!base.isLocalPlayer)
		{
			return;
		}
		if (NetworkServer.active)
		{
			if (string.IsNullOrEmpty(this.ServerRandom))
			{
				this.NetworkServerRandom = RandomGenerator.GetStringSecure(32);
				ServerConsole.AddLog("Generated random salt: " + this.ServerRandom, ConsoleColor.Gray, false);
			}
			Action<NetworkConnectionToClient, EncryptedChannelManager.EncryptedMessageOutside> handler;
			if ((handler = EncryptedChannelManager.<>O.<0>__ServerReceivePackedMessage) == null)
			{
				handler = (EncryptedChannelManager.<>O.<0>__ServerReceivePackedMessage = new Action<NetworkConnectionToClient, EncryptedChannelManager.EncryptedMessageOutside>(EncryptedChannelManager.ServerReceivePackedMessage));
			}
			NetworkServer.ReplaceHandler<EncryptedChannelManager.EncryptedMessageOutside>(handler, true);
		}
		Action<EncryptedChannelManager.EncryptedMessageOutside> handler2;
		if ((handler2 = EncryptedChannelManager.<>O.<1>__ClientReceivePackedMessage) == null)
		{
			handler2 = (EncryptedChannelManager.<>O.<1>__ClientReceivePackedMessage = new Action<EncryptedChannelManager.EncryptedMessageOutside>(EncryptedChannelManager.ClientReceivePackedMessage));
		}
		NetworkClient.ReplaceHandler<EncryptedChannelManager.EncryptedMessageOutside>(handler2, true);
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x000206A0 File Offset: 0x0001E8A0
	private void ReceivedSaltHook(string p, string n)
	{
		if (!NetworkServer.active && n != null)
		{
			GameCore.Console.AddLog("Received random salt from game server: " + n, Color.gray, false, ConsoleLogType.Log);
		}
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x000206C4 File Offset: 0x0001E8C4
	private static void ServerReceivePackedMessage(NetworkConnection conn, EncryptedChannelManager.EncryptedMessageOutside packed)
	{
		ReferenceHub referenceHub;
		if (!NetworkServer.active || !ReferenceHub.TryGetHub(conn, out referenceHub))
		{
			return;
		}
		EncryptedChannelManager.EncryptedMessage encryptedMessage;
		EncryptedChannelManager.SecurityLevel level;
		if (!referenceHub.encryptedChannelManager.TryUnpack(packed, out encryptedMessage, out level, NetworkServer.active && conn.identity.isLocalPlayer))
		{
			return;
		}
		EncryptedChannelManager.EncryptedMessageServerHandler encryptedMessageServerHandler;
		if (!EncryptedChannelManager.ServerChannelHandlers.TryGetValue(encryptedMessage.Channel, out encryptedMessageServerHandler))
		{
			GameCore.Console.AddLog(string.Format("No handler is registered for encrypted channel {0} (server).", encryptedMessage.Channel), Color.red, false, ConsoleLogType.Log);
			return;
		}
		try
		{
			encryptedMessageServerHandler(referenceHub, encryptedMessage, level);
		}
		catch (Exception ex)
		{
			GameCore.Console.AddLog(string.Format("Exception while handling encrypted message on channel {0} (server, running a handler). Exception: {1}", encryptedMessage.Channel, ex.Message), Color.red, false, ConsoleLogType.Log);
			GameCore.Console.AddLog(ex.StackTrace, Color.red, false, ConsoleLogType.Log);
		}
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x0002079C File Offset: 0x0001E99C
	private static void ClientReceivePackedMessage(EncryptedChannelManager.EncryptedMessageOutside packed)
	{
		ReferenceHub referenceHub;
		EncryptedChannelManager.EncryptedMessage encryptedMessage;
		EncryptedChannelManager.SecurityLevel level;
		if (!ReferenceHub.TryGetLocalHub(out referenceHub) || !referenceHub.encryptedChannelManager.TryUnpack(packed, out encryptedMessage, out level, NetworkServer.active))
		{
			return;
		}
		EncryptedChannelManager.EncryptedMessageClientHandler encryptedMessageClientHandler;
		if (!EncryptedChannelManager.ClientChannelHandlers.TryGetValue(encryptedMessage.Channel, out encryptedMessageClientHandler))
		{
			GameCore.Console.AddLog(string.Format("No handler is registered for encrypted channel {0} (client).", encryptedMessage.Channel), Color.red, false, ConsoleLogType.Log);
			return;
		}
		try
		{
			encryptedMessageClientHandler(encryptedMessage, level);
		}
		catch (Exception ex)
		{
			GameCore.Console.AddLog(string.Format("Exception while handling encrypted message on channel {0} (client, running a handler). Exception: {1}", encryptedMessage.Channel, ex.Message), Color.red, false, ConsoleLogType.Log);
			GameCore.Console.AddLog(ex.StackTrace, Color.red, false, ConsoleLogType.Log);
		}
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x0002085C File Offset: 0x0001EA5C
	public bool TrySendMessageToServer(string content, EncryptedChannelManager.EncryptedChannel channel)
	{
		if (this._txCounter == 4294967295U)
		{
			this._txCounter = 0U;
		}
		uint num = this._txCounter + 1U;
		this._txCounter = num;
		EncryptedChannelManager.EncryptedMessageOutside message;
		if (!this.TryPack(new EncryptedChannelManager.EncryptedMessage(channel, content, num), out message, NetworkServer.active))
		{
			return false;
		}
		NetworkClient.Send<EncryptedChannelManager.EncryptedMessageOutside>(message, 0);
		return true;
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x000208AC File Offset: 0x0001EAAC
	public bool TrySendMessageToServer(byte[] content, EncryptedChannelManager.EncryptedChannel channel)
	{
		if (this._txCounter == 4294967295U)
		{
			this._txCounter = 0U;
		}
		uint num = this._txCounter + 1U;
		this._txCounter = num;
		EncryptedChannelManager.EncryptedMessageOutside message;
		if (!this.TryPack(new EncryptedChannelManager.EncryptedMessage(channel, content, num), out message, NetworkServer.active))
		{
			return false;
		}
		NetworkClient.Send<EncryptedChannelManager.EncryptedMessageOutside>(message, 0);
		return true;
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x000208FA File Offset: 0x0001EAFA
	public static bool TrySendMessageToClient(ReferenceHub hub, string content, EncryptedChannelManager.EncryptedChannel channel)
	{
		return hub.encryptedChannelManager.TrySendMessageToClient(content, channel);
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x00020909 File Offset: 0x0001EB09
	public static bool TrySendMessageToClient(ReferenceHub hub, byte[] content, EncryptedChannelManager.EncryptedChannel channel)
	{
		return hub.encryptedChannelManager.TrySendMessageToClient(content, channel);
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x00020918 File Offset: 0x0001EB18
	[Server]
	public bool TrySendMessageToClient(string content, EncryptedChannelManager.EncryptedChannel channel)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Boolean EncryptedChannelManager::TrySendMessageToClient(System.String,EncryptedChannelManager/EncryptedChannel)' called when server was not active");
			return default(bool);
		}
		if (this._txCounter == 4294967295U)
		{
			this._txCounter = 0U;
		}
		uint num = this._txCounter + 1U;
		this._txCounter = num;
		EncryptedChannelManager.EncryptedMessageOutside message;
		if (!this.TryPack(new EncryptedChannelManager.EncryptedMessage(channel, content, num), out message, base.isLocalPlayer))
		{
			return false;
		}
		base.connectionToClient.Send<EncryptedChannelManager.EncryptedMessageOutside>(message, 0);
		return true;
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x00020990 File Offset: 0x0001EB90
	[Server]
	public bool TrySendMessageToClient(byte[] content, EncryptedChannelManager.EncryptedChannel channel)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Boolean EncryptedChannelManager::TrySendMessageToClient(System.Byte[],EncryptedChannelManager/EncryptedChannel)' called when server was not active");
			return default(bool);
		}
		if (this._txCounter == 4294967295U)
		{
			this._txCounter = 0U;
		}
		uint num = this._txCounter + 1U;
		this._txCounter = num;
		EncryptedChannelManager.EncryptedMessageOutside message;
		if (!this.TryPack(new EncryptedChannelManager.EncryptedMessage(channel, content, num), out message, base.isLocalPlayer))
		{
			return false;
		}
		base.connectionToClient.Send<EncryptedChannelManager.EncryptedMessageOutside>(message, 0);
		return true;
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x00020A08 File Offset: 0x0001EC08
	private bool TryPack(EncryptedChannelManager.EncryptedMessage msg, out EncryptedChannelManager.EncryptedMessageOutside packed, bool localClient = false)
	{
		bool flag = this.EncryptionKey != null && !localClient;
		if (!localClient && EncryptedChannelManager.RequiredSecurityLevels[msg.Channel] == EncryptedChannelManager.SecurityLevel.EncryptedAndAuthenticated && !flag)
		{
			GameCore.Console.AddLog(string.Format("Failed to send encrypted message to {0} channel. Encryption key is not available.", msg.Channel), Color.red, false, ConsoleLogType.Log);
			packed = default(EncryptedChannelManager.EncryptedMessageOutside);
			return false;
		}
		byte[] array = new byte[msg.GetLength];
		msg.Serialize(array);
		if (!flag)
		{
			packed = new EncryptedChannelManager.EncryptedMessageOutside(EncryptedChannelManager.SecurityLevel.Unsecured, array);
			return true;
		}
		packed = new EncryptedChannelManager.EncryptedMessageOutside(EncryptedChannelManager.SecurityLevel.EncryptedAndAuthenticated, AES.AesGcmEncrypt(array, this.EncryptionKey, EncryptedChannelManager.SecureRandom, 0, 0));
		return true;
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x00020AB0 File Offset: 0x0001ECB0
	private bool TryUnpack(EncryptedChannelManager.EncryptedMessageOutside packed, out EncryptedChannelManager.EncryptedMessage msg, out EncryptedChannelManager.SecurityLevel level, bool localClient)
	{
		level = packed.Level;
		bool flag = packed.Level == EncryptedChannelManager.SecurityLevel.EncryptedAndAuthenticated;
		if (flag && this.EncryptionKey == null)
		{
			GameCore.Console.AddLog("Failed to decrypt received message. Encryption key is not available.", Color.red, false, ConsoleLogType.Log);
			msg = default(EncryptedChannelManager.EncryptedMessage);
			return false;
		}
		if (flag)
		{
			try
			{
				msg = EncryptedChannelManager.EncryptedMessage.Deserialize(AES.AesGcmDecrypt(packed.Data, this.EncryptionKey, 0, 0));
			}
			catch (Exception ex)
			{
				GameCore.Console.AddLog("Failed to decrypt received message. Exception: " + ex.Message, Color.red, false, ConsoleLogType.Log);
				GameCore.Console.AddLog(ex.StackTrace, Color.red, false, ConsoleLogType.Log);
				msg = default(EncryptedChannelManager.EncryptedMessage);
				return false;
			}
			if (this._rxCounter == 4294967295U)
			{
				this._rxCounter = 0U;
			}
			if (msg.Counter <= this._rxCounter)
			{
				GameCore.Console.AddLog(string.Format("Received message with counter {0}, which is lower or equal to last received message counter {1}. Discarding message!", msg.Counter, this._rxCounter), Color.red, false, ConsoleLogType.Log);
				msg = default(EncryptedChannelManager.EncryptedMessage);
				return false;
			}
			this._rxCounter = msg.Counter;
			return true;
		}
		msg = EncryptedChannelManager.EncryptedMessage.Deserialize(packed.Data);
		if (!localClient && EncryptedChannelManager.RequiredSecurityLevels[msg.Channel] == EncryptedChannelManager.SecurityLevel.EncryptedAndAuthenticated)
		{
			GameCore.Console.AddLog(string.Format("Message on channel {0} was sent without encryption. Discarding message!", msg.Channel), Color.red, false, ConsoleLogType.Log);
			msg = default(EncryptedChannelManager.EncryptedMessage);
			return false;
		}
		return true;
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000821 RID: 2081 RVA: 0x00020C68 File Offset: 0x0001EE68
	// (set) Token: 0x06000822 RID: 2082 RVA: 0x00020C7B File Offset: 0x0001EE7B
	public string NetworkServerRandom
	{
		get
		{
			return this.ServerRandom;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<string>(value, ref this.ServerRandom, 1UL, new Action<string, string>(this.ReceivedSaltHook));
		}
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x00020CA0 File Offset: 0x0001EEA0
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteString(this.ServerRandom);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteString(this.ServerRandom);
		}
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x00020CF8 File Offset: 0x0001EEF8
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this.ServerRandom, new Action<string, string>(this.ReceivedSaltHook), reader.ReadString());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this.ServerRandom, new Action<string, string>(this.ReceivedSaltHook), reader.ReadString());
		}
	}

	// Token: 0x0400076A RID: 1898
	internal byte[] EncryptionKey;

	// Token: 0x0400076B RID: 1899
	internal static bool CryptographyDebug;

	// Token: 0x0400076C RID: 1900
	[SyncVar(hook = "ReceivedSaltHook")]
	[SerializeField]
	[HideInInspector]
	internal string ServerRandom;

	// Token: 0x0400076D RID: 1901
	private uint _txCounter;

	// Token: 0x0400076E RID: 1902
	private uint _rxCounter;

	// Token: 0x0400076F RID: 1903
	private ECDHBasicAgreement _exchange;

	// Token: 0x04000770 RID: 1904
	private static readonly SecureRandom SecureRandom = new SecureRandom();

	// Token: 0x04000771 RID: 1905
	private static readonly Dictionary<EncryptedChannelManager.EncryptedChannel, EncryptedChannelManager.EncryptedMessageClientHandler> ClientChannelHandlers = new Dictionary<EncryptedChannelManager.EncryptedChannel, EncryptedChannelManager.EncryptedMessageClientHandler>();

	// Token: 0x04000772 RID: 1906
	private static readonly Dictionary<EncryptedChannelManager.EncryptedChannel, EncryptedChannelManager.EncryptedMessageServerHandler> ServerChannelHandlers = new Dictionary<EncryptedChannelManager.EncryptedChannel, EncryptedChannelManager.EncryptedMessageServerHandler>();

	// Token: 0x04000773 RID: 1907
	private static readonly Dictionary<EncryptedChannelManager.EncryptedChannel, EncryptedChannelManager.SecurityLevel> RequiredSecurityLevels = new Dictionary<EncryptedChannelManager.EncryptedChannel, EncryptedChannelManager.SecurityLevel>
	{
		{
			EncryptedChannelManager.EncryptedChannel.RemoteAdmin,
			EncryptedChannelManager.SecurityLevel.EncryptedAndAuthenticated
		},
		{
			EncryptedChannelManager.EncryptedChannel.GameConsole,
			EncryptedChannelManager.SecurityLevel.Unsecured
		},
		{
			EncryptedChannelManager.EncryptedChannel.AdminChat,
			EncryptedChannelManager.SecurityLevel.EncryptedAndAuthenticated
		}
	};

	// Token: 0x02000194 RID: 404
	// (Invoke) Token: 0x06000826 RID: 2086
	public delegate void EncryptedMessageClientHandler(EncryptedChannelManager.EncryptedMessage message, EncryptedChannelManager.SecurityLevel level);

	// Token: 0x02000195 RID: 405
	// (Invoke) Token: 0x0600082A RID: 2090
	public delegate void EncryptedMessageServerHandler(ReferenceHub hub, EncryptedChannelManager.EncryptedMessage message, EncryptedChannelManager.SecurityLevel level);

	// Token: 0x02000196 RID: 406
	public enum EncryptedChannel : byte
	{
		// Token: 0x04000775 RID: 1909
		RemoteAdmin,
		// Token: 0x04000776 RID: 1910
		GameConsole,
		// Token: 0x04000777 RID: 1911
		AdminChat
	}

	// Token: 0x02000197 RID: 407
	public enum SecurityLevel : byte
	{
		// Token: 0x04000779 RID: 1913
		Unsecured,
		// Token: 0x0400077A RID: 1914
		EncryptedAndAuthenticated
	}

	// Token: 0x02000198 RID: 408
	public readonly struct EncryptedMessage
	{
		// Token: 0x0600082D RID: 2093 RVA: 0x00020D69 File Offset: 0x0001EF69
		public EncryptedMessage(EncryptedChannelManager.EncryptedChannel channel, string content, uint counter)
		{
			this.Channel = channel;
			this.Content = Utf8.GetBytes(content);
			this.Counter = counter;
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x00020D85 File Offset: 0x0001EF85
		public EncryptedMessage(EncryptedChannelManager.EncryptedChannel channel, byte[] content, uint counter)
		{
			this.Channel = channel;
			this.Content = content;
			this.Counter = counter;
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x0600082F RID: 2095 RVA: 0x00020D9C File Offset: 0x0001EF9C
		public int GetLength
		{
			get
			{
				return this.Content.Length + 5;
			}
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x00020DA8 File Offset: 0x0001EFA8
		public override string ToString()
		{
			return Utf8.GetString(this.Content);
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x00020DB5 File Offset: 0x0001EFB5
		internal void Serialize(byte[] array)
		{
			array[0] = (byte)this.Channel;
			BinaryPrimitives.WriteUInt32BigEndian(new Span<byte>(array, 1, 4), this.Counter);
			Array.Copy(this.Content, 0, array, 5, this.Content.Length);
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x00020DEC File Offset: 0x0001EFEC
		internal static EncryptedChannelManager.EncryptedMessage Deserialize(byte[] array)
		{
			byte[] array2 = new byte[array.Length - 5];
			Array.Copy(array, 5, array2, 0, array.Length - 5);
			return new EncryptedChannelManager.EncryptedMessage((EncryptedChannelManager.EncryptedChannel)array[0], array2, BinaryPrimitives.ReadUInt32BigEndian(new ReadOnlySpan<byte>(array, 1, 4)));
		}

		// Token: 0x0400077B RID: 1915
		public readonly EncryptedChannelManager.EncryptedChannel Channel;

		// Token: 0x0400077C RID: 1916
		public readonly byte[] Content;

		// Token: 0x0400077D RID: 1917
		internal readonly uint Counter;

		// Token: 0x0400077E RID: 1918
		private const int HeaderSize = 5;
	}

	// Token: 0x02000199 RID: 409
	internal readonly struct EncryptedMessageOutside : NetworkMessage
	{
		// Token: 0x06000833 RID: 2099 RVA: 0x00020E28 File Offset: 0x0001F028
		internal EncryptedMessageOutside(EncryptedChannelManager.SecurityLevel level, byte[] data)
		{
			this.Level = level;
			this.Data = data;
		}

		// Token: 0x0400077F RID: 1919
		internal readonly EncryptedChannelManager.SecurityLevel Level;

		// Token: 0x04000780 RID: 1920
		internal readonly byte[] Data;
	}

	// Token: 0x0200019A RID: 410
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x04000781 RID: 1921
		public static Action<NetworkConnectionToClient, EncryptedChannelManager.EncryptedMessageOutside> <0>__ServerReceivePackedMessage;

		// Token: 0x04000782 RID: 1922
		public static Action<EncryptedChannelManager.EncryptedMessageOutside> <1>__ClientReceivePackedMessage;
	}
}
