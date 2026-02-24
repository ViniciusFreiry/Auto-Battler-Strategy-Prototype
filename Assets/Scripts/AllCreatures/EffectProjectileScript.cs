using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectProjectileScript : MonoBehaviour
{
    public GameObject aim;
    public GameObject caster;
    public int damage;

    GameObject flow;
    float speed = 0.05f;
    float distance = 0;

    // Start is called before the first frame update
    void Start()
    {
        flow = caster;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        distance += speed;

        transform.position = new Vector3(flow.transform.position.x, flow.transform.position.y + distance, flow.transform.position.z - 0.1f);
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - 5);

        if (distance >= 0.75f)
        {
            speed *= -1;
            flow = aim;
        }
        else if (distance <= 0)
        {
            aim.GetComponent<BasicCreatureScript>().RemoveAndGetHp(damage);
            Destroy(this.gameObject);
        }
    }
}
