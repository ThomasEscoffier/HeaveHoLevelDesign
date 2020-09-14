using UnityEngine;

public abstract class ALevelRules : ScriptableObject
{
    public bool IsScoreTime = false;
    public bool IsUsingCostumes = false;
    public bool KeepHair = false;
    public bool KeepGlasses = false;
    public bool KeepFacialFeatures = false;
    public Character.Outfit[] Costumes;

    protected float timeStarted = 0f;
    protected LevelManager levelManager = null;

    public virtual void OnStart()
    {
        timeStarted = Time.time;
        levelManager = FindObjectOfType<LevelManager>();
    }

    public abstract void OnUpdate();

    public virtual void InitPlayer(Player player)
    {
        if (IsUsingCostumes)
        {
            if (player.OrderInGame >= Costumes.Length)
            {
                int order = Costumes.Length % GameManager.Instance.GetPlayers().Count;
                player.SetTemporaryOutfit(Costumes[order], KeepHair, KeepGlasses, KeepFacialFeatures);
            }
            else
            {
                player.SetTemporaryOutfit(Costumes[player.OrderInGame], KeepHair, KeepGlasses, KeepFacialFeatures);
            }
        }
    }

    public abstract void InitCharacter(Character character);
    public abstract bool CheckIsFinished();
    public abstract void OnFinish();
    public abstract float GetCurrentScore();
    public abstract void PlayerLeaves(Player player);

    public float GetTimeStarted()
    {
        return timeStarted;
    }

    public virtual void ResetAfterMinigame(float startTime)
    {
        timeStarted = startTime;
    }
}
