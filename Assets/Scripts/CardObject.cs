using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    [SerializeField] private CardObjectSO cardObjectSO;

    public CardObjectSO GetCardObjectSO() {
        return cardObjectSO;
    }
}
