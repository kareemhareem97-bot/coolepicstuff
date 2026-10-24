using System;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class ReproProjectSettings : ScriptableObject
{
	// Token: 0x0400004E RID: 78
	public string ProjectName;

	// Token: 0x0400004F RID: 79
	public string ProjectPath;

	// Token: 0x04000050 RID: 80
	public bool OpenProject;

	// Token: 0x04000051 RID: 81
	public int TextureScale;

	// Token: 0x04000052 RID: 82
	public ReproProjectSettings.InputItem[] InputFiles;

	// Token: 0x04000053 RID: 83
	public ReproProjectSettings.InputItem[] ProjectFiles;

	// Token: 0x02000020 RID: 32
	[Serializable]
	public struct InputItem
	{
		// Token: 0x04000054 RID: 84
		public ReproProjectAssetType AssetType;

		// Token: 0x04000055 RID: 85
		public string AssetPath;
	}
}
