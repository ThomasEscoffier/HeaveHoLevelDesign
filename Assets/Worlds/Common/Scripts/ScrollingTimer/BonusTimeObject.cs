using UnityEngine;

public class BonusTimeObject : MonoBehaviour
{
    public float AddedTime = 5f;
    ScrollingTimerRules rules;

    public void SetRules(ScrollingTimerRules newRules)
    {
        rules = newRules;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            rules.AddTime(AddedTime);
            Destroy(gameObject);
        }
    }
}
