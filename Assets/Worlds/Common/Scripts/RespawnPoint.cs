using System;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour, IComparable<RespawnPoint> {

    Animator anim = null;

    public int Priority = 0;

    public float RespawnInterval = 1f;
    public float TimeBeforeRetrySpawn = 0.2f;
    float timer = 0f;

    public float TimeRespawnEffect = 0.5f;
    float timerRespawnEffect = 0f;

    public float ForcePushCharacters = 1000f;
    public float TimeDisableHandsWhenPushed = 0.5f;
    public float TimeDisableHandsWhenSpawning = 0.5f;

    public bool SpawnOnlyOnce = false;

    bool isPaused = false;
    bool dontPlayStinger = false;

    bool isEffectLaunched = false;
    List<BodyPart> bodyPartsInTrigger = new List<BodyPart>();

    protected Queue<Player> poolPlayersToRewpawn = new Queue<Player>();

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public int CompareTo(RespawnPoint other)
    {
        return Priority.CompareTo(other.Priority);
    }

    public virtual Character Respawn(Player player)
    {
        if (player.GetCurrentCharacter() != null)
            return null;

        Character newCharacter = Instantiate(player.BaseCharacterPrefab, transform.position, transform.rotation);
        player.InitCharacterWithPlayerInfo(newCharacter);
        player.SetCurrentCharacter(newCharacter);
        newCharacter.SetPlayer(player);

        newCharacter.LeftHand.DisableHand(TimeDisableHandsWhenSpawning);
        newCharacter.RightHand.DisableHand(TimeDisableHandsWhenSpawning);

        if (SpawnOnlyOnce)
        {
            SetIsPaused(true);
        }

        return newCharacter;
    }

    public void AddPlayerToRespawnQueue(Player player)
    {
        if (!poolPlayersToRewpawn.Contains(player))
        {
            poolPlayersToRewpawn.Enqueue(player);
        }
    }

    public void RemovePlayerFromRespawnQueue(Player player)
    {
        Queue<Player> newQueue = new Queue<Player>();
        while (poolPlayersToRewpawn.Count > 0)
        {
            Player currentPlayer = poolPlayersToRewpawn.Dequeue();
            if (currentPlayer != player)
            {
                newQueue.Enqueue(currentPlayer);
            }
        }
        poolPlayersToRewpawn = newQueue;
    }

    public int GetNumberPLayersInQueue()
    {
        return poolPlayersToRewpawn.Count;
    }

    void Start()
    {
        timer = RespawnInterval; 
    }

    void Update()
    {
        if (isPaused)
            return;

        if (poolPlayersToRewpawn.Count > 0)
        {
            if (poolPlayersToRewpawn.Peek() == null)
            {
                poolPlayersToRewpawn.Dequeue();
                timer = 0f;
                timerRespawnEffect = 0f;
                isEffectLaunched = false;
            }

            timer = Mathf.Min(timer + Time.deltaTime, RespawnInterval);
            if (timer >= RespawnInterval)
            {
                if (bodyPartsInTrigger.Count > 0)
                {
                    timer -= TimeBeforeRetrySpawn;
                    List<Character> checkedCharacters = new List<Character>();
                    foreach (BodyPart part in bodyPartsInTrigger)
                    {
                        Character character = Character.GetCharacterFromGameObject(part.gameObject);
                        if (!checkedCharacters.Contains(character))
                        {
                            if (character.LeftHand != null)
                            {
                                character.LeftHand.UnHook();
                                character.LeftHand.DisableHand(TimeDisableHandsWhenPushed);
                            }
                            if (character.RightHand != null)
                            {
                                character.RightHand.UnHook();
                                character.RightHand.DisableHand(TimeDisableHandsWhenPushed);
                            }
                            character.Push((transform.position - character.transform.position) * ForcePushCharacters);
                            checkedCharacters.Add(character);
                        }
                    }
                }
                else if (!isEffectLaunched)
                {
                    isEffectLaunched = true;
                    anim.SetTrigger("Respawn");
                }
                else
                {
                    timerRespawnEffect = Mathf.Min(timerRespawnEffect + Time.deltaTime, TimeRespawnEffect);
                    if (timerRespawnEffect >= TimeRespawnEffect)
                    {
                        timer = 0f;
                        timerRespawnEffect = 0f;
                        isEffectLaunched = false;
                        Player player = poolPlayersToRewpawn.Dequeue();
                        Character character = Respawn(player);

                        if (GameManager.Instance.GetMusicManager().GetIsDynamic())
                        {
                            if (dontPlayStinger)
                            {
                                dontPlayStinger = false;
                            }
                            else
                            {
                                GameManager.Instance.GetMusicManager().LaunchRespawnMusicEvent();
                            }
                        }
                    }
                }
            }
        }
    }

    public void DontPlayStingerOnNextSpawn()
    {
        dontPlayStinger = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            BodyPart part = collision.GetComponent<BodyPart>();
            if (part != null && !bodyPartsInTrigger.Contains(part))
            {
                bodyPartsInTrigger.Add(part);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            BodyPart part = collision.GetComponent<BodyPart>();
            if (part != null)
            {
                bodyPartsInTrigger.Remove(part);
            }
        }
    }

    public void RemoveCharacterFromTrigger(BodyPart part)
    {
        bodyPartsInTrigger.Remove(part);
    }

    public void SetIsPaused(bool state)
    {
        isPaused = state;
    }
}
