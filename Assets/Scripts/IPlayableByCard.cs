using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayableByCard
{
    public IEnumerator MoveLeft();
    public IEnumerator MoveRight();
    public IEnumerator MoveVertical();
    public IEnumerator Shoot();
    public IEnumerator Fight();
    public IEnumerator Dodge();
    public void TakeDamage(int amount);
    public bool isObjectValid();
    public IEnumerator ShowCard();
    public IEnumerator ShowCharacterTurn();

}
