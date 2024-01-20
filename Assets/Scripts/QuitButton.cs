using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour {
    // Start is called before the first frame update
    private void Start() {
        Button button = GetComponent<Button>();
        if (button != null) {
            button.onClick.AddListener(QuitGame);
        }
    }

    // Update is called once per frame
    private void QuitGame() {
        Debug.Log("Quit game");
        Application.Quit();
    }
}
