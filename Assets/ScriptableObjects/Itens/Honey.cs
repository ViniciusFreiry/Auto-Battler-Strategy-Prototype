using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Honey : MonoBehaviour
{
    public GameObject creature_to_summon;

    bool eff_used = false;
    BasicItemScript item;
    GameObject delete_in_final = null;

    // Start is called before the first frame update
    void Start()
    {
        item = this.gameObject.GetComponent<BasicItemScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (item.GetUsingCreature() != null)
        {
            switch (BattleManager.Instance.GetState())
            {
                case "item_phase":
                    if (item.GetUsingCreature().tag == "Friend")
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
                    if (eff_used == false && item.GetUsingCreature().GetComponent<BasicCreatureScript>().GetActionTake() == "dead")
                    {
                        delete_in_final = Instantiate(creature_to_summon, item.GetUsingCreature().transform.position, Quaternion.identity);
                        delete_in_final.GetComponent<BasicCreatureScript>().SetVelocity(0, -10);
                        delete_in_final.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                        
                        if (item.GetUsingCreature().tag == "Friend")
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
    }
}
