using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class DebugMode : MonoBehaviour {

    public Player PlayerPrefab = null;

    protected Rewired.Player SystemInput = null;

    LevelSelector levelSelector = null;

    void Awake()
    {
        ListenSystemId();
        levelSelector = FindObjectOfType<LevelSelector>();
    }

    void ListenSystemId()
    {
        SystemInput = ReInput.players.SystemPlayer;
    }

    void Update()
    {
//#if UNITY_EDITOR
        //Global cheats
            if (SystemInput.GetButtonDown(RewiredConsts.Action.Cheats_NextLevel))
            {
                if (levelSelector.GetIsRandomMode())
                {
                    levelSelector.SelectRandomNextLevel();
                }
                else
                {
                    levelSelector.SelectNextLevel();
                }
            }
            if (SystemInput.GetButtonDown(RewiredConsts.Action.Cheats_ReloadLevel))
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
            }
            if (SystemInput.GetButtonDown(RewiredConsts.Action.Cheats_AddPlayer))
            {
                CreatePlayer();
            }

            if (SystemInput.GetButtonDown(RewiredConsts.Action.Cheats_StartRandomEvent))
            {
                RandomLevelEventsManager randomManager = FindObjectOfType<RandomLevelEventsManager>();
                randomManager.StopAllEvents();
                randomManager.PlayNextEvent();
            }

        //Cheats on one player
        foreach (Rewired.Player rewiredPlayer in ReInput.players.GetPlayers())
        {
            if (rewiredPlayer.GetButtonDown("ChangeControlPlayer"))
            {
                ChangePlayerControl(rewiredPlayer.id);
            }

            if (rewiredPlayer.GetButtonDown("BlockLeftHand"))
            {
                Player player = GameManager.Instance.GetPlayerFromId(rewiredPlayer.id);
                if (GameManager.Instance.GetIsInSelection())
                {
                    CharacterPresetTryOn[] characters = FindObjectsOfType<CharacterPresetTryOn>();
                    for (int i = 0; i < characters.Length; ++i)
                    {
                        if (characters[i].gameObject.activeSelf && characters[i].GetPlayerId() == rewiredPlayer.id)
                        {
                            characters[i].GetLeftHand().debugBlockHand = !characters[i].GetLeftHand().debugBlockHand;
                        }
                    }
                }
                else
                {
                    Character character = player.GetCurrentCharacter();
                    if (character != null && character.LeftHand != null && character.LeftHand.GetIsHooked())
                    {
                        character.LeftHand.debugBlockHand = !character.LeftHand.debugBlockHand;
                    }
                }
            }
            if (rewiredPlayer.GetButtonDown("BlockRightHand"))
            {
                Player player = GameManager.Instance.GetPlayerFromId(rewiredPlayer.id);
                if (GameManager.Instance.GetIsInSelection())
                {
                    CharacterPresetTryOn[] characters = FindObjectsOfType<CharacterPresetTryOn>();
                    for (int i = 0; i < characters.Length; ++i)
                    {
                        if (characters[i].gameObject.activeSelf && characters[i].GetPlayerId() == rewiredPlayer.id)
                        {
                            characters[i].GetRightHand().debugBlockHand = !characters[i].GetRightHand().debugBlockHand;
                        }
                    }
                }
                else
                {
                    Character character = player.GetCurrentCharacter();
                    if (character != null && character.RightHand != null && character.RightHand.GetIsHooked())
                    {
                        character.RightHand.debugBlockHand = !character.RightHand.debugBlockHand;
                    }
                }
            }
        }
//#endif
    }

    void OnEnable()
    {
        foreach (Rewired.Player rewiredPlayer in ReInput.players.GetPlayers())
        {
            rewiredPlayer.controllers.maps.SetMapsEnabled(true, "Cheats");
        }
    }

    void OnDisable()
    {
        foreach (Rewired.Player rewiredPlayer in ReInput.players.GetPlayers())
        {
            rewiredPlayer.controllers.maps.SetMapsEnabled(false, "Cheats");
        }
    }

    public void CreatePlayer()
    {
        if (GameManager.Instance.GetIsInSelection())
        {
            List<Player> players = new List<Player>();
            for (int i = 0; i < GameManager.Instance.GetSelectionMenu().PlayersSelections.Length; ++i)
            {
                if (GameManager.Instance.GetSelectionMenu().PlayersSelections[i].GetCurrentPlayer() != null)
                {
                    players.Add(GameManager.Instance.GetSelectionMenu().PlayersSelections[i].GetCurrentPlayer());
                }
            }
            if (players.Count < GameManager.Instance.GetSelectionMenu().PlayersSelections.Length)
            {
                Player player = Instantiate(PlayerPrefab);
                if (GameManager.Instance.GetSelectionMenu().SetUnusedId(player))
                {
                    PlayerMenuSelection menu = GameManager.Instance.GetSelectionMenu().GetPlayerMenuFromId(player.GetControls().playerId);
                    menu.SetPlayer(player);
                }
            }
        }
        else
        {
            int newPlayerId = GameManager.Instance.GetUnusedPlayerIdInGame();
            if (newPlayerId != -1)
            {
                Player player = Instantiate(PlayerPrefab);
                player.OrderInGame = GameManager.Instance.GetUnusedOrderInGame();
                player.GetControls().ListenPlayerInput(newPlayerId);
                GameManager.Instance.AddPlayer(player);
                LevelManager levelManager = FindObjectOfType<LevelManager>();
                levelManager.LevelRules.InitPlayer(player);

                player.SetCharacterOutfit(Character.LoadOutfitFromPreset(GameManager.Instance.GetCharactersPresets().GetRandomOutfit(GameManager.Instance.GetSaveManager().GameSaveData.UnlockedOutfits)));
                player.Respawn();
            }
        }
    }

    void ChangePlayerControl(int rewiredPlayerId)
    {
        if (GameManager.Instance.GetIsInSelection())
        {
            List<Player> players = new List<Player>();
            for (int i = 0; i < GameManager.Instance.GetSelectionMenu().PlayersSelections.Length; ++i)
            {
                if (GameManager.Instance.GetSelectionMenu().PlayersSelections[i].GetCurrentPlayer() != null)
                {
                    players.Add(GameManager.Instance.GetSelectionMenu().PlayersSelections[i].GetCurrentPlayer());
                }
            }
            SwitchPlayerControls(rewiredPlayerId, players);
            for (int i = 0; i < GameManager.Instance.GetSelectionMenu().PlayersSelections.Length; ++i)
            {
                Player player = GameManager.Instance.GetSelectionMenu().PlayersSelections[i].GetCurrentPlayer();
                if (player != null)
                {
                    GameManager.Instance.GetSelectionMenu().PlayersSelections[i].ListenToId(player.GetControls().GetPlayerId());
                }
            }
        }
        else
        {
            SwitchPlayerControls(rewiredPlayerId, GameManager.Instance.GetPlayers());
        }
    }

    void SwitchPlayerControls(int rewiredPlayerId, List<Player> players)
    {
        if (players.Count <= 1)
            return;

        Player currentPlayer = null;
        foreach (Player player in players)
        {
            if (player.GetControls().GetPlayerId() == rewiredPlayerId)
            {
                currentPlayer = player;
            }
        }
        int index = players.IndexOf(currentPlayer);
        index++;
        if (index >= players.Count)
        {
            index = 0;
        }

        if (currentPlayer != null)
        {
            int previousId = currentPlayer.GetControls().GetPlayerId();
            currentPlayer.ChangePlayerId(players[index].GetControls().GetPlayerId());
            players[index].ChangePlayerId(previousId);
        }
    }
}
