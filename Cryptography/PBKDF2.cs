using System;
using System.Security.Cryptography;

namespace Cryptography
{
	// Token: 0x0200099E RID: 2462
	public static class PBKDF2
	{
		// Token: 0x060037BC RID: 14268 RVA: 0x000B4BA0 File Offset: 0x000B2DA0
		public static string Pbkdf2HashString(string password, byte[] salt, int iterations, int outputBytes)
		{
			return Convert.ToBase64String(PBKDF2.Pbkdf2HashBytes(password, salt, iterations, outputBytes));
		}

		// Token: 0x060037BD RID: 14269 RVA: 0x000B4BB0 File Offset: 0x000B2DB0
		public static byte[] Pbkdf2HashBytes(string password, byte[] salt, int iterations, int outputBytes)
		{
			byte[] bytes;
			using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt)
			{
				IterationCount = iterations
			})
			{
				bytes = rfc2898DeriveBytes.GetBytes(outputBytes);
			}
			return bytes;
		}
	}
}
