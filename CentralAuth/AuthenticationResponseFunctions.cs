using System;
using Cryptography;
using Mirror;

namespace CentralAuth
{
	// Token: 0x02000AA7 RID: 2727
	public static class AuthenticationResponseFunctions
	{
		// Token: 0x06003D71 RID: 15729 RVA: 0x000C9BF0 File Offset: 0x000C7DF0
		public static void SerializeAuthenticationResponse(this NetworkWriter writer, AuthenticationResponse value)
		{
			AuthenticationResponseFunctions.AuthenticationResponseFlags authenticationResponseFlags = (AuthenticationResponseFunctions.AuthenticationResponseFlags)0;
			if (value.SignedAuthToken != null)
			{
				authenticationResponseFlags |= AuthenticationResponseFunctions.AuthenticationResponseFlags.AuthToken;
			}
			if (value.SignedBadgeToken != null)
			{
				authenticationResponseFlags |= AuthenticationResponseFunctions.AuthenticationResponseFlags.BadgeToken;
			}
			if (value.DoNotTrack)
			{
				authenticationResponseFlags |= AuthenticationResponseFunctions.AuthenticationResponseFlags.DoNotTrack;
			}
			if (value.HideBadge)
			{
				authenticationResponseFlags |= AuthenticationResponseFunctions.AuthenticationResponseFlags.HideBadge;
			}
			writer.WriteByte((byte)authenticationResponseFlags);
			SignedToken signedAuthToken = value.SignedAuthToken;
			if (signedAuthToken != null)
			{
				signedAuthToken.Serialize(writer);
			}
			SignedToken signedBadgeToken = value.SignedBadgeToken;
			if (signedBadgeToken != null)
			{
				signedBadgeToken.Serialize(writer);
			}
			writer.WriteString(ECDSA.KeyToString(value.PublicKey));
			writer.WriteString(value.EcdhPublicKey);
			writer.WriteArray(value.EcdhPublicKeySignature);
		}

		// Token: 0x06003D72 RID: 15730 RVA: 0x000C9C84 File Offset: 0x000C7E84
		public static AuthenticationResponse DeserializeAuthenticationResponse(this NetworkReader reader)
		{
			AuthenticationResponseFunctions.AuthenticationResponseFlags flags = (AuthenticationResponseFunctions.AuthenticationResponseFlags)reader.ReadByte();
			return new AuthenticationResponse(flags.HasFlagFast(AuthenticationResponseFunctions.AuthenticationResponseFlags.AuthToken) ? SignedToken.Deserialize(reader) : null, flags.HasFlagFast(AuthenticationResponseFunctions.AuthenticationResponseFlags.BadgeToken) ? SignedToken.Deserialize(reader) : null, reader.ReadString(), reader.ReadString(), reader.ReadArray<byte>(), flags.HasFlagFast(AuthenticationResponseFunctions.AuthenticationResponseFlags.DoNotTrack), flags.HasFlagFast(AuthenticationResponseFunctions.AuthenticationResponseFlags.HideBadge));
		}

		// Token: 0x06003D73 RID: 15731 RVA: 0x0001C83C File Offset: 0x0001AA3C
		private static bool HasFlagFast(this AuthenticationResponseFunctions.AuthenticationResponseFlags flags, AuthenticationResponseFunctions.AuthenticationResponseFlags flag)
		{
			return (flags & flag) == flag;
		}

		// Token: 0x02000AA8 RID: 2728
		[Flags]
		private enum AuthenticationResponseFlags : byte
		{
			// Token: 0x04002CF7 RID: 11511
			AuthToken = 1,
			// Token: 0x04002CF8 RID: 11512
			BadgeToken = 2,
			// Token: 0x04002CF9 RID: 11513
			DoNotTrack = 4,
			// Token: 0x04002CFA RID: 11514
			HideBadge = 8
		}
	}
}
