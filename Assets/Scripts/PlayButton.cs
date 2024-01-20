using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null) {
            button.onClick.AddListener(StartGame);
        }
    }

    // Update is called once per frame
    private void StartGame()
    {
        if (GameManager.Instance.IsGameStarted()) {
            StartCoroutine(GameManager.Instance.ContinueGame());
        } else {
            StartCoroutine(GameManager.Instance.StartGame());
        }
    }
}
