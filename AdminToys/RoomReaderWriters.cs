using System;
using MapGeneration;
using Mirror;
using UnityEngine;

namespace AdminToys
{
	// Token: 0x02000ADF RID: 2783
	public static class RoomReaderWriters
	{
		// Token: 0x06003F11 RID: 16145 RVA: 0x000D0F14 File Offset: 0x000CF114
		public static void WriteRoomIdentifier(this NetworkWriter writer, RoomIdentifier room)
		{
			Vector3Int mainCoords = room.MainCoords;
			writer.WriteSByte((sbyte)mainCoords.x);
			writer.WriteSByte((sbyte)mainCoords.y);
			writer.WriteSByte((sbyte)mainCoords.z);
		}

		// Token: 0x06003F12 RID: 16146 RVA: 0x000D0F54 File Offset: 0x000CF154
		public static RoomIdentifier ReadRoomIdentifier(this NetworkReader reader)
		{
			sbyte x = reader.ReadSByte();
			sbyte y = reader.ReadSByte();
			sbyte z = reader.ReadSByte();
			RoomIdentifier result;
			if (!RoomIdentifier.RoomsByCoords.TryGetValue(new Vector3Int((int)x, (int)y, (int)z), out result))
			{
				return null;
			}
			return result;
		}
	}
}
