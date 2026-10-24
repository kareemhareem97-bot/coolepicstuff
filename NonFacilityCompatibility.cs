using System;
using System.Runtime.CompilerServices;
using PlayerRoles;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Token: 0x0200009E RID: 158
public class NonFacilityCompatibility : MonoBehaviour
{
	// Token: 0x06000360 RID: 864 RVA: 0x0000CC76 File Offset: 0x0000AE76
	private void Awake()
	{
		NonFacilityCompatibility.singleton = this;
		UnityAction<Scene, LoadSceneMode> value;
		if ((value = NonFacilityCompatibility.<>O.<0>__RefreshDescription) == null)
		{
			value = (NonFacilityCompatibility.<>O.<0>__RefreshDescription = new UnityAction<Scene, LoadSceneMode>(NonFacilityCompatibility.RefreshDescription));
		}
		SceneManager.sceneLoaded += value;
	}

	// Token: 0x06000361 RID: 865 RVA: 0x0000CC9E File Offset: 0x0000AE9E
	private void OnDestroy()
	{
		UnityAction<Scene, LoadSceneMode> value;
		if ((value = NonFacilityCompatibility.<>O.<0>__RefreshDescription) == null)
		{
			value = (NonFacilityCompatibility.<>O.<0>__RefreshDescription = new UnityAction<Scene, LoadSceneMode>(NonFacilityCompatibility.RefreshDescription));
		}
		SceneManager.sceneLoaded -= value;
	}

	// Token: 0x06000362 RID: 866 RVA: 0x0000CCC0 File Offset: 0x0000AEC0
	public static void RefreshDescription(Scene scene, LoadSceneMode mode)
	{
		foreach (NonFacilityCompatibility.SceneDescription sceneDescription in NonFacilityCompatibility.singleton.allScenes)
		{
			if (sceneDescription.sceneName == scene.name)
			{
				NonFacilityCompatibility.currentSceneSettings = sceneDescription;
			}
		}
	}

	// Token: 0x040002EF RID: 751
	public NonFacilityCompatibility.SceneDescription[] allScenes;

	// Token: 0x040002F0 RID: 752
	public static NonFacilityCompatibility singleton;

	// Token: 0x040002F1 RID: 753
	public static NonFacilityCompatibility.SceneDescription currentSceneSettings;

	// Token: 0x0200009F RID: 159
	[Serializable]
	public class SceneDescription
	{
		// Token: 0x040002F2 RID: 754
		public string sceneName;

		// Token: 0x040002F3 RID: 755
		public NonFacilityCompatibility.SceneDescription.VoiceChatSupportMode voiceChatSupport = NonFacilityCompatibility.SceneDescription.VoiceChatSupportMode.FullySupported;

		// Token: 0x040002F4 RID: 756
		public bool enableWorldGeneration = true;

		// Token: 0x040002F5 RID: 757
		public bool enableRespawning = true;

		// Token: 0x040002F6 RID: 758
		public bool enableStandardGamplayItems = true;

		// Token: 0x040002F7 RID: 759
		public bool roundAutostart;

		// Token: 0x040002F8 RID: 760
		public Vector3 constantRespawnPoint = Vector3.zero;

		// Token: 0x040002F9 RID: 761
		public RoleTypeId forcedClass = RoleTypeId.None;

		// Token: 0x020000A0 RID: 160
		public enum VoiceChatSupportMode
		{
			// Token: 0x040002FB RID: 763
			Unsupported,
			// Token: 0x040002FC RID: 764
			WithoutIntercom,
			// Token: 0x040002FD RID: 765
			FullySupported
		}
	}

	// Token: 0x020000A1 RID: 161
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x040002FE RID: 766
		public static UnityAction<Scene, LoadSceneMode> <0>__RefreshDescription;
	}
}
