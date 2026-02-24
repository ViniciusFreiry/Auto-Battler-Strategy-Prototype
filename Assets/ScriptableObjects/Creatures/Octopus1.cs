using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Octopus1 : MonoBehaviour
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

                if (this_creature.GetActionTake() == "atk1")
                {
                    Skill();
                    Atk1();
                }
                else if (this_creature.GetActionTake() == "atk2")
                {
                    Skill();
                    Atk2();
                }
                else
                {
                    can_use_eff = true;
                }
            }
        }
    }

    void Atk1()
    {
        float animation_percent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (animation_percent >= 3 / 6f && animation_percent < 5 / 6f)
        {
            this_creature.Atk1ActiveHitBox(true);
            this_creature.SetVelocity(500 * Time.deltaTime * this_creature.GetLookSide(), 0);
        }
        else
        {
            this.this_creature.Atk1ActiveHitBox(false);
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

        if (animation_percent >= 3 / 6f)
        {
            this_creature.Atk2ActiveHitBox(true);
            this_creature.SetVelocity(500 * Time.deltaTime * this_creature.GetLookSide(), 0);
        }

        if (this_creature.AnimationEnd())
        {
            this_creature.Atk2ActiveHitBox(false);
            this_creature.SetVelocity(0, 0);
            this_creature.ToIdle();
        }
    }

    bool can_use_eff = true;

    void Skill()
    {
        if (can_use_eff == true)
        {
            List<GameObject> enemies = (this.tag == "Friend" ? BattleManager.Instance.GetEnemyList() : BattleManager.Instance.GetTeamList());
            List<GameObject> aims = new List<GameObject>();

            foreach (GameObject enemy in enemies)
            {
                if (enemy != null && enemy.GetComponent<BasicCreatureScript>().RemoveAndGetHp(0) > 0)
                {
                    aims.Add(enemy);
                }
            }

            if (aims.Count > 0)
            {
                this_creature.RemoveAndGetHpByEffect(this_creature.AddAndGetSpecial(0) * this_creature.GetOrSetLevel(), this.gameObject, aims[UnityEngine.Random.Range(0, aims.Count)]);
            }

            can_use_eff = false;
        }
    }
}