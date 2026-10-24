using System;
using System.Collections.Generic;
using UnityEngine;

namespace FastResources
{
	// Token: 0x0200086A RID: 2154
	[CreateAssetMenu(fileName = "Fast Resource Manifest", menuName = "ScriptableObject/Fast Resource Manifest")]
	public class FastResourceManifest : ScriptableObject
	{
		// Token: 0x04002646 RID: 9798
		[HideInInspector]
		public string TypeName;

		// Token: 0x04002647 RID: 9799
		[HideInInspector]
		public bool SearchInChildren;

		// Token: 0x04002648 RID: 9800
		[HideInInspector]
		public bool FetchAllComponentsPerObject;

		// Token: 0x04002649 RID: 9801
		[HideInInspector]
		public bool IncludeSubfolders;

		// Token: 0x0400264A RID: 9802
		public List<UnityEngine.Object> SpecificObjects;
	}
}
