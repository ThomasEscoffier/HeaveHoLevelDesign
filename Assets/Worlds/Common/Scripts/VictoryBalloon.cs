using UnityEngine;

public class VictoryBalloon : MonoBehaviour
{
    [System.Serializable]
    public struct ColorAnim
    {
        public Color CharacterColor;
        public AnimatorOverrideController AnimatorBalloon;
    }

    public ColorAnim[] ColorsAnimations;
    Animator anim = null;
    SpriteRenderer spriteRend = null;
    SoundModule soundModule = null;
    Player currentPlayer = null;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        soundModule = GetComponent<SoundModule>();

        if (soundModule != null)
        {
            soundModule.InitEvent("EndBalloon");
            soundModule.AddParameter("EndBalloon", "endballoon", 1f);
        }
    }

    public void SetPlayer(Player player)
    {
        currentPlayer = player;
        for (int i = 0; i < ColorsAnimations.Length; ++i)
        {
            if (ColorsAnimations[i].CharacterColor == player.GetCurrentCharacterOutfit().BodyColor)
            {
                anim.runtimeAnimatorController = ColorsAnimations[i].AnimatorBalloon;
                return;
            }
        }
    }

    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsVisible()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("IsVisible");
    }

    public Animator GetAnimator()
    {
        return anim;
    }
}
