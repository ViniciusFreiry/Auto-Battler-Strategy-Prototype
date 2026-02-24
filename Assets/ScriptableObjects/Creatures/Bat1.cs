using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat1 : MonoBehaviour
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

            Skill();
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

    void Atk2()
    {
        float animation_percent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (animation_percent >= 2 / 7f && animation_percent < 5 / 7f)
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

    bool eff_used = false;

    void Skill()
    {
        if (BattleManager.Instance.GetState() == "battle_phase" && this_creature.RemoveAndGetHp(0) <= 0)
        {
            if (eff_used == false)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.tag == "Friend" ? "Enemy" : "Friend");
                eff_used = true;

                foreach (GameObject enemy in enemies)
                {
                    BasicCreatureScript script = enemy.GetComponent<BasicCreatureScript>();

                    if (script.RemoveAndGetHp(0) > 0)
                    {
                        script.RemoveAndGetHpByEffect(Mathf.Max(1, this_creature.AddAndGetSpecial(0) / (4 - this_creature.GetOrSetLevel())), this.gameObject, enemy);
                    }
                }
            }
        }
        else
        {
            eff_used = false;
        }
    }
}