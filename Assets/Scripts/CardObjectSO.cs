using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class CardObjectSO : ScriptableObject {
    public CardType.actionType actionType;
    public string cardName;
    public string cardDescription;
    public Image cardImage;
}
