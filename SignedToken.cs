using System;
using Cryptography;
using GameCore;
using Mirror;
using NorthwoodLib;
using UnityEngine;
using Utf8Json;

// Token: 0x02000210 RID: 528
public class SignedToken : IJsonSerializable
{
	// Token: 0x06000B14 RID: 2836 RVA: 0x0002B096 File Offset: 0x00029296
	[SerializationConstructor]
	public SignedToken(string token, string signature)
	{
		this.token = token;
		this.signature = signature;
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x0002B0AC File Offset: 0x000292AC
	public void Serialize(NetworkWriter writer)
	{
		writer.WriteString(this.token);
		writer.WriteString(this.signature);
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x0002B0C6 File Offset: 0x000292C6
	public static SignedToken Deserialize(NetworkReader reader)
	{
		return new SignedToken(reader.ReadString(), reader.ReadString());
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x0002B0DC File Offset: 0x000292DC
	public bool TryGetToken<T>(string usage, out T token, out string error, out string userId, int expTimeOffset = 0) where T : Token, IJsonSerializable
	{
		bool result;
		try
		{
			token = default(T);
			error = null;
			userId = null;
			if (!ECDSA.Verify(this.token, this.signature, ServerConsole.PublicKey))
			{
				error = "invalid signature of token used for " + usage;
				result = false;
			}
			else
			{
				T t = JsonSerialize.FromJson<T>(StringUtils.Base64Decode(this.token));
				userId = t.UserId;
				if (t.TokenVersion != 2)
				{
					error = string.Format("invalid token version. Expected: {0}, got: {1}", 2, t.TokenVersion);
					result = false;
				}
				else if (!t.Usage.Equals(usage, StringComparison.Ordinal))
				{
					error = "invalid token usage. Expected: " + usage + ", got: " + t.Usage;
					result = false;
				}
				else if (t.ExpirationTime < DateTime.UtcNow.AddMinutes((double)(-(double)expTimeOffset)))
				{
					error = "expired token used for " + usage;
					result = false;
				}
				else if (t.IssuanceTime > DateTime.UtcNow.AddMinutes(20.0))
				{
					error = "non-issued token used for " + usage;
					result = false;
				}
				else if (t.TestSignature && !CentralServer.TestServer)
				{
					error = "test-only token used for " + usage;
					result = false;
				}
				else
				{
					token = t;
					result = true;
				}
			}
		}
		catch (Exception ex)
		{
			GameCore.Console.AddLog("Failed to TryGetToken<>: " + ex.Message, Color.red, false, ConsoleLogType.Log);
			GameCore.Console.AddLog(ex.StackTrace, Color.red, false, ConsoleLogType.Log);
			throw;
		}
		return result;
	}

	// Token: 0x04000981 RID: 2433
	public readonly string token;

	// Token: 0x04000982 RID: 2434
	public readonly string signature;
}
