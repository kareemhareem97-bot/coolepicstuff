using System;
using Utf8Json;

namespace Authenticator
{
	// Token: 0x020002F9 RID: 761
	public readonly struct AuthenticatorPlayerObject : IEquatable<AuthenticatorPlayerObject>, IJsonSerializable
	{
		// Token: 0x0600107F RID: 4223 RVA: 0x0003D9E4 File Offset: 0x0003BBE4
		[SerializationConstructor]
		public AuthenticatorPlayerObject(string Id, string Ip, string RequestIp, string Asn, string AuthSerial, string VacSession)
		{
			this.Id = Id;
			this.Ip = Ip;
			this.RequestIp = RequestIp;
			this.Asn = Asn;
			this.AuthSerial = AuthSerial;
			this.VacSession = VacSession;
		}

		// Token: 0x06001080 RID: 4224 RVA: 0x0003DA14 File Offset: 0x0003BC14
		public bool Equals(AuthenticatorPlayerObject other)
		{
			return this.Id == other.Id && this.Ip == other.Ip && this.RequestIp == other.RequestIp && this.Asn == other.Asn && this.AuthSerial == other.AuthSerial && this.VacSession == other.VacSession;
		}

		// Token: 0x06001081 RID: 4225 RVA: 0x0003DA94 File Offset: 0x0003BC94
		public override bool Equals(object obj)
		{
			if (obj is AuthenticatorPlayerObject)
			{
				AuthenticatorPlayerObject other = (AuthenticatorPlayerObject)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x0003DABC File Offset: 0x0003BCBC
		public override int GetHashCode()
		{
			return ((((((this.Id != null) ? this.Id.GetHashCode() : 0) * 397 ^ ((this.Ip != null) ? this.Ip.GetHashCode() : 0)) * 397 ^ ((this.RequestIp != null) ? this.RequestIp.GetHashCode() : 0)) * 397 ^ ((this.Asn != null) ? this.Asn.GetHashCode() : 0)) * 397 ^ ((this.AuthSerial != null) ? this.AuthSerial.GetHashCode() : 0)) * 397 ^ ((this.VacSession != null) ? this.VacSession.GetHashCode() : 0);
		}

		// Token: 0x06001083 RID: 4227 RVA: 0x0003DB70 File Offset: 0x0003BD70
		public static bool operator ==(AuthenticatorPlayerObject left, AuthenticatorPlayerObject right)
		{
			return left.Equals(right);
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x0003DB7A File Offset: 0x0003BD7A
		public static bool operator !=(AuthenticatorPlayerObject left, AuthenticatorPlayerObject right)
		{
			return !left.Equals(right);
		}

		// Token: 0x04000CE3 RID: 3299
		public readonly string Id;

		// Token: 0x04000CE4 RID: 3300
		public readonly string Ip;

		// Token: 0x04000CE5 RID: 3301
		public readonly string RequestIp;

		// Token: 0x04000CE6 RID: 3302
		public readonly string Asn;

		// Token: 0x04000CE7 RID: 3303
		public readonly string AuthSerial;

		// Token: 0x04000CE8 RID: 3304
		public readonly string VacSession;
	}
}
