using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class Turtle1 : MonoBehaviour
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

        if (animation_percent >= 2 / 6f)
        {
            this_creature.Atk1ActiveHitBox(true);
            this_creature.SetVelocity(500 * Time.deltaTime * this_creature.GetLookSide(), 0);
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

    public GameObject melon;
    List<GameObject> aims = new List<GameObject>();
    List<GameObject> aims_items = new List<GameObject>();
    bool eff_used = false;

    void Skill()
    {
        switch(BattleManager.Instance.GetState())
        {
            case "item_phase":
                eff_used = false;

                aims = new List<GameObject>();
                aims_items = new List<GameObject>();
                break;

            case "battle_phase":
                if (eff_used == false && this_creature.RemoveAndGetHp(0) <= 0)
                {
                    List<GameObject> team = (this.tag == "Friend" ? BattleManager.Instance.GetTeamList() : BattleManager.Instance.GetEnemyList());
                    bool can_add = false;

                    switch (this.tag)
                    {
                        case "Friend":

                            for (int i = 4; i >= 0; i--)
                            {
                                if (team[i] != null)
                                {
                                    if (can_add == true && team[i].GetComponent<BasicCreatureScript>().RemoveAndGetHp(0) > 0)
                                    {
                                        aims.Add(team[i]);

                                        GameObject item = team[i].GetComponent<BasicCreatureScript>().GetItem();

                                        if (item != null)
                                        {
                                            item = Instantiate(item, transform.position, Quaternion.identity);
                                            item.SetActive(false);
                                        }

                                        aims_items.Add(item);

                                        if (aims.Count >= this_creature.GetOrSetLevel()) break;
                                    }

                                    if (team[i] == this.gameObject)
                                    {
                                        can_add = true;
                                    }
                                }
                            }
                            break;

                        case "Enemy":
                            for (int i = 0; i < 5; i++)
                            {
                                if (team[i] != null)
                                {
                                    if (can_add == true && team[i].GetComponent<BasicCreatureScript>().RemoveAndGetHp(0) > 0)
                                    {
                                        aims.Add(team[i]);

                                        GameObject item = team[i].GetComponent<BasicCreatureScript>().GetItem();

                                        if (item != null)
                                        {
                                            item = Instantiate(item, transform.position, Quaternion.identity);
                                            item.SetActive(false);
                                        }

                                        aims_items.Add(item);

                                        if (aims.Count >= this_creature.GetOrSetLevel()) break;
                                    }

                                    if (team[i] == this.gameObject)
                                    {
                                        can_add = true;
                                    }
                                }
                            }
                            break;
                    }

                    if (aims.Count > 0)
                    {
                        foreach (GameObject aim in aims)
                        {
                            GameObject new_melon = Instantiate(melon, transform.position, Quaternion.identity);
                            BasicCreatureScript script = aim.GetComponent<BasicCreatureScript>();

                            new_melon.GetComponent<BasicItemScript>().DoEquipItem(aim);
                            script.ResetFoodBuffs();
                            script.GetOrSetArmor(true, new_melon.GetComponent<Melon>().shield);
                        }
                    }

                    eff_used |= true;
                }
                break;

            case "move_camera_down":
                if (aims.Count > 0)
                {
                    for (int i = 0; i < aims.Count; i++)
                    {
                        if (aims_items[i] != null)
                        {
                            aims_items[i].SetActive(true);
                            aims_items[i].GetComponent<BasicItemScript>().DoEquipItem(aims[i]);
                        }
                        else
                        {
                            Destroy(aims[i].GetComponent<BasicCreatureScript>().GetItem());
                        }
                    }

                    aims = new List<GameObject>();
                    aims_items = new List<GameObject>();
                }
                break;
        }
    }
}