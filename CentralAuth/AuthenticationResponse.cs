using System;
using Cryptography;
using Mirror;
using NorthwoodLib;
using Org.BouncyCastle.Crypto;

namespace CentralAuth
{
	// Token: 0x02000AA6 RID: 2726
	public struct AuthenticationResponse : NetworkMessage
	{
		// Token: 0x17000DFE RID: 3582
		// (get) Token: 0x06003D6B RID: 15723 RVA: 0x000C9B09 File Offset: 0x000C7D09
		// (set) Token: 0x06003D6C RID: 15724 RVA: 0x000C9B11 File Offset: 0x000C7D11
		public AuthenticationToken AuthToken { readonly get; internal set; }

		// Token: 0x17000DFF RID: 3583
		// (get) Token: 0x06003D6D RID: 15725 RVA: 0x000C9B1A File Offset: 0x000C7D1A
		// (set) Token: 0x06003D6E RID: 15726 RVA: 0x000C9B22 File Offset: 0x000C7D22
		public BadgeToken BadgeToken { readonly get; internal set; }

		// Token: 0x06003D6F RID: 15727 RVA: 0x000C9B2C File Offset: 0x000C7D2C
		internal AuthenticationResponse(SignedToken signedAuthToken, SignedToken signedBadgeToken, string publicKey, string ecdhPublicKey, byte[] ecdhPublicKeySignature, bool doNotTrack, bool hideBadge)
		{
			this.SignedAuthToken = signedAuthToken;
			this.SignedBadgeToken = signedBadgeToken;
			this.PublicKey = ECDSA.PublicKeyFromString(publicKey);
			this.EcdhPublicKey = ecdhPublicKey;
			this.EcdhPublicKeySignature = ecdhPublicKeySignature;
			this.AuthToken = null;
			this.BadgeToken = null;
			this.DoNotTrack = doNotTrack;
			this.HideBadge = hideBadge;
			this.PublicKeyHash = StringUtils.Base64Encode(Sha.HashToString(Sha.Sha256(publicKey)));
		}

		// Token: 0x06003D70 RID: 15728 RVA: 0x000C9B98 File Offset: 0x000C7D98
		internal AuthenticationResponse(SignedToken signedAuthToken, SignedToken signedBadgeToken, AsymmetricKeyParameter publicKey, string ecdhPublicKey, byte[] ecdhPublicKeySignature, bool doNotTrack, bool hideBadge)
		{
			this.SignedAuthToken = signedAuthToken;
			this.SignedBadgeToken = signedBadgeToken;
			this.PublicKey = publicKey;
			this.EcdhPublicKey = ecdhPublicKey;
			this.EcdhPublicKeySignature = ecdhPublicKeySignature;
			this.AuthToken = null;
			this.BadgeToken = null;
			this.DoNotTrack = doNotTrack;
			this.HideBadge = hideBadge;
			this.PublicKeyHash = null;
		}

		// Token: 0x04002CEC RID: 11500
		public readonly SignedToken SignedAuthToken;

		// Token: 0x04002CED RID: 11501
		public readonly SignedToken SignedBadgeToken;

		// Token: 0x04002CF0 RID: 11504
		public readonly AsymmetricKeyParameter PublicKey;

		// Token: 0x04002CF1 RID: 11505
		public readonly string EcdhPublicKey;

		// Token: 0x04002CF2 RID: 11506
		public readonly byte[] EcdhPublicKeySignature;

		// Token: 0x04002CF3 RID: 11507
		public readonly bool DoNotTrack;

		// Token: 0x04002CF4 RID: 11508
		public readonly bool HideBadge;

		// Token: 0x04002CF5 RID: 11509
		public readonly string PublicKeyHash;
	}
}
