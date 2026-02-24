using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitaya : MonoBehaviour
{
    bool eff_used = false;
    BasicItemScript item;

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
                    eff_used = false;
                    break;

                case "battle_phase":
                    if (eff_used == false)
                    {
                        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(item.GetUsingCreature().gameObject.tag == "Friend" ? "Enemy" : "Friend"))
                        {
                            BasicCreatureScript script = enemy.GetComponent<BasicCreatureScript>();

                            if (script.GetActionTake() == "hurt" && script.RemoveAndGetHp(0) > 0)
                            {
                                script.RemoveAndGetHp(script.RemoveAndGetHp(0));

                                eff_used = true;
                            }
                        }
                    }
                    break;
            }
        }
    }
}
