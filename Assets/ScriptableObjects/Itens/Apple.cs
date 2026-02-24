using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    BasicItemScript item;
    int buff_atk = 1;
    int buff_spc = 1;
    int buff_hp = 1;

    // Start is called before the first frame update
    void Start()
    {
        item = this.gameObject.GetComponent<BasicItemScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        buff_atk = item.atk_buff;
        buff_spc = item.special_buff;
        buff_hp = item.hp_buff;

        GameObject creature = item.GetUsingCreature();

        if (creature != null)
        {
            BasicCreatureScript scrip = creature.GetComponent<BasicCreatureScript>();

            scrip.AddAndGetAtk(buff_atk);
            scrip.AddAndGetSpecial(buff_spc);
            scrip.AddAndGetMaxHp(buff_hp);

            Destroy(this.gameObject);
        }
    }
}
