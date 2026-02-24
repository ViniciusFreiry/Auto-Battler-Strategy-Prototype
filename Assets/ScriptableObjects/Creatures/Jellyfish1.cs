using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jellyfish1 : MonoBehaviour
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
        this_creature.Atk2ActiveHitBox(true);
        this_creature.SetVelocity(500 * Time.deltaTime * this_creature.GetLookSide(), 0);

        if (this_creature.AnimationEnd())
        {
            this_creature.Atk2ActiveHitBox(false);
            this_creature.SetVelocity(0, 0);
            this_creature.ToIdle();
        }
    }

    List<GameObject> team = new List<GameObject>();
    List<int> team_level = new List<int>();

    void Skill()
    {
        if (BattleManager.Instance.GetState() == "draw_phase")
        {
            foreach (GameObject creature in (this.tag == "Friend" ? BattleManager.Instance.GetTeamList() : BattleManager.Instance.GetEnemyList()))
            {
                if (creature != null)
                {
                    if (team.Contains(creature))
                    {
                        int index = team.IndexOf(creature);
                        int levels_up = team[index].GetComponent<BasicCreatureScript>().GetOrSetLevel() - team_level[index];

                        this_creature.AddAndGetAtk(levels_up * this_creature.GetOrSetLevel());
                        this_creature.AddAndGetSpecial(levels_up * this_creature.GetOrSetLevel());
                        this_creature.AddAndGetMaxHp(levels_up * this_creature.GetOrSetLevel());

                        team_level[index] += levels_up;
                    }
                    else
                    {
                        team.Add(creature);
                        team_level.Add(creature.GetComponent<BasicCreatureScript>().GetOrSetLevel());
                    }
                }
            }
        }
        else
        {
            team = new List<GameObject>();
            team_level = new List<int>();
        }
    }
}