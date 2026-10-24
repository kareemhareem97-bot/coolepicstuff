using System;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;

namespace Cryptography
{
	// Token: 0x0200099B RID: 2459
	public static class ECDH
	{
		// Token: 0x060037AC RID: 14252 RVA: 0x000B47D4 File Offset: 0x000B29D4
		public static AsymmetricCipherKeyPair GenerateKeys(int size = 384)
		{
			ECKeyPairGenerator eckeyPairGenerator = new ECKeyPairGenerator("ECDH");
			KeyGenerationParameters parameters = new KeyGenerationParameters(new SecureRandom(), size);
			eckeyPairGenerator.Init(parameters);
			return eckeyPairGenerator.GenerateKeyPair();
		}

		// Token: 0x060037AD RID: 14253 RVA: 0x000B4803 File Offset: 0x000B2A03
		public static ECDHBasicAgreement Init(AsymmetricCipherKeyPair localKey)
		{
			ECDHBasicAgreement ecdhbasicAgreement = new ECDHBasicAgreement();
			ecdhbasicAgreement.Init(localKey.Private);
			return ecdhbasicAgreement;
		}

		// Token: 0x060037AE RID: 14254 RVA: 0x000B4818 File Offset: 0x000B2A18
		public static byte[] DeriveKey(ECDHBasicAgreement exchange, AsymmetricKeyParameter remoteKey)
		{
			byte[] result;
			using (SHA256 sha = SHA256.Create())
			{
				result = sha.ComputeHash(exchange.CalculateAgreement(remoteKey).ToByteArray());
			}
			return result;
		}
	}
}
