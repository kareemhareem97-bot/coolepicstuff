using System;
using System.Buffers;
using System.IO;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Cryptography
{
	// Token: 0x0200099F RID: 2463
	public static class RSA
	{
		// Token: 0x060037BE RID: 14270 RVA: 0x000B4BF4 File Offset: 0x000B2DF4
		public static bool Verify(string data, string signature, string key)
		{
			bool result;
			using (TextReader textReader = new StringReader(key))
			{
				AsymmetricKeyParameter parameters = (AsymmetricKeyParameter)new PemReader(textReader).ReadObject();
				ISigner signer = SignerUtilities.GetSigner("SHA256withRSA");
				signer.Init(false, parameters);
				byte[] signature2 = Convert.FromBase64String(signature);
				byte[] array = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(data.Length));
				int bytes = Utf8.GetBytes(data, array);
				signer.BlockUpdate(array, 0, bytes);
				ArrayPool<byte>.Shared.Return(array, false);
				result = signer.VerifySignature(signature2);
			}
			return result;
		}
	}
}
