using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public Animator animator;

    // Stats
    public string creature_tag;
    public int damage;
    public int projectile_speed;
    public int projectile_life;
    public int look_side;
    public bool invert_sprite = false;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.localScale = new Vector3((invert_sprite == true ? this.gameObject.transform.localScale.x * look_side * (-1) : this.gameObject.transform.localScale.x * look_side), this.gameObject.transform.localScale.y, this.gameObject.transform.localScale.z);
        gameObject.AddComponent<PolygonCollider2D>();
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(projectile_speed * Time.deltaTime * look_side, 0, 0);

        if (projectile_life <= 0 || BattleManager.Instance.IsSomeTeamDead())
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            Destroy(this.gameObject);
        }

        if (collision.tag != creature_tag && (collision.tag == "Friend" || collision.tag == "Enemy"))
        {
            BasicCreatureScript creature = collision.GetComponent<BasicCreatureScript>();

            if (creature.RemoveAndGetHp(0) > 0 && BattleManager.Instance.GetState() == "battle_phase")
            {
                creature.RemoveAndGetHp(damage);
                if (creature.GetImune() == false) projectile_life--;
            }
        }
    }
}