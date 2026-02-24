using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat1 : MonoBehaviour
{
    BasicCreatureScript this_creature;
    Animator animator;

    public GameObject apple;

    // Start is called before the first frame update
    void Start()
    {
        this_creature = gameObject.GetComponent<BasicCreatureScript>();
        animator = gameObject.GetComponent<Animator>();

        this_creature.CreateLifeBar(0, 45, 100, 20);
        this_creature.CreateXpBar(0, 45, 100, 20);
        this_creature.CreateStats(0, -50);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this_creature.item == false)
        {
            if (this_creature.GetActionTake() != "stop")
            {
                this_creature.TakeAction();

                if (this_creature.GetActionTake() == "atk1") Atk1();
                else if (this_creature.GetActionTake() == "atk2") Atk2();
            }

            if (this_creature.IsSold()) 
            {
                Skill();
            }
        }
    }

    void Atk1()
    {
        float animation_percent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (animation_percent >= 2 / 8f && animation_percent < 5 / 8f)
        {
            this_creature.Atk1ActiveHitBox(true);
            this_creature.SetVelocity(500 * Time.deltaTime * this_creature.GetLookSide(), 0);
        }
        else
        {
            this_creature.Atk1ActiveHitBox(false);
            this_creature.SetVelocity(0, 0);
        }

        if (this_creature.AnimationEnd())
        {
            this_creature.Atk1ActiveHitBox(false);
            this_creature.SetVelocity(0, 0);
            this_creature.ToIdle();
        }
    }

    void Atk2()
    {
        float animation_percent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (animation_percent >= 2 / 6f && animation_percent < 4 / 6f)
        {
            this_creature.Atk2ActiveHitBox(true);
            this_creature.SetVelocity(500 * Time.deltaTime * this_creature.GetLookSide(), 0);
        }
        else
        {
            this_creature.Atk2ActiveHitBox(false);
            this_creature.SetVelocity(0, 0);
        }

        if (this_creature.AnimationEnd())
        {
            this_creature.Atk2ActiveHitBox(false);
            this_creature.SetVelocity(0, 0);
            this_creature.ToIdle();
        }
    }

    void Skill()
    {
        BasicItemScript new_apple = BattleManager.Instance.SwitchThingsInStore(6, apple).GetComponent<BasicItemScript>();

        new_apple.atk_buff = this_creature.GetOrSetLevel();
        new_apple.special_buff = this_creature.GetOrSetLevel();
        new_apple.hp_buff = this_creature.GetOrSetLevel();
        new_apple.AddAndGetDiscount(3);
    }
}