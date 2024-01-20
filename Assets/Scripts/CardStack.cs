using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardStack : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Transform cardSlotTransform;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float moveSpeed;
    private int insertedCards;
    private Vector3 originalPosition;
    private Coroutine moveCoroutine;

    private void Start() {
        insertedCards = 0;
        originalPosition = cardSlotTransform.localPosition;
    }
    public void OnDrop(PointerEventData eventData) {
        insertedCards++;
        GameObject droppedObject = eventData.pointerDrag;
        CardObjectSO cardObjectSO = droppedObject.GetComponent<CardObject>().GetCardObjectSO();

        Destroy(droppedObject);
        GameManager.Instance.AddPlayerCard(cardObjectSO.actionType);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        // Move the visuals down when the cursor enters
        //MoveVisuals(originalPosition - Vector3.up * 1.0f);
        if (moveCoroutine != null) {
            StopCoroutine(moveCoroutine);
        }

        cardSlotTransform.localPosition = Vector3.Lerp(cardSlotTransform.localPosition, targetPosition, Time.deltaTime * moveSpeed);
        
    }

    public void OnPointerExit(PointerEventData eventData) {
        // Move the visuals back to the original position when the cursor exits
        //MoveVisuals(originalPosition);
        moveCoroutine = StartCoroutine(MoveVisualsSmoothlyToOrigin());
    }

    private IEnumerator MoveVisualsSmoothlyToOrigin() {
        while (GameManager.Instance.IsGamePaused()) {
            yield return null;
        }
        float elapsedTime = 0f;
        Vector3 startingPosition = cardSlotTransform.localPosition;

        while (cardSlotTransform.localPosition != originalPosition) {
            cardSlotTransform.localPosition = Vector3.Lerp(startingPosition, originalPosition, elapsedTime);
            elapsedTime += Time.deltaTime * moveSpeed;
            yield return null;
        }
    }
}
