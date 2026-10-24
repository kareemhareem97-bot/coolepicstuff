using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000E3 RID: 227
public class PlayButton : MonoBehaviour
{
	// Token: 0x06000450 RID: 1104 RVA: 0x00010800 File Offset: 0x0000EA00
	private void Start()
	{
		if (SceneManager.GetActiveScene().name == "Facility")
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
