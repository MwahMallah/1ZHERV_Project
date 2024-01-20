using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainPosition : MonoBehaviour
{
    [SerializeField] private Transform trainCarriagePositionTransform;
    [SerializeField] private int horizontalCarriage;
    [SerializeField] private int verticalCarriage;

    public int GetHorizontalCarriage() {
        return horizontalCarriage;
    }
    public int GetVerticalCarriage() {  
        return verticalCarriage; 
    }

    public Transform GetTrainCarriageTransform() {
        return trainCarriagePositionTransform;
    }

    public void SetHorizontalCarriage(int newHorizontalCarriage) {
        horizontalCarriage = newHorizontalCarriage;
    }
    public void SetVerticalCarriage(int newVerticalCarriage) {
        verticalCarriage = newVerticalCarriage;
    }
}
