using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float verticalMovement = 0.5f;
    [Range(0, 2f), SerializeField] private float scaleFactor = 1.1f;
    [SerializeField] private float moveTime = 0.1f;

    private Vector3 startPos;
    private Vector3 startScale;

    private void Start() {
        startPos = transform.localPosition;
        startScale = transform.localScale;
    }

    private IEnumerator MoveCard(bool startingAnimation) {

        Vector3 targetPos;
        Vector3 targetScale;

        float elapsedTime = 0f;

        while (elapsedTime < moveTime) {
            while (GameManager.Instance.IsGamePaused()) {
                yield return null;
            }

            elapsedTime += Time.deltaTime;
            if (startingAnimation) {
                Vector3 direction = new Vector3(0f, verticalMovement, 0f);
                targetPos = startPos + direction;
                targetScale = startScale * scaleFactor;
            } else {
                targetPos = startPos;
                targetScale = startScale;
            }

            Vector3 lerpedPos = Vector3.Lerp(startPos, targetPos, (elapsedTime / moveTime));
            Vector3 lerpedScale = Vector3.Lerp(startScale, targetScale, (elapsedTime / moveTime));

            transform.localPosition = lerpedPos;
            transform.localScale = lerpedScale;

            yield return null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        eventData.selectedObject = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData) {
        eventData.selectedObject = null;
    }

    public void OnSelect(BaseEventData eventData) {
        StartCoroutine(MoveCard(true));
    }

    public void OnDeselect(BaseEventData eventData) {
        StartCoroutine(MoveCard(false));
    }
}
