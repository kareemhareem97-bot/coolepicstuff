using System;
using Decals;
using Mirror;

namespace CommandSystem.Commands.RemoteAdmin.Cleanup
{
	// Token: 0x02000A7C RID: 2684
	public readonly struct DecalCleanupMessage : NetworkMessage
	{
		// Token: 0x06003C9E RID: 15518 RVA: 0x000C79A6 File Offset: 0x000C5BA6
		public void Serialize(NetworkWriter writer)
		{
			writer.WriteByte((byte)this.DecalPoolType);
			writer.WriteInt(this.Amount);
		}

		// Token: 0x06003C9F RID: 15519 RVA: 0x000C79C1 File Offset: 0x000C5BC1
		public DecalCleanupMessage(NetworkReader reader)
		{
			this.DecalPoolType = (DecalPoolType)reader.ReadByte();
			this.Amount = reader.ReadInt();
		}

		// Token: 0x06003CA0 RID: 15520 RVA: 0x000C79DB File Offset: 0x000C5BDB
		public DecalCleanupMessage(DecalPoolType decalPoolType, int amount)
		{
			this.DecalPoolType = decalPoolType;
			this.Amount = amount;
		}

		// Token: 0x04002C70 RID: 11376
		public readonly DecalPoolType DecalPoolType;

		// Token: 0x04002C71 RID: 11377
		public readonly int Amount;
	}
}
