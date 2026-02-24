using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Vulture1 : MonoBehaviour
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

            if (can_use_eff == true) Skill();
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
            can_use_eff = true;
        }
    }

    void Atk2()
    {
        this_creature.Atk2ActiveHitBox(true);
        this_creature.SetVelocity(500 * Time.deltaTime * this_creature.GetLookSide(), 0);

        if (this_creature.AnimationEnd())
        {
            this_creature.Atk2ActiveHitBox(false);
            this_creature.SetVelocity(0, 0);
            this_creature.ToIdle();
            can_use_eff = true;
        }
    }

    bool can_use_eff = false;
    int damage = 5;

    void Skill()
    {
        List<GameObject> creatures = (this.tag == "Friend" ? BattleManager.Instance.GetEnemyList() : BattleManager.Instance.GetTeamList());
        List<GameObject> enemies = new List<GameObject>();

        foreach (GameObject creature in creatures)
        {
            if (creature != null && creature.GetComponent<BasicCreatureScript>().RemoveAndGetHp(0) > 0)
            {
                enemies.Add(creature);
            }
        }

        if (enemies.Count > 0)
        {
            this_creature.RemoveAndGetHpByEffect(damage * this_creature.GetOrSetLevel(), this.gameObject, enemies[UnityEngine.Random.Range(0, enemies.Count)]);
        }

        can_use_eff = false;
    }
}