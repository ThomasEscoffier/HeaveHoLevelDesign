using UnityEngine;

public class NoobGloves : AAccessory
{
    public Sprite GloveOpen = null;
    public Sprite GloveClosed = null;
    public Sprite GlovePoint = null;

    public float TimeBeforeEffect = 2f;
    float timer = 0f;
    bool isClosedAnimLaunched = false;
    SpriteRenderer spriteRend = null;

    public override void Awake()
    {
        base.Awake();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    public override void OnStart(Hand hand)
    {
        parentHand = hand;
        sprite = GetComponent<SpriteRenderer>();
        float stencilID = sprite.material.GetFloat("_Stencil");
        sprite.material.SetFloat("_Stencil", stencilID + (parentHand.GetParentCharacter().GetPlayer().OrderInGame) * GameManager.Instance.GetPaintManager().PaintMaterialOffset);
        stencilID = sprite.material.GetFloat("_Stencil");
        if (!parentHand.IsLeft)
        {
            sprite.flipX = true;
        }
        parentHand.SetHandVisible(false);
    }

    public override void OnStart(HandTryOn hand)
    {
        uiParentHand = hand;
        sprite = GetComponent<SpriteRenderer>();
        if (!uiParentHand.IsLeft)
        {
            sprite.flipX = true;
        }
        uiParentHand.GetParentCharacter().SetHandVisible(uiParentHand, false);
    }

    public override void Update()
    {
        base.Update();

        if (((parentHand != null && !parentHand.GetIsPointing() && parentHand.GetIsClosed()) || (uiParentHand != null && !uiParentHand.GetIsPointing() && uiParentHand.GetIsClosed())) && !isClosedAnimLaunched)
        {
            timer = Mathf.Min(timer + Time.deltaTime, TimeBeforeEffect);
            if (timer == TimeBeforeEffect)
            {
                if (parentHand != null)
                {
                    parentHand.GetEffectAnimator().SetBool(parentHand.IsLeft ? "AssistLeft" : "AssistRight", true);
                }
                else if (uiParentHand != null)
                {
                    uiParentHand.GetEffectAnimator().SetBool(uiParentHand.IsLeft ? "AssistLeft" : "AssistRight", true);
                }
                timer = 0f;
                isClosedAnimLaunched = true;
            }
        }

        if (parentHand != null)
        {
            if (parentHand.GetIsPointing())
            {
                spriteRend.sprite = GlovePoint;
                isClosedAnimLaunched = false;
                parentHand.GetEffectAnimator().SetBool(parentHand.IsLeft ? "AssistLeft" : "AssistRight", false);
            }
            else if (parentHand.GetIsClosed())
            {
                spriteRend.sprite = GloveClosed;
            }
            else if (!parentHand.GetIsPointing() && !parentHand.GetIsClosed())
            {
                spriteRend.sprite = GloveOpen;
                isClosedAnimLaunched = false;
                timer = 0f;
                parentHand.GetEffectAnimator().SetBool(parentHand.IsLeft ? "AssistLeft" : "AssistRight", false);
            }
        }
        else if (uiParentHand != null)
        {
            if (uiParentHand.GetIsPointing())
            {
                spriteRend.sprite = GlovePoint;
                isClosedAnimLaunched = false;
                uiParentHand.GetEffectAnimator().SetBool(uiParentHand.IsLeft ? "AssistLeft" : "AssistRight", false);
            }
            else if (uiParentHand.GetIsClosed())
            {
                spriteRend.sprite = GloveClosed;
            }
            else if (!uiParentHand.GetIsPointing() && !uiParentHand.GetIsClosed())
            {
                spriteRend.sprite = GloveOpen;
                isClosedAnimLaunched = false;
                timer = 0f;
                uiParentHand.GetEffectAnimator().SetBool(uiParentHand.IsLeft ? "AssistLeft" : "AssistRight", false);
            }
        }

        if (isClosedAnimLaunched)
        {
            Vector3 direction = Vector3.up - transform.position;
            direction.Normalize();
            float newRot = Mathf.Atan2(Vector2.up.y, Vector2.up.x) * Mathf.Rad2Deg;
            if (parentHand != null && parentHand.GetEffectAnimator() != null)
            {
                parentHand.GetEffectAnimator().transform.rotation = Quaternion.Euler(0f, 0f, newRot - 90f);
            }
            else if (uiParentHand != null && uiParentHand.GetEffectAnimator() != null)
            {
                uiParentHand.GetEffectAnimator().transform.rotation = Quaternion.Euler(0f, 0f, newRot - 90f);
            }
        }
    }

    public override void OnStop()
    {
        if (parentHand != null)
        {
            parentHand.SetHandVisible(true);
        }
        if (uiParentHand)
        {
            uiParentHand.GetParentCharacter().SetHandVisible(uiParentHand, true);
        }
    }

    public override void OnStopUse() { }

    public override void OnUse() { }
}
