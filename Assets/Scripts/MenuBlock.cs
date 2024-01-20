using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject objectToInstantiate;

    public void Update() {
        if (GameManager.Instance.IsGamePaused() && Input.GetMouseButtonDown(0)) {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 parentOrigin = GetComponent<RectTransform>().anchoredPosition;
            
            if (parentOrigin.x > 0 && mousePosition.x < 960) {
                GameObject newObject = Instantiate(objectToInstantiate, this.GetComponent<RectTransform>());
                newObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(mousePosition.x - parentOrigin.x, mousePosition.y - 540, 0f);
            } else if (parentOrigin.x < 0 && mousePosition.x > 960) {
                GameObject newObject = Instantiate(objectToInstantiate, this.GetComponent<RectTransform>());
                newObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(mousePosition.x + 3*parentOrigin.x, mousePosition.y - 540, 0f);
            }
        }
    }
}
