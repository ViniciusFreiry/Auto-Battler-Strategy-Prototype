using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doberman1 : MonoBehaviour
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
        if (this_creature.AnimationEnd())
        {
            ProjectileScript projectile = this_creature.CreateProjectile(this_creature.AddAndGetSpecial(0), 600, 1);
            projectile.GetComponent<SpriteRenderer>().color = Color.red;
            projectile.invert_sprite = true;
            this_creature.ToIdle();
        }
    }

    int atk_buff = 0;
    int hp_buff = 0;

    void Skill()
    {
        switch (BattleManager.Instance.GetState())
        {
            case "item_phase":
                int atual_life = this_creature.AddAndGetMaxHp(0);
                int atual_atk = this_creature.AddAndGetAtk(0);

                this_creature.AddAndGetMaxHp(this_creature.AddAndGetSpecial(0) * this_creature.GetOrSetLevel());
                hp_buff = atual_life - this_creature.AddAndGetMaxHp(0);

                this_creature.AddAndGetAtk(this_creature.AddAndGetSpecial(0) * this_creature.GetOrSetLevel());
                atk_buff = atual_atk - this_creature.AddAndGetAtk(0);
                break;

            case "move_camera_down":
                this_creature.AddAndGetMaxHp(hp_buff);
                hp_buff = 0;

                this_creature.AddAndGetAtk(atk_buff);
                atk_buff = 0;
                break;
        }
    }
}