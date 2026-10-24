using System;
using UnityEngine;

// Token: 0x02000117 RID: 279
public class DetectorController : MonoBehaviour
{
	// Token: 0x06000558 RID: 1368 RVA: 0x00013BB6 File Offset: 0x00011DB6
	private void Start()
	{
		base.InvokeRepeating("RefreshDetectorsList", 10f, 10f);
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x00013BCD File Offset: 0x00011DCD
	public void RefreshDetectorsList()
	{
		this.detectors = GameObject.FindGameObjectsWithTag("Detector");
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x00013BE0 File Offset: 0x00011DE0
	private void Update()
	{
		if (this.detectors.Length == 0)
		{
			return;
		}
		bool flag = false;
		foreach (GameObject gameObject in this.detectors)
		{
			if (Vector3.Distance(gameObject.transform.position, base.transform.position) > this.viewRange)
			{
				Vector3 normalized = (base.transform.position - gameObject.transform.position).normalized;
				RaycastHit raycastHit;
				if (Vector3.Dot(gameObject.transform.forward, normalized) < this.fov && Physics.Raycast(gameObject.transform.position, normalized, out raycastHit) && raycastHit.transform.CompareTag("Detector"))
				{
					flag = true;
					break;
				}
			}
		}
		this.detectionProgress += Time.deltaTime * (flag ? 0.3f : -0.5f);
		this.detectionProgress = Mathf.Clamp01(this.detectionProgress);
	}

	// Token: 0x04000478 RID: 1144
	public float detectionProgress;

	// Token: 0x04000479 RID: 1145
	public float viewRange = 30f;

	// Token: 0x0400047A RID: 1146
	public float fov = -0.75f;

	// Token: 0x0400047B RID: 1147
	public GameObject[] detectors;
}
