using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Scorpion1: MonoBehaviour
{
    public GameObject pitaya;

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

        level = this_creature.GetOrSetLevel();
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

        if (animation_percent >= 0.25f)
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
        float animation_percent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (animation_percent >= 0.25f)
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

    bool buyed = false;
    int level = 1;

    void Skill()
    {
        if (level != this_creature.GetOrSetLevel())
        {
            level = this_creature.GetOrSetLevel();

            buyed = false;
        }

        if (this_creature.item == false)
        {
            if (buyed == false)
            {
                GameObject item = Instantiate(pitaya, this.transform.position, Quaternion.identity);

                item.GetComponent<BasicItemScript>().DoEquipItem(this.gameObject);
            }

            buyed = true;
        }
    }
}