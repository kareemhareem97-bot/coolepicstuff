using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using CentralAuth;
using GameCore;
using NorthwoodLib;
using RoundRestarting;
using ServerOutput;

namespace Authenticator
{
	// Token: 0x020002F5 RID: 757
	public static class AuthenticatorQuery
	{
		// Token: 0x06001073 RID: 4211 RVA: 0x0003D1D8 File Offset: 0x0003B3D8
		public static bool SendData(IEnumerable<string> data)
		{
			bool result;
			try
			{
				string text = HttpQuery.Post(CentralServer.MasterUrl + "v5/authenticator.php", HttpQuery.ToPostArgs(data), "application/x-www-form-urlencoded");
				result = (text.StartsWith("{\"") ? AuthenticatorQuery.ProcessResponse(text) : AuthenticatorQuery.ProcessLegacyResponse(text));
			}
			catch (Exception ex)
			{
				ServerConsole.AddLog("Could not update server data on server list - (LOCAL EXCEPTION) " + ex.Message, ConsoleColor.DarkRed, false);
				result = false;
			}
			return result;
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x0003D250 File Offset: 0x0003B450
		private static void SendContactAddress()
		{
			try
			{
				List<string> list = new List<string>
				{
					"ip=" + ServerConsole.Ip,
					string.Format("port={0}", ServerConsole.PortToReport),
					"version=2",
					"address=" + StringUtils.Base64Encode(ConfigFile.ServerConfig.GetString("contact_email", ""))
				};
				if (!string.IsNullOrEmpty(ServerConsole.Password))
				{
					list.Add("passcode=" + ServerConsole.Password);
				}
				HttpQuery.Post(CentralServer.MasterUrl + "v5/contactaddress.php", HttpQuery.ToPostArgs(list), "application/x-www-form-urlencoded");
			}
			catch (Exception ex)
			{
				ServerConsole.AddLog("Could not submit server contact info - (LOCAL EXCEPTION) " + ex.Message, ConsoleColor.DarkRed, false);
			}
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x0003D334 File Offset: 0x0003B534
		private static bool ProcessResponse(string response)
		{
			bool result;
			try
			{
				AuthenticatorResponse authenticatorResponse = JsonSerialize.FromJson<AuthenticatorResponse>(response);
				if (!string.IsNullOrEmpty(authenticatorResponse.verificationChallenge) && !string.IsNullOrEmpty(authenticatorResponse.verificationResponse))
				{
					CustomLiteNetLib4MirrorTransport.VerificationChallenge = authenticatorResponse.verificationChallenge;
					CustomLiteNetLib4MirrorTransport.VerificationResponse = authenticatorResponse.verificationResponse;
					ServerConsole.AddLog("Verification challenge and response have been obtained.\nThe system will attempt to check your server shortly, please allow up to 5 minutes.", ConsoleColor.Green, false);
				}
				if (!authenticatorResponse.success)
				{
					ServerConsole.AddLog("Could not update server data on server list - " + authenticatorResponse.error, ConsoleColor.DarkRed, false);
					result = false;
				}
				else
				{
					if (!string.IsNullOrEmpty(authenticatorResponse.token))
					{
						ServerConsole.AddLog("Received verification token from central server.", ConsoleColor.Gray, false);
						AuthenticatorQuery.SaveNewToken(authenticatorResponse.token);
					}
					if (authenticatorResponse.actions != null && authenticatorResponse.actions.Length != 0)
					{
						string[] array = authenticatorResponse.actions;
						for (int i = 0; i < array.Length; i++)
						{
							string action = array[i];
							MainThreadDispatcher.Dispatch(delegate
							{
								AuthenticatorQuery.HandleAction(action);
							}, MainThreadDispatcher.DispatchTime.Update);
						}
					}
					if (authenticatorResponse.messages != null && authenticatorResponse.messages.Length != 0)
					{
						foreach (string str in authenticatorResponse.messages)
						{
							ServerConsole.AddLog("[MESSAGE FROM CENTRAL SERVER] " + str, ConsoleColor.Cyan, false);
						}
					}
					if (authenticatorResponse.authAccepted != null && authenticatorResponse.authAccepted.Length != 0)
					{
						foreach (string str2 in authenticatorResponse.authAccepted)
						{
							ServerConsole.AddLog("Authentication token of player " + str2 + " has been confirmed by central server.", ConsoleColor.Gray, false);
						}
					}
					if (authenticatorResponse.authRejected != null && authenticatorResponse.authRejected.Length != 0)
					{
						using (HashSet<ReferenceHub>.Enumerator enumerator = ReferenceHub.AllHubs.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ReferenceHub player = enumerator.Current;
								PlayerAuthenticationManager pam = player.authManager;
								if (!authenticatorResponse.authRejected.All((AuthenticatiorAuthReject rj) => rj.Id != pam.UserId))
								{
									AuthenticatiorAuthReject authenticatiorAuthReject = authenticatorResponse.authRejected.FirstOrDefault((AuthenticatiorAuthReject rj) => rj.Id == pam.UserId);
									string reason = authenticatiorAuthReject.Reason ?? "<ERROR>";
									ServerConsole.AddLog("Authentication token of player " + pam.UserId + " has been revoked by central server with reason: " + reason, ConsoleColor.Gray, false);
									MainThreadDispatcher.Dispatch(delegate
									{
										player.gameConsoleTransmission.SendToClient(reason, "red");
										ServerConsole.Disconnect(player.connectionToClient, reason);
									}, MainThreadDispatcher.DispatchTime.Update);
								}
							}
						}
					}
					result = authenticatorResponse.verified;
				}
			}
			catch (Exception ex)
			{
				ServerConsole.AddLog("Could not update server data on server list - (LOCAL EXCEPTION) " + ex.Message, ConsoleColor.DarkRed, false);
				result = false;
			}
			return result;
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x0003D5E8 File Offset: 0x0003B7E8
		private static bool ProcessLegacyResponse(string response)
		{
			if (response == "YES")
			{
				return true;
			}
			if (response.StartsWith("New code generated:"))
			{
				CustomNetworkManager.IsVerified = true;
				string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/verkey.txt";
				try
				{
					File.Delete(path);
				}
				catch
				{
					ServerConsole.AddLog("New password could not be saved.", ConsoleColor.DarkRed, false);
				}
				try
				{
					StreamWriter streamWriter = new StreamWriter(path);
					string text = response.Remove(0, response.IndexOf(":", StringComparison.Ordinal)).Remove(response.IndexOf(":", StringComparison.Ordinal));
					while (text.Contains(":"))
					{
						text = text.Replace(":", string.Empty);
					}
					streamWriter.WriteLine(text);
					streamWriter.Close();
					ServerConsole.AddLog("New password saved.", ConsoleColor.DarkRed, false);
					ServerConsole.Update = true;
					return true;
				}
				catch
				{
					ServerConsole.AddLog("New password could not be saved.", ConsoleColor.DarkRed, false);
					return true;
				}
			}
			if (response.Contains(":Restart:"))
			{
				AuthenticatorQuery.HandleAction("Restart");
			}
			else if (response.Contains(":RoundRestart:"))
			{
				AuthenticatorQuery.HandleAction("RoundRestart");
			}
			else if (response.Contains(":UpdateData:"))
			{
				AuthenticatorQuery.HandleAction("UpdateData");
			}
			else if (response.Contains(":RefreshKey:"))
			{
				AuthenticatorQuery.HandleAction("RefreshKey");
			}
			else if (response.Contains(":Message - "))
			{
				string text2 = response.Substring(response.IndexOf(":Message - ", StringComparison.Ordinal) + 11);
				text2 = text2.Substring(0, text2.IndexOf(":::", StringComparison.Ordinal));
				ServerConsole.AddLog("[MESSAGE FROM CENTRAL SERVER] " + text2, ConsoleColor.Cyan, false);
			}
			else if (response.Contains(":GetContactAddress:"))
			{
				AuthenticatorQuery.HandleAction("GetContactAddress");
			}
			else
			{
				if (response.Contains("Server is not verified."))
				{
					return false;
				}
				ServerConsole.AddLog("Could not update data on server list (legacy)- " + response, ConsoleColor.DarkRed, false);
			}
			return true;
		}

		// Token: 0x06001077 RID: 4215 RVA: 0x0003D7DC File Offset: 0x0003B9DC
		internal static void HandleAction(string action)
		{
			if (!(action == "Restart"))
			{
				if (!(action == "RoundRestart"))
				{
					if (action == "UpdateData")
					{
						ServerConsole.Update = true;
						return;
					}
					if (action == "RefreshKey")
					{
						ServerConsole.RunRefreshPublicKeyOnce();
						return;
					}
					if (!(action == "GetContactAddress"))
					{
						return;
					}
					ThreadStart start;
					if ((start = AuthenticatorQuery.<>O.<0>__SendContactAddress) == null)
					{
						start = (AuthenticatorQuery.<>O.<0>__SendContactAddress = new ThreadStart(AuthenticatorQuery.SendContactAddress));
					}
					new Thread(start)
					{
						Name = "SCP:SL Response to central servers (contact data request)",
						Priority = ThreadPriority.BelowNormal,
						IsBackground = true
					}.Start();
				}
				else
				{
					ServerConsole.AddLog("Round restart requested by central server.", ConsoleColor.DarkRed, false);
					ReferenceHub referenceHub;
					if (ReferenceHub.TryGetLocalHub(out referenceHub) && referenceHub.networkIdentity.isServer)
					{
						RoundRestart.InitiateRoundRestart();
						return;
					}
				}
				return;
			}
			ServerConsole.AddOutputEntry(default(ExitActionRestartEntry));
			ServerConsole.AddLog("Server restart requested by central server.", ConsoleColor.DarkRed, false);
			Shutdown.Quit(true, false);
		}

		// Token: 0x06001078 RID: 4216 RVA: 0x0003D8C8 File Offset: 0x0003BAC8
		private static void SaveNewToken(string token)
		{
			CustomNetworkManager.IsVerified = true;
			string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/verkey.txt";
			try
			{
				File.Delete(path);
			}
			catch (Exception ex)
			{
				ServerConsole.AddLog("New verification token could not be saved (1): " + ex.Message, ConsoleColor.DarkRed, false);
			}
			try
			{
				StreamWriter streamWriter = new StreamWriter(path);
				streamWriter.WriteLine(token);
				streamWriter.Close();
				ServerConsole.AddLog("New verification token saved.", ConsoleColor.DarkRed, false);
				ServerConsole.Update = true;
				ServerConsole.ScheduleTokenRefresh = true;
			}
			catch (Exception ex2)
			{
				ServerConsole.AddLog("New verification token could not be saved (2): " + ex2.Message, ConsoleColor.DarkRed, false);
			}
		}

		// Token: 0x020002F6 RID: 758
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000CDE RID: 3294
			public static ThreadStart <0>__SendContactAddress;
		}
	}
}
