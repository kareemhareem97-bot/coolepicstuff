using System;
using UnityEngine;

namespace Cmdbinding
{
	// Token: 0x02000781 RID: 1921
	public readonly struct Bind
	{
		// Token: 0x06002D1D RID: 11549 RVA: 0x00098DB0 File Offset: 0x00096FB0
		public Bind(KeyCode key, string command)
		{
			this.Key = key;
			this.Command = command;
		}

		// Token: 0x04002325 RID: 8997
		public readonly KeyCode Key;

		// Token: 0x04002326 RID: 8998
		public readonly string Command;
	}
}
