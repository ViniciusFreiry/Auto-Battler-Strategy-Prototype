using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swordfish1 : MonoBehaviour
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

            this_creature.StartBattleEff();
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
        this_creature.Atk2ActiveHitBox(true);
        this_creature.SetVelocity(500 * Time.deltaTime * this_creature.GetLookSide(), 0);

        if (this_creature.AnimationEnd())
        {
            this_creature.Atk2ActiveHitBox(false);
            this_creature.SetVelocity(0, 0);
            this_creature.ToIdle();
        }
    }

    void Skill()
    {
        if (BattleManager.Instance.GetState() == "item_phase")
        {
            List<GameObject> enemies = (this.tag == "Friend" ? BattleManager.Instance.GetEnemyList() : BattleManager.Instance.GetTeamList());
            int most_helth = 0;
            List<GameObject> aims = new List<GameObject>();

            foreach (GameObject enemy in enemies)
            {
                if (enemy != null && enemy.GetComponent<BasicCreatureScript>().RemoveAndGetHp(0) > most_helth) most_helth = enemy.GetComponent<BasicCreatureScript>().RemoveAndGetHp(0);
            }

            foreach (GameObject enemy in enemies)
            {
                if (enemy != null && enemy.GetComponent<BasicCreatureScript>().RemoveAndGetHp(0) == most_helth)
                {
                    aims.Add(enemy);
                }
            }

            if (aims.Count > 0) this_creature.RemoveAndGetHpByEffect(this_creature.AddAndGetSpecial(0) * this_creature.GetOrSetLevel(), this.gameObject, aims[UnityEngine.Random.Range(0, aims.Count)]);
            this_creature.RemoveAndGetHpByEffect(this_creature.AddAndGetSpecial(0) * this_creature.GetOrSetLevel(), this.gameObject, this.gameObject);
        }
    }
}