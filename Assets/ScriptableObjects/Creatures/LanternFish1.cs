using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LanternFish1 : MonoBehaviour
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
        float animation_percent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (animation_percent >= 2 / 6f && animation_percent < 5 / 6f)
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
        this_creature.Atk2ActiveHitBox(true);
        this_creature.SetVelocity(500 * Time.deltaTime * this_creature.GetLookSide(), 0);

        if (this_creature.AnimationEnd())
        {
            this_creature.Atk2ActiveHitBox(false);
            this_creature.SetVelocity(0, 0);
            this_creature.ToIdle();
        }
    }

    GameObject creature_to_summon = null;
    bool summoned = false;

    void Skill()
    {
        switch(BattleManager.Instance.GetState())
        {
            case "item_phase":
                if (this.tag == "Friend")
                {
                    BattleManager.Instance.RemoveFromTeamCount(false);
                }
                else
                {
                    BattleManager.Instance.RemoveFromEnemyCount(false);
                }
                break;

            case "battle_phase":
                if(summoned == false && this_creature.IsAlive() == false)
                {
                    List<GameObject> aims = new List<GameObject>();

                    foreach (GameObject enemy in (this.tag == "Friend" ? BattleManager.Instance.GetEnemyList() : BattleManager.Instance.GetTeamList()))
                    {
                        if (enemy != null)
                        {
                            aims.Add(enemy);
                        }
                    }

                    if (aims.Count > 0)
                    {
                        creature_to_summon = aims[UnityEngine.Random.Range(0, aims.Count)];

                        BasicCreatureScript friend = Instantiate(creature_to_summon, transform.position, Quaternion.identity).GetComponent<BasicCreatureScript>();
                        friend.enemy = (this.tag == "Friend" ? false : true);
                        friend.RemoveAndGetHp(-friend.AddAndGetMaxHp(0));
                        friend.SetVelocity(0, -10);
                        friend.SetActionTake("");
                        friend.SetAlive();
                        friend.gameObject.GetComponent<Animator>().speed = 1;
                        creature_to_summon = friend.gameObject;
                    }

                    summoned = true;
                }
                break;

            case "move_camera_down":
                if(creature_to_summon != null)
                {
                    Destroy(creature_to_summon);
                    creature_to_summon = null;
                    summoned = false;
                }
                break;
        }
    }
}