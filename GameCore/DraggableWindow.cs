using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCore
{
	// Token: 0x0200085D RID: 2141
	public class DraggableWindow : MonoBehaviour, IDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler
	{
		// Token: 0x060030F5 RID: 12533 RVA: 0x000A4518 File Offset: 0x000A2718
		public void OnBeginDrag(PointerEventData eventData)
		{
			WindowsCursorApi.SetCursor(SystemCursorID.SizeAll);
			this._isDragging = true;
		}

		// Token: 0x060030F6 RID: 12534 RVA: 0x000A452B File Offset: 0x000A272B
		public void OnDrag(PointerEventData eventData)
		{
			this._targetPosition += eventData.delta / this._targetCanvas.scaleFactor;
		}

		// Token: 0x060030F7 RID: 12535 RVA: 0x000A4554 File Offset: 0x000A2754
		public void OnEndDrag(PointerEventData eventData)
		{
			this.HandleEnd();
		}

		// Token: 0x060030F8 RID: 12536 RVA: 0x000A455C File Offset: 0x000A275C
		private void Start()
		{
			this._targetPosition = this._dragWindow.anchoredPosition;
		}

		// Token: 0x060030F9 RID: 12537 RVA: 0x000A4570 File Offset: 0x000A2770
		private void Update()
		{
			Vector2 vector = Vector2.Lerp(this._dragWindow.anchoredPosition, this._targetPosition, Time.deltaTime * this._movementSpeed);
			vector.x = Mathf.Clamp(vector.x, this._horizontalClamp.x, this._horizontalClamp.y);
			vector.y = Mathf.Clamp(vector.y, this._verticalClamp.x, this._verticalClamp.y);
			this._dragWindow.anchoredPosition = vector;
		}

		// Token: 0x060030FA RID: 12538 RVA: 0x000A4554 File Offset: 0x000A2754
		private void OnDisable()
		{
			this.HandleEnd();
		}

		// Token: 0x060030FB RID: 12539 RVA: 0x000A45FC File Offset: 0x000A27FC
		private void HandleEnd()
		{
			if (!this._isDragging)
			{
				return;
			}
			this._isDragging = false;
			WindowsCursorApi.SetCursor(SystemCursorID.Default);
		}

		// Token: 0x040025E4 RID: 9700
		[SerializeField]
		private RectTransform _dragWindow;

		// Token: 0x040025E5 RID: 9701
		[SerializeField]
		private Canvas _targetCanvas;

		// Token: 0x040025E6 RID: 9702
		[SerializeField]
		private float _movementSpeed = 25f;

		// Token: 0x040025E7 RID: 9703
		[SerializeField]
		private Vector2 _verticalClamp;

		// Token: 0x040025E8 RID: 9704
		[SerializeField]
		private Vector2 _horizontalClamp;

		// Token: 0x040025E9 RID: 9705
		private Vector2 _targetPosition;

		// Token: 0x040025EA RID: 9706
		private bool _isDragging;
	}
}
