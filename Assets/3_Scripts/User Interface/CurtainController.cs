using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoodHub.Core;

public class CurtainController : Singleton<CurtainController> {

    [Header("Animator")]
    public Animator curtainAnimator;

    public void LowerCurtain() {
        curtainAnimator.SetTrigger("Lower");
    }

    public void RaiseCurtain() {
        curtainAnimator.SetTrigger("Raise");
    }
}

