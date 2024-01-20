using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    public void AddEnemiesToScene(int numberOfEnemies) {
        int enemiesAdded = 0;
        while (enemiesAdded != numberOfEnemies) {
            Transform parentTransform = GameManager.Instance.GetRandomTrainTransform();
            if (parentTransform != null && parentTransform.childCount == 0) {
                enemiesAdded++;
                Instantiate(enemyPrefab, parentTransform);
            }
        }
    }
}
