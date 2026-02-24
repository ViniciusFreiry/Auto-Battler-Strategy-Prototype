using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Crab1 : MonoBehaviour
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
            if (BattleManager.Instance.GetState() == "item_phase" || BattleManager.Instance.GetState() == "move_camera_down") Skill();
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
        float animation_percent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (animation_percent >= 3 / 11f && animation_percent < 9 / 11f)
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

    int buff = 0;

    void Skill()
    {
        GameObject[] teams = GameObject.FindGameObjectsWithTag(this.tag);
        int biggest_life = 0;

        foreach (GameObject creature in teams)
        {
            BasicCreatureScript creature_status = creature.GetComponent<BasicCreatureScript>();

            if (creature_status.item == false && biggest_life < creature_status.AddAndGetMaxHp(0))
            {
                biggest_life = creature_status.AddAndGetMaxHp(0);
            }
        }

        switch(BattleManager.Instance.GetState())
        {
            case "item_phase":
                int atual_life = this_creature.AddAndGetMaxHp(0);

                switch (this_creature.GetOrSetLevel())
                {
                    case 1:
                        this_creature.AddAndGetMaxHp((biggest_life / 2) - atual_life);
                        buff = atual_life - this_creature.AddAndGetMaxHp(0);
                        break;

                    case 2:
                        this_creature.AddAndGetMaxHp(biggest_life - atual_life);
                        buff = atual_life - this_creature.AddAndGetMaxHp(0);
                        break;

                    case 3:
                        this_creature.AddAndGetMaxHp((biggest_life / 2 * 3) - atual_life);
                        buff = atual_life - this_creature.AddAndGetMaxHp(0);
                        break;
                }
                break;

            case "move_camera_down":
                this_creature.AddAndGetMaxHp(buff);
                buff = 0;
                break;
        }
    }
}