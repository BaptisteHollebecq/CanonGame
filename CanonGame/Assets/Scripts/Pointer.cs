using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pointer : MonoBehaviour
{
    [SerializeField]
    public float gap = 50;

    Transform leftArrow;
    Transform rightArrow;

    private void Awake()
    {
        leftArrow = transform.GetChild(0);
        rightArrow = transform.GetChild(1);

        float startXLeft = leftArrow.localPosition.x;
        float startXRight = rightArrow.localPosition.x;

        Sequence moveLeft = DOTween.Sequence();
        Sequence moveRight = DOTween.Sequence();

        moveLeft.SetLoops(-1);
        moveRight.SetLoops(-1);

        moveLeft.Append(leftArrow.DOLocalMoveX(startXLeft - gap, .5f).SetEase(Ease.OutCubic));
        moveLeft.Append(leftArrow.DOLocalMoveX(startXLeft, .5f).SetEase(Ease.InCubic));

        moveRight.Append(rightArrow.DOLocalMoveX(startXRight + gap, .5f).SetEase(Ease.OutCubic));
        moveRight.Append(rightArrow.DOLocalMoveX(startXRight, .5f).SetEase(Ease.InCubic));

        moveLeft.Play();
        moveRight.Play();
    }
}
