using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AkitaInu1 : MonoBehaviour
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

                if (this_creature.GetActionTake() == "atk1")
                {
                    if (this_creature.GetOrSetLevel() >= 2 && BattleManager.Instance.GetState() == "battle_phase") Skill();
                    Atk1();
                }
                else if (this_creature.GetActionTake() == "atk2")
                {
                    if (BattleManager.Instance.GetState() == "battle_phase") Skill();
                    Atk2();
                }
                else if (this_creature.GetActionTake() == "jump")
                {
                    if (this_creature.GetOrSetLevel() == 3 && BattleManager.Instance.GetState() == "battle_phase") Skill();
                }
                else
                {
                    skill_used = false;
                }
            }
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
            projectile.invert_sprite = true;
            projectile.GetComponent<SpriteRenderer>().color = Color.red;
            this_creature.ToIdle();
        }
    }

    bool skill_used = false;

    void Skill()
    {
        if (skill_used == false)
        {
            foreach (GameObject enemy in (this.tag == "Friend" ? BattleManager.Instance.GetEnemyList() : BattleManager.Instance.GetTeamList()))
            {
                if (enemy != null)
                {
                    this_creature.RemoveAndGetHpByEffect(this_creature.AddAndGetSpecial(0), this.gameObject, enemy);
                }
            }

            skill_used = true;
        }
    }
}