using UnityEngine;

public class CatabombsLight : MonoBehaviour {

    public Sprite[] Sprites;
    public float IntervalBetweenSprites = 0.1f;

    float timer = 0f;
    SpriteRenderer[] masks;
    int currentIndex = 0;

    void Awake()
    {
        masks = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        timer = Mathf.Min(timer + Time.deltaTime, IntervalBetweenSprites);
        if (timer >= IntervalBetweenSprites)
        {
            foreach (SpriteRenderer mask in masks)
            {
                mask.sprite = Sprites[currentIndex];
            }

            currentIndex++;
            if (currentIndex == Sprites.Length)
            {
                currentIndex = 0;
            }

            timer = 0f;
        }
	}
}
