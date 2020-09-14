using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleAnimator : MonoBehaviour
{
    public bool isRawDeltaTime = false;

    [SerializeField] 
    protected Sprite[] sprites;

    [SerializeField] 
    protected float frameRate = 10;

    private SpriteRenderer spriteRenderer;
    private float timeSinceLastFrame;
    private float frameTime;
    private int currentIndex = 0;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (sprites.Length != 0)
        {
            if (sprites[0] != null)
            {
                spriteRenderer.sprite = sprites[0];
            }
        }
        else
        {
            enabled = false;
        }

        frameTime = 1 / frameRate;
    }

    protected virtual void Update()
    {
        if (!spriteRenderer.isVisible)
        {
            return;
        }
        
        #if UNITY_EDITOR
        frameTime = 1 / frameRate;
        #endif

        timeSinceLastFrame += isRawDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;
        
        //If the game is using RawDeltaTime and unfocused/paused/SWITCH Applet is on, then the UnscaledDeltaTime
        //will get very high when returning to focus, which causes the animation to rush. 
        if (isRawDeltaTime && timeSinceLastFrame > frameTime * 2.0f)
        {
            timeSinceLastFrame = 0.0f;
        }

        if (timeSinceLastFrame >= frameTime)
        {
            currentIndex++;

            if (currentIndex >= sprites.Length)
            {
                currentIndex = 0;
            }

            spriteRenderer.sprite = sprites[currentIndex];

            timeSinceLastFrame  -= frameTime;
        }
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        var animator = GetComponent<Animator>();
        if (animator != null && animator.enabled)
        {
            var path = AssetDatabase.GetAssetPath(GetComponent<SpriteRenderer>().sprite);
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            sprites = assets.OfType<Sprite>().ToArray();
            animator.enabled = false;
        }
}
#endif
}