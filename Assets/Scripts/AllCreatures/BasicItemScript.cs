using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.UI;
using UnityEngine;

public class BasicItemScript : MonoBehaviour
{
    public bool is_equip = false;
    public bool is_buffer = false;
    public int atk_buff = 0;
    public int special_buff = 0;
    public int hp_buff = 0;
    public int xp_buff = 0;
    public bool add_in_count = false;
    public int count_quantity = 0;
    public GameObject particle_sistem;

    GameObject creature_to_use = null;
    bool move_up = true;
    int discount = 0;
    bool sold = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (creature_to_use != null)
        {
            this.gameObject.transform.position = new Vector3(creature_to_use.transform.position.x, creature_to_use.transform.position.y - 0.25f, creature_to_use.transform.position.z);
            this.gameObject.transform.localScale = creature_to_use.transform.parent.localScale;
        }

        if (this.transform.parent != null)
        {
            if (move_up == true)
            {
                if (this.transform.position.y - this.transform.parent.transform.position.y < 0.24f)
                {
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.07f, this.transform.position.z);
                }
                else
                {
                    move_up = false;
                }
            }
            else
            {
                if (this.transform.position.y - this.transform.parent.transform.position.y > 0)
                {
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.07f, this.transform.position.z);
                }
            }
        }
    }

    public void DoEquipItem(GameObject creature_to_item)
    {
        creature_to_use = creature_to_item;
        GameObject creature_to_use_item = creature_to_use.GetComponent<BasicCreatureScript>().GetItem();

        if (creature_to_use_item != null)
        {
            if (creature_to_use_item.GetComponent<BasicItemScript>().add_in_count == true)
            {
                for (int i = 0; i < creature_to_use_item.GetComponent<BasicItemScript>().count_quantity; i++)
                {
                    if (creature_to_use.tag == "Friend") BattleManager.Instance.RemoveFromTeamCount();
                    else BattleManager.Instance.RemoveFromEnemyCount();
                }
            }

            Destroy(creature_to_use_item);
        }
            
        creature_to_use.GetComponent<BasicCreatureScript>().SetItem(this.gameObject);

        if (transform.childCount == 0)
        {
            GameObject particles = Instantiate(particle_sistem, this.transform.position, Quaternion.Euler(-90, 0, 0), this.gameObject.transform);

            particles.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, this.GetComponent<SpriteRenderer>().sprite);
        }

        this.gameObject.GetComponent<SpriteRenderer>().forceRenderingOff = true;

        sold = true;
    }

    public void DoNoEquipItem(GameObject creature_to_item)
    {
        creature_to_use = creature_to_item;

        sold = true;
    }

    public GameObject GetUsingCreature()
    {
        return creature_to_use;
    }

    public int AddAndGetDiscount(int add)
    {
        discount = math.max(0, discount + add);

        return discount;
    }

    public bool IsSold()
    {
        return sold;
    }
}
