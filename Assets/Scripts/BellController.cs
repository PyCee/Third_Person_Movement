using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(ParticleSystem))]
public class BellController : MonoBehaviour
{
    private Sequence floatSequence;
    void Start()
    {
        floatSequence = DOTween.Sequence();
        floatSequence.Append(transform.DOMoveY(transform.position.y + 0.5f, 4.0f).SetEase(Ease.InOutSine));
        floatSequence.SetLoops(-1, LoopType.Yoyo);
    }
    public void GetBell(){
        GetComponent<ParticleSystem>().Play();
    }
}
