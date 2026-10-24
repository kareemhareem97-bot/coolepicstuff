using System;
using System.Collections.Generic;
using Interactables.Interobjects;
using MapGeneration;
using UnityEngine;
using Utils.NonAllocLINQ;

// Token: 0x02000050 RID: 80
public class ConnectorLight : MonoBehaviour
{
	// Token: 0x060001F1 RID: 497 RVA: 0x00008401 File Offset: 0x00006601
	private void Awake()
	{
		this._roomConnector = base.GetComponent<IRoomConnector>();
		if (this._roomConnector.RoomsAlreadyRegistered)
		{
			this.FindControllers();
			return;
		}
		this._roomConnector.OnRoomsRegistered += this.FindControllers;
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0000843C File Offset: 0x0000663C
	private void FindControllers()
	{
		RoomIdentifier[] rooms = this._roomConnector.Rooms;
		foreach (RoomLightController roomLightController in RoomLightController.Instances)
		{
			if (rooms.Contains(roomLightController.Room))
			{
				this._lightControllers.Add(roomLightController);
				roomLightController.OnLightsSet += this.OnAnyUpdated;
			}
		}
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x000084C0 File Offset: 0x000066C0
	private void OnAnyUpdated(bool _)
	{
		bool active = this._lightControllers.Any((RoomLightController x) => x.LightsEnabled);
		foreach (GameObject gameObject in this._objectsToDisable)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(active);
			}
		}
	}

	// Token: 0x040001A7 RID: 423
	[SerializeField]
	private GameObject[] _objectsToDisable;

	// Token: 0x040001A8 RID: 424
	private IRoomConnector _roomConnector;

	// Token: 0x040001A9 RID: 425
	private readonly List<RoomLightController> _lightControllers = new List<RoomLightController>();
}
