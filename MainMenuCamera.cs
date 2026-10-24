using System;
using UnityEngine;

// Token: 0x020000CF RID: 207
public class MainMenuCamera : MonoBehaviour
{
	// Token: 0x06000404 RID: 1028 RVA: 0x0000FBDC File Offset: 0x0000DDDC
	private void Update()
	{
		float num = (float)Screen.width * (this.borderWidthPercent / 100f);
		Vector3 vector = Vector3.zero;
		Vector3 mousePosition = Input.mousePosition;
		if (mousePosition.x < num && base.transform.localRotation.eulerAngles.y > 41f)
		{
			vector += Vector3.down;
		}
		if (mousePosition.x > (float)Screen.width - num && base.transform.localRotation.eulerAngles.y < 74f)
		{
			vector += Vector3.up;
		}
		if (vector == Vector3.zero)
		{
			this.rotSpeed = 0f;
		}
		else
		{
			this.rotSpeed += Time.deltaTime * 200f;
			this.rotSpeed = Mathf.Clamp(this.rotSpeed, 0f, 120f);
		}
		vector.Normalize();
		base.transform.localRotation = Quaternion.Euler(base.transform.localRotation.eulerAngles + Time.deltaTime * this.rotSpeed * vector);
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			this.Raycast();
		}
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x0000FD18 File Offset: 0x0000DF18
	private void Raycast()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(base.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out raycastHit))
		{
			this.ElementChoosen(raycastHit.transform.name);
		}
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x0000FD50 File Offset: 0x0000DF50
	public void ElementChoosen(string id)
	{
		if (id == "EXIT")
		{
			Debug.Log("Application closed by the user.");
			Shutdown.Quit(true, false);
			return;
		}
		if (!(id == "PLAY"))
		{
			return;
		}
		UnityEngine.Object.FindObjectOfType<NetManagerValueSetter>().HostGame();
	}

	// Token: 0x040003A1 RID: 929
	public float borderWidthPercent;

	// Token: 0x040003A2 RID: 930
	private float rotSpeed;
}
