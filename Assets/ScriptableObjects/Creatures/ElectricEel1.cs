using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricEel1 : MonoBehaviour
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

    int buff = 0;

    void Skill()
    {
        switch(BattleManager.Instance.GetState())
        {
            case "item_phase":
                buff = this_creature.AddAndGetMaxHp(0) / 2 * this_creature.GetOrSetLevel();
                this_creature.AddAndGetMaxHp(buff);
                break;

            case "move_camera_down":
                this_creature.AddAndGetMaxHp(-buff);
                buff = 0;
                break;
        }
    }
}