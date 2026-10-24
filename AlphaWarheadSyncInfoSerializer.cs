using System;
using Mirror;

// Token: 0x02000045 RID: 69
public static class AlphaWarheadSyncInfoSerializer
{
	// Token: 0x060001C5 RID: 453 RVA: 0x00007C05 File Offset: 0x00005E05
	public static void WriteAlphaWarheadSyncInfo(this NetworkWriter writer, AlphaWarheadSyncInfo value)
	{
		writer.WriteDouble(value.StartTime);
		if (value.StartTime == 0.0)
		{
			return;
		}
		writer.WriteByte(value.ScenarioId);
		writer.WriteByte((byte)value.ScenarioType);
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x00007C40 File Offset: 0x00005E40
	public static AlphaWarheadSyncInfo ReadAlphaWarheadSyncInfo(this NetworkReader reader)
	{
		double num = reader.ReadDouble();
		if (num == 0.0)
		{
			return new AlphaWarheadSyncInfo
			{
				StartTime = 0.0
			};
		}
		byte scenarioId = reader.ReadByte();
		byte scenarioType = reader.ReadByte();
		return new AlphaWarheadSyncInfo
		{
			ScenarioId = scenarioId,
			ScenarioType = (WarheadScenarioType)scenarioType,
			StartTime = num
		};
	}
}
