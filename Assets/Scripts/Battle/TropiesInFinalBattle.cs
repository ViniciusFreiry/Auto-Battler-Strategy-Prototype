using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TropiesInFinalBattle : MonoBehaviour
{
    bool active = false;
    Image this_image;

    // Start is called before the first frame update
    void Start()
    {
        this_image = GetComponent<Image>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active == true)
        {
            this_image.color = Color.Lerp(this_image.color, Color.white, 0.05f);
        }
        else
        {
            this_image.color = Color.Lerp(this_image.color, Color.black, 0.05f);
        }
    }

    public void Activate(bool activate)
    {
        active = activate;
    }
}
