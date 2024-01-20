using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragAndDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler{
    [Range(20, 35f), SerializeField] private float dragSpeedX;
    [Range(20, 35f), SerializeField] private float dragSpeedY;
    private Camera mainCamera;

    [HideInInspector] public Transform parentAfterDrag;
    public Image image;
    private RectTransform rectTransform;
    private RectTransform parentCanvas;

    public void Start() {
        mainCamera = GameManager.Instance.GetMainCamera();
        rectTransform = GetComponent<RectTransform>();
        //parentCanvas = transform.parent.parent.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        parentAfterDrag = transform.parent;
        //transform.SetParent(transform.root);
        //transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData) {
        // Get the current mouse position in screen coordinates
        Vector3 mousePositionScreen = Input.mousePosition;

        // Set the z-coordinate to the desired distance from the camera
        mousePositionScreen.z = mainCamera.nearClipPlane + 5;

        // Convert the mouse position from screen space to world space
        Vector3 mousePositionWorld = mainCamera.ScreenToWorldPoint(mousePositionScreen);

        transform.position = mousePositionWorld;
        //rectTransform.anchoredPosition += eventData.delta / parentCanvas.localScale.x;
    }

    public void OnEndDrag(PointerEventData eventData) {
        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;
        image.raycastTarget = true;
    }
}
