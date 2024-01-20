using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> cards;
    [SerializeField] private List<InventorySlot> cardSlots;
    [SerializeField] private List<Transform> cardsActionsVisual;
    [SerializeField] private List<Transform> cardsToMove;
    [SerializeField] private int maxNumberOfTimesForPossibleCard;
    [SerializeField] private float moveSpeed;

    public void EnableActionCard(CardType.actionType actionType, bool showActionCard) {
        switch (actionType) {
            case CardType.actionType.MoveLeft:
                cardsActionsVisual[0].gameObject.SetActive(showActionCard);
                break;
            case CardType.actionType.MoveRight:
                cardsActionsVisual[1].gameObject.SetActive(showActionCard);
                break;
            case CardType.actionType.MoveVertical:
                cardsActionsVisual[2].gameObject.SetActive(showActionCard);
                break;
            case CardType.actionType.Shoot:
                cardsActionsVisual[3].gameObject.SetActive(showActionCard);
                break;
            case CardType.actionType.Fight:
                cardsActionsVisual[4].gameObject.SetActive(showActionCard);
                break;
            case CardType.actionType.Dodge:
                cardsActionsVisual[5].gameObject.SetActive(showActionCard);
                break;
        }
    }

    public GameManager.CardToPlay GetRandomEnemyCard(IPlayableByCard character) {
        Array actionTypes = Enum.GetValues(typeof(CardType.actionType));
        int indexInActionsType = UnityEngine.Random.Range(0, actionTypes.Length);
        CardType.actionType randomActionType = (CardType.actionType)actionTypes.GetValue(indexInActionsType);
        return new GameManager.CardToPlay(randomActionType, character);
    }

    public GameManager.CardToPlay GetEnemyCardGreaterPossibility(Enemy enemy, CardType.actionType possibleCardType) {
        Array actionTypes = Enum.GetValues(typeof(CardType.actionType));
        int numberOfTimesPossibleCardNotChosen = 0;
        CardType.actionType chosenAction = possibleCardType;

        while (numberOfTimesPossibleCardNotChosen < maxNumberOfTimesForPossibleCard) {
            int indexInActionsType = UnityEngine.Random.Range(0, actionTypes.Length);
            chosenAction = (CardType.actionType)actionTypes.GetValue(indexInActionsType);

            if (chosenAction != possibleCardType) {
                numberOfTimesPossibleCardNotChosen++;    
            } else {
                break;
            }
        }

        return new GameManager.CardToPlay(chosenAction, enemy);
    }

    public IEnumerator GiveCards() {
        for (int i = 0; i < cardSlots.Count; i++) {
            while (GameManager.Instance.IsGamePaused()) {
                yield return null;
            }
            InventorySlot cardSlot = cardSlots[i];
            if (cardSlot.transform.childCount == 0) {
                int indexInCards = UnityEngine.Random.Range(0, cards.Count);
                StartCoroutine(MoveCardToSlot(i, indexInCards, cardSlot));

                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private IEnumerator MoveCardToSlot(int slotPosition, int cardSpriteNumber, InventorySlot cardSlot) {

        Transform movableCard = cardsToMove[slotPosition].GetChild(cardSpriteNumber);
        float elapsedTime = 0f;
        Vector3 startingPosition = movableCard.localPosition;
        Vector3 targetPosition = new Vector3(0, 4.25f, 0);

        while (movableCard.localPosition != targetPosition) {
            while (GameManager.Instance.IsGamePaused()) {
                yield return null;
            }
            movableCard.localPosition = Vector3.Lerp(movableCard.localPosition, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * moveSpeed / 10000;
            yield return null;
        }

        movableCard.localPosition = Vector3.zero;
    
        GameObject newObject = Instantiate(cards[cardSpriteNumber], cardSlot.transform);
        newObject.gameObject.SetActive(true);
        yield return null;
    }
}
