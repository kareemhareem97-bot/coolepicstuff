using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using CentralAuth;
using Cryptography;
using GameCore;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LiteNetLib;
using LiteNetLib.Utils;
using Mirror.LiteNetLib4Mirror;

// Token: 0x02000189 RID: 393
public class CustomLiteNetLib4MirrorTransport : LiteNetLib4MirrorTransport
{
	// Token: 0x1700013B RID: 315
	// (get) Token: 0x060007BF RID: 1983 RVA: 0x0001D6A7 File Offset: 0x0001B8A7
	// (set) Token: 0x060007C0 RID: 1984 RVA: 0x0001D6AE File Offset: 0x0001B8AE
	public static bool PreVerifySignatures { get; set; }

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x060007C1 RID: 1985 RVA: 0x0001D6B6 File Offset: 0x0001B8B6
	// (set) Token: 0x060007C2 RID: 1986 RVA: 0x0001D6C0 File Offset: 0x0001B8C0
	public static bool DelayConnections
	{
		get
		{
			return CustomLiteNetLib4MirrorTransport._delayConnections;
		}
		set
		{
			if (CustomLiteNetLib4MirrorTransport._delayConnections == value)
			{
				return;
			}
			if (!value)
			{
				CustomLiteNetLib4MirrorTransport.UserIds.Clear();
			}
			CustomLiteNetLib4MirrorTransport._delayConnections = value;
			ServerConsole.AddLog(value ? string.Format("Incoming connections will be now delayed by {0} seconds.", CustomLiteNetLib4MirrorTransport.DelayTime) : "Incoming connections will be no longer delayed.", ConsoleColor.Gray, false);
		}
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x0001D794 File Offset: 0x0001B994
	protected internal override void ProcessConnectionRequest(ConnectionRequest request)
	{
		try
		{
			byte b;
			if (!request.Data.TryGetByte(out b) || b >= 2)
			{
				CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
				CustomLiteNetLib4MirrorTransport.RequestWriter.Put(2);
				request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
			}
			else if (b == 1)
			{
				string a;
				if (CustomLiteNetLib4MirrorTransport.VerificationChallenge != null && request.Data.TryGetString(out a) && a == CustomLiteNetLib4MirrorTransport.VerificationChallenge)
				{
					CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
					CustomLiteNetLib4MirrorTransport.RequestWriter.Put(18);
					CustomLiteNetLib4MirrorTransport.RequestWriter.Put(CustomLiteNetLib4MirrorTransport.VerificationResponse, 0);
					request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
					CustomLiteNetLib4MirrorTransport.VerificationChallenge = null;
					CustomLiteNetLib4MirrorTransport.VerificationResponse = null;
					ServerConsole.AddLog("Verification challenge and response have been sent.\nThe system has successfully checked your server, a verification response will be printed to your console shortly, please allow up to 5 minutes.", ConsoleColor.Green, false);
				}
				else
				{
					CustomLiteNetLib4MirrorTransport.Rejected += 1U;
					if (CustomLiteNetLib4MirrorTransport.Rejected > CustomLiteNetLib4MirrorTransport.RejectionThreshold)
					{
						CustomLiteNetLib4MirrorTransport.SuppressRejections = true;
					}
					if (!CustomLiteNetLib4MirrorTransport.SuppressRejections && CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
					{
						ServerConsole.AddLog(string.Format("Invalid verification challenge has been received from endpoint {0}.", request.RemoteEndPoint), ConsoleColor.Gray, false);
					}
					CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
					CustomLiteNetLib4MirrorTransport.RequestWriter.Put(19);
					request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
				}
			}
			else
			{
				byte cBackwardRevision = 0;
				byte cMajor;
				byte cMinor;
				byte cRevision;
				bool flag;
				if (!request.Data.TryGetByte(out cMajor) || !request.Data.TryGetByte(out cMinor) || !request.Data.TryGetByte(out cRevision) || !request.Data.TryGetBool(out flag) || (flag && !request.Data.TryGetByte(out cBackwardRevision)))
				{
					CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
					CustomLiteNetLib4MirrorTransport.RequestWriter.Put(3);
					request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
				}
				else if (!GameCore.Version.CompatibilityCheck(GameCore.Version.Major, GameCore.Version.Minor, GameCore.Version.Revision, cMajor, cMinor, cRevision, flag, cBackwardRevision))
				{
					CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
					CustomLiteNetLib4MirrorTransport.RequestWriter.Put(3);
					request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
				}
				else
				{
					int num;
					bool flag2 = request.Data.TryGetInt(out num);
					byte[] array = null;
					if (num != 0 && !request.Data.TryGetBytesWithLength(out array))
					{
						flag2 = false;
					}
					if (!flag2)
					{
						CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
						CustomLiteNetLib4MirrorTransport.RequestWriter.Put(15);
						request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
					}
					else
					{
						if (CustomLiteNetLib4MirrorTransport.UseChallenge)
						{
							if (num == 0 || array == null || array.Length == 0)
							{
								if (PlayerAuthenticationManager.OnlineMode)
								{
									string text;
									if (!request.Data.TryGetString(out text) || !text.Contains('@', StringComparison.Ordinal))
									{
										CustomLiteNetLib4MirrorTransport.Rejected += 1U;
										if (CustomLiteNetLib4MirrorTransport.Rejected > CustomLiteNetLib4MirrorTransport.RejectionThreshold)
										{
											CustomLiteNetLib4MirrorTransport.SuppressRejections = true;
										}
										if (!CustomLiteNetLib4MirrorTransport.SuppressRejections && CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
										{
											ServerConsole.AddLog(string.Format("REJECTED challenge request from endpoint {0} due to missing PreAuthentication data.", request.RemoteEndPoint), ConsoleColor.Gray, false);
										}
										if (CustomLiteNetLib4MirrorTransport.SuppressRejections)
										{
											return;
										}
										CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
										CustomLiteNetLib4MirrorTransport.RequestWriter.Put(2);
										request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
										return;
									}
									else
									{
										long @long = request.Data.GetLong();
										byte @byte = request.Data.GetByte();
										string @string = request.Data.GetString();
										byte[] bytesWithLength = request.Data.GetBytesWithLength();
										bool flag3 = (long)LiteNetLib4MirrorServer.GetPendingConnectionRequestsCount() >= (long)LiteNetLib4MirrorTransport.Singleton.reconnectDelay / this._lastPreAuthSpeed;
										bool flag4 = false;
										if (!flag3 || CustomLiteNetLib4MirrorTransport.PreVerifySignatures)
										{
											CustomLiteNetLib4MirrorTransport._preAuthSpeed.Reset();
											CustomLiteNetLib4MirrorTransport._preAuthSpeed.Start();
											flag4 = ECDSA.VerifyBytes(string.Format("{0};{1};{2};{3}", new object[]
											{
												text,
												@byte,
												@string,
												@long
											}), bytesWithLength, ServerConsole.PublicKey);
											CustomLiteNetLib4MirrorTransport._preAuthSpeed.Stop();
											this._lastPreAuthSpeed = Math.Max(1L, CustomLiteNetLib4MirrorTransport._preAuthSpeed.ElapsedMilliseconds);
											if (flag3)
											{
												CustomLiteNetLib4MirrorTransport.PreVerifySignatures = false;
											}
										}
										if (!flag3 && !flag4)
										{
											CustomLiteNetLib4MirrorTransport.Rejected += 1U;
											if (CustomLiteNetLib4MirrorTransport.Rejected > CustomLiteNetLib4MirrorTransport.RejectionThreshold)
											{
												CustomLiteNetLib4MirrorTransport.SuppressRejections = true;
											}
											if (!CustomLiteNetLib4MirrorTransport.SuppressRejections && CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
											{
												ServerConsole.AddLog(string.Format("REJECTED challenge request from endpoint {0} due to invalid digital signature.", request.RemoteEndPoint), ConsoleColor.Gray, false);
											}
											if (CustomLiteNetLib4MirrorTransport.SuppressRejections)
											{
												return;
											}
											CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
											CustomLiteNetLib4MirrorTransport.RequestWriter.Put(2);
											request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
											return;
										}
										else if (!flag3 && !CustomLiteNetLib4MirrorTransport.CheckUserRateLimit(request, text, request.RemoteEndPoint.ToString()))
										{
											return;
										}
									}
								}
								if (!CustomLiteNetLib4MirrorTransport.CheckIpRateLimit(request))
								{
									return;
								}
								int num2 = 0;
								string key = string.Empty;
								for (byte b2 = 0; b2 < 3; b2 += 1)
								{
									num2 = RandomGenerator.GetInt32(false);
									if (num2 == 0)
									{
										num2 = 1;
									}
									IPAddress address = request.RemoteEndPoint.Address;
									key = ((address != null) ? address.ToString() : null) + "-" + num2.ToString();
									if (!CustomLiteNetLib4MirrorTransport.Challenges.ContainsKey(key))
									{
										break;
									}
									if (b2 == 2)
									{
										CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
										CustomLiteNetLib4MirrorTransport.RequestWriter.Put(4);
										request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
										if (CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
										{
											ServerConsole.AddLog(string.Format("Failed to generate ID for challenge for incoming connection from endpoint {0}.", request.RemoteEndPoint), ConsoleColor.Gray, false);
										}
										return;
									}
								}
								byte[] bytes = RandomGenerator.GetBytes((int)(CustomLiteNetLib4MirrorTransport.ChallengeInitLen + CustomLiteNetLib4MirrorTransport.ChallengeSecretLen), true);
								CustomLiteNetLib4MirrorTransport.ChallengeIssued += 1U;
								if (CustomLiteNetLib4MirrorTransport.ChallengeIssued > CustomLiteNetLib4MirrorTransport.IssuedThreshold)
								{
									CustomLiteNetLib4MirrorTransport.SuppressIssued = true;
								}
								if (!CustomLiteNetLib4MirrorTransport.SuppressIssued && CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
								{
									ServerConsole.AddLog(string.Format("Requested challenge for incoming connection from endpoint {0}.", request.RemoteEndPoint), ConsoleColor.Gray, false);
								}
								CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
								CustomLiteNetLib4MirrorTransport.RequestWriter.Put(13);
								CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)CustomLiteNetLib4MirrorTransport.ChallengeMode);
								CustomLiteNetLib4MirrorTransport.RequestWriter.Put(num2);
								ChallengeType challengeMode = CustomLiteNetLib4MirrorTransport.ChallengeMode;
								if (challengeMode != ChallengeType.MD5)
								{
									if (challengeMode != ChallengeType.SHA1)
									{
										CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(bytes);
										CustomLiteNetLib4MirrorTransport.Challenges.TryAdd(key, new PreauthChallengeItem(new ArraySegment<byte>(bytes)));
									}
									else
									{
										CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(bytes, 0, CustomLiteNetLib4MirrorTransport.ChallengeInitLen);
										CustomLiteNetLib4MirrorTransport.RequestWriter.Put(CustomLiteNetLib4MirrorTransport.ChallengeSecretLen);
										CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(Sha.Sha1(bytes));
										CustomLiteNetLib4MirrorTransport.Challenges.TryAdd(key, new PreauthChallengeItem(new ArraySegment<byte>(bytes, (int)CustomLiteNetLib4MirrorTransport.ChallengeInitLen, (int)CustomLiteNetLib4MirrorTransport.ChallengeSecretLen)));
									}
								}
								else
								{
									CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(bytes, 0, CustomLiteNetLib4MirrorTransport.ChallengeInitLen);
									CustomLiteNetLib4MirrorTransport.RequestWriter.Put(CustomLiteNetLib4MirrorTransport.ChallengeSecretLen);
									CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(Md.Md5(bytes));
									CustomLiteNetLib4MirrorTransport.Challenges.TryAdd(key, new PreauthChallengeItem(new ArraySegment<byte>(bytes, (int)CustomLiteNetLib4MirrorTransport.ChallengeInitLen, (int)CustomLiteNetLib4MirrorTransport.ChallengeSecretLen)));
								}
								request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
								return;
							}
							else
							{
								IPAddress address2 = request.RemoteEndPoint.Address;
								string key2 = ((address2 != null) ? address2.ToString() : null) + "-" + num.ToString();
								if (!CustomLiteNetLib4MirrorTransport.Challenges.ContainsKey(key2))
								{
									CustomLiteNetLib4MirrorTransport.Rejected += 1U;
									if (CustomLiteNetLib4MirrorTransport.Rejected > CustomLiteNetLib4MirrorTransport.RejectionThreshold)
									{
										CustomLiteNetLib4MirrorTransport.SuppressRejections = true;
									}
									if (!CustomLiteNetLib4MirrorTransport.SuppressRejections && CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
									{
										ServerConsole.AddLog(string.Format("Security challenge response of incoming connection from endpoint {0} has been REJECTED (invalid Challenge ID).", request.RemoteEndPoint), ConsoleColor.Gray, false);
									}
									CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
									CustomLiteNetLib4MirrorTransport.RequestWriter.Put(14);
									request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
									return;
								}
								PreauthChallengeItem preauthChallengeItem = CustomLiteNetLib4MirrorTransport.Challenges[key2];
								ArraySegment<byte> validResponse = preauthChallengeItem.ValidResponse;
								if (!array.SequenceEqual(validResponse))
								{
									CustomLiteNetLib4MirrorTransport.Rejected += 1U;
									if (CustomLiteNetLib4MirrorTransport.Rejected > CustomLiteNetLib4MirrorTransport.RejectionThreshold)
									{
										CustomLiteNetLib4MirrorTransport.SuppressRejections = true;
									}
									if (!CustomLiteNetLib4MirrorTransport.SuppressRejections && CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
									{
										ServerConsole.AddLog(string.Format("Security challenge response of incoming connection from endpoint {0} has been REJECTED (invalid response).", request.RemoteEndPoint), ConsoleColor.Gray, false);
									}
									CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
									CustomLiteNetLib4MirrorTransport.RequestWriter.Put(15);
									request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
									return;
								}
								CustomLiteNetLib4MirrorTransport.Challenges.TryRemove(key2, out preauthChallengeItem);
								CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
								if (CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
								{
									ServerConsole.AddLog(string.Format("Security challenge response of incoming connection from endpoint {0} has been accepted.", request.RemoteEndPoint), ConsoleColor.Gray, false);
								}
							}
						}
						else if (!CustomLiteNetLib4MirrorTransport.UseChallenge && !CustomLiteNetLib4MirrorTransport.CheckIpRateLimit(request))
						{
							return;
						}
						if (CustomLiteNetLib4MirrorTransport.DelayConnections)
						{
							CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
							CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
							CustomLiteNetLib4MirrorTransport.RequestWriter.Put(17);
							CustomLiteNetLib4MirrorTransport.RequestWriter.Put(CustomLiteNetLib4MirrorTransport.DelayTime);
							if (CustomLiteNetLib4MirrorTransport.DelayVolume < 255)
							{
								CustomLiteNetLib4MirrorTransport.DelayVolume += 1;
							}
							if (CustomLiteNetLib4MirrorTransport.DelayVolume < CustomLiteNetLib4MirrorTransport.DelayVolumeThreshold)
							{
								if (CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
								{
									ServerConsole.AddLog(string.Format("Delayed connection incoming from endpoint {0} by {1} seconds.", request.RemoteEndPoint, CustomLiteNetLib4MirrorTransport.DelayTime), ConsoleColor.Gray, false);
								}
								request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
							}
							else
							{
								if (CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
								{
									ServerConsole.AddLog(string.Format("Force delayed connection incoming from endpoint {0} by {1} seconds.", request.RemoteEndPoint, CustomLiteNetLib4MirrorTransport.DelayTime), ConsoleColor.Gray, false);
								}
								request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
							}
						}
						else
						{
							int position = request.Data.Position;
							string text2;
							long num3;
							byte b3;
							string text3;
							byte[] signature;
							if (!PlayerAuthenticationManager.OnlineMode)
							{
								KeyValuePair<BanDetails, BanDetails> keyValuePair = BanHandler.QueryBan(null, request.RemoteEndPoint.Address.ToString());
								if (keyValuePair.Value != null)
								{
									if (CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
									{
										ServerConsole.AddLog(string.Format("Player tried to connect from banned endpoint {0}.", request.RemoteEndPoint), ConsoleColor.Gray, false);
									}
									CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
									CustomLiteNetLib4MirrorTransport.RequestWriter.Put(6);
									CustomLiteNetLib4MirrorTransport.RequestWriter.Put(keyValuePair.Value.Expires);
									NetDataWriter requestWriter = CustomLiteNetLib4MirrorTransport.RequestWriter;
									BanDetails value = keyValuePair.Value;
									requestWriter.Put(((value != null) ? value.Reason : null) ?? string.Empty, 0);
									request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
									CustomLiteNetLib4MirrorTransport.ResetIdleMode();
								}
								else
								{
									PlayerPreAuthenticatingEventArgs playerPreAuthenticatingEventArgs = new PlayerPreAuthenticatingEventArgs(!CustomLiteNetLib4MirrorTransport.IsServerFull(null, CentralAuthPreauthFlags.None), string.Empty, request.RemoteEndPoint.Address.ToString(), 0L, CentralAuthPreauthFlags.None, string.Empty, null, request, position);
									PlayerEvents.OnPreAuthenticating(playerPreAuthenticatingEventArgs);
									if (!playerPreAuthenticatingEventArgs.IsAllowed)
									{
										if (playerPreAuthenticatingEventArgs.CustomReject != null)
										{
											if (playerPreAuthenticatingEventArgs.ForceReject)
											{
												request.RejectForce(playerPreAuthenticatingEventArgs.CustomReject);
											}
											else
											{
												request.Reject(playerPreAuthenticatingEventArgs.CustomReject);
											}
										}
										else
										{
											CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
											CustomLiteNetLib4MirrorTransport.RequestWriter.Put(4);
											request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
										}
									}
									else if (playerPreAuthenticatingEventArgs.CanJoin)
									{
										request.Accept();
										CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
										PlayerEvents.OnPreAuthenticated(new PlayerPreAuthenticatedEventArgs(string.Empty, request.RemoteEndPoint.Address.ToString(), 0L, CentralAuthPreauthFlags.None, string.Empty, null, request, position));
									}
									else
									{
										CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
										CustomLiteNetLib4MirrorTransport.RequestWriter.Put(1);
										request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
										CustomLiteNetLib4MirrorTransport.ResetIdleMode();
									}
								}
							}
							else if (!request.Data.TryGetString(out text2) || text2 == string.Empty)
							{
								CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
								CustomLiteNetLib4MirrorTransport.RequestWriter.Put(5);
								request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
							}
							else if (!request.Data.TryGetLong(out num3) || !request.Data.TryGetByte(out b3) || !request.Data.TryGetString(out text3) || !request.Data.TryGetBytesWithLength(out signature))
							{
								CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
								CustomLiteNetLib4MirrorTransport.RequestWriter.Put(4);
								request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
							}
							else
							{
								string text4 = null;
								string text5;
								if (!CustomLiteNetLib4MirrorTransport.IpPassthroughEnabled || !CustomLiteNetLib4MirrorTransport.TrustedProxies.ContainsKey(request.RemoteEndPoint.Address) || !request.Data.TryGetString(out text4))
								{
									text5 = request.RemoteEndPoint.ToString();
								}
								else
								{
									text5 = string.Format("{0} [routed via {1}]", text4, request.RemoteEndPoint);
								}
								CentralAuthPreauthFlags flags = (CentralAuthPreauthFlags)b3;
								try
								{
									if (!ECDSA.VerifyBytes(string.Format("{0};{1};{2};{3}", new object[]
									{
										text2,
										b3,
										text3,
										num3
									}), signature, ServerConsole.PublicKey))
									{
										CustomLiteNetLib4MirrorTransport.Rejected += 1U;
										if (CustomLiteNetLib4MirrorTransport.Rejected > CustomLiteNetLib4MirrorTransport.RejectionThreshold)
										{
											CustomLiteNetLib4MirrorTransport.SuppressRejections = true;
										}
										if (!CustomLiteNetLib4MirrorTransport.SuppressRejections && CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
										{
											ServerConsole.AddLog("Player from endpoint " + text5 + " sent preauthentication token with invalid digital signature.", ConsoleColor.Gray, false);
										}
										CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
										CustomLiteNetLib4MirrorTransport.RequestWriter.Put(2);
										request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
										CustomLiteNetLib4MirrorTransport.ResetIdleMode();
									}
									else if (TimeBehaviour.CurrentUnixTimestamp > num3)
									{
										CustomLiteNetLib4MirrorTransport.Rejected += 1U;
										if (CustomLiteNetLib4MirrorTransport.Rejected > CustomLiteNetLib4MirrorTransport.RejectionThreshold)
										{
											CustomLiteNetLib4MirrorTransport.SuppressRejections = true;
										}
										if (!CustomLiteNetLib4MirrorTransport.SuppressRejections && CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
										{
											ServerConsole.AddLog("Player from endpoint " + text5 + " sent expired preauthentication token.", ConsoleColor.Gray, false);
											ServerConsole.AddLog("Make sure that time and timezone set on server is correct. We recommend synchronizing the time.", ConsoleColor.Gray, false);
										}
										CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
										CustomLiteNetLib4MirrorTransport.RequestWriter.Put(11);
										request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
										CustomLiteNetLib4MirrorTransport.ResetIdleMode();
									}
									else if (CustomLiteNetLib4MirrorTransport.UseChallenge || CustomLiteNetLib4MirrorTransport.CheckUserRateLimit(request, text2, text5))
									{
										if (!flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreBans) || !CustomNetworkManager.IsVerified)
										{
											KeyValuePair<BanDetails, BanDetails> keyValuePair2 = BanHandler.QueryBan(text2, text4 ?? request.RemoteEndPoint.Address.ToString());
											if (keyValuePair2.Key != null || keyValuePair2.Value != null)
											{
												CustomLiteNetLib4MirrorTransport.Rejected += 1U;
												if (CustomLiteNetLib4MirrorTransport.Rejected > CustomLiteNetLib4MirrorTransport.RejectionThreshold)
												{
													CustomLiteNetLib4MirrorTransport.SuppressRejections = true;
												}
												if (!CustomLiteNetLib4MirrorTransport.SuppressRejections && CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
												{
													ServerConsole.AddLog(string.Concat(new string[]
													{
														(keyValuePair2.Key == null) ? "Player" : "Banned player",
														" ",
														text2,
														" tried to connect from",
														(keyValuePair2.Value == null) ? "" : " banned",
														" endpoint ",
														text5,
														"."
													}), ConsoleColor.Gray, false);
													ServerLogs.AddLog(ServerLogs.Modules.Networking, string.Concat(new string[]
													{
														(keyValuePair2.Key == null) ? "Player" : "Banned player",
														" ",
														text2,
														" tried to connect from",
														(keyValuePair2.Value == null) ? "" : " banned",
														" endpoint ",
														text5,
														"."
													}), ServerLogs.ServerLogType.ConnectionUpdate, false);
												}
												CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
												CustomLiteNetLib4MirrorTransport.RequestWriter.Put(6);
												NetDataWriter requestWriter2 = CustomLiteNetLib4MirrorTransport.RequestWriter;
												BanDetails key3 = keyValuePair2.Key;
												requestWriter2.Put((key3 != null) ? key3.Expires : keyValuePair2.Value.Expires);
												NetDataWriter requestWriter3 = CustomLiteNetLib4MirrorTransport.RequestWriter;
												BanDetails key4 = keyValuePair2.Key;
												string value2;
												if ((value2 = ((key4 != null) ? key4.Reason : null)) == null)
												{
													BanDetails value3 = keyValuePair2.Value;
													value2 = (((value3 != null) ? value3.Reason : null) ?? string.Empty);
												}
												requestWriter3.Put(value2, 0);
												request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
												CustomLiteNetLib4MirrorTransport.ResetIdleMode();
												return;
											}
										}
										if (flags.HasFlagFast(CentralAuthPreauthFlags.AuthRejected))
										{
											if (CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
											{
												ServerConsole.AddLog(string.Concat(new string[]
												{
													"Player ",
													text2,
													" (",
													text5,
													") kicked due to auth rejection by central server."
												}), ConsoleColor.Gray, false);
											}
											CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
											CustomLiteNetLib4MirrorTransport.RequestWriter.Put(20);
											request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
											CustomLiteNetLib4MirrorTransport.ResetIdleMode();
										}
										else if (flags.HasFlagFast(CentralAuthPreauthFlags.GloballyBanned) && (CustomNetworkManager.IsVerified || CustomLiteNetLib4MirrorTransport.UseGlobalBans))
										{
											if (CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
											{
												ServerConsole.AddLog(string.Concat(new string[]
												{
													"Player ",
													text2,
													" (",
													text5,
													") kicked due to an active global ban."
												}), ConsoleColor.Gray, false);
											}
											CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
											CustomLiteNetLib4MirrorTransport.RequestWriter.Put(8);
											request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
											CustomLiteNetLib4MirrorTransport.ResetIdleMode();
										}
										else if ((!flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreWhitelist) || !CustomNetworkManager.IsVerified) && !WhiteList.IsWhitelisted(text2))
										{
											if (CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
											{
												ServerConsole.AddLog(string.Concat(new string[]
												{
													"Player ",
													text2,
													" tried joined from endpoint ",
													text5,
													", but is not whitelisted."
												}), ConsoleColor.Gray, false);
											}
											CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
											CustomLiteNetLib4MirrorTransport.RequestWriter.Put(7);
											request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
											CustomLiteNetLib4MirrorTransport.ResetIdleMode();
										}
										else if (CustomLiteNetLib4MirrorTransport.Geoblocking != GeoblockingMode.None && (!flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreGeoblock) || !ServerStatic.PermissionsHandler.BanTeamBypassGeo) && (!CustomLiteNetLib4MirrorTransport.GeoblockIgnoreWhitelisted || !WhiteList.IsOnWhitelist(text2)) && ((CustomLiteNetLib4MirrorTransport.Geoblocking == GeoblockingMode.Whitelist && !CustomLiteNetLib4MirrorTransport.GeoblockingList.ContainsKey(text3)) || (CustomLiteNetLib4MirrorTransport.Geoblocking == GeoblockingMode.Blacklist && CustomLiteNetLib4MirrorTransport.GeoblockingList.ContainsKey(text3))))
										{
											CustomLiteNetLib4MirrorTransport.Rejected += 1U;
											if (CustomLiteNetLib4MirrorTransport.Rejected > CustomLiteNetLib4MirrorTransport.RejectionThreshold)
											{
												CustomLiteNetLib4MirrorTransport.SuppressRejections = true;
											}
											if (!CustomLiteNetLib4MirrorTransport.SuppressRejections && CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
											{
												ServerConsole.AddLog(string.Concat(new string[]
												{
													"Player ",
													text2,
													" (",
													text5,
													") tried joined from blocked country ",
													text3,
													"."
												}), ConsoleColor.Gray, false);
											}
											CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
											CustomLiteNetLib4MirrorTransport.RequestWriter.Put(9);
											request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
											CustomLiteNetLib4MirrorTransport.ResetIdleMode();
										}
										else
										{
											byte b4;
											CustomLiteNetLib4MirrorTransport.UserIdFastReload.TryRemove(text2, out b4);
											PlayerPreAuthenticatingEventArgs playerPreAuthenticatingEventArgs2 = new PlayerPreAuthenticatingEventArgs(!CustomLiteNetLib4MirrorTransport.IsServerFull(text2, flags), text2, (text4 == null) ? request.RemoteEndPoint.Address.ToString() : text4, num3, flags, text3, signature, request, position);
											PlayerEvents.OnPreAuthenticating(playerPreAuthenticatingEventArgs2);
											if (!playerPreAuthenticatingEventArgs2.IsAllowed)
											{
												if (playerPreAuthenticatingEventArgs2.CustomReject != null)
												{
													if (playerPreAuthenticatingEventArgs2.ForceReject)
													{
														request.RejectForce(playerPreAuthenticatingEventArgs2.CustomReject);
													}
													else
													{
														request.Reject(playerPreAuthenticatingEventArgs2.CustomReject);
													}
												}
												else
												{
													CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
													CustomLiteNetLib4MirrorTransport.RequestWriter.Put(4);
													request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
												}
											}
											else if (playerPreAuthenticatingEventArgs2.CanJoin)
											{
												if (CustomLiteNetLib4MirrorTransport.UserIds.ContainsKey(request.RemoteEndPoint))
												{
													CustomLiteNetLib4MirrorTransport.UserIds[request.RemoteEndPoint].SetUserId(text2);
												}
												else
												{
													CustomLiteNetLib4MirrorTransport.UserIds.TryAdd(request.RemoteEndPoint, new PreauthItem(text2));
												}
												LiteNetPeer liteNetPeer = request.Accept();
												if (text4 != null)
												{
													if (CustomLiteNetLib4MirrorTransport.RealIpAddresses.ContainsKey(liteNetPeer.Id))
													{
														CustomLiteNetLib4MirrorTransport.RealIpAddresses[liteNetPeer.Id] = text4;
													}
													else
													{
														CustomLiteNetLib4MirrorTransport.RealIpAddresses.TryAdd(liteNetPeer.Id, text4);
													}
												}
												ServerConsole.AddLog(string.Concat(new string[]
												{
													"Player ",
													text2,
													" preauthenticated from endpoint ",
													text5,
													"."
												}), ConsoleColor.Gray, false);
												ServerLogs.AddLog(ServerLogs.Modules.Networking, text2 + " preauthenticated from endpoint " + text5 + ".", ServerLogs.ServerLogType.ConnectionUpdate, false);
												CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
												PlayerEvents.OnPreAuthenticated(new PlayerPreAuthenticatedEventArgs(text2, (text4 == null) ? request.RemoteEndPoint.Address.ToString() : text4, num3, flags, text3, signature, request, position));
											}
											else
											{
												CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
												CustomLiteNetLib4MirrorTransport.RequestWriter.Put(1);
												request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
												CustomLiteNetLib4MirrorTransport.ResetIdleMode();
											}
										}
									}
								}
								catch (Exception ex)
								{
									CustomLiteNetLib4MirrorTransport.Rejected += 1U;
									if (CustomLiteNetLib4MirrorTransport.Rejected > CustomLiteNetLib4MirrorTransport.RejectionThreshold)
									{
										CustomLiteNetLib4MirrorTransport.SuppressRejections = true;
									}
									if (!CustomLiteNetLib4MirrorTransport.SuppressRejections && CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
									{
										ServerConsole.AddLog("Player from endpoint " + text5 + " sent an invalid preauthentication token. " + ex.Message, ConsoleColor.Gray, false);
									}
									CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
									CustomLiteNetLib4MirrorTransport.RequestWriter.Put(2);
									request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
									CustomLiteNetLib4MirrorTransport.ResetIdleMode();
								}
							}
						}
					}
				}
			}
		}
		catch (Exception ex2)
		{
			CustomLiteNetLib4MirrorTransport.Rejected += 1U;
			if (CustomLiteNetLib4MirrorTransport.Rejected > CustomLiteNetLib4MirrorTransport.RejectionThreshold)
			{
				CustomLiteNetLib4MirrorTransport.SuppressRejections = true;
			}
			if (!CustomLiteNetLib4MirrorTransport.SuppressRejections)
			{
				ServerConsole.AddLog(string.Format("Player from endpoint {0} failed to preauthenticate: {1}", request.RemoteEndPoint, ex2.Message), ConsoleColor.Gray, false);
			}
			CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
			CustomLiteNetLib4MirrorTransport.RequestWriter.Put(4);
			request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
		}
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x0001EBD0 File Offset: 0x0001CDD0
	private static bool IsServerFull(string userId = null, CentralAuthPreauthFlags flags = CentralAuthPreauthFlags.None)
	{
		return (string.IsNullOrEmpty(userId) || !CustomLiteNetLib4MirrorTransport.HasReservedSlot(userId, flags)) && LiteNetLib4MirrorCore.Host.ConnectedPeersCount >= CustomNetworkManager.slots;
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x0001EBFC File Offset: 0x0001CDFC
	private static bool HasReservedSlot(string userId, CentralAuthPreauthFlags flags)
	{
		return (flags.HasFlagFast(CentralAuthPreauthFlags.ReservedSlot) && ServerStatic.PermissionsHandler.BanTeamSlots) || (CustomLiteNetLib4MirrorTransport.UseReservedSlots && ReservedSlot.HasReservedSlot(userId) && CustomNetworkManager.slots + CustomNetworkManager.reservedSlots - LiteNetLib4MirrorCore.Host.ConnectedPeersCount > 0);
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x0001EC4C File Offset: 0x0001CE4C
	private static bool CheckIpRateLimit(ConnectionRequest request)
	{
		if (!CustomLiteNetLib4MirrorTransport.IpRateLimiting)
		{
			return true;
		}
		if (CustomLiteNetLib4MirrorTransport.IpRateLimit.ContainsKey(request.RemoteEndPoint.Address))
		{
			CustomLiteNetLib4MirrorTransport.Rejected += 1U;
			if (CustomLiteNetLib4MirrorTransport.Rejected > CustomLiteNetLib4MirrorTransport.RejectionThreshold)
			{
				CustomLiteNetLib4MirrorTransport.SuppressRejections = true;
			}
			if (!CustomLiteNetLib4MirrorTransport.SuppressRejections && !CustomLiteNetLib4MirrorTransport.SuppressIssued)
			{
				ServerConsole.AddLog(string.Format("Incoming connection from endpoint {0} rejected due to exceeding the rate limit.", request.RemoteEndPoint), ConsoleColor.Gray, false);
				ServerLogs.AddLog(ServerLogs.Modules.Networking, string.Format("Incoming connection from endpoint {0} rejected due to exceeding the rate limit.", request.RemoteEndPoint), ServerLogs.ServerLogType.AuthRateLimit, false);
			}
			CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
			CustomLiteNetLib4MirrorTransport.RequestWriter.Put(12);
			request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
			return false;
		}
		CustomLiteNetLib4MirrorTransport.IpRateLimit.TryAdd(request.RemoteEndPoint.Address, 0);
		return true;
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x0001ED10 File Offset: 0x0001CF10
	private static bool CheckUserRateLimit(ConnectionRequest request, string userId, string ep)
	{
		if (!CustomLiteNetLib4MirrorTransport.UserRateLimiting)
		{
			return true;
		}
		if (CustomLiteNetLib4MirrorTransport.UserRateLimit.ContainsKey(userId))
		{
			CustomLiteNetLib4MirrorTransport.Rejected += 1U;
			if (CustomLiteNetLib4MirrorTransport.Rejected > CustomLiteNetLib4MirrorTransport.RejectionThreshold)
			{
				CustomLiteNetLib4MirrorTransport.SuppressRejections = true;
			}
			if (!CustomLiteNetLib4MirrorTransport.SuppressRejections && CustomLiteNetLib4MirrorTransport.DisplayPreauthLogs)
			{
				ServerConsole.AddLog(string.Concat(new string[]
				{
					"Incoming connection from ",
					userId,
					" (",
					ep,
					") rejected due to exceeding the rate limit."
				}), ConsoleColor.Gray, false);
			}
			CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
			CustomLiteNetLib4MirrorTransport.RequestWriter.Put(12);
			request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
			CustomLiteNetLib4MirrorTransport.ResetIdleMode();
			return false;
		}
		CustomLiteNetLib4MirrorTransport.UserRateLimit.TryAdd(userId, 0);
		return true;
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x00002E9A File Offset: 0x0000109A
	protected override void GetConnectData(NetDataWriter writer)
	{
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x0001EDCC File Offset: 0x0001CFCC
	protected internal override void OnConncetionRefused(DisconnectInfo disconnectinfo)
	{
		byte lastRejectionReason;
		if (disconnectinfo.AdditionalData.TryGetByte(out lastRejectionReason))
		{
			CustomLiteNetLib4MirrorTransport.LastRejectionReason = (RejectionReason)lastRejectionReason;
			return;
		}
		CustomLiteNetLib4MirrorTransport.LastRejectionReason = RejectionReason.NotSpecified;
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0001EDF5 File Offset: 0x0001CFF5
	private static void ResetIdleMode()
	{
		MainThreadDispatcher.Dispatch(delegate
		{
			if (LiteNetLib4MirrorCore.Host.ConnectedPeersCount != 0)
			{
				return;
			}
			IdleMode.SetIdleMode(true);
		}, MainThreadDispatcher.DispatchTime.Update);
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x0001EE1C File Offset: 0x0001D01C
	public static void PreauthDisableIdleMode()
	{
		MainThreadDispatcher.Dispatch(delegate
		{
			if (!ServerStatic.IsDedicated || !IdleMode.IdleModeActive)
			{
				return;
			}
			IdleMode.PreauthStopwatch.Restart();
			IdleMode.SetIdleMode(false);
		}, MainThreadDispatcher.DispatchTime.Update);
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x0001EE44 File Offset: 0x0001D044
	public static void ReloadChallengeOptions()
	{
		CustomLiteNetLib4MirrorTransport.UseChallenge = ConfigFile.ServerConfig.GetBool("preauth_challenge", true);
		CustomLiteNetLib4MirrorTransport.ChallengeInitLen = ConfigFile.ServerConfig.GetUShort("preauth_challenge_base_length", 16);
		CustomLiteNetLib4MirrorTransport.ChallengeSecretLen = ConfigFile.ServerConfig.GetUShort("preauth_challenge_secret_length", 5);
		string a = ConfigFile.ServerConfig.GetString("preauth_challenge_mode", "reply").ToLower();
		if (a == "md5")
		{
			CustomLiteNetLib4MirrorTransport.ChallengeMode = ChallengeType.MD5;
			return;
		}
		if (!(a == "sha1"))
		{
			CustomLiteNetLib4MirrorTransport.ChallengeMode = ChallengeType.Reply;
			CustomLiteNetLib4MirrorTransport.ChallengeSecretLen = 0;
			return;
		}
		CustomLiteNetLib4MirrorTransport.ChallengeMode = ChallengeType.SHA1;
	}

	// Token: 0x04000702 RID: 1794
	private static readonly NetDataWriter RequestWriter = new NetDataWriter();

	// Token: 0x04000703 RID: 1795
	public static GeoblockingMode Geoblocking = GeoblockingMode.None;

	// Token: 0x04000704 RID: 1796
	public static ChallengeType ChallengeMode = ChallengeType.Reply;

	// Token: 0x04000705 RID: 1797
	public static ushort ChallengeInitLen;

	// Token: 0x04000706 RID: 1798
	public static ushort ChallengeSecretLen;

	// Token: 0x04000707 RID: 1799
	public static readonly ConcurrentDictionary<IPEndPoint, PreauthItem> UserIds = new ConcurrentDictionary<IPEndPoint, PreauthItem>();

	// Token: 0x04000708 RID: 1800
	public static readonly ConcurrentDictionary<string, PreauthChallengeItem> Challenges = new ConcurrentDictionary<string, PreauthChallengeItem>();

	// Token: 0x04000709 RID: 1801
	public static bool UserRateLimiting;

	// Token: 0x0400070A RID: 1802
	public static bool IpRateLimiting;

	// Token: 0x0400070B RID: 1803
	public static bool UseGlobalBans;

	// Token: 0x0400070C RID: 1804
	public static bool GeoblockIgnoreWhitelisted;

	// Token: 0x0400070D RID: 1805
	public static bool UseChallenge;

	// Token: 0x0400070E RID: 1806
	public static bool DisplayPreauthLogs;

	// Token: 0x0400070F RID: 1807
	private static bool _delayConnections = true;

	// Token: 0x04000710 RID: 1808
	public static bool SuppressRejections;

	// Token: 0x04000711 RID: 1809
	public static bool SuppressIssued;

	// Token: 0x04000712 RID: 1810
	public static uint Rejected;

	// Token: 0x04000713 RID: 1811
	public static uint ChallengeIssued;

	// Token: 0x04000714 RID: 1812
	public static byte DelayTime = 3;

	// Token: 0x04000715 RID: 1813
	internal static byte DelayVolume;

	// Token: 0x04000716 RID: 1814
	internal static byte DelayVolumeThreshold;

	// Token: 0x04000717 RID: 1815
	public static readonly ConcurrentDictionary<string, byte> UserIdFastReload = new ConcurrentDictionary<string, byte>();

	// Token: 0x04000718 RID: 1816
	public static readonly ConcurrentDictionary<string, byte> UserRateLimit = new ConcurrentDictionary<string, byte>();

	// Token: 0x04000719 RID: 1817
	public static readonly ConcurrentDictionary<IPAddress, byte> IpRateLimit = new ConcurrentDictionary<IPAddress, byte>();

	// Token: 0x0400071A RID: 1818
	public static readonly ConcurrentDictionary<string, byte> GeoblockingList = new ConcurrentDictionary<string, byte>();

	// Token: 0x0400071B RID: 1819
	public static RejectionReason LastRejectionReason;

	// Token: 0x0400071C RID: 1820
	public static string LastCustomReason;

	// Token: 0x0400071D RID: 1821
	public static string VerificationChallenge;

	// Token: 0x0400071E RID: 1822
	public static string VerificationResponse;

	// Token: 0x0400071F RID: 1823
	public static long LastBanExpiration;

	// Token: 0x04000720 RID: 1824
	public static bool IpPassthroughEnabled;

	// Token: 0x04000721 RID: 1825
	public static ConcurrentDictionary<IPAddress, byte> TrustedProxies;

	// Token: 0x04000722 RID: 1826
	public static ConcurrentDictionary<int, string> RealIpAddresses;

	// Token: 0x04000723 RID: 1827
	public static uint RejectionThreshold = 60U;

	// Token: 0x04000724 RID: 1828
	public static uint IssuedThreshold = 50U;

	// Token: 0x04000725 RID: 1829
	public static bool UseReservedSlots;

	// Token: 0x04000727 RID: 1831
	private static Stopwatch _preAuthSpeed = new Stopwatch();

	// Token: 0x04000728 RID: 1832
	private long _lastPreAuthSpeed = 1000L;

	// Token: 0x0200018A RID: 394
	private enum ClientType : byte
	{
		// Token: 0x0400072A RID: 1834
		GameClient,
		// Token: 0x0400072B RID: 1835
		VerificationService
	}
}
