using System;
using System.Linq;
using UnityEngine;

// Token: 0x020000B2 RID: 178
public class CreditsEntry
{
	// Token: 0x0600039A RID: 922 RVA: 0x00003370 File Offset: 0x00001570
	public CreditsEntry()
	{
	}

	// Token: 0x0600039B RID: 923 RVA: 0x0000E85B File Offset: 0x0000CA5B
	public CreditsEntry(string title, string name)
	{
		this.Multi = false;
		this.Title = title;
		this.Name = name;
		this.Color = Color.white;
	}

	// Token: 0x0600039C RID: 924 RVA: 0x0000E883 File Offset: 0x0000CA83
	public CreditsEntry(string title, string name, Color color)
	{
		this.Multi = false;
		this.Title = title;
		this.Name = name;
		this.Color = color;
	}

	// Token: 0x0600039D RID: 925 RVA: 0x0000E8A7 File Offset: 0x0000CAA7
	public CreditsEntry(string name)
	{
		this.Multi = false;
		this.Title = "";
		this.Name = name;
	}

	// Token: 0x0600039E RID: 926 RVA: 0x0000E8C8 File Offset: 0x0000CAC8
	public CreditsEntry(string[] names)
	{
		this.Multi = true;
		this.Title = "";
		this.Name = names.Aggregate("", (string current, string n) => current + n + "\n");
	}

	// Token: 0x0400033E RID: 830
	public string Title;

	// Token: 0x0400033F RID: 831
	public string Name;

	// Token: 0x04000340 RID: 832
	public bool Multi;

	// Token: 0x04000341 RID: 833
	public Color Color;
}
