using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class BasicCreatureScript : MonoBehaviour
{
    public Creature pre_sets;
    public GameObject projectile;
    public GameObject life_bar_canvas;
    public GameObject xp_bar_canvas;
    public GameObject stats_canvas;
    public GameObject eff_projectile;
    public GameObject arrow_up;

    // Aux
    public bool enemy = true;
    public bool item = false;
    int look_side = 1;
    float hurt_cd = 0;
    float fly_distance = 0;
    GameObject atk1_hit_box;
    GameObject atk2_hit_box;
    GameObject stats_view = null;
    GameObject life_bar = null;
    GameObject xp_bar = null;
    Slider life_bar_slider;
    Slider xp_bar_slider;
    bool fly = false;
    bool dead = false;
    bool sold = false;
    int discount = 0;
    bool imunity = false;

    // Stats
    int level = 1;
    int xp = 0;
    int hp = 0;
    int creature_hp = 0;
    int creature_atk = 0;
    int creature_special = 0;
    int creature_int = 0;
    float creature_jump_force = 0;
    float cd = 0;
    string creature_class = "";
    GameObject item_obj = null;
    food_buffs food_stats = new food_buffs();

    struct food_buffs
    {
        public int armor;
        public int eff_buff;

        public food_buffs(bool clear = true)
        {
            armor = 0;
            eff_buff = 0;
        }

        public void ResetFoodBuffs()
        {
            armor = 0;
            eff_buff = 0;
        }
    }

    // Sprites
    string creature_idle_sprite;
    string creature_hurt_sprite;
    string creature_death_sprite;
    string creature_atk_1_sprite;
    string creature_atk_2_sprite;
    string creature_jump_sprite;
    Animator animator;

    // Scales
    float start_scale_x;
    float start_scale_y;

    // Velocity
    float velocity_x = 0;
    float velocity_y = 0;

    // Move on Create
    float move_up_spd = 0.07f;

    // Start is called before the first frame update
    void Start()
    {
        // Disable Creature Collisions
        NegateCreatures();

        // Get Creatures Hit Box
        atk1_hit_box = gameObject.transform.GetChild(0).gameObject;
        atk2_hit_box = atk1_hit_box.transform.GetChild(0).gameObject;
        atk1_hit_box.tag = "Atk1";
        atk2_hit_box.tag = "Atk2";
        Atk1ActiveHitBox(false);
        Atk2ActiveHitBox(false);

        // Set Stats
        creature_class = pre_sets.creature_class;
        AddAndGetMaxHp(pre_sets.creature_hp);
        AddAndGetAtk(pre_sets.creature_atk);
        AddAndGetSpecial(pre_sets.creature_special);
        AddAndGetInt(pre_sets.creature_int);
        AddAndGetJumpForce(pre_sets.creature_jump_force);

        // Set Sprites
        creature_idle_sprite = pre_sets.creature_idle_sprite;
        creature_hurt_sprite = pre_sets.creature_hurt_sprite;
        creature_death_sprite = pre_sets.creature_death_sprite;
        creature_atk_1_sprite = pre_sets.creature_atk_1_sprite;
        creature_atk_2_sprite = pre_sets.creature_atk_2_sprite;
        creature_jump_sprite = pre_sets.creature_jump_sprite;
        animator = this.gameObject.GetComponent<Animator>();
        ChangeSprite(creature_idle_sprite);

        // Fly
        fly = pre_sets.fly;
        fly_distance = pre_sets.ground_distance;

        if (pre_sets.enemy == false) enemy = false;

        if (enemy == true)
        {
            this.tag = "Enemy";
        }
        else
        {
            this.tag = "Friend";
        }

        if (BattleManager.Instance.GetState() == "battle_phase") this.SetImune();

        start_scale_x = this.transform.localScale.x;
        start_scale_y = this.transform.localScale.y;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (item == false)
        {
            switch (BattleManager.Instance.GetState())
            {
                case "draw_phase":
                    animator.speed = 1;
                    action_cooldown = 0;

                    if (action_take != "stop")
                    {
                        ChangeSprite(creature_idle_sprite);
                        action_take = "stop";
                    }
                    break;

                case "move_camera_up":
                    this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    SetVelocity(0, -10);
                    FlyScript();
                    break;

                case "final_screen":
                    FlyScript();
                    break;

                case "move_camera_down":
                    FlyScript();
                    break;

                case "item_phase":
                    FlyScript();
                    break;

                case "eff_phase":
                    FlyScript();
                    break;

                case "":
                    FlyScript();
                    break;

                case "battle_phase":
                    if (action_take == "stop" && item == false)
                    {
                        action_take = "";
                        in_ground = true;
                        action_cooldown = 0;
                        this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    }

                    FlyScript();
                    break;
            }

            if (life_bar != null) DoLifeBar();
            if (xp_bar != null) DoXpBar();

            velocity_x = this.gameObject.GetComponent<Rigidbody2D>().velocity.x;
            velocity_y = this.gameObject.GetComponent<Rigidbody2D>().velocity.y;
        }
        else if (move_up_spd != 0)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + move_up_spd, this.transform.position.z);

            float distance = this.transform.position.y - this.transform.parent.transform.position.y;

            if (distance >= 0.35f)
            {
                move_up_spd *= -1;
            }
            else if (distance < 0.01f)
            {
                move_up_spd = 0;
            }
        }

        if (stats_view != null) DoStatsView();
    }

    // Check if is in Ground
    bool in_ground = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (BattleManager.Instance.GetState() == "battle_phase") 
        {
            if (collision.gameObject.tag == "Ground")
            {
                in_ground = true;
            }

            if (collision.gameObject.tag == "Wall")
            {
                if (in_ground == true)
                {
                    float new_xscale = this.gameObject.transform.localScale.x * (-1);

                    this.gameObject.transform.localScale = new Vector3(new_xscale, this.gameObject.transform.localScale.y, 0);
                    life_bar.GetComponent<RectTransform>().localScale = new Vector3((new_xscale > 0 ? 1 : -1), 1, 1);
                }

                this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(velocity_x * (-1), velocity_y, 0);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground") in_ground = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Atk1")
        {
            BasicCreatureScript creature = collision.transform.parent.GetComponent<BasicCreatureScript>();
            if(creature.tag != this.gameObject.tag) RemoveAndGetHp(creature.AddAndGetAtk(0));
        }
        else if (collision.tag == "Atk2")
        {
            BasicCreatureScript creature = collision.transform.parent.transform.parent.GetComponent<BasicCreatureScript>();
            if(creature.tag != this.gameObject.tag) RemoveAndGetHp(creature.AddAndGetAtk(0));
        }
    }

    // For Negate Another Creature Collisions
    void NegateCreatures()
    {
        BoxCollider2D self_collider = gameObject.GetComponent<BoxCollider2D>();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] friends = GameObject.FindGameObjectsWithTag("Friend");

        foreach (GameObject enemy in enemies)
        {
            Physics2D.IgnoreCollision(self_collider, enemy.GetComponent<BoxCollider2D>(), true);
        }

        foreach (GameObject friend in friends)
        {
            Physics2D.IgnoreCollision(self_collider, friend.GetComponent<BoxCollider2D>(), true);
        }
    }

    // For Sprite Changes
    void ChangeSprite(string new_sprite)
    {
        animator.Rebind();
        animator.Play(new_sprite);
    }

    // For Fly Creatures
    void FlyScript()
    {
        if (fly == true)
        {
            GameObject ground = GameObject.FindGameObjectWithTag("Ground");
            BoxCollider2D ground_collider = ground.GetComponent<BoxCollider2D>();
            float ground_top_y = ground.transform.position.y + (ground.transform.localScale.y * ground_collider.size.y / 2) + ground_collider.offset.y * ground.transform.localScale.y;

            if (this.transform.position.y <= ground_top_y + fly_distance)
            {
                this.transform.position = new Vector3(transform.position.x, ground_top_y + fly_distance, transform.position.z);
                SetVelocity(0, 0);
                in_ground = true;
            }
        }
    }

    // Life Bar Controll
    void DoLifeBar()
    {
        if (item == true || BattleManager.Instance.GetState() != "battle_phase")
        {
            SetLifeBarActive(false);
        }
        else
        {
            SetLifeBarActive(true);
            life_bar_slider.fillRect.gameObject.GetComponentInChildren<Image>().color = (this.gameObject.tag == "Friend" ? new Color(0, 255, 0) : new Color(255, 0, 0));
            life_bar_slider.maxValue = creature_hp;
            life_bar_slider.value = Mathf.Lerp(life_bar_slider.value, hp, 0.1f);

            if (life_bar_slider.value < 0.1)
            {
                life_bar_slider.fillRect.gameObject.SetActive(false);
            }
            else
            {
                life_bar_slider.fillRect.gameObject.SetActive(true);
            }
        }
    }

    void DoXpBar()
    {
        if (item == true || BattleManager.Instance.GetState() != "draw_phase" || this.gameObject.tag == "Enemy")
        {
            SetXpBarActive(false);
        }
        else
        {
            SetXpBarActive(true);

            switch (level)
            {
                case 1:
                    xp_bar_slider.maxValue = 2;
                    xp_bar_slider.value = xp;
                    break;

                case 2:
                    xp_bar_slider.maxValue = 3;
                    xp_bar_slider.value = xp;
                    break;

                case 3:
                    xp_bar_slider.maxValue = 10;
                    xp_bar_slider.value = 10;
                    break;
            }

            if (xp_bar_slider.value == 0)
            {
                xp_bar_slider.fillRect.gameObject.SetActive(false);
            }
            else
            {
                xp_bar_slider.fillRect.gameObject.SetActive(true);
            }
        }
    }

    void DoStatsView()
    {
        if (BattleManager.Instance.GetState() != "draw_phase")
        {
            SetStatsViewActive(false);
        }
        else
        {
            SetStatsViewActive(true);

            Text life = stats_view.transform.GetChild(0).GetComponentInChildren<Text>();
            Text atk = stats_view.transform.GetChild(1).GetComponentInChildren<Text>();
            Text range = stats_view.transform.GetChild(2).GetComponentInChildren<Text>();

            if(int.Parse(life.text) < creature_hp)
            {
                GameObject stats_arrow = Instantiate(arrow_up, Vector3.zero, Quaternion.identity);
                StatsArrow arrow_script = stats_arrow.GetComponent<StatsArrow>();

                stats_arrow.GetComponent<SpriteRenderer>().color = Color.green;
                arrow_script.stat_to_change = life;
                life.text = creature_hp.ToString();
            }
            else if(int.Parse(life.text) > creature_hp)
            {
                GameObject stats_arrow = Instantiate(arrow_up, Vector3.zero, Quaternion.Euler(0, 0, 180));
                StatsArrow arrow_script = stats_arrow.GetComponent<StatsArrow>();

                stats_arrow.GetComponent<SpriteRenderer>().color = Color.red;
                arrow_script.stat_to_change = life;
                life.text = creature_hp.ToString();
            }

            if (int.Parse(atk.text) < creature_atk)
            {
                GameObject stats_arrow = Instantiate(arrow_up, Vector3.zero, Quaternion.identity);
                StatsArrow arrow_script = stats_arrow.GetComponent<StatsArrow>();

                stats_arrow.GetComponent<SpriteRenderer>().color = Color.green;
                arrow_script.stat_to_change = atk;
                atk.text = creature_atk.ToString();
            }
            else if (int.Parse(atk.text) > creature_atk)
            {
                GameObject stats_arrow = Instantiate(arrow_up, Vector3.zero, Quaternion.Euler(0, 0, 180));
                StatsArrow arrow_script = stats_arrow.GetComponent<StatsArrow>();

                stats_arrow.GetComponent<SpriteRenderer>().color = Color.red;
                arrow_script.stat_to_change = atk;
                atk.text = creature_atk.ToString();
            }

            if (int.Parse(range.text) < creature_special)
            {
                GameObject stats_arrow = Instantiate(arrow_up, Vector3.zero, Quaternion.identity);
                StatsArrow arrow_script = stats_arrow.GetComponent<StatsArrow>();

                stats_arrow.GetComponent<SpriteRenderer>().color = Color.green;
                arrow_script.stat_to_change = range;
                range.text = creature_special.ToString();
            }
            else if (int.Parse(range.text) > creature_special)
            {
                GameObject stats_arrow = Instantiate(arrow_up, Vector3.zero, Quaternion.Euler(0, 0, 180));
                StatsArrow arrow_script = stats_arrow.GetComponent<StatsArrow>();

                stats_arrow.GetComponent<SpriteRenderer>().color = Color.red;
                arrow_script.stat_to_change = range;
                range.text = creature_special.ToString();
            }
        }
    }

    // Basic Action Script
    float action_cooldown = 0;
    string action_take = "";

    public void TakeAction()
    {
        // Alive
        if (hp > 0)
        {
            animator.speed = 1;
            dead = false;

            // Action End
            switch (action_take)
            {
                case "jump":
                    if (in_ground == true)
                    {
                        action_take = "";
                        ChangeSprite(creature_idle_sprite);
                    }
                    break;

                case "to_idle":
                    action_take = "";
                    ChangeSprite(creature_idle_sprite);
                    SetVelocity(0, 0);
                    break;

                case "hurt":
                    if (hurt_cd > 0)
                    {
                        hurt_cd -= Time.deltaTime;
                    }
                    else
                    {
                        if (in_ground == true)
                        {
                            action_take = "";
                            ChangeSprite(creature_idle_sprite);
                        }
                        else
                        {
                            action_take = "jump";
                            ChangeSprite(creature_jump_sprite);
                        }

                        this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                    break;
            }

            // Flip Sprite
            if (action_take == "")
            {
                int left = 0;
                int right = 0;
                GameObject[] creatures = GameObject.FindGameObjectsWithTag(this.tag == "Friend" ? "Enemy" : "Friend");

                foreach (GameObject c in creatures)
                {
                    BasicCreatureScript creature_aux = c.GetComponent<BasicCreatureScript>();

                    if (creature_aux.IsAlive() == true && creature_aux.item == false)
                    {
                        if (this.gameObject.transform.position.x <= c.transform.position.x) right++;
                        else left++;
                    }
                }

                float xscale = this.gameObject.transform.localScale.x;
                float yscale = this.gameObject.transform.localScale.y;

                if (right >= left && xscale < 0)
                {
                    this.gameObject.transform.localScale = new Vector3(xscale * (-1), yscale, 0);
                    life_bar.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                }
                if (right < left && xscale > 0)
                {
                    this.gameObject.transform.localScale = new Vector3(xscale * (-1), yscale, 0);
                    life_bar.GetComponent<RectTransform>().localScale = new Vector3(-1, 1, 1);
                }
            }

            // Action Cooldown
            if (action_cooldown >= cd && in_ground == true)
            {
                if (action_take == "")
                {
                    imunity = false;
                    float random_action = UnityEngine.Random.Range(0f, 100f);

                    if (random_action < 100 / 3f)
                    {
                        SetVelocity(0, 0);

                        // Jumps
                        int jump_dir = UnityEngine.Random.Range(0, 3);

                        switch (jump_dir)
                        {
                            // Jump
                            case 0:
                                this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector3(0, creature_jump_force, 0));
                                break;

                            // Front Jump
                            case 1:
                                this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector3(creature_jump_force / 3, creature_jump_force, 0));
                                break;

                            // Back Jump
                            case 2:
                                this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector3(-creature_jump_force / 3, creature_jump_force, 0));
                                break;
                        }

                        in_ground = false;
                        action_take = "jump";
                        ChangeSprite(creature_jump_sprite);
                    }
                    else if (random_action < 100 * 2 / 3f)
                    {
                        // Attack 1
                        action_take = "atk1";
                        ChangeSprite(creature_atk_1_sprite);
                    }
                    else
                    {
                        // Attack 2
                        action_take = "atk2";
                        ChangeSprite(creature_atk_2_sprite);
                    }

                    action_cooldown = 0;
                }
            }
            else if (action_take == "" || action_take == "hurt")
            {
                action_cooldown += Time.deltaTime;
            }

            look_side = (this.gameObject.transform.localScale.x < 0 ? -1 : 1);
        }
        // Dead
        else
        {
            Atk1ActiveHitBox(false);
            Atk2ActiveHitBox(false);
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;

            if (dead == false)
            {
                if (in_ground == true)
                {
                    dead = true;
                    action_take = "dead";
                    SetVelocity(0, 0);
                    ChangeSprite(creature_death_sprite);
                }
                else if (action_take != "jump")
                {
                    action_take = "jump";
                    ChangeSprite(creature_jump_sprite);
                }
            }

            if (dead == true && AnimationEnd())
            {
                animator.Rebind();
                animator.Play(creature_death_sprite, 0, 0.99f);
                animator.speed = 0;
            }
        }
    }

    public void ResetCreature()
    {
        this.gameObject.transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, this.gameObject.transform.position.z);
        this.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        Atk1ActiveHitBox(false);
        Atk2ActiveHitBox(false);
        hp = creature_hp;
        food_stats.ResetFoodBuffs();
        imunity = false;
        action_take = "";

        if (this.gameObject.transform.localScale.x < 0)
        {
            this.gameObject.transform.localScale = new Vector3(this.gameObject.transform.localScale.x * (-1), this.gameObject.transform.localScale.y, this.gameObject.transform.localScale.z);
            life_bar.GetComponent<RectTransform>().localScale = new Vector3(life_bar.GetComponent<RectTransform>().localScale.x * (-1), 1, 1);
        }
    }

    public void SetImune()
    {
        imunity = true;
    }

    public bool GetImune()
    {
        return imunity;
    }

    public void CreateLifeBar(float x, float y, float width, float height)
    {
        GameObject creature_life_bar = Instantiate(life_bar_canvas, this.gameObject.transform.position, this.gameObject.transform.rotation, this.gameObject.transform);

        // Take Life Bar
        life_bar = creature_life_bar.transform.GetChild(0).gameObject;
        life_bar_slider = life_bar.GetComponent<Slider>();
        life_bar.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
        life_bar.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        life_bar.GetComponent<RectTransform>().localScale = new Vector3((this.gameObject.transform.localScale.x > 0 ? 1 : -1), 1, 1);
        if (item == true) life_bar.SetActive(false);
    }

    public void SetLifeBarActive(bool active)
    {
        if (active == true) life_bar.SetActive(true);
        else life_bar.SetActive(false);
    }

    public void CreateXpBar(float x, float y, float width, float height)
    {
        GameObject creature_xp_bar = Instantiate(xp_bar_canvas, this.gameObject.transform.position, this.gameObject.transform.rotation, this.gameObject.transform);

        // Take Life Bar
        xp_bar = creature_xp_bar.transform.GetChild(0).gameObject;
        xp_bar_slider = xp_bar.GetComponent<Slider>();
        xp_bar.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
        xp_bar.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        xp_bar.GetComponent<RectTransform>().localScale = new Vector3((this.gameObject.transform.localScale.x > 0 ? 1 : -1), 1, 1);
        xp_bar.SetActive(false);
    }

    public void SetXpBarActive(bool active)
    {
        if (active == true) xp_bar.SetActive(true);
        else xp_bar.SetActive(false);
    }

    public void CreateStats(float x, float y)
    {
        stats_view = Instantiate(stats_canvas, this.transform.position, this.gameObject.transform.rotation, this.gameObject.transform);
        stats_view.GetComponent<RectTransform>().localScale = new Vector3((this.gameObject.transform.localScale.x > 0 ? 0.01f : -0.01f), 0.01f, 1);

        GameObject life = stats_view.transform.GetChild(0).gameObject;
        life.GetComponent<RectTransform>().localPosition = new Vector3(x - 35, y, 0);
        life.GetComponentInChildren<Text>().text = creature_hp.ToString();

        GameObject atk = stats_view.transform.GetChild(1).gameObject;
        atk.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);
        atk.GetComponentInChildren<Text>().text = creature_atk.ToString();

        GameObject range = stats_view.transform.GetChild(2).gameObject;
        range.GetComponent<RectTransform>().localPosition = new Vector3(x + 35, y, 0);
        range.GetComponentInChildren<Text>().text = creature_special.ToString();
    }

    public void SetStatsViewActive(bool active)
    {
        if (active == true) stats_view.SetActive(true);
        else stats_view.SetActive(false);
    }

    public string GetClass()
    {
        return creature_class;
    }

    public bool IsAlive()
    {
        return !dead;
    }
    
    public void SetAlive(bool alive = true)
    {
        dead = !alive;
    }

    public string GetActionTake()
    {
        return action_take;
    }

    public void SetActionTake(string action)
    {
        action_take = action;
    }

    public void ToIdle()
    {
        action_take = "to_idle";
    }

    public int GetLookSide()
    {
        return look_side;
    }

    public bool AnimationEnd()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 - (Time.deltaTime * 1.5f / animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public int RemoveAndGetHp(int damage, bool hurt = true)
    {
        if (imunity == true || action_take == "hurt") return hp;

        bool remove_from_count = (hp > 0 ? true : false);

        if (damage > 0)
        {
            hp = Math.Max(0, hp - Math.Max(1, damage - food_stats.armor));

            if (hurt == true)
            {
                if (hp > 0)
                {
                    action_take = "hurt";
                    ChangeSprite(creature_hurt_sprite);
                    this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                    hurt_cd = 0.25f;
                    SetVelocity(0, 0);
                    Atk1ActiveHitBox(false);
                    Atk2ActiveHitBox(false);
                }
            }
        }
        else
        {
            hp = Math.Min(creature_hp, hp - damage);
        }

        if (hp == 0 && remove_from_count == true)
        {
            if (this.gameObject.tag == "Friend") BattleManager.Instance.RemoveFromTeamCount();
            else BattleManager.Instance.RemoveFromEnemyCount();
        }

        return hp;
    }

    public void RemoveAndGetHpByEffect(int damage, GameObject caster, GameObject aim, bool hurt = true)
    {
        EffectProjectileScript rock = Instantiate(eff_projectile, caster.transform.position, Quaternion.identity).GetComponent<EffectProjectileScript>();

        rock.damage = damage + food_stats.eff_buff;
        rock.aim = aim;
        rock.caster = caster;
    }

    public int AddAndGetMaxHp(int add)
    {
        if (add != 0)
        {
            creature_hp = Math.Max(1, creature_hp + add);

            if (add > 0) hp += add;
            else if (hp > creature_hp) hp = creature_hp;
        }

        return creature_hp;
    }

    public int GetOrSetLevel(bool set = false, int new_level = 0, int new_xp = 0)
    {
        if(set == true) {
            level = new_level;
            xp = new_xp;
        }

        return level;
    }

    public int GetGiveXpAndDestroySelf()
    {
        if (item_obj != null) Destroy(item_obj);
        Destroy(this.gameObject, Time.deltaTime);

        switch(level)
        {
            case 1: return xp + 1;

            case 2: return xp + 3;

            default: return 6;
        }
    }

    public int AddAndGetXp(int add)
    {
        AddAndGetAtk(add);
        AddAndGetSpecial(add);
        AddAndGetMaxHp(add);

        if (add > 0)
        {
            if (level == 1)
            {
                if (xp + add >= 2)
                {
                    BattleManager.Instance.CreatureDoLevelUp();
                    level = 2;
                    xp = xp + add - 2;
                    add = 0;
                }
                else
                {
                    xp += add;
                }
            }

            if (level == 2)
            {
                if (xp + add >= 3)
                {
                    BattleManager.Instance.CreatureDoLevelUp();
                    level = 3;
                    xp = xp + add - 3;
                }
                else
                {
                    xp += add;
                }
            }

            if (level == 3)
            {
                xp = 0;
            }
        }
        else if (add < 0)
        {
            if (level == 3)
            {
                if (xp + add < 0)
                {
                    level = 2;
                    xp = xp + add + 3;
                }
                else
                {
                    xp += add;
                }
            }

            if (level == 2)
            {
                if (xp + add < 0)
                {
                    level = 1;
                    xp = xp + add + 2;
                }
                else
                {
                    xp += add;
                }
            }

            if (level == 1)
            {
                xp = Math.Max(0, xp + add);
            }
        }

        return xp;
    }

    public int AddAndGetAtk(int add)
    {
        creature_atk = Math.Max(1, creature_atk + add);
        return creature_atk;
    }

    public int AddAndGetSpecial(int add)
    {
        creature_special = Math.Max(1, creature_special + add);
        return creature_special;
    }

    public int AddAndGetInt(int add)
    {
        creature_int = Math.Max(0, creature_int + add);
        cd = 5 / (5f + creature_int);
        return creature_int;
    }

    public void ResetFoodBuffs()
    {
        food_stats.ResetFoodBuffs();
    }

    public int GetOrSetArmor(bool set = false, int new_armor = 0)
    {
        if (set == true)
        {
            food_stats.armor = new_armor;
        }

        return food_stats.armor;
    }

    public int GetOrSetEffBuff(bool set = false, int new_eff_buff = 0)
    {
        if (set == true)
        {
            food_stats.eff_buff = new_eff_buff;
        }

        return food_stats.eff_buff;
    }

    public int AddAndGetDiscount(int add)
    {
        discount = math.max(0, discount + add);

        return discount;
    }

    public float GetAnimationSpeed()
    {
        return animator.speed;
    }

    public float AddAndGetJumpForce(float add)
    {
        creature_jump_force = Math.Max(1, creature_jump_force + add);
        return creature_jump_force;
    }

    public void Atk1ActiveHitBox(bool active)
    {
        atk1_hit_box.SetActive(active);
    }

    public void Atk2ActiveHitBox(bool active)
    {
        atk1_hit_box.SetActive(active);
        atk1_hit_box.GetComponent<BoxCollider2D>().enabled = !active;

        atk2_hit_box.SetActive(active);
    }

    public void SetVelocity(float x, float y)
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(x, y, 0);
    }

    public void SetItem(GameObject equip_item)
    {
        item_obj = equip_item;
    }

    public GameObject GetItem()
    {
        return item_obj;
    }

    public ProjectileScript CreateProjectile(int damage, int speed, int life)
    {
        float x = (action_take == "Atk1" ? atk1_hit_box.transform.position.x : atk2_hit_box.transform.position.x);
        float y = (action_take == "Atk1" ? atk1_hit_box.transform.position.y : atk2_hit_box.transform.position.y);

        ProjectileScript new_projectile = Instantiate(projectile, new Vector3(x, y, Camera.main.transform.position.z + 1), Quaternion.identity).GetComponent<ProjectileScript>();
        new_projectile.damage = damage;
        new_projectile.projectile_speed = speed;
        new_projectile.projectile_life = life;
        new_projectile.look_side = look_side;
        new_projectile.creature_tag = this.gameObject.tag;
        new_projectile.GetComponent<Animator>().Play(action_take == "Atk1" ? creature_atk_1_sprite : creature_atk_2_sprite);

        return new_projectile;
    }

    float eff_speed = 0.03f;

    public void StartBattleEff()
    {
        switch(BattleManager.Instance.GetState())
        {
            case "item_phase":
                BattleManager.Instance.AddToEffList(this.gameObject);
                eff_speed = 0.03f;
                break;

            case "eff_phase":
                if (BattleManager.Instance.GetEffsToDoCont() > 0)
                {
                    BattleManager.Instance.SetState("");
                }
                break;

            case "":
                if (this.gameObject == BattleManager.Instance.GetNextListEff())
                {
                    this.transform.localScale = new Vector3(this.transform.localScale.x - eff_speed, this.transform.localScale.y + eff_speed, 1);

                    if (this.transform.localScale.y >= start_scale_y * 1.5)
                    {
                        eff_speed *= -1;
                    }
                    else if (this.transform.localScale.y <= start_scale_y)
                    {
                        this.transform.localScale = new Vector3(start_scale_x, start_scale_y, 1);
                        eff_speed = 0;
                    }

                    if (eff_speed == 0)
                    {
                        BattleManager.Instance.RemoveFromEffList(this.gameObject);

                        if (BattleManager.Instance.GetEffsToDoCont() == 0)
                        {
                            BattleManager.Instance.SetState("eff_phase");
                        }
                    }
                }
                break;
        }
    }

    public bool SellCreature()
    {
        List<GameObject> store = BattleManager.Instance.GetStoreList();
        List<GameObject> team = BattleManager.Instance.GetTeamList();
        int lower_value = 4;
        bool team_empy = true;

        foreach (GameObject thing in store)
        {
            if (thing != null && (thing.tag == "Friend" || thing.tag == "Enemy") && 3 - thing.GetComponent<BasicCreatureScript>().AddAndGetDiscount(0) < lower_value)
            {
                lower_value = 3 - thing.GetComponent<BasicCreatureScript>().AddAndGetDiscount(0);
            }
        }

        foreach (GameObject thing in team)
        {
            if (thing != null)
            {
                team_empy = false;
                break;
            }
        }

        if (team_empy == false || BattleManager.Instance.AddAndGetGold(0) + level >= lower_value) { 
            BattleManager.Instance.AddAndGetGold(level);
            sold = true;

            if (item_obj != null) Destroy(item_obj);
            Destroy(this.gameObject, Time.fixedDeltaTime);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsSold()
    {
        return sold;
    }
}