using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public float speed = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);

        if (transform.position.x < -24) transform.position = new Vector3(transform.position.x + 48, transform.position.y, transform.position.z);
        else if (transform.position.x > 24) transform.position = new Vector3(transform.position.x - 48, transform.position.y, transform.position.z);
    }
}
