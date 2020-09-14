using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Victory : MonoBehaviour
{
    public float VictoryZoomSize = 2f;
    public float VictoryZoomTime = 0.5f;
    public Transform ZoomPosition = null;
    public bool IsWaitingForPlayersToRespawn = true;
    public bool IsSoloActivated = false;

    int nbPlayerInZone = 0;
    protected bool isVictory = false;
    protected bool isActivated = false;
    protected bool isKeyCollected = false;

    public float TimeBeforeBalloon = 1f;
    float timerBeforeBalloon = 0f;
    bool isWaitEndPlayer = false;

    public float TimeBeforeVictory = 1f;
    float timerBeforeVictory = 0f;
    protected bool isTimerStarted = false;

    public int NbCollectibleNeededToWin = 0;
    int collectiblesCollected = 0;
    public RespawnableItem.ItemType CollectibleTypeNeeded = RespawnableItem.ItemType.NONE;

    public Transform TokenLerpPosition = null;

    protected ParticleSystem[] particles;

    public VictoryBalloon[] Balloons;
    public Animation LevelScoreAnim = null;
    public GameObject ScorePanel = null;
    public GameObject BestScorePanel = null;

    public HelpVictory VictoryHelp = null;

    protected CameraEffects cam;
    protected LevelManager levelManager = null;
    protected Text uiText = null;
    protected Text uiTextBestScore = null;
    //protected SoundModule soundModule = null;

    bool isWaitingForScore = false;
    bool areAllBalloonsActivated = false;
    bool isBestScore = false;

    public virtual void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        //soundModule = GetComponent<SoundModule>();
        
        cam = Camera.main.GetComponent<CameraEffects>();
        
        uiText = ScorePanel.GetComponentInChildren<Text>(true);
        uiTextBestScore = BestScorePanel.GetComponentInChildren<Text>(true);
        if (GameManager.Instance.GetLevelSelector().GetCurrentLevel() != null)
        {
            ScorePanel.GetComponent<SpriteRenderer>().color = GameManager.Instance.GetLevelSelector().GetCurrentLevel().ColorScorePanel;
            BestScorePanel.GetComponent<SpriteRenderer>().color = GameManager.Instance.GetLevelSelector().GetCurrentLevel().ColorScorePanel;
        }
        particles = GetComponentsInChildren<ParticleSystem>(true);

        if (Balloons.Length > 0)
        {
            for (int i = 0; i < GameManager.Instance.GetPlayers().Count; ++i)
            {
                Balloons[i].SetPlayer(GameManager.Instance.GetPlayers()[i]);
            }
        }
    }

    public virtual void Update()
    {
        if (isWaitEndPlayer)
        {
            timerBeforeBalloon = Mathf.Min(timerBeforeBalloon + Time.deltaTime, TimeBeforeBalloon);
            if (timerBeforeBalloon == TimeBeforeBalloon)
            {
                if (GameManager.Instance.GetMusicManager().GetIsDynamic())
                {
                    GameManager.Instance.GetMusicManager().LaunchEndMusicEvent();
                }
                isTimerStarted = true;
                ComputeScore();
                areAllBalloonsActivated = true;
                isWaitEndPlayer = false;
            }
        }

        if (isTimerStarted)
        {
            timerBeforeVictory = Mathf.Min(timerBeforeVictory + Time.deltaTime, TimeBeforeVictory);
            if (timerBeforeVictory == TimeBeforeVictory)
            {
                StartVictory();
                isTimerStarted = false;
            }
        }

        if (isWaitingForScore)
        {
            bool areBalloonsOk = true;
            for (int i = 0; i < GameManager.Instance.GetPlayers().Count; ++i)
            {
                if (GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.COOP)
                {
                    if (Balloons.Length != 0 && !Balloons[i].IsVisible())
                    {
                        areBalloonsOk = false;
                    }
                }
            }
            if (areBalloonsOk)
            {
                if (BestScorePanel != null && ScorePanel != null)
                {
                    //Show score
                    if (isBestScore)
                    {
                        BestScorePanel.SetActive(true);
                    }
                    else
                    {
                        ScorePanel.SetActive(true);
                    }
                    LevelScoreAnim.Play();
                }
                isWaitingForScore = false;
            }
        }
    }

    public bool IsValidated()
    {
        return isVictory;
    }

    public void RefreshCountdownText(float currentScore)
    {
        int minutes = (int)(currentScore / 60f);
        int seconds = (int)(currentScore % 60f);
        float miliseconds = (currentScore * 1000f) % 1000f / 10f;
        if (uiText != null)
        {
            uiText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, miliseconds);
        }
        if (uiTextBestScore != null)
        {
            uiTextBestScore.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, miliseconds);
        }
    }

    void ComputeScore()
    {
        string levelName = GameManager.Instance.GetLevelSelector().GetCurrentLevel().LevelName;
        float currentScore = levelManager.LevelRules.GetCurrentScore();
        if (GameManager.Instance.GetScoreManager().IsBetterScore(levelName, currentScore, GameManager.Instance.GetLevelSelector().GetCurrentGameMode()))
        {
            GameManager.Instance.GetScoreManager().AddNewScore(levelName, currentScore, GameManager.Instance.GetLevelSelector().GetCurrentRun().NbPlayerMin, levelManager.LevelRules.IsScoreTime, GameManager.Instance.GetLevelSelector().GetCurrentGameMode());
            isBestScore = true;
        }
        RefreshCountdownText(currentScore);
        isWaitingForScore = true;
        areAllBalloonsActivated = true;

        GameManager.Instance.GetMetricsManager().AddLevelTime(currentScore);

        /*if (soundModule != null)
        {
            if (isBestScore)
            {
                soundModule.SetParameterValue("EndBalloon", "endBalloon", 2f);
            }
            else
            {
                soundModule.SetParameterValue("EndBalloon", "endBalloon", 1f);
            }
            soundModule.PlayEvent("EndBalloon");
        }*/
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Collectible"))
        {
            RespawnableItem collectible = collision.GetComponent<RespawnableItem>();
            collectible.SetIsInVictoryTrigger(true);
            if (collectible.Type == RespawnableItem.ItemType.KEY)
            {          
                isKeyCollected = true;
            }
            else if (collectible.Type == RespawnableItem.ItemType.ANIMAL)
            {
                collectiblesCollected++;
            }
            collectible.ValidateItem(TokenLerpPosition);
            GameManager.Instance.GetLevelSelector().SetIsTokenAcquired();
            if (!GameManager.Instance.GetSaveManager().IsTokenAlreadyAcquiredInCurrentLevel())
            {
                GameManager.Instance.GetMetricsManager().AddToken(1);
                GameManager.Instance.GetSaveManager().AddTokenAcquiredFromLevel(GameManager.Instance.GetLevelSelector().GetCurrentWorld().WorldName, GameManager.Instance.GetLevelSelector().GetCurrentStep() - 1);

            }
            /*else
            {
                collectiblesInTrigger.Add(collectible);
            }*/
        }

        Character character = collision.GetComponent<Character>();
        if (character != null && !areAllBalloonsActivated)
        {
            nbPlayerInZone++;
            if (GameManager.Instance.GetMusicManager().GetIsDynamic())
            {
                GameManager.Instance.GetMusicManager().LaunchPlayerInVictoryEvent(nbPlayerInZone);
            }
            if (VictoryHelp && !VictoryHelp.gameObject.activeSelf
                && GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.COOP
                && GameManager.Instance.GetPlayers().Count > 1 && nbPlayerInZone == 1)
            {
                VictoryHelp.gameObject.SetActive(true);
            }
            character.SetIsInVictoryTrigger(true);
            if (Balloons.Length > 0)
            {
                for (int i = 0; i < Balloons.Length; ++i)
                {
                    if (Balloons[i].GetCurrentPlayer() == character.GetPlayer())
                    {
                        Balloons[i].GetAnimator().SetBool("IsHidden", false);
                        break;
                    }
                }
            }
        }

        if (!isTimerStarted && !isVictory && nbPlayerInZone > 0)
        {
            if (IsSoloActivated || nbPlayerInZone >= (IsWaitingForPlayersToRespawn ? GameManager.Instance.GetPlayers().Count : GameManager.Instance.GetNbPlayersAlive()))
            {
                if (NbCollectibleNeededToWin == 0 || !isTimerStarted && NbCollectibleNeededToWin == collectiblesCollected)
                {
                    isWaitEndPlayer = true;
                    timerBeforeBalloon = 0f;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        /*if (collision.CompareTag("Collectible"))
        {
            RespawnableItem collectible = collision.GetComponent<RespawnableItem>();
            collectiblesInTrigger.Remove(collectible);
            collectible.SetIsInVictoryTrigger(false);
            if (isTimerStarted && !AreCollectiblesNeededInVictoryTrigger())
            {
                isTimerStarted = false;
                timerBeforeVictory = 0f;
            }
            return;
        }*/

        Character character = collision.GetComponent<Character>();
        if (character != null && !areAllBalloonsActivated)
        {
            if (Balloons.Length > 0)
            {
                for (int i = 0; i < Balloons.Length; ++i)
                {
                    if (Balloons[i].GetCurrentPlayer() == character.GetPlayer())
                    {
                        Balloons[i].GetAnimator().SetBool("IsHidden", true);
                        break;
                    }
                }
            }
            nbPlayerInZone--;
            character.SetIsInVictoryTrigger(false);
            isTimerStarted = false;
            timerBeforeVictory = 0f;

            isWaitEndPlayer = false;
        }
    }

    public void StartVictory()
    {
        isVictory = true;
        cam.StartLerp(ZoomPosition.position, VictoryZoomSize, VictoryZoomTime);
        cam.EndLerpEvent.AddListener(levelManager.GetFlashEffect().Launch);
        cam.EndLerpEvent.AddListener(LaunchParticles);
        List<Player> players = GameManager.Instance.GetPlayers();
        foreach (Player player in players)
        {
            if (player.GetCurrentCharacter() != null)
            {
                player.GetCurrentCharacter().ManageVictory();
            }
        }
    }

    /*bool AreCollectiblesNeededInVictoryTrigger()
    {
        int nbCollectibles = 0;
        for(int i = 0; i < collectiblesInTrigger.Count; ++i)
        {
            if (collectiblesInTrigger[i].Type == CollectibleTypeNeeded)
            {
                nbCollectibles++;
            }
        }
        return nbCollectibles >= NbCollectibleNeededToWin;
    }*/

    /*public bool IsCollectibleInVictoryTrigger()
    {
        RespawnableItem item = collectiblesInTrigger.Find(i => i.Type == RespawnableItem.ItemType.KEY);
        return item != null;
    }*/

    protected bool IsAnyCharacterHoldingCollectible()
    {
        foreach (Player player in GameManager.Instance.GetPlayers())
        {
            Character currentCharacter = player.GetCurrentCharacter();
            if (currentCharacter.LeftHand != null && currentCharacter.LeftHand.GetIsHooked() && currentCharacter.LeftHand.GetHook().CompareTag("Collectible"))
            {
                RespawnableItem item = currentCharacter.LeftHand.GetHook().GetComponent<RespawnableItem>();
                if (item != null && item.Type == RespawnableItem.ItemType.KEY)
                {
                    return true;
                }
            }
            if (currentCharacter.RightHand != null && currentCharacter.RightHand.GetIsHooked() && currentCharacter.RightHand.GetHook().CompareTag("Collectible"))
            {
                RespawnableItem item = currentCharacter.RightHand.GetHook().GetComponent<RespawnableItem>();
                if (item != null && item.Type == RespawnableItem.ItemType.KEY)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void LaunchParticles()
    {
        for (int i = 0; i < particles.Length; ++i)
        {
            particles[i].gameObject.SetActive(true);
        }
    }

    //When comming back from a Minigame
    public void SetCollectiblesCollected(int nb)
    {
        collectiblesCollected = nb;
    }

    public int GetCollectiblesCollecteed()
    {
        return collectiblesCollected;
    }
}
