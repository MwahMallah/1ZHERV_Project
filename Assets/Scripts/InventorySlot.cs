using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler {
    private void Start() {
        GameManager.Instance.OnPlayerInsertedCard += EnablePlayerCard;
    }

    private void EnablePlayerCard(object sender, GameManager.OnPlayerInsertedCardEventArgs e) {
        if (transform.childCount != 0) {
            Image childCardImage = transform.GetChild(0).GetComponent<Image>();
            childCardImage.raycastTarget = e.slotMachineProcessed;
        }
    }

    public void OnDrop(PointerEventData eventData) {
        Debug.Log("Dropped on InventorySlot");
        GameObject droppedObject = eventData.pointerDrag;
        CardDragAndDrop draggableCard = droppedObject.GetComponent<CardDragAndDrop>();
        draggableCard.parentAfterDrag = transform;
    }
}
