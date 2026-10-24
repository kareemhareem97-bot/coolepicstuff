using System;
using MapGeneration.Holidays;
using Steam;

namespace GameCore
{
	// Token: 0x02000862 RID: 2146
	public static class Version
	{
		// Token: 0x06003119 RID: 12569 RVA: 0x000A4D48 File Offset: 0x000A2F48
		static Version()
		{
			SteamServerInfo.Version = Version.VersionString;
		}

		// Token: 0x1700090C RID: 2316
		// (get) Token: 0x0600311A RID: 12570 RVA: 0x000A4DFB File Offset: 0x000A2FFB
		public static bool PublicBeta
		{
			get
			{
				return Version.BuildType == Version.VersionType.PublicBeta || Version.BuildType == Version.VersionType.PublicRC;
			}
		}

		// Token: 0x1700090D RID: 2317
		// (get) Token: 0x0600311B RID: 12571 RVA: 0x000A4E10 File Offset: 0x000A3010
		public static bool PrivateBeta
		{
			get
			{
				Version.VersionType buildType = Version.BuildType;
				return buildType == Version.VersionType.PrivateBeta || buildType == Version.VersionType.PrivateBetaStreamingForbidden || buildType == Version.VersionType.PrivateRC || buildType == Version.VersionType.PrivateRCStreamingForbidden || buildType == Version.VersionType.Development || buildType == Version.VersionType.Nightly;
			}
		}

		// Token: 0x1700090E RID: 2318
		// (get) Token: 0x0600311C RID: 12572 RVA: 0x000A4E44 File Offset: 0x000A3044
		public static bool ReleaseCandidate
		{
			get
			{
				Version.VersionType buildType = Version.BuildType;
				return buildType == Version.VersionType.PublicRC || buildType == Version.VersionType.PrivateRC || buildType == Version.VersionType.PrivateRCStreamingForbidden;
			}
		}

		// Token: 0x1700090F RID: 2319
		// (get) Token: 0x0600311D RID: 12573 RVA: 0x000A4E6C File Offset: 0x000A306C
		public static bool StreamingAllowed
		{
			get
			{
				Version.VersionType buildType = Version.BuildType;
				return buildType != Version.VersionType.PrivateBetaStreamingForbidden && buildType != Version.VersionType.PrivateRCStreamingForbidden && buildType != Version.VersionType.Development && buildType != Version.VersionType.Nightly;
			}
		}

		// Token: 0x17000910 RID: 2320
		// (get) Token: 0x0600311E RID: 12574 RVA: 0x000A4E94 File Offset: 0x000A3094
		public static bool ExtendedVersionCheckNeeded
		{
			get
			{
				return Version.BuildType > Version.VersionType.Release;
			}
		}

		// Token: 0x0600311F RID: 12575 RVA: 0x000A4E9E File Offset: 0x000A309E
		public static bool CompatibilityCheck(byte sMajor, byte sMinor, byte sRevision, byte cMajor, byte cMinor, byte cRevision, bool cBackwardEnabled, byte cBackwardRevision)
		{
			if (sMajor != cMajor || sMinor != cMinor)
			{
				return false;
			}
			if (!cBackwardEnabled)
			{
				return sRevision == cRevision;
			}
			return sRevision >= cBackwardRevision && sRevision <= cRevision;
		}

		// Token: 0x04002616 RID: 9750
		public static readonly byte Major = 14;

		// Token: 0x04002617 RID: 9751
		public static readonly byte Minor = 2;

		// Token: 0x04002618 RID: 9752
		public static readonly byte Revision = 8;

		// Token: 0x04002619 RID: 9753
		public static readonly bool AlwaysAcceptReleaseBuilds = false;

		// Token: 0x0400261A RID: 9754
		public static readonly Version.VersionType BuildType = Version.VersionType.Release;

		// Token: 0x0400261B RID: 9755
		public static readonly HolidayType ActiveHoliday = HolidayType.Halloween;

		// Token: 0x0400261C RID: 9756
		public static readonly bool BackwardCompatibility = false;

		// Token: 0x0400261D RID: 9757
		public static readonly byte BackwardRevision = 0;

		// Token: 0x0400261E RID: 9758
		public static readonly string DescriptionOverride = null;

		// Token: 0x0400261F RID: 9759
		public static readonly string VersionString = string.Format("{0}.{1}.{2}{3}", new object[]
		{
			Version.Major,
			Version.Minor,
			Version.Revision,
			(!Version.ExtendedVersionCheckNeeded) ? string.Empty : ("-" + (Version.DescriptionOverride ?? "release-halloween-963b276a"))
		});

		// Token: 0x02000863 RID: 2147
		public enum VersionType : byte
		{
			// Token: 0x04002621 RID: 9761
			Release,
			// Token: 0x04002622 RID: 9762
			PublicRC,
			// Token: 0x04002623 RID: 9763
			PublicBeta,
			// Token: 0x04002624 RID: 9764
			PrivateRC,
			// Token: 0x04002625 RID: 9765
			PrivateRCStreamingForbidden,
			// Token: 0x04002626 RID: 9766
			PrivateBeta,
			// Token: 0x04002627 RID: 9767
			PrivateBetaStreamingForbidden,
			// Token: 0x04002628 RID: 9768
			Development,
			// Token: 0x04002629 RID: 9769
			Nightly
		}
	}
}
