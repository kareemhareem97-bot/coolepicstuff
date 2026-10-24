using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCore
{
	// Token: 0x0200085E RID: 2142
	public class ResizableWindow : MonoBehaviour, IDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler
	{
		// Token: 0x140000A6 RID: 166
		// (add) Token: 0x060030FD RID: 12541 RVA: 0x000A462C File Offset: 0x000A282C
		// (remove) Token: 0x060030FE RID: 12542 RVA: 0x000A4664 File Offset: 0x000A2864
		public event Action<Vector2> OnResizingEnded;

		// Token: 0x17000908 RID: 2312
		// (get) Token: 0x060030FF RID: 12543 RVA: 0x000A4699 File Offset: 0x000A2899
		public Vector2 CurrentSize
		{
			get
			{
				return this._panelRectTransform.sizeDelta;
			}
		}

		// Token: 0x06003100 RID: 12544 RVA: 0x000A46A8 File Offset: 0x000A28A8
		public void SetScaleAndResetPosition(Vector2 scale)
		{
			scale.x = Mathf.Clamp(scale.x, this._minSize.x, this._maxSize.x);
			scale.y = Mathf.Clamp(scale.y, this._minSize.y, this._maxSize.y);
			this._targetSize = scale;
		}

		// Token: 0x06003101 RID: 12545 RVA: 0x000A470C File Offset: 0x000A290C
		public void OnDrag(PointerEventData eventData)
		{
			if (this._resizeDirection == ResizableWindow.ResizeDirection.None)
			{
				return;
			}
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this._panelRectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out vector);
			Vector2 originalSizeDelta = this._originalSizeDelta;
			Vector2 originalAnchoredPosition = this._originalAnchoredPosition;
			switch (this._resizeDirection)
			{
			case ResizableWindow.ResizeDirection.Left:
			{
				float num = this._originalAnchoredPosition.x + this._originalSizeDelta.x * 0.5f;
				float num2 = Mathf.Clamp(num - vector.x, this._minSize.x, this._maxSize.x);
				originalSizeDelta.x = num2;
				originalAnchoredPosition.x = num - num2 * 0.5f;
				break;
			}
			case ResizableWindow.ResizeDirection.Right:
			{
				float num3 = this._originalAnchoredPosition.x - this._originalSizeDelta.x * 0.5f;
				float num4 = Mathf.Clamp(vector.x - num3, this._minSize.x, this._maxSize.x);
				originalSizeDelta.x = num4;
				originalAnchoredPosition.x = num3 + num4 * 0.5f;
				break;
			}
			case ResizableWindow.ResizeDirection.Top:
			{
				float num5 = this._originalAnchoredPosition.y - this._originalSizeDelta.y * 0.5f;
				float num6 = Mathf.Clamp(vector.y - num5, this._minSize.y, this._maxSize.y);
				originalSizeDelta.y = num6;
				originalAnchoredPosition.y = num5 + num6 * 0.5f;
				break;
			}
			case ResizableWindow.ResizeDirection.Bottom:
			{
				float num7 = this._originalAnchoredPosition.y + this._originalSizeDelta.y * 0.5f;
				float num8 = Mathf.Clamp(num7 - vector.y, this._minSize.y, this._maxSize.y);
				originalSizeDelta.y = num8;
				originalAnchoredPosition.y = num7 - num8 * 0.5f;
				break;
			}
			}
			this._targetSize = originalSizeDelta;
			this._targetAnchoredPosition = originalAnchoredPosition;
		}

		// Token: 0x06003102 RID: 12546 RVA: 0x000A4914 File Offset: 0x000A2B14
		public void OnBeginDrag(PointerEventData eventData)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this._panelRectTransform, eventData.position, eventData.pressEventCamera, out this._originalLocalPointerPosition);
			this._originalSizeDelta = this._panelRectTransform.sizeDelta;
			this._originalAnchoredPosition = this._panelRectTransform.anchoredPosition;
			this._resizeDirection = this.DetectResizeDirection(this._originalLocalPointerPosition);
			WindowsCursorApi.SetCursor(this.GetCursorFromDirection(this._resizeDirection));
			if (this._resizeDirection != ResizableWindow.ResizeDirection.None)
			{
				this._isResizing = true;
			}
		}

		// Token: 0x06003103 RID: 12547 RVA: 0x000A4994 File Offset: 0x000A2B94
		private void Update()
		{
			this._panelRectTransform.anchoredPosition = Vector2.Lerp(this._panelRectTransform.anchoredPosition, this._targetAnchoredPosition, Time.deltaTime * this._scaleSpeed);
			this._panelRectTransform.sizeDelta = Vector2.Lerp(this._panelRectTransform.sizeDelta, this._targetSize, Time.deltaTime * this._scaleSpeed);
			this.EdgeHoverDetection();
		}

		// Token: 0x06003104 RID: 12548 RVA: 0x000A4A04 File Offset: 0x000A2C04
		private void EdgeHoverDetection()
		{
			if (this._isResizing)
			{
				return;
			}
			Vector2 localPointer;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this._panelRectTransform, Input.mousePosition, null, out localPointer);
			ResizableWindow.ResizeDirection resizeDirection = this.DetectResizeDirection(localPointer);
			if (!this._isHovering && resizeDirection != ResizableWindow.ResizeDirection.None)
			{
				this._isHovering = true;
				WindowsCursorApi.SetCursor(this.GetCursorFromDirection(resizeDirection));
				return;
			}
			if (this._isHovering && resizeDirection == ResizableWindow.ResizeDirection.None)
			{
				this._isHovering = false;
				WindowsCursorApi.SetCursor(SystemCursorID.Default);
			}
		}

		// Token: 0x06003105 RID: 12549 RVA: 0x000A4A76 File Offset: 0x000A2C76
		private void OnDisable()
		{
			this.HandleEnd();
		}

		// Token: 0x06003106 RID: 12550 RVA: 0x000A4A76 File Offset: 0x000A2C76
		public void OnEndDrag(PointerEventData eventData)
		{
			this.HandleEnd();
		}

		// Token: 0x06003107 RID: 12551 RVA: 0x000A4A80 File Offset: 0x000A2C80
		private void HandleEnd()
		{
			if (this._isResizing)
			{
				Action<Vector2> onResizingEnded = this.OnResizingEnded;
				if (onResizingEnded != null)
				{
					onResizingEnded(this._targetSize);
				}
			}
			if (this._isResizing || this._isHovering)
			{
				this._isResizing = false;
				this._isHovering = false;
				WindowsCursorApi.SetCursor(SystemCursorID.Default);
			}
		}

		// Token: 0x06003108 RID: 12552 RVA: 0x000A4AD4 File Offset: 0x000A2CD4
		private ResizableWindow.ResizeDirection DetectResizeDirection(Vector2 localPointer)
		{
			Rect rect = this._panelRectTransform.rect;
			if (Mathf.Abs(localPointer.x - rect.xMin) <= this._edgeDetectSize)
			{
				return ResizableWindow.ResizeDirection.Left;
			}
			if (Mathf.Abs(localPointer.x - rect.xMax) <= this._edgeDetectSize)
			{
				return ResizableWindow.ResizeDirection.Right;
			}
			if (Mathf.Abs(localPointer.y - rect.yMax) <= this._edgeDetectSize)
			{
				return ResizableWindow.ResizeDirection.Top;
			}
			if (Mathf.Abs(localPointer.y - rect.yMin) <= this._edgeDetectSize)
			{
				return ResizableWindow.ResizeDirection.Bottom;
			}
			return ResizableWindow.ResizeDirection.None;
		}

		// Token: 0x06003109 RID: 12553 RVA: 0x000A4B62 File Offset: 0x000A2D62
		private SystemCursorID GetCursorFromDirection(ResizableWindow.ResizeDirection direction)
		{
			if (direction - ResizableWindow.ResizeDirection.Left <= 1)
			{
				return SystemCursorID.SizeHorizontal;
			}
			if (direction - ResizableWindow.ResizeDirection.Top > 1)
			{
				return SystemCursorID.Default;
			}
			return SystemCursorID.SizeVertical;
		}

		// Token: 0x040025EC RID: 9708
		[SerializeField]
		private float _scaleSpeed = 20f;

		// Token: 0x040025ED RID: 9709
		[SerializeField]
		private RectTransform _panelRectTransform;

		// Token: 0x040025EE RID: 9710
		[SerializeField]
		private float _edgeDetectSize = 15f;

		// Token: 0x040025EF RID: 9711
		[SerializeField]
		private Vector2 _minSize = new Vector2(100f, 100f);

		// Token: 0x040025F0 RID: 9712
		[SerializeField]
		private Vector2 _maxSize = new Vector2(400f, 400f);

		// Token: 0x040025F1 RID: 9713
		private ResizableWindow.ResizeDirection _resizeDirection;

		// Token: 0x040025F2 RID: 9714
		private Vector2 _originalLocalPointerPosition;

		// Token: 0x040025F3 RID: 9715
		private Vector2 _originalSizeDelta;

		// Token: 0x040025F4 RID: 9716
		private Vector2 _originalAnchoredPosition;

		// Token: 0x040025F5 RID: 9717
		private Vector2 _targetSize;

		// Token: 0x040025F6 RID: 9718
		private Vector2 _targetAnchoredPosition;

		// Token: 0x040025F7 RID: 9719
		private bool _isResizing;

		// Token: 0x040025F8 RID: 9720
		private bool _isHovering;

		// Token: 0x0200085F RID: 2143
		private enum ResizeDirection
		{
			// Token: 0x040025FA RID: 9722
			None,
			// Token: 0x040025FB RID: 9723
			Left,
			// Token: 0x040025FC RID: 9724
			Right,
			// Token: 0x040025FD RID: 9725
			Top,
			// Token: 0x040025FE RID: 9726
			Bottom
		}
	}
}
