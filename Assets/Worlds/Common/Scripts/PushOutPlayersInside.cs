using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushOutPlayersInside : MonoBehaviour {

    public float TimeIntervalPush = 1f;
    float timerPush = 0f;
    public float ForcePushCharacters = 1000f;
    public bool DoesUnhookHands = false;
    public float TimeDisableHandsWhenPushed = 0.5f;

    bool isStarted = false;

    List<BodyPart> bodyPartsInTrigger = new List<BodyPart>();

    void Update()
    {
        if (!isStarted)
            return;

        timerPush = Mathf.Max(timerPush - Time.deltaTime, 0);
        if (timerPush <= 0 && bodyPartsInTrigger.Count > 0)
        {
            timerPush = TimeIntervalPush;
            List<Character> checkedCharacters = new List<Character>();
            foreach (BodyPart part in bodyPartsInTrigger)
            {
                if (part == null)
                {
                    continue;
                }
                Character character = Character.GetCharacterFromGameObject(part.gameObject);
                if (!checkedCharacters.Contains(character))
                {
                    if (DoesUnhookHands)
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
                    }
                    character.Push((character.transform.position - transform.position) * ForcePushCharacters);
                    checkedCharacters.Add(character);
                }
            }
        }
    }

    public void StartCheckTrigger()
    {
        timerPush = TimeIntervalPush;
        isStarted = true;
    }

    public void StopCheckTrigger()
    {
        isStarted = false;
    }

    public bool GetHasPlayersInside()
    {
        return bodyPartsInTrigger.Count > 0;
    }

    public bool GetIsStarted()
    {
        return isStarted;
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
}
