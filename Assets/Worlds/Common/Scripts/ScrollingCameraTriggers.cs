using System.Collections.Generic;
using UnityEngine;

public class ScrollingCameraTriggers : MonoBehaviour {

    public Collider2D TriggerUp = null;
    public Collider2D TriggerLeft = null;
    public Collider2D TriggerRight = null;
    public Collider2D TriggerDown = null;

    LevelManager levelManager = null;
    ScrollingRules scrollingRules = null;
    List<Character> charactersInTrigger = new List<Character>();

    bool isSpeedMax = false;
    int currentOrder = -1;

    bool startLimitUp = false;
    bool startLimitLeft = false;
    bool startLimitRight = false;
    bool startLimitDown = false;

    public enum eDirection
    {
        NONE,
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
    eDirection currentDirection = eDirection.NONE;

    void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
        startLimitUp = levelManager.IsNotUsingUpLimit;
        startLimitLeft = levelManager.IsNotUsingLeftLimit;
        startLimitRight = levelManager.IsNotUsingRightLimit;
        startLimitDown = levelManager.IsNotUsingDownLimit;
    }

    void Update()
    {
        charactersInTrigger.RemoveAll(i => i == null);

        if (!isSpeedMax && GameManager.Instance.GetNbPlayersAlive() > 0 && charactersInTrigger.Count == GameManager.Instance.GetNbPlayersAlive())
        {
            scrollingRules.SetMaxSpeed();
            isSpeedMax = true;
        }
        else if (isSpeedMax && (GameManager.Instance.GetNbPlayersAlive() == 0 || charactersInTrigger.Count < GameManager.Instance.GetNbPlayersAlive()))
        {
            scrollingRules.SetNormalSpeed();
            isSpeedMax = false;
        }
    }

    public void SetScrollingRules(ScrollingRules rules)
    {
        scrollingRules = rules;
    }

    public void ActivateTrigger(ScrollingTriggerDirection direction)
    {
        if (direction == null || direction.Direction == currentDirection || (!scrollingRules.GetIsRewinding() && direction.Order < currentOrder) || (scrollingRules.GetIsRewinding() && direction.Order > currentOrder))
        {
            return;
        }

        TriggerUp.enabled = false;
        TriggerLeft.enabled = false;
        TriggerRight.enabled = false;
        TriggerDown.enabled = false;

        levelManager.IsNotUsingUpLimit = startLimitUp;
        levelManager.IsNotUsingLeftLimit = startLimitLeft;
        levelManager.IsNotUsingRightLimit = startLimitRight;
        levelManager.IsNotUsingDownLimit = startLimitDown;

        switch (direction.Direction)
        {
            case eDirection.UP:
                TriggerUp.enabled = true;
                levelManager.IsNotUsingUpLimit = true;
                break;
            case eDirection.LEFT:
                TriggerLeft.enabled = true;
                levelManager.IsNotUsingLeftLimit = true;
                break;
            case eDirection.RIGHT:
                TriggerRight.enabled = true;
                levelManager.IsNotUsingRightLimit = true;
                break;
            case eDirection.DOWN:
                TriggerDown.enabled = true;
                levelManager.IsNotUsingDownLimit = true;
                break;
            case eDirection.NONE:
            default:
                break;
        }

        currentDirection = direction.Direction;
        currentOrder = direction.Order;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {       
        if (collision.CompareTag("Character"))
        {
            Character character = collision.GetComponent<Character>();
            if (character != null && !charactersInTrigger.Contains(character))
            {
                charactersInTrigger.Add(character);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            Character character = collision.GetComponent<Character>();
            if (character != null)
            {
                charactersInTrigger.Remove(character);
            }
        }
    }

    public void SetPermanentNotUsingLimitUp(bool state)
    {
        startLimitUp = state;
        if (currentDirection != eDirection.UP)
        {
            levelManager.IsNotUsingUpLimit = state;
        }
    }

    public void SetPermanentNotUsingLimitDown(bool state)
    {
        startLimitDown = state;
        if (currentDirection != eDirection.DOWN)
        {
            levelManager.IsNotUsingDownLimit = state;
        }
    }

    public void SetPermanentNotUsingLimitLeft(bool state)
    {
        startLimitLeft = state;
        if (currentDirection != eDirection.LEFT)
        {
            levelManager.IsNotUsingLeftLimit = state;
        }
    }

    public void SetPermanentNotUsingLimitRight(bool state)
    {
        startLimitRight = state;
        if (currentDirection != eDirection.RIGHT)
        {
            levelManager.IsNotUsingRightLimit = state;
        }
    }
}
