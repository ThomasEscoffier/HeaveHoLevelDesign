using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Rules/Basic")]
public class BasicRules : ALevelRules {

    public bool IsUsingSpecificRespawnPoints = true;
    Victory victory = null;
    List<RespawnPoint> respawnPoints;

    int respawnCount = 0;
    int nbDeath = 0;

    bool isStartSingerDone = false;
    bool isFirstPlayer = true;

    public override void OnStart()
    {
        base.OnStart();

        isStartSingerDone = false;
        isFirstPlayer = true;

        victory = FindObjectOfType<Victory>();
        if (GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.VERSUS)
        {
            if (victory != null)
            {
                victory.IsSoloActivated = true;
            }
        }
        else
        {
            if (victory != null)
            {
                victory.IsSoloActivated = false;
            }
        }
        List<Player> players = new List<Player>(GameManager.Instance.GetPlayers());
        respawnPoints = new List<RespawnPoint>(FindObjectsOfType<RespawnPoint>());
        respawnPoints.Sort();
        respawnCount = 0;

        //Shuffle player respawn order at start
        while (players.Count > 0)
        {
            Player player = players[Random.Range(0, players.Count)];
            InitPlayer(player);
            players.Remove(player);
        }

        nbDeath = 0;
    }

    public override void InitPlayer(Player player)
    {
        base.InitPlayer(player);

        if (!IsUsingSpecificRespawnPoints)
        {
            player.SetRewpawnPoints(respawnPoints.ToArray());
        }
        else
        {
            RespawnPoint[] specificRespawnPoint = new RespawnPoint[1];
            specificRespawnPoint[0] = respawnPoints[respawnCount];
            player.SetRewpawnPoints(specificRespawnPoint);
            respawnCount++;
        }

        if (player.GetCurrentCharacter() == null)
        {
            player.Respawn(isFirstPlayer);
            if (isFirstPlayer)
            {
                isFirstPlayer = false;
            }
        }
    }

    public override void InitCharacter(Character character)
    {
        if (character.GetPlayer() != null && character.GetPlayer().GetIsAssisted())
        {
            if (character.LeftHand != null)
            {
                character.LeftHand.AddNewAccessory(Instantiate(character.AssistanceLeftHand, character.LeftHand.HandVisualisation));
            }
            if (character.RightHand != null)
            {
                character.RightHand.AddNewAccessory(Instantiate(character.AssistanceRightHand, character.RightHand.HandVisualisation));
            }
        }

        if (!isStartSingerDone && GameManager.Instance.GetMusicManager().GetIsDynamic())
        {
            character.SetIsFirstCharacterToSpawn();
            isStartSingerDone = true;
        }

        character.DeadEvent.AddListener(OnCharacterDeath);
    }

    public override bool CheckIsFinished()
    {
        if (victory != null && victory.IsValidated())
        {
            return true;
        }
        return false;
    }

    public override void OnFinish()
    {
        levelManager.GetSoundModule().PlayOneShot("EndLevel");
    }

    public override float GetCurrentScore()
    {
        return Time.time - timeStarted;
    }

    public override void PlayerLeaves(Player player)
    {
        if (player.GetCurrentCharacter() != null)
        {
            player.GetCurrentCharacter().Die();
        }
    }

    public override void OnUpdate()
    {
    }

    void OnCharacterDeath(Character character)
    {
        nbDeath++;
    }
}
