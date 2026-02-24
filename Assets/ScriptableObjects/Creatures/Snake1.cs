using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Snake1 : MonoBehaviour
{
    BasicCreatureScript this_creature;
    Animator animator;

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

            Skill();
        }
    }

    void Atk1()
    {
        float animation_percent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (animation_percent >= 3 / 6f && animation_percent < 5 / 6f)
        {
            this_creature.Atk1ActiveHitBox(true);
            this_creature.SetVelocity(1000 * Time.deltaTime * this_creature.GetLookSide(), 0);
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
        this_creature.Atk2ActiveHitBox(true);
        this_creature.SetVelocity(500 * Time.deltaTime * this_creature.GetLookSide(), 0);

        if (this_creature.AnimationEnd())
        {
            this_creature.Atk2ActiveHitBox(false);
            this_creature.SetVelocity(0, 0);
            this_creature.ToIdle();
        }
    }

    bool can_use_eff = true;
    int damage = 8;

    void Skill()
    {
        GameObject creature_in_front = null;
        List<GameObject> team = (this.tag == "Friend" ? BattleManager.Instance.GetTeamList() : BattleManager.Instance.GetEnemyList());
        bool can_add = false;

        switch (this.tag)
        {
            case "Friend":

                for (int i = 0; i < team.Count; i++)
                {
                    if (team[i] != null)
                    {
                        if (can_add == true && team[i].GetComponent<BasicCreatureScript>().RemoveAndGetHp(0) > 0)
                        {
                            creature_in_front = team[i];
                            break;
                        }

                        if (team[i] == this.gameObject)
                        {
                            can_add = true;
                        }
                    }
                }
                break;

            case "Enemy":
                for (int i = 4; i >= 0; i--)
                {
                    if (team[i] != null)
                    {
                        if (can_add == true && team[i].GetComponent<BasicCreatureScript>().RemoveAndGetHp(0) > 0)
                        {
                            creature_in_front = team[i];
                            break;
                        }

                        if (team[i] == this.gameObject)
                        {
                            can_add = true;
                        }
                    }
                }
                break;
        }

        if (creature_in_front != null && (creature_in_front.GetComponent<BasicCreatureScript>().GetActionTake() == "atk1" || creature_in_front.GetComponent<BasicCreatureScript>().GetActionTake() == "atk2"))
        {
            if (can_use_eff == true) {
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
        else
        {
            can_use_eff = true;
        }
    }
}