using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsArrow : MonoBehaviour
{
    public Text stat_to_change;

    float distance = 0;
    float speed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(stat_to_change.transform.position.x, stat_to_change.transform.position.y, stat_to_change.transform.position.z - 0.1f);

        if (GetComponent<SpriteRenderer>().color == Color.red) speed *= -1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        distance += speed;

        if (distance >= 0.25f || distance <= -0.25f)
        {
            Destroy(this.gameObject);
        }

        transform.position = new Vector3(stat_to_change.transform.position.x, stat_to_change.transform.position.y + distance, stat_to_change.transform.position.z - 0.1f);
    }
}