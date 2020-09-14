using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroLevel : MonoBehaviour {

    [Tooltip("The animators needs to have a state named Finish after the intro animtation")]
    public Animator anim = null;

    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Finish"))
        {
            GameManager.Instance.GetLevelSelector().SelectRandomNextLevel();
        }
    }
}
