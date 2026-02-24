using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melon : MonoBehaviour
{
    bool eff_used = false;
    BasicItemScript item;

    public int shield = 50;

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
            BasicCreatureScript script = item.GetUsingCreature().GetComponent<BasicCreatureScript>();

            switch (BattleManager.Instance.GetState())
            {
                case "item_phase":
                    eff_used = false;

                    script.GetOrSetArmor(true, shield);
                    break;

                case "battle_phase":
                    if (eff_used == false && script.GetActionTake() == "hurt") 
                    {
                        script.GetOrSetArmor(true, 0);

                        transform.GetChild(0).gameObject.SetActive(false);

                        eff_used = true;
                    }
                    break;

                case "move_camera_down":
                    if (eff_used == false)
                    {
                        eff_used = true;
                    }

                    transform.GetChild(0).gameObject.SetActive(true);
                    break;
            }
        }
    }
}
