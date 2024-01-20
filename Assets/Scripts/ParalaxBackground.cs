using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxBackground : MonoBehaviour
{
    [SerializeField] float paralaxEffectMultiplier;
    [SerializeField] SpriteRenderer paralaxObjectImage;
    [SerializeField] int distanceRepositionMultiplier;

    private float distanceBeforeReposition;
    private float verticalPosition;
    // Start is called before the first frame update
    void Start()
    {
        verticalPosition = transform.position.y;
        Sprite paralaxObjectSprite = paralaxObjectImage.sprite;
        Texture2D paralaxObjectTexture = paralaxObjectSprite.texture;
        distanceBeforeReposition = distanceRepositionMultiplier * transform.localScale.x * paralaxObjectTexture.width / paralaxObjectSprite.pixelsPerUnit;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.Instance.IsGamePaused()) {
            transform.position -= new Vector3(1,0, 0) * Time.deltaTime * paralaxEffectMultiplier;
            if (Math.Abs(transform.position.x) > distanceBeforeReposition ) {
                transform.position = new Vector3(0, verticalPosition, 0);
            }
        }
    }
}
