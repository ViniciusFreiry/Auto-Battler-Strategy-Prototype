using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiFinalScreen : MonoBehaviour
{
    float rotation_speed = 0.5f;
    public Sprite spr_win;
    public Sprite spr_draw;
    public Sprite spr_lose;
    public Sprite spr_absolute_win;
    RectTransform rect_trans;

    // Start is called before the first frame update
    void Start()
    {
        rect_trans = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rect_trans.eulerAngles = new Vector3(0, 0, rect_trans.eulerAngles.z + rotation_speed);

        if ((rect_trans.eulerAngles.z < 180 && rect_trans.eulerAngles.z >= 5) || (rect_trans.eulerAngles.z > 180 && rect_trans.eulerAngles.z <= 355))
        {
            rotation_speed *= -1;
        }
    }

    public void BattleResult(string result)
    {
        Image this_image = GetComponent<Image>();

        switch (result)
        {
            case "win":
                this_image.sprite = spr_win;
                break;

            case "draw":
                this_image.sprite = spr_draw;
                break;

            case "lose":
                this_image.sprite = spr_lose;
                break;

            case "absolute_win":
                this_image.sprite= spr_absolute_win;
                break;
        }
    }
}
