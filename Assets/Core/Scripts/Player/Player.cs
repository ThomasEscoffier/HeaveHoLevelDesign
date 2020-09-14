using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Character BaseCharacterPrefab = null;
    [Tooltip("Used to keep the same order in game as in the Selection menu")]
    public int OrderInGame = 0;

    bool isAssisted = false;
    PlayerControls controls;

    bool isUsingTmpOutfit = false;
    bool keepHair = false;
    bool keepGlasses = false;
    bool keepFacialFeatures = false;
    Character.Outfit tmpOutfit;
    Character.Outfit currentOutfit;
    Character currentCharacter = null;
    RespawnPoint[] respawnPoints = null;
    Checkpoint checkPoint = null;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        controls = new PlayerControls();
    }

    public PlayerControls GetControls()
    {
        return controls;
    }

    public Character.ePersonality GetCurrentPersonality()
    {
        return currentOutfit.Personality;
    }

    public Character GetCurrentCharacter()
    {
        return currentCharacter;
    }

    public void SetCharacterNull()
    {
        currentCharacter = null;
    }

    public void SetCurrentCharacter(Character character)
    {
        currentCharacter = character;
    }

    public void SetRewpawnPoints(RespawnPoint[] rezPoints)
    {
        respawnPoints = rezPoints;
    }

    public void SetCharacterOutfit(Character.Outfit outfit)
    {
       currentOutfit = outfit;
        if (currentCharacter != null)
        {
            currentCharacter.InitAppearanceAndPersonality(outfit);
        }
    }

    public Character.Outfit GetCurrentCharacterOutfit()
    {
        return currentOutfit;
    }

    public void SetTemporaryOutfit(Character.Outfit outfit, bool keepOldHair = false, bool keepOldGlasses = false, bool keepOldFF = false)
    {
        isUsingTmpOutfit = true;
        tmpOutfit = outfit;
        keepHair = keepOldHair;
        keepGlasses = keepOldGlasses;
        keepFacialFeatures = keepOldFF;
        if (currentCharacter != null)
        {
            currentCharacter.InitAppearance(outfit, keepHair, keepGlasses, keepFacialFeatures);
        }
    }

    public void PutBackOldOutfit()
    {
        isUsingTmpOutfit = false;
        currentCharacter.InitAppearance(currentOutfit);
    }

    public void InitCharacterWithPlayerInfo(Character character)
    {
        character.InitAppearanceAndPersonality(currentOutfit);
        if (isUsingTmpOutfit)
        {
            character.InitAppearance(tmpOutfit, keepHair, keepGlasses, keepFacialFeatures);
            character.SetPersonality(currentOutfit.Personality, tmpOutfit.IsRobot, tmpOutfit.IsMask);
        }
        else
        {
            character.SetPersonality(currentOutfit.Personality, currentOutfit.IsRobot, currentOutfit.IsMask);
        }
    }

    public Checkpoint GetCheckPoint()
    {
        return checkPoint;
    }

    public RespawnPoint[] GetRespawnPoints()
    {
        return respawnPoints;
    }

    public void SetCheckpoint(Checkpoint newCheckPoint, RespawnPoint[] rezPoints)
    {
        checkPoint = newCheckPoint;
        SetRewpawnPoints(rezPoints);
    }

    public void Respawn(bool isFirstSpawn = false)
    {
        if (GameManager.Instance.GetLevelSelector().GetCurrentGameMode() == LevelSelector.eGameMode.VERSUS)
        {
            for (int i = 0; i < respawnPoints.Length; ++i)
            {
                if (respawnPoints[i].GetNumberPLayersInQueue() == 0)
                {
                    respawnPoints[i].AddPlayerToRespawnQueue(this);
                    if (isFirstSpawn)
                    {
                        respawnPoints[i].DontPlayStingerOnNextSpawn();
                    }
                    break;
                }
            }
        }
        else
        {
            if (respawnPoints.Length != 0 && respawnPoints[0].enabled)
            {
                respawnPoints[0].AddPlayerToRespawnQueue(this);
                if (isFirstSpawn)
                {
                    respawnPoints[0].DontPlayStingerOnNextSpawn();
                }
            }
        }
    }

    public void CancelRespawn()
    {
        if (respawnPoints.Length != 0 && respawnPoints[0].enabled)
        {
            respawnPoints[0].RemovePlayerFromRespawnQueue(this);
        }
    }

    public void ChangePlayerId(int newId)
    {
        controls.ListenPlayerInput(newId);
    }

    public void SetIsAssisted(bool state)
    {
        isAssisted = state;
    }

    public bool GetIsAssisted()
    {
        return isAssisted;
    }
}
