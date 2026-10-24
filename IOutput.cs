using System;
using UnityEngine;

// Token: 0x020001AF RID: 431
public interface IOutput
{
	// Token: 0x17000150 RID: 336
	// (get) Token: 0x0600086F RID: 2159
	string OutputId { get; }

	// Token: 0x06000870 RID: 2160
	void Print(string text);

	// Token: 0x06000871 RID: 2161
	void Print(string text, ConsoleColor c);

	// Token: 0x06000872 RID: 2162
	void Print(string text, ConsoleColor c, Color rgbColor);

	// Token: 0x06000873 RID: 2163
	bool Available();
}
