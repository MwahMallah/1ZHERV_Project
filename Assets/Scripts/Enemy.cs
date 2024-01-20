using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IPlayableByCard
{
    [SerializeField] SpriteRenderer cardToShow;
    [SerializeField] SpriteRenderer enemyVisual;
    [SerializeField] private int healthPoints = 2;
    [SerializeField] private float unsuccessfulActionWaitTime = 0.5f;
    [SerializeField] private float successfulActionWaitTime = 1.5f;
    [SerializeField] private int enemyMeleeDamage;
    [SerializeField] private int enemyRangeDamage;
    [SerializeField] private float distanceBetweenNeighbour = 0.4f;

    private TrainPosition positionInTrain;
    private bool isImmune = false;
    private void Start() {
        GameManager.Instance.RegisterEnemy(this);
        positionInTrain = GetComponentInParent<TrainPosition>();
    }

    public IEnumerator MoveLeft() {
        while (GameManager.Instance.IsGamePaused()) {
            yield return null;
        }
        TrainPosition targetCarriage = GameManager.Instance.GetCarriage(positionInTrain.GetVerticalCarriage(), positionInTrain.GetHorizontalCarriage() - 1);
        if (targetCarriage == null) {
            yield return new WaitForSeconds(unsuccessfulActionWaitTime);
        } else {
            transform.SetParent(targetCarriage.GetTrainCarriageTransform());
            transform.localPosition = new Vector3(distanceBetweenNeighbour * (transform.parent.childCount - 1), 0, 0); //Excluding yourself set local position to 0.4 right from your copies
            positionInTrain.SetHorizontalCarriage(positionInTrain.GetHorizontalCarriage() - 1);
            yield return new WaitForSeconds(successfulActionWaitTime);
        }
    }

    public IEnumerator MoveRight() {
        while (GameManager.Instance.IsGamePaused()) {
            yield return null;
        }
        TrainPosition targetCarriage = GameManager.Instance.GetCarriage(positionInTrain.GetVerticalCarriage(), positionInTrain.GetHorizontalCarriage() + 1);
        if (targetCarriage == null) {
            yield return new WaitForSeconds(unsuccessfulActionWaitTime);
        } else {
            transform.SetParent(targetCarriage.GetTrainCarriageTransform());
            transform.localPosition = new Vector3(distanceBetweenNeighbour * (transform.parent.childCount - 1), 0, 0); //Excluding yourself set local position to 0.4 right from your copies
            positionInTrain.SetHorizontalCarriage(positionInTrain.GetHorizontalCarriage() + 1);
            yield return new WaitForSeconds(successfulActionWaitTime);
        }
    }

    public IEnumerator MoveVertical() {
        while (GameManager.Instance.IsGamePaused()) {
            yield return null;
        }
        int targetVerticalPosition = positionInTrain.GetVerticalCarriage() == 0 ? 1 : 0;

        TrainPosition targetCarriage = GameManager.Instance.GetCarriage(targetVerticalPosition, positionInTrain.GetHorizontalCarriage());
        if (targetCarriage == null) {
            yield return new WaitForSeconds(unsuccessfulActionWaitTime);
        } else {
            transform.SetParent(targetCarriage.GetTrainCarriageTransform());
            transform.localPosition = new Vector3(distanceBetweenNeighbour * (transform.parent.childCount - 1), 0, 0); //Excluding yourself set local position to 0.4 right from your copies
            positionInTrain.SetVerticalCarriage(targetVerticalPosition);
            yield return new WaitForSeconds(successfulActionWaitTime);
        }
    }

    public IEnumerator Shoot() {
        while (GameManager.Instance.IsGamePaused()) {
            yield return null;
        }
        List<(int, int)> positions = new List<(int, int)> {
            (-1, 0),
            (1, 0),
            (0, 1),
            (0, -1)
        };

        List<Transform> possiblePlayerCarriages = new List<Transform>();
        foreach ((int offsetHorizontal, int offsetVertical) in positions) {
            TrainPosition targetCarriage = GameManager.Instance.GetCarriage(positionInTrain.GetVerticalCarriage() + offsetVertical, positionInTrain.GetHorizontalCarriage() + offsetHorizontal);
            if (targetCarriage != null) {
                possiblePlayerCarriages.Add(targetCarriage.GetTrainCarriageTransform());
            }
        }

        Player player = null;
        foreach (Transform possibleEnemyCarriage in possiblePlayerCarriages) {
            player = possibleEnemyCarriage.GetComponentInChildren<Player>();
            if (player != null) {
                break;
            }
        }

        if (player != null) {
            player.TakeDamage(enemyRangeDamage);
            yield return new WaitForSeconds(successfulActionWaitTime);
        } else {
            yield return new WaitForSeconds(unsuccessfulActionWaitTime);
        }
    }

    public IEnumerator Dodge() {
        while (GameManager.Instance.IsGamePaused()) {
            yield return null;
        }
        isImmune = true;
        enemyVisual.color = new Color(110f / 255f, 110f / 255, 1);
        yield return new WaitForSeconds(successfulActionWaitTime);
    }

    public IEnumerator Fight() {
        while (GameManager.Instance.IsGamePaused()) {
            yield return null;
        }
        Transform carriageTransform = transform.parent;
        Player player = carriageTransform.GetComponentInChildren<Player>();
        if (player != null) {
            player.TakeDamage(enemyMeleeDamage);
            yield return new WaitForSeconds(successfulActionWaitTime);
        } else {
            yield return new WaitForSeconds(unsuccessfulActionWaitTime);
        }
    }

    public void TakeDamage(int amount) {
        if (!isImmune) {
            healthPoints -= amount;
            if (healthPoints <= 0) {
                GameManager.Instance.RemoveEnemy(this);
                Destroy(this.gameObject);
            }
        } else {
            enemyVisual.color = new Color(1, 1, 1);
            isImmune = false;
        }

    }

    public bool isObjectValid() {
        return this != null && gameObject != null;
    }

    public IEnumerator ShowCard() {
        while (GameManager.Instance.IsGamePaused()) {
            yield return null;
        }
        cardToShow.enabled = true;
        yield return new WaitForSeconds(successfulActionWaitTime);
        if (cardToShow != null) {
            cardToShow.enabled = false;
        }
    }

    public IEnumerator ShowCharacterTurn() {
        while (GameManager.Instance.IsGamePaused()) {
            yield return null;
        }
        cardToShow.enabled = true;
        yield return new WaitForSeconds(successfulActionWaitTime);
        cardToShow.enabled = false;
    }

    public TrainPosition GetPositionInTrain() {
        return positionInTrain;
    }
}
