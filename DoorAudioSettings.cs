using System;
using UnityEngine;

// Token: 0x0200012C RID: 300
[CreateAssetMenu(fileName = "New Door Audio Settings", menuName = "ScriptableObject/Doors/DoorAudioSettings")]
public class DoorAudioSettings : ScriptableObject
{
	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x060005B2 RID: 1458 RVA: 0x00015205 File Offset: 0x00013405
	public AudioClip RandomOpeningSound
	{
		get
		{
			return this.DoorOpeningSound[DoorAudioSettings.Rng.Next(this.DoorOpeningSound.Length)];
		}
	}

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x060005B3 RID: 1459 RVA: 0x00015220 File Offset: 0x00013420
	public AudioClip RandomClosingSound
	{
		get
		{
			return this.DoorClosingSound[DoorAudioSettings.Rng.Next(this.DoorClosingSound.Length)];
		}
	}

	// Token: 0x040004F1 RID: 1265
	private static readonly System.Random Rng = Misc.CreateRandom();

	// Token: 0x040004F2 RID: 1266
	public AudioClip[] DoorOpeningSound;

	// Token: 0x040004F3 RID: 1267
	public AudioClip[] DoorClosingSound;

	// Token: 0x040004F4 RID: 1268
	public AudioClip AccessDenied;

	// Token: 0x040004F5 RID: 1269
	public AudioClip AccessGranted;
}
