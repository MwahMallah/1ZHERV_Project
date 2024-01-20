using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour, IPlayableByCard {
    [SerializeField] private TrainPosition positionInTrain;
    [SerializeField] private float unsuccessfulActionWaitTime = 0.5f;
    [SerializeField] private float successfulActionWaitTime = 1.5f;
    [SerializeField] SpriteRenderer cardToShow;
    [SerializeField] SpriteRenderer playerVisual;

    [SerializeField] private int healthPoints;
    [SerializeField] private Transform healtIndicator;
    [SerializeField] private int playerMeleeDamage;
    [SerializeField] private int playerRangeDamage;

    private bool isImmune = false;
    public static Player Instance { get; private set; }
    public event EventHandler<EventArgs> OnPlayerDies;

    private void Awake() {
        Instance = this;
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
            transform.localPosition = Vector2.zero;
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
            transform.localPosition = Vector2.zero;
            positionInTrain.SetHorizontalCarriage(positionInTrain.GetHorizontalCarriage() + 1);
            yield return new WaitForSeconds(successfulActionWaitTime);
        }
    }

    public IEnumerator MoveVertical() {
        while (GameManager.Instance.IsGamePaused()) {
            yield return null;
        }
        int targetVerticalPosition = positionInTrain.GetVerticalCarriage() == 0? 1 : 0;

        TrainPosition targetCarriage = GameManager.Instance.GetCarriage(targetVerticalPosition, positionInTrain.GetHorizontalCarriage());
        if (targetCarriage == null) {
            yield return new WaitForSeconds(unsuccessfulActionWaitTime);
        } else {
            transform.SetParent(targetCarriage.GetTrainCarriageTransform());
            transform.localPosition = Vector2.zero;
            positionInTrain.SetVerticalCarriage(targetVerticalPosition);
            yield return new WaitForSeconds(successfulActionWaitTime);
        }
    }

    public IEnumerator Fight() {
        while (GameManager.Instance.IsGamePaused()) {
            yield return null;
        }
        Transform carriageTransform = transform.parent;
        Enemy[] enemies = carriageTransform.GetComponentsInChildren<Enemy>();
        if (enemies.Length > 0) {
            int randomEnemyIndex = UnityEngine.Random.Range(0, enemies.Length);
            enemies[randomEnemyIndex].TakeDamage(playerMeleeDamage);
            yield return new WaitForSeconds(successfulActionWaitTime);
        } else {
            yield return new WaitForSeconds(unsuccessfulActionWaitTime);
        }
    }
    public IEnumerator Shoot() {
        List<(int, int)> positions = new List<(int, int)> {
            (-1, 0),
            (1, 0),
            (0, 1),
            (0, -1)
        };

        List<Transform> possibleEnemyCarriages = new List<Transform>();
        foreach((int offsetHorizontal, int offsetVertical) in positions) {
            TrainPosition targetCarriage = GameManager.Instance.GetCarriage(positionInTrain.GetVerticalCarriage() + offsetVertical, positionInTrain.GetHorizontalCarriage() + offsetHorizontal);
            if (targetCarriage != null) {
                possibleEnemyCarriages.Add(targetCarriage.GetTrainCarriageTransform());
            }
        }
        List<Enemy> enemies = new List<Enemy>();
        
        foreach(Transform possibleEnemyCarriage in possibleEnemyCarriages) {
            Enemy[] enemiesToAdd = possibleEnemyCarriage.GetComponentsInChildren<Enemy>();
            foreach(Enemy enemy in enemiesToAdd) {
                enemies.Add(enemy);
            }
        }

        if (enemies.Count > 0) {
            int randomEnemyIndex = UnityEngine.Random.Range(0, enemies.Count);
            enemies[randomEnemyIndex].TakeDamage(playerRangeDamage);
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
        playerVisual.color = new Color(110f / 255f, 110f / 255, 1);
        yield return new WaitForSeconds(successfulActionWaitTime);
    }

    public IEnumerator ShowCard() {
        while (GameManager.Instance.IsGamePaused()) {
            yield return null;
        }
        cardToShow.enabled = true;
        yield return new WaitForSeconds(successfulActionWaitTime);
        cardToShow.enabled = false;
    }

    public IEnumerator ShowCharacterTurn() {
        while (GameManager.Instance.IsGamePaused()) {
            yield return null;
        }
        cardToShow.enabled = true;
        yield return new WaitForSeconds(successfulActionWaitTime);
        cardToShow.enabled = false;
    }

    public void TakeDamage(int amount) {
        if (!isImmune) {
            healthPoints -= amount;
            for (int i = 2; i >= healthPoints; i--) {
                healtIndicator.GetChild(i).gameObject.SetActive(false);
            }


            if (healthPoints <= 0) {
                OnPlayerDies?.Invoke(this, EventArgs.Empty);
                Destroy(this.gameObject);
            }
        } else {
            playerVisual.color = new Color(1, 1, 1);
            isImmune = false;
        }

    }
    public TrainPosition GetPositionInTrain() {
        return positionInTrain;
    }
    public bool isObjectValid() {
        return this != null && gameObject != null;
    }
}
