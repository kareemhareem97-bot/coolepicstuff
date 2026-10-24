using System;

// Token: 0x02000110 RID: 272
[Serializable]
public class BroadcastMessage
{
	// Token: 0x06000516 RID: 1302 RVA: 0x00012FE1 File Offset: 0x000111E1
	public BroadcastMessage(string content, uint t, bool mono)
	{
		this.Text = content;
		this.Time = t;
		this.Truncated = mono;
	}

	// Token: 0x04000453 RID: 1107
	public string Text;

	// Token: 0x04000454 RID: 1108
	public uint Time;

	// Token: 0x04000455 RID: 1109
	public bool Truncated;
}
