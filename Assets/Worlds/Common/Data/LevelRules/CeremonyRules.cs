using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Rules/Ceremony")]
public class CeremonyRules : BasicRules {

    ScoreRecap scoreRecap = null;

    public override void OnStart()
    {
        base.OnStart();
        scoreRecap = FindObjectOfType<ScoreRecap>();
        scoreRecap.SetRules(this);
    }
}
