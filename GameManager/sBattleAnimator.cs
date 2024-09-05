using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sBattleAnimator : MonoBehaviour
{
    public bool AnimationFinished = true;
    public Transform EnemyVantage;
    public Vector3 MeshSize;

    private Animator anim;
    private BattleChoice currentBattleChoice;
    private GameObject errorParticleEffect;
    private float timeInAttack;
    private Vector3 lookTarget;
    private Vector3 attackTarget;
    [SerializeField] private Transform meshRootTransform;


    void Start()
    {
        anim = GetComponent<Animator>();
        MeshSize = GetComponentInChildren<MeshFilter>().mesh.bounds.size;
        errorParticleEffect = GameManager.instance.gameObjectDictionary.GetValueOrDefault("DAMAGE_FX");
    }
    private void Update()
    {
        if (!AnimationFinished)
        {
            if (currentBattleChoice.Type == BattleChoiceTypeEnum.ATTACK)
            {
                timeInAttack += Time.deltaTime;
                meshRootTransform.LookAt(lookTarget);
                meshRootTransform.position = transform.position + (new Vector3(
                    AnimationCurves.ATTACK_CURVE.Evaluate(timeInAttack) * (attackTarget.x - transform.position.x), 
                    0f,
                    AnimationCurves.ATTACK_CURVE.Evaluate(timeInAttack) * (attackTarget.z - transform.position.z))
                );
            }
            else if (currentBattleChoice.Type == BattleChoiceTypeEnum.SPELL)
            {
                //SPELL POSITIONING AND LOOK
            }
            else if (currentBattleChoice.Type == BattleChoiceTypeEnum.ITEM)
            {
                //ITEM POSITIONING AND LOOK
            }
        }
        else
        {
            meshRootTransform.rotation = transform.rotation;
        }
    }

    public void PlayAttackAnimation(BattleChoice choices)
    {
        currentBattleChoice = choices;
        timeInAttack = 0f;
        AnimationFinished = false;

        lookTarget = choices.Target.animator.gameObject.transform.position;
        attackTarget = choices.Target.animator.EnemyVantage.position;

        //print("play attack animation");
        StartCoroutine(PlayAnimationCoroutine(choices));
    }

    public void PlaySpellAnimation(BattleChoice choices)
    {
        currentBattleChoice = choices;
        timeInAttack = 0f;
        AnimationFinished = false;

        lookTarget = choices.Target.animator.gameObject.transform.position;
        attackTarget = choices.Target.animator.EnemyVantage.position;

        AnimationFinished = false;
        print("play animation for: " + choices.Spell.Name);
        StartCoroutine(PlayAnimationCoroutine(choices));
    }

    public void PlayItemAnimation(BattleChoice choices)
    {
        currentBattleChoice = choices;
        timeInAttack = 0f;
        AnimationFinished = false;

        lookTarget = choices.Target.animator.gameObject.transform.position;
        attackTarget = choices.Target.animator.EnemyVantage.position;

        AnimationFinished = false;
        print("play animation for: " + choices.Item.Name);
        StartCoroutine(PlayAnimationCoroutine(choices));
    }

    public void PlayDeathAnimation()
    {
        //dummy death indicator
        transform.localScale = Vector3.zero;

        //AnimationFinished = false;
        //StartCoroutine(PlayDeathCoroutine());
    }

    public IEnumerator PlayAnimationCoroutine(BattleChoice choices)
    {
        //anim.SetInteger(1, 1);

        switch (choices.Type)
        {
            // Eventually this will get moved to a separate function called at a keyframe in the animation
            case BattleChoiceTypeEnum.ATTACK:
                // nothing?
                break;
            case BattleChoiceTypeEnum.SPELL:
                if (GameManager.instance.gameObjectDictionary.TryGetValue(choices.Spell.PrefabDictionaryName, out GameObject gSO))
                {
                    Instantiate(gSO, choices.TargetPosition, Quaternion.identity, null);
                }
                else
                {
                    Debug.LogWarning("COULD NOT GET SPELL FX GO!");
                }
                break;
            case BattleChoiceTypeEnum.ITEM:
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
        AnimationFinished = true;

        //reset
        //ResetForNextAnimation();
    }

    public IEnumerator PlayDeathCoroutine()
    {
        //anim.SetInteger(1, 1);
        yield return null;
        yield return new WaitForSeconds(1f);
        AnimationFinished = true;
    }    

    private void ResetForNextAnimation()
    {
        //meshRootTransform.position = transform.position;
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