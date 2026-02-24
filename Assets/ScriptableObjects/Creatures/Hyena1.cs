using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class Hyena1 : MonoBehaviour
{
    BasicCreatureScript this_creature;
    Animator animator;

    public GameObject hyena;

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

            if (can_use_skill == true) Skill();
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

    bool eff_used = false;
    GameObject delete_in_final = null;
    bool can_use_skill = true;

    void Skill()
    {
        switch (BattleManager.Instance.GetState())
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

                eff_used = false;
                break;

            case "battle_phase":
                if (eff_used == false && this.GetComponent<BasicCreatureScript>().GetActionTake() == "dead")
                {
                    delete_in_final = Instantiate(hyena, this.transform.position, Quaternion.identity);
                    delete_in_final.GetComponent<Hyena1>().DesactivateSkill();
                    delete_in_final.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

                    BasicCreatureScript script = delete_in_final.GetComponent<BasicCreatureScript>();
                    int level = script.GetOrSetLevel();

                    script.SetVelocity(0, -10);
                    script.AddAndGetAtk(script.AddAndGetAtk(0) * level - script.AddAndGetAtk(0));
                    script.AddAndGetSpecial(script.AddAndGetSpecial(0) * level - script.AddAndGetSpecial(0));
                    script.AddAndGetMaxHp(script.AddAndGetMaxHp(0) * level - script.AddAndGetMaxHp(0));

                    if (this.tag == "Friend")
                    {
                        delete_in_final.GetComponent<BasicCreatureScript>().enemy = false;
                    }
                    else
                    {
                        delete_in_final.GetComponent<BasicCreatureScript>().enemy = true;
                    }

                    eff_used = true;
                }
                break;

            default:
                if (delete_in_final != null)
                {
                    Destroy(delete_in_final);
                }
                break;
        }
    }

    public void DesactivateSkill()
    {
        can_use_skill = false;
    }
}