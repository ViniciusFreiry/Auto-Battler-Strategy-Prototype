using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LifesInFinalBattle : MonoBehaviour
{
    Image this_image;
    RectTransform rect_trans;
    float size_speed = 0.5f;
    float max_size;
    float min_size;
    bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        this_image = GetComponent<Image>();
        rect_trans = GetComponent<RectTransform>();
        max_size = rect_trans.sizeDelta.x * 1.1f;
        min_size = rect_trans.sizeDelta.x * 0.9f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rect_trans.sizeDelta = new Vector2(rect_trans.sizeDelta.x + size_speed, rect_trans.sizeDelta.y + size_speed);

        if (rect_trans.sizeDelta.x >= max_size || rect_trans.sizeDelta.x <= min_size)
        {
            size_speed *= -1;
        }

        if (active == true)
        {
            this_image.color = Color.Lerp(this_image.color, Color.white, 0.05f);
        }
        else
        {
            this_image.color = Color.Lerp(this_image.color, Color.black, 0.05f);
        }
    }

    public void Active(bool activate)
    {
        active = activate;
    }
}
