using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PaintManager : MonoBehaviour {

    public int NumberMaxSplashes = 100;
    public int NumberMaxSplashesOnCharacterPart = 4;
    public int PaintMaterialOffset = 6;
    Queue<PaintSplash> splashes = new Queue<PaintSplash>();

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void AddNewSplashes(PaintSplash splash)
    {
        splashes.Enqueue(splash);
        if (splashes.Count > NumberMaxSplashes)
        {
            PaintSplash oldSplash = splashes.Dequeue();
            if (oldSplash != null)
            {
                oldSplash.Kill();
            }
        }
    }

    public void RemoveSplash(PaintSplash splash)
    {
        // Destroy instanced material
        var spriteRenderer = splash.GetSpriteRenderer();
        if (spriteRenderer.sharedMaterial.name.Contains("Instance"))
        {
            Destroy(spriteRenderer.material);
        }
        if (splashes.Contains(splash))
        {
            int queueSize = splashes.Count;
            while (queueSize > 0)
            {
                PaintSplash currentSplash = splashes.Dequeue();
                if (currentSplash != splash)
                {
                    splashes.Enqueue(currentSplash);
                }
                else
                {
                    Destroy(currentSplash.gameObject);
                }
                queueSize--;
            }
        }
        else
        {
            Destroy(splash.gameObject);
        }
    }

    public Queue<PaintSplash> GetQueuePaint()
    {
        return splashes;
    }

    public void ReplaceQueue(Queue<PaintSplash> paints)
    {
        splashes = paints;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        splashes.Clear();
    }
}
