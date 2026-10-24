using System;
using System.IO;
using System.Linq;
using Cryptography;
using GameCore;
using Org.BouncyCastle.Crypto;
using UnityEngine;

// Token: 0x0200017D RID: 381
public static class CentralServerKeyCache
{
	// Token: 0x060007A6 RID: 1958 RVA: 0x0001D25C File Offset: 0x0001B45C
	public static string ReadCache()
	{
		string result;
		try
		{
			string appFolder = FileManager.GetAppFolder(true, false, "");
			string path = appFolder + "internal/KeyCache";
			string path2 = appFolder + "internal/KeySignatureCache";
			if (!File.Exists(path))
			{
				ServerConsole.AddLog("Central server public key not found in cache.", ConsoleColor.Gray, false);
				result = null;
			}
			else if (!File.Exists(path2))
			{
				ServerConsole.AddLog("Central server public key signature not found in cache.", ConsoleColor.Gray, false);
				result = null;
			}
			else
			{
				string[] source = FileManager.ReadAllLines(path);
				string[] array = FileManager.ReadAllLines(path2);
				if (array.Length == 0)
				{
					ServerConsole.AddLog("Can't load central server public key from cache - empty signature.", ConsoleColor.Gray, false);
					result = null;
				}
				else
				{
					string text = source.Aggregate("", (string current, string line) => current + line + "\r\n").Trim();
					try
					{
						if (ECDSA.Verify(text, array[0], CentralServerKeyCache.MasterKey))
						{
							result = text;
						}
						else
						{
							GameCore.Console.AddLog("Invalid signature of Central Server Key in cache!", Color.red, false, ConsoleLogType.Log);
							result = null;
						}
					}
					catch (Exception ex)
					{
						if (ServerStatic.IsDedicated)
						{
							ServerConsole.AddLog("Can't load central server public key from cache - " + ex.Message, ConsoleColor.Gray, false);
						}
						else
						{
							GameCore.Console.AddLog("Can't load central server public key from cache - " + ex.Message, Color.magenta, false, ConsoleLogType.Log);
						}
						result = null;
					}
				}
			}
		}
		catch (Exception ex2)
		{
			ServerConsole.AddLog("Can't read public key cache - " + ex2.Message, ConsoleColor.Gray, false);
			result = null;
		}
		return result;
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0001D3E4 File Offset: 0x0001B5E4
	public static void SaveCache(string key, string signature)
	{
		try
		{
			if (!ECDSA.Verify(key, signature, CentralServerKeyCache.MasterKey))
			{
				GameCore.Console.AddLog("Invalid signature of Central Server Key!", Color.red, false, ConsoleLogType.Log);
			}
			else
			{
				string appFolder = FileManager.GetAppFolder(true, false, "");
				string path = appFolder + "internal/KeyCache";
				if (!Directory.Exists(FileManager.GetAppFolder(true, false, "") + "internal/"))
				{
					Directory.CreateDirectory(FileManager.GetAppFolder(true, false, "") + "internal/");
				}
				if (File.Exists(path))
				{
					if (key == CentralServerKeyCache.ReadCache())
					{
						ServerConsole.AddLog("Key cache is up to date.", ConsoleColor.Gray, false);
						return;
					}
					File.Delete(path);
				}
				ServerConsole.AddLog("Updating key cache...", ConsoleColor.Gray, false);
				FileManager.WriteStringToFile(key, path);
				FileManager.WriteStringToFile(signature, appFolder + "internal/KeySignatureCache");
				ServerConsole.AddLog("Key cache updated.", ConsoleColor.Gray, false);
			}
		}
		catch (Exception ex)
		{
			ServerConsole.AddLog("Can't write public key cache - " + ex.Message, ConsoleColor.Gray, false);
		}
	}

	// Token: 0x040006D3 RID: 1747
	private const string CacheLocation = "internal/KeyCache";

	// Token: 0x040006D4 RID: 1748
	private const string CacheSignatureLocation = "internal/KeySignatureCache";

	// Token: 0x040006D5 RID: 1749
	private const string InternalDir = "internal/";

	// Token: 0x040006D6 RID: 1750
	internal static readonly AsymmetricKeyParameter MasterKey = ECDSA.PublicKeyFromString("-----BEGIN PUBLIC KEY-----\r\nMIGbMBAGByqGSM49AgEGBSuBBAAjA4GGAAQAbL0YvrhVB2meqCq5XzjAJD8Ii0hb\r\nBHdIQ587N583cP8twjDhcITjZhBHJPJDuA85XdpgG04HwT0SD3WcAvoQXBUAUsG1\r\nLS9TR4urHwfgfroq4tH2HAQE6ZxFZeIFSglLO8nxySim4yKBj96HLG624lzKvzoD\r\nId+GOwjcd3XskOq9Dwc=\r\n-----END PUBLIC KEY-----");

	// Token: 0x040006D7 RID: 1751
	private const string MasterPublicKey = "-----BEGIN PUBLIC KEY-----\r\nMIGbMBAGByqGSM49AgEGBSuBBAAjA4GGAAQAbL0YvrhVB2meqCq5XzjAJD8Ii0hb\r\nBHdIQ587N583cP8twjDhcITjZhBHJPJDuA85XdpgG04HwT0SD3WcAvoQXBUAUsG1\r\nLS9TR4urHwfgfroq4tH2HAQE6ZxFZeIFSglLO8nxySim4yKBj96HLG624lzKvzoD\r\nId+GOwjcd3XskOq9Dwc=\r\n-----END PUBLIC KEY-----";
}
