using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf1 : MonoBehaviour
{
    BasicCreatureScript this_creature;
    Animator animator;
    int last_level;

    // Start is called before the first frame update
    void Start()
    {
        this_creature = gameObject.GetComponent<BasicCreatureScript>();
        animator = gameObject.GetComponent<Animator>();
        last_level = this_creature.GetOrSetLevel();

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

            if (this_creature.RemoveAndGetHp(0) > 0) Skill();
        }
    }

    void Atk1()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f)
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
        if (this_creature.AnimationEnd())
        {
            ProjectileScript projectile = this_creature.CreateProjectile(this_creature.AddAndGetSpecial(0), 600, 1);
            projectile.invert_sprite = true;
            this_creature.ToIdle();
        }
    }

    int wolf_atual_count = 0;
    int wolf_next_count = 0;

    void Skill()
    {
        wolf_next_count = 0;
        int atk_buff = 1;
        int spc_buff = 1;
        int hp_buff = 2;
        GameObject[] friends = GameObject.FindGameObjectsWithTag("Friend");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (last_level != this_creature.GetOrSetLevel())
        {
            int multiplier = wolf_atual_count * last_level * (-1);

            this_creature.AddAndGetAtk(atk_buff * multiplier);
            this_creature.AddAndGetSpecial(spc_buff * multiplier);
            this_creature.AddAndGetMaxHp(hp_buff * multiplier);

            wolf_atual_count = 0;
            last_level = this_creature.GetOrSetLevel();
        }

        foreach (GameObject friend in friends)
        {
            BasicCreatureScript creature = friend.GetComponent<BasicCreatureScript>();

            if (creature.GetClass() == "Wolf1" && creature.item == false) wolf_next_count++;
        }

        foreach (GameObject enemy in enemies)
        {
            BasicCreatureScript creature = enemy.GetComponent<BasicCreatureScript>();

            if (creature.GetClass() == "Wolf1" && creature.item == false) wolf_next_count++;
        }

        if (wolf_next_count != wolf_atual_count)
        {
            int multiplier = (wolf_next_count - wolf_atual_count) * this_creature.GetOrSetLevel();

            this_creature.AddAndGetAtk(atk_buff * multiplier);
            this_creature.AddAndGetSpecial(spc_buff * multiplier);
            this_creature.AddAndGetMaxHp(hp_buff * multiplier);

            wolf_atual_count = wolf_next_count;
        }
    }
}