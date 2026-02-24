using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chocolate : MonoBehaviour
{
    BasicItemScript item;
    int buff_xp = 1;

    // Start is called before the first frame update
    void Start()
    {
        item = this.gameObject.GetComponent<BasicItemScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        buff_xp = item.xp_buff;

        GameObject creature = item.GetUsingCreature();
        Debug.Log(creature);
        if (creature != null)
        {
            BasicCreatureScript scrip = creature.GetComponent<BasicCreatureScript>();

            scrip.AddAndGetXp(buff_xp);

            Destroy(this.gameObject);
        }
    }
}
