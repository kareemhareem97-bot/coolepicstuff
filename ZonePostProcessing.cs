using System;
using MapGeneration;
using UnityEngine;
using UnityEngine.Rendering;
using Utils.NonAllocLINQ;

// Token: 0x02000082 RID: 130
public class ZonePostProcessing : MonoBehaviour
{
	// Token: 0x060002BF RID: 703 RVA: 0x0000A723 File Offset: 0x00008923
	private void Start()
	{
		SeedSynchronizer.OnGenerationFinished += this.Initalize;
		if (!SeedSynchronizer.MapGenerated)
		{
			return;
		}
		this.Initalize();
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x0000A744 File Offset: 0x00008944
	private void OnDestroy()
	{
		SeedSynchronizer.OnGenerationFinished -= this.Initalize;
		MainCameraController.OnUpdated -= this.UpdateWeights;
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x0000A768 File Offset: 0x00008968
	private void UpdateWeights()
	{
		Vector3 lastPosition = MainCameraController.LastPosition;
		RoomIdentifier roomIdentifier;
		if (lastPosition.TryGetRoom(out roomIdentifier) && roomIdentifier != this._lastDetectedRoom)
		{
			if (this.CheckWhitelisted(roomIdentifier))
			{
				this._lastWhitelistedRoom = roomIdentifier;
				this._lastBounds = this.GetRoomBounds(roomIdentifier);
				this._lastZone = roomIdentifier.Zone;
			}
			this._lastDetectedRoom = roomIdentifier;
		}
		if (this._lastWhitelistedRoom == roomIdentifier)
		{
			foreach (ZonePostProcessing.ZoneVolumePair zoneVolumePair in this._zoneVols)
			{
				zoneVolumePair.SetWeight((float)((zoneVolumePair.Zone == this._lastZone) ? 1 : 0));
			}
			return;
		}
		FacilityZone facilityZone = FacilityZone.None;
		float num = this._maxTransitionDis * this._maxTransitionDis;
		foreach (ZonePostProcessing.ZoneVolumePair zoneVolumePair2 in this._zoneVols)
		{
			if (zoneVolumePair2.Zone != this._lastZone)
			{
				float num2 = this._boundsPerZone[(int)zoneVolumePair2.Zone].SqrDistance(lastPosition);
				if (num2 < num)
				{
					facilityZone = zoneVolumePair2.Zone;
					num = num2;
				}
			}
		}
		if (facilityZone == FacilityZone.None)
		{
			return;
		}
		float num3 = Mathf.Sqrt(num);
		float num4 = Mathf.Sqrt(this._lastBounds.SqrDistance(lastPosition));
		float num5 = Mathf.Min(this._maxTransitionDis, num3 + num4);
		float num6 = Mathf.Clamp01(num3 / num5);
		foreach (ZonePostProcessing.ZoneVolumePair zoneVolumePair3 in this._zoneVols)
		{
			if (zoneVolumePair3.Zone == this._lastZone)
			{
				zoneVolumePair3.SetWeight(num6);
			}
			else if (zoneVolumePair3.Zone == facilityZone)
			{
				zoneVolumePair3.SetWeight(1f - num6);
			}
			else
			{
				zoneVolumePair3.SetWeight(0f);
			}
		}
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x0000A928 File Offset: 0x00008B28
	private void Initalize()
	{
		if (this._initalized)
		{
			return;
		}
		this._initalized = true;
		this._whitelistedZones = new FacilityZone[this._zoneVols.Length];
		for (int i = 0; i < this._zoneVols.Length; i++)
		{
			this._whitelistedZones[i] = this._zoneVols[i].Zone;
		}
		this.GenerateZoneBounds();
		MainCameraController.OnUpdated += this.UpdateWeights;
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x0000A99C File Offset: 0x00008B9C
	private void GenerateZoneBounds()
	{
		int maxValue = 0;
		this._zoneVols.ForEach(delegate(ZonePostProcessing.ZoneVolumePair x)
		{
			maxValue = Mathf.Max(maxValue, (int)x.Zone);
		});
		int num = maxValue + 1;
		this._boundsSet = new bool[num];
		this._boundsPerZone = new Bounds[num];
		this._boundsSize = Vector3.Scale(RoomIdentifier.GridScale, new Vector3(1f, this._heightMultiplier, 1f));
		RoomIdentifier.AllRoomIdentifiers.ForEach(new Action<RoomIdentifier>(this.AddRoomToBounds));
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x0000AA2C File Offset: 0x00008C2C
	private void AddRoomToBounds(RoomIdentifier room)
	{
		if (!this.CheckWhitelisted(room))
		{
			return;
		}
		int zone = (int)room.Zone;
		Bounds roomBounds = this.GetRoomBounds(room);
		if (this._boundsSet[zone])
		{
			this._boundsPerZone[zone].Encapsulate(roomBounds);
			return;
		}
		this._boundsSet[zone] = true;
		this._boundsPerZone[zone] = roomBounds;
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x0000AA85 File Offset: 0x00008C85
	private bool CheckWhitelisted(RoomIdentifier room)
	{
		return !this._excludedRooms.Contains(room.Name) && this._whitelistedZones.Contains(room.Zone);
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x0000AAAD File Offset: 0x00008CAD
	private Bounds GetRoomBounds(RoomIdentifier room)
	{
		return new Bounds(room.transform.position, this._boundsSize);
	}

	// Token: 0x04000251 RID: 593
	[SerializeField]
	private ZonePostProcessing.ZoneVolumePair[] _zoneVols;

	// Token: 0x04000252 RID: 594
	[SerializeField]
	private RoomName[] _excludedRooms;

	// Token: 0x04000253 RID: 595
	[SerializeField]
	private float _maxTransitionDis;

	// Token: 0x04000254 RID: 596
	[SerializeField]
	private float _heightMultiplier;

	// Token: 0x04000255 RID: 597
	private bool _initalized;

	// Token: 0x04000256 RID: 598
	private bool[] _boundsSet;

	// Token: 0x04000257 RID: 599
	private Bounds[] _boundsPerZone;

	// Token: 0x04000258 RID: 600
	private Bounds _lastBounds;

	// Token: 0x04000259 RID: 601
	private FacilityZone _lastZone;

	// Token: 0x0400025A RID: 602
	private RoomIdentifier _lastDetectedRoom;

	// Token: 0x0400025B RID: 603
	private RoomIdentifier _lastWhitelistedRoom;

	// Token: 0x0400025C RID: 604
	private FacilityZone[] _whitelistedZones;

	// Token: 0x0400025D RID: 605
	private Vector3 _boundsSize;

	// Token: 0x02000083 RID: 131
	[Serializable]
	private struct ZoneVolumePair
	{
		// Token: 0x060002C8 RID: 712 RVA: 0x0000AAC5 File Offset: 0x00008CC5
		public void SetWeight(float weight)
		{
			if (weight > 0f)
			{
				this._volume.enabled = true;
				this._volume.weight = Mathf.Clamp01(weight);
				return;
			}
			this._volume.enabled = false;
		}

		// Token: 0x0400025E RID: 606
		public FacilityZone Zone;

		// Token: 0x0400025F RID: 607
		[SerializeField]
		private Volume _volume;
	}
}
