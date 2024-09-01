using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class sBattleAnimator : MonoBehaviour
{
    private Animator anim;
    public bool animationFinished = true;

    private GameObject errorParticleEffect;


    void Start()
    {
        anim = GetComponent<Animator>();
        errorParticleEffect = GameManager.instance.gameObjectDictionary.GetValueOrDefault("DAMAGE_FX");
    }

    public void PlayAttackAnimation(BattleChoice choices)
    {
        Vector3 target = choices.Target.animator.gameObject.transform.position;
        animationFinished = false;
        print("play attack animation");
        transform.LookAt(target);
        StartCoroutine(PlayAnimationCoroutine(choices));
    }

    public void PlaySpellAnimation(BattleChoice choices)
    {
        Vector3 target = choices.Target.animator.gameObject.transform.position;
        animationFinished = false;
        print("play animation for: " + choices.Spell.Name);
        transform.LookAt(target);
        StartCoroutine(PlayAnimationCoroutine(choices));
    }

    public void PlayItemAnimation(BattleChoice choices)
    {
        Vector3 target = choices.Target.animator.gameObject.transform.position;
        animationFinished = false;
        print("play animation for: " + choices.Item.Name);
        transform.LookAt(target);
        StartCoroutine(PlayAnimationCoroutine(choices));
    }

    public void PlayDeathAnimation()
    {
        //dummy death indicator
        transform.localScale = Vector3.one * 0.1f;

        animationFinished = false;
        StartCoroutine(PlayDeathCoroutine());
    }

    public IEnumerator PlayAnimationCoroutine(BattleChoice choices)
    {
        //anim.SetInteger(1, 1);

        switch (choices.Type)
        {
            // Eventually this will get moved to a separate function called at a keyframe in the animation
            case BattleChoiceEnum.ATTACK:
                // nothing?
                break;
            case BattleChoiceEnum.SPELL:
                if (GameManager.instance.gameObjectDictionary.TryGetValue(choices.Spell.PrefabDictionaryName, out GameObject gSO))
                {
                    Instantiate(gSO, choices.TargetPosition, Quaternion.identity, null);
                }
                else
                {
                    Debug.LogWarning("COULD NOT GET SPELL FX GO!");
                }
                break;
            case BattleChoiceEnum.ITEM:
                if (GameManager.instance.gameObjectDictionary.TryGetValue(choices.Item.PrefabDictionaryName, out GameObject gIO))
                {
                    Instantiate(gIO, choices.TargetPosition, Quaternion.identity, null);
                }
                else
                {
                    Debug.LogWarning("COULD NOT GET ITEM FX GO!");
                }
                break;
            default:
                break;
        }

        yield return null;
        yield return new WaitForSeconds(1f);
        animationFinished = true;

        //reset
        //ResetForNextAnimation();
    }
    public IEnumerator PlayDeathCoroutine()
    {
        //anim.SetInteger(1, 1);
        yield return null;
        yield return new WaitForSeconds(1f);
        animationFinished = true;
    }    

    private void ResetForNextAnimation()
    {
        transform.LookAt(Vector3.zero);
    }
}

// - - - Pattern for Animation Coroutines

//anim.SetInteger(1, 1);

// Wait one frame to let the animator enter the next animation first
//yield return null;

// From here, wait for the new animation to finish
// - animation must be on first layer
// - animation must have 0 transition time, else we must also check !m_Animator.IsInTransition(0)
//yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
//yield return new WaitForSeconds(1f);

//animationFinished = true;