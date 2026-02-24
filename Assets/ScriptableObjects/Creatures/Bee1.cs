using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee1 : MonoBehaviour
{
    BasicCreatureScript this_creature;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        this_creature = gameObject.GetComponent<BasicCreatureScript>();
        animator = gameObject.GetComponent<Animator>();

        this_creature.CreateLifeBar(0, 30, 100, 20);
        this_creature.CreateXpBar(0, 30, 100, 20);
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
        }
    }

    void Atk1()
    {
        this_creature.Atk1ActiveHitBox(true);
        this_creature.SetVelocity(500 * Time.deltaTime * this_creature.GetLookSide(), 0);

        if (this_creature.AnimationEnd())
        {
            this_creature.Atk1ActiveHitBox(false);
            this_creature.SetVelocity(0, 0);
            this_creature.ToIdle();
        }
    }

    bool can_atk2 = true;

    void Atk2()
    {
        if (can_atk2 == true && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.25f)
        {
            ProjectileScript projectile = this_creature.CreateProjectile(this_creature.AddAndGetSpecial(0), 600, 1);
            can_atk2 = false;
        }

        if (this_creature.AnimationEnd())
        {
            can_atk2 = true;
            this_creature.ToIdle();
        }
    }
}