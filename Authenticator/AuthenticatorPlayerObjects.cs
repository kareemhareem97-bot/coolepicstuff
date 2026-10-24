using System;
using System.Collections.Generic;
using Utf8Json;

namespace Authenticator
{
	// Token: 0x020002FA RID: 762
	public readonly struct AuthenticatorPlayerObjects : IEquatable<AuthenticatorPlayerObjects>, IJsonSerializable
	{
		// Token: 0x06001085 RID: 4229 RVA: 0x0003DB87 File Offset: 0x0003BD87
		[SerializationConstructor]
		public AuthenticatorPlayerObjects(List<AuthenticatorPlayerObject> objects)
		{
			this.objects = objects;
		}

		// Token: 0x06001086 RID: 4230 RVA: 0x0003DB90 File Offset: 0x0003BD90
		public bool Equals(AuthenticatorPlayerObjects other)
		{
			return this.objects == other.objects;
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x0003DBA0 File Offset: 0x0003BDA0
		public override bool Equals(object obj)
		{
			if (obj is AuthenticatorPlayerObjects)
			{
				AuthenticatorPlayerObjects other = (AuthenticatorPlayerObjects)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x0003DBC5 File Offset: 0x0003BDC5
		public override int GetHashCode()
		{
			if (this.objects == null)
			{
				return 0;
			}
			return this.objects.GetHashCode();
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x0003DBDC File Offset: 0x0003BDDC
		public static bool operator ==(AuthenticatorPlayerObjects left, AuthenticatorPlayerObjects right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600108A RID: 4234 RVA: 0x0003DBE6 File Offset: 0x0003BDE6
		public static bool operator !=(AuthenticatorPlayerObjects left, AuthenticatorPlayerObjects right)
		{
			return !left.Equals(right);
		}

		// Token: 0x04000CE9 RID: 3305
		public readonly List<AuthenticatorPlayerObject> objects;
	}
}
