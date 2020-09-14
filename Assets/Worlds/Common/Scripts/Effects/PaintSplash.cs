using UnityEngine;
using System.Collections.Generic;

public class PaintSplash : MonoBehaviour {

    public float LifeTime = 60f;
    public float FadeTime = 10f;

    bool isDisparearing = false;
    float timer = 0f;
    SpriteRenderer spriteRend = null;

    Color oldColor;
    Color endColor;

    void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        GameManager.Instance.GetPaintManager().AddNewSplashes(this);

        if (transform.parent.CompareTag("Character"))
        {
            Character character = Character.GetCharacterFromGameObject(transform.parent.gameObject);
            List<PaintSplash> oldSplash = new List<PaintSplash>();
            for (int i = 0; i < transform.parent.childCount; ++i)
            {
                if (transform.parent.GetChild(i).CompareTag("Paint") && transform.parent.GetChild(i) != transform)
                {
                    oldSplash.Add(transform.parent.GetChild(i).GetComponent<PaintSplash>());
                }
            }
            if (oldSplash.Count > GameManager.Instance.GetPaintManager().NumberMaxSplashesOnCharacterPart)
            {
                int nbToRemove = oldSplash.Count - GameManager.Instance.GetPaintManager().NumberMaxSplashesOnCharacterPart;
                while (nbToRemove > 0)
                { 
                    GameManager.Instance.GetPaintManager().RemoveSplash(oldSplash[0]);
                    oldSplash.RemoveAt(0);
                    nbToRemove--;
                }
            }
        }
    }

    void Update()
    {
        if (isDisparearing)
        {
            timer = Mathf.Min(timer + Time.deltaTime, FadeTime);
            spriteRend.color = Color.Lerp(oldColor, endColor, timer / FadeTime);
            if (timer == FadeTime)
            {
                timer = 0f;
                GameManager.Instance.GetPaintManager().RemoveSplash(this);
            }
        }
        else
        {
            timer = Mathf.Min(timer + Time.deltaTime, LifeTime);
            if (timer == LifeTime)
            {
                Kill();
            }
        }
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRend;
    }

    public void SetColor(Color color)
    {
        spriteRend.color = color;
        ParticleSystem.MainModule main = GetComponentInChildren<ParticleSystem>().main;
        main.startColor = spriteRend.color;
    }

    public void Kill()
    {
        if (!isDisparearing)
        {
            timer = 0f;
            isDisparearing = true;
            oldColor = spriteRend.color;
            endColor = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, 0f);
        }
    }
}
