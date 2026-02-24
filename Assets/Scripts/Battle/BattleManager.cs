using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject SlotsHandler;
    public List<GameObject> store_slots;
    public List<GameObject> team_slots;
    public List<GameObject> enemy_slots;
    public List<GameObject> rank1_creatures;
    public List<GameObject> rank2_creatures;
    public List<GameObject> rank3_creatures;
    public List<GameObject> rank4_creatures;
    public List<GameObject> rank5_creatures;
    public List<GameObject> rank6_creatures;
    public List<GameObject> rank1_itens;
    public List<GameObject> rank2_itens;
    public List<GameObject> rank3_itens;
    public List<GameObject> rank4_itens;
    public List<GameObject> rank5_itens;
    public List<GameObject> rank6_itens;
    public List<Sprite> dice_rank_sprites;
    public GameObject mouse;
    public Image dice_rank;
    public Text gold_text;
    public Text victory_text;
    public Text life_text;
    public Text turn_text;
    public Button end_turn_button;
    public GameObject final_battle_screen;
    public GameObject final_battle_background;
    public Image lifes_handler;
    public Image trophy_handler;
    public Image emoji;

    int lifes = 5;
    int gold = 10;
    int rank = 1;
    int turn = 1;
    int wins = 0;
    int loses = 0;
    int team_count = 0;
    int enemy_count = 0;
    int index_of_store = -1;
    int index_of_team = -1;
    float camera_speed = 0.075f;
    float slot_cd = 0;
    float final_screen_cd = 0;
    string selected_team_class = null;
    string mouse_class = null;
    string state = "move_camera_down";
    GameObject slot_store = null;
    GameObject slot_team = null;
    GameObject slot_enemy = null;
    List<GameObject> things_in_slots = new List<GameObject> { null, null, null, null, null, null, null };
    List<GameObject> things_in_team = new List<GameObject> { null, null, null, null, null };
    List<GameObject> things_in_enemy = new List<GameObject> { null, null, null, null, null };
    List<GameObject> monsters_with_eff = new List<GameObject> { null, null, null, null, null, null, null, null, null, null };

    // For Store Slots Animation
    float store_slot_scale_limit_up;
    float store_slot_scale_limit_down;
    float store_slot_scale_speed = 0.005f;

    // For Team Slots Animation
    float team_slot_scale_limit_up;
    float team_slot_scale_limit_down;
    float team_slot_scale_speed = 0.005f;


    // Start is called before the first frame update
    void Start()
    {
        dice_rank.sprite = dice_rank_sprites[rank - 1];
        final_battle_screen.SetActive(false);
        store_slots[3].SetActive(false);
        store_slots[4].SetActive(false);
        store_slots[5].SetActive(false);

        store_slot_scale_limit_up = store_slots[0].transform.GetChild(0).transform.localScale.x * 1.035f;
        store_slot_scale_limit_down = store_slots[0].transform.GetChild(0).transform.localScale.x * 0.965f;

        team_slot_scale_limit_up = team_slots[0].transform.GetChild(0).transform.localScale.x * 1.035f;
        team_slot_scale_limit_down = team_slots[0].transform.GetChild(0).transform.localScale.x * 0.965f;

        ActiveStoreSlotsSprites(false);
        ActiveTeamSlotSprites(false);
        ActiveEnemySlotSprites(false);
    }

    private void Update()
    {
        if (state == "draw_phase") BuyStoreItem();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.transform.position = new Vector3(mouse_position.x, mouse_position.y, -8.5f);

        switch (state)
        {
            case "item_phase":
                state = "eff_phase";
                break;

            case "eff_phase":
                state = "battle_phase";
                break;

            case "move_camera_up":
                Camera.main.transform.position = new Vector3(Mathf.Lerp(Camera.main.transform.position.x, 0, camera_speed), Mathf.Lerp(Camera.main.transform.position.y, 0, camera_speed), Camera.main.transform.position.z);
                gold_text.transform.parent.position = new Vector3(Mathf.Lerp(gold_text.transform.parent.position.x, -9, camera_speed), Mathf.Lerp(gold_text.transform.parent.position.y, 3, camera_speed), gold_text.transform.parent.position.z);
                victory_text.transform.parent.position = new Vector3(Mathf.Lerp(victory_text.transform.parent.position.x, -9, camera_speed), Mathf.Lerp(victory_text.transform.parent.position.y, 1.5f, camera_speed), victory_text.transform.parent.position.z);
                life_text.transform.parent.position = new Vector3(Mathf.Lerp(life_text.transform.parent.position.x, -9, camera_speed), Mathf.Lerp(life_text.transform.parent.position.y, 0, camera_speed), life_text.transform.parent.position.z);
                turn_text.transform.parent.position = new Vector3(Mathf.Lerp(turn_text.transform.parent.position.x, 9, camera_speed), Mathf.Lerp(turn_text.transform.parent.position.y, 3, camera_speed), turn_text.transform.parent.position.z);

                if (Camera.main.transform.position.y > -0.1f)
                {
                    state = "item_phase";
                }
                break;

            case "final_screen":
                final_screen_cd += Time.fixedDeltaTime;
                final_battle_screen.transform.localScale = Vector3.Lerp(final_battle_screen.transform.localScale, Vector3.one, 0.05f);

                if (final_battle_screen.transform.localScale.x >= 0.99f)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (i + 1 > lifes - loses) lifes_handler.transform.GetChild(i).GetComponent<LifesInFinalBattle>().Active(false);
                        else lifes_handler.transform.GetChild(i).GetComponent<LifesInFinalBattle>().Active(true);
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        if (i < wins) trophy_handler.transform.GetChild(i).GetComponent<TropiesInFinalBattle>().Activate(true);
                        else trophy_handler.transform.GetChild(i).GetComponent<TropiesInFinalBattle>().Activate(false);
                    }
                }

                if (final_screen_cd >= 3)
                {
                    victory_text.text = wins.ToString();
                    life_text.text = (lifes - loses).ToString();
                    turn_text.text = turn.ToString();
                    state = "move_camera_down";
                    final_battle_screen.SetActive(false);
                    final_battle_background.SetActive(false);
                }
                break;

            case "move_camera_down":
                Camera.main.transform.position = new Vector3(Mathf.Lerp(Camera.main.transform.position.x, 0, camera_speed), Mathf.Lerp(Camera.main.transform.position.y, -6, camera_speed), Camera.main.transform.position.z);
                gold_text.transform.parent.position = new Vector3(Mathf.Lerp(gold_text.transform.parent.position.x, -9, camera_speed), Mathf.Lerp(gold_text.transform.parent.position.y, -2, camera_speed), gold_text.transform.parent.position.z);
                victory_text.transform.parent.position = new Vector3(Mathf.Lerp(victory_text.transform.parent.position.x, -9, camera_speed), Mathf.Lerp(victory_text.transform.parent.position.y, -3.5f, camera_speed), victory_text.transform.parent.position.z);
                life_text.transform.parent.position = new Vector3(Mathf.Lerp(life_text.transform.parent.position.x, -9, camera_speed), Mathf.Lerp(life_text.transform.parent.position.y, -5, camera_speed), life_text.transform.parent.position.z);
                turn_text.transform.parent.position = new Vector3(Mathf.Lerp(turn_text.transform.parent.position.x, 9, camera_speed), Mathf.Lerp(turn_text.transform.parent.position.y, -2, camera_speed), turn_text.transform.parent.position.z);

                if (Camera.main.transform.position.y < -5.9f)
                {
                    foreach (GameObject creature in things_in_team)
                    {
                        if (creature != null)
                        {
                            creature.GetComponent<BasicCreatureScript>().ResetCreature();
                        }
                    }

                    foreach (GameObject creature in things_in_enemy)
                    {
                        if (creature != null)
                        {
                            Destroy(creature);
                        }
                    }

                    gold = 10;
                    RefreshStore();
                    CreateEnemies();
                    ActiveEnemySlotSprites(true);
                    state = "draw_phase";

                    if (turn < 5)
                    {
                        store_slots[3].SetActive(false);
                        store_slots[4].SetActive(false);
                        store_slots[5].SetActive(false);
                    }
                    else if (turn < 9)
                    {
                        store_slots[3].SetActive(true);
                        store_slots[5].SetActive(true);
                    }
                    else
                    {
                        store_slots[4].SetActive(true);
                    }
                }
                break;

            case "draw_phase":
                // Store Slots Animation
                if (index_of_store == -1 && index_of_team == -1)
                {
                    ActiveTeamSlotSprites(false);
                    ActiveStoreSlotsSprites(true);

                    foreach (GameObject slot in store_slots)
                    {
                        GameObject image = slot.transform.GetChild(0).gameObject;
                        image.transform.localScale = new Vector3(image.transform.localScale.x + store_slot_scale_speed, image.transform.localScale.y + store_slot_scale_speed, image.transform.localScale.z);
                    }

                    float image_scale = store_slots[0].transform.GetChild(0).localScale.x;
                    if (image_scale >= store_slot_scale_limit_up || image_scale <= store_slot_scale_limit_down)
                    {
                        store_slot_scale_speed *= -1;
                    }
                }
                else
                {
                    ActiveTeamSlotSprites(true);
                    ActiveStoreSlotsSprites(false);

                    foreach (GameObject slot in team_slots)
                    {
                        GameObject image = slot.transform.GetChild(0).gameObject;
                        image.transform.localScale = new Vector3(image.transform.localScale.x + team_slot_scale_speed, image.transform.localScale.y + team_slot_scale_speed, image.transform.localScale.z);
                    }

                    float image_scale = team_slots[0].transform.GetChild(0).localScale.x;
                    if (image_scale >= team_slot_scale_limit_up || image_scale <= team_slot_scale_limit_down)
                    {
                        team_slot_scale_speed *= -1;
                    }
                }
                break;

            case "battle_phase":
                if ((team_count == 0 || enemy_count == 0) && DeadAnimationEnds())
                {
                    EmojiFinalScreen emoji_script = emoji.GetComponent<EmojiFinalScreen>();

                    turn++;
                    emoji_script.BattleResult("draw");

                    if (team_count == 0 && enemy_count != 0)
                    {
                        emoji_script.BattleResult("lose");
                        loses++;
                    }
                    else if (team_count != 0 && enemy_count == 0)
                    {
                        emoji_script.BattleResult("win");
                        wins++;
                    }

                    if (rank < 6 && (turn + 1) % 2 == 0) rank++;

                    dice_rank.sprite = dice_rank_sprites[rank - 1];
                    state = "final_screen";
                    final_screen_cd = 0;
                    final_battle_screen.transform.localScale = Vector3.zero;
                    final_battle_screen.SetActive(true);
                    final_battle_background.SetActive(true);

                    if (wins == 10) emoji_script.BattleResult("absolute_win");
                }
                break;
        }

        gold_text.text = gold.ToString();
    }

    void RefreshStore()
    {
        for (int i = 0; i < 7; i++)
        {
            if (things_in_slots[i] != null)
            {
                if (things_in_slots[i].GetComponent<SpriteRenderer>().color == Color.cyan) continue;

                Destroy(things_in_slots[i]);
                things_in_slots[i] = null;
            }
            
            GameObject new_thing = null;

            switch (UnityEngine.Random.Range(1, rank + 1))
            {
                case 1:
                    if (i < 5) new_thing = rank1_creatures[UnityEngine.Random.Range(0, rank1_creatures.Count)];
                    else new_thing = rank1_itens[UnityEngine.Random.Range(0, rank1_itens.Count)];
                    break;

                case 2:
                    if (i < 5) new_thing = rank2_creatures[UnityEngine.Random.Range(0, rank2_creatures.Count)];
                    else new_thing = rank2_itens[UnityEngine.Random.Range(0, rank2_itens.Count)];
                    break;

                case 3:
                    if (i < 5) new_thing = rank3_creatures[UnityEngine.Random.Range(0, rank3_creatures.Count)];
                    else new_thing = rank3_itens[UnityEngine.Random.Range(0, rank3_itens.Count)];
                    break;

                case 4:
                    if (i < 5) new_thing = rank4_creatures[UnityEngine.Random.Range(0, rank4_creatures.Count)];
                    else new_thing = rank4_itens[UnityEngine.Random.Range(0, rank4_itens.Count)];
                    break;

                case 5:
                    if (i < 5) new_thing = rank5_creatures[UnityEngine.Random.Range(0, rank5_creatures.Count)];
                    else new_thing = rank5_itens[UnityEngine.Random.Range(0, rank5_itens.Count)];
                    break;

                case 6:
                    if (i < 5) new_thing = rank6_creatures[UnityEngine.Random.Range(0, rank6_creatures.Count)];
                    else new_thing = rank6_itens[UnityEngine.Random.Range(0, rank6_itens.Count)];
                    break;
            }

            things_in_slots[i] = Instantiate(new_thing, store_slots[i].transform.position, quaternion.identity, store_slots[i].transform);
            things_in_slots[i].transform.position = new Vector3(things_in_slots[i].transform.position.x, things_in_slots[i].transform.position.y, store_slots[i].transform.position.z - 1);

            if (i < 5)
            {
                things_in_slots[i].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                things_in_slots[i].GetComponent<BasicCreatureScript>().enemy = false;
                things_in_slots[i].GetComponent<BasicCreatureScript>().item = true;
            }
        }
    }

    void BuyStoreItem()
    {
        BasicCreatureScript temp_selected_creature = null;
        int selected_team_level = -1;

        if (slot_team == null || things_in_team[team_slots.IndexOf(slot_team)] == null)
        {
            selected_team_class = null;
        }
        else
        {
            temp_selected_creature = things_in_team[team_slots.IndexOf(slot_team)].gameObject.GetComponent<BasicCreatureScript>();
            selected_team_level = temp_selected_creature.GetOrSetLevel();
            selected_team_class = temp_selected_creature.GetClass();
        }

        mouse_class = (mouse.transform.childCount == 1 && (mouse.transform.GetChild(0).gameObject.tag == "Friend" || mouse.transform.GetChild(0).gameObject.tag == "Enemy") ? mouse.transform.GetChild(0).gameObject.GetComponent<BasicCreatureScript>().GetClass() : null);
        
        if (slot_store != null)
        {
            // Click Down On Store Slot
            if (Input.GetMouseButtonDown(0))
            {
                index_of_store = store_slots.IndexOf(slot_store);

                if (things_in_slots[index_of_store] != null) things_in_slots[index_of_store].transform.SetParent(mouse.transform, false);
                else index_of_store = -1;
            }
        }
        else if (slot_team != null)
        {
            // Click Down On Team Slot
            if (Input.GetMouseButtonDown(0))
            {
                index_of_team = team_slots.IndexOf(slot_team);

                if (things_in_team[index_of_team] != null)
                {
                    things_in_team[index_of_team].transform.SetParent(mouse.transform, false);
                    things_in_team[index_of_team] = null;
                }
                else
                {
                    index_of_team = -1;
                }
            }
        }

        // Change Text of End Button To Freeze And Sell
        if (index_of_store == -1 && index_of_team == -1)
        {
            end_turn_button.GetComponentInChildren<Text>().text = "End Turn";
            end_turn_button.GetComponentInChildren<Text>().color = Color.white;
        }
        else if (index_of_store != -1)
        {
            end_turn_button.GetComponentInChildren<Text>().text = "Freeze";
            end_turn_button.GetComponentInChildren<Text>().color = Color.cyan;
        }
        else if (index_of_team != -1)
        {
            end_turn_button.GetComponentInChildren<Text>().text = "Sell";
            end_turn_button.GetComponentInChildren<Text>().color = Color.yellow;
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Click Up Out of a Slot or Team Full
            if (slot_team == null || (index_of_store != -1 && gold < 3) || (selected_team_class != mouse_class && mouse_class != null && things_in_team[0] != null && things_in_team[1] != null && things_in_team[2] != null && things_in_team[3] != null && things_in_team[4] != null) || (mouse.transform.childCount == 1 && mouse_class == null && selected_team_class == null))
            {
                if (index_of_store != -1)
                {
                    things_in_slots[index_of_store].transform.SetParent(store_slots[index_of_store].transform, false);

                    // For Freeze
                    RectTransform end_rect = end_turn_button.GetComponent<RectTransform>();
                    SpriteRenderer creature_sprite = things_in_slots[index_of_store].GetComponentInChildren<SpriteRenderer>();

                    if (mouse.transform.position.x < end_turn_button.transform.position.x + end_rect.sizeDelta.x / 200  && mouse.transform.position.x > end_turn_button.transform.position.x - end_rect.sizeDelta.x / 200 && mouse.transform.position.y < end_turn_button.transform.position.y + end_rect.sizeDelta.y / 200 && mouse.transform.position.y > end_turn_button.transform.position.y - end_rect.sizeDelta.y / 200)
                    {
                        if (creature_sprite.color != Color.cyan)
                        {
                            creature_sprite.color = Color.cyan;
                            if (creature_sprite.gameObject.tag == "Friend" || creature_sprite.gameObject.tag == "Enemy") creature_sprite.gameObject.GetComponent<Animator>().speed = 0;
                        }
                        else
                        {
                            creature_sprite.color = Color.white;
                            if (creature_sprite.gameObject.tag == "Friend" || creature_sprite.gameObject.tag == "Enemy") creature_sprite.gameObject.GetComponent<Animator>().speed = 1;
                        }
                    }
                }

                if (index_of_team != -1)
                {
                    // For Sell
                    RectTransform end_rect = end_turn_button.GetComponent<RectTransform>();
                    BasicCreatureScript creature_script = mouse.transform.GetChild(0).gameObject.GetComponentInChildren<BasicCreatureScript>();

                    if (mouse.transform.position.x < end_turn_button.transform.position.x + end_rect.sizeDelta.x / 200 && mouse.transform.position.x > end_turn_button.transform.position.x - end_rect.sizeDelta.x / 200 && mouse.transform.position.y < end_turn_button.transform.position.y + end_rect.sizeDelta.y / 200 && mouse.transform.position.y > end_turn_button.transform.position.y - end_rect.sizeDelta.y / 200)
                    {
                        if (creature_script.SellCreature() == false)
                        {
                            things_in_team[index_of_team] = mouse.transform.GetChild(0).gameObject;
                            things_in_team[index_of_team].transform.SetParent(team_slots[index_of_team].transform, false);
                        }
                    }
                    else
                    {
                        things_in_team[index_of_team] = mouse.transform.GetChild(0).gameObject;
                        things_in_team[index_of_team].transform.SetParent(team_slots[index_of_team].transform, false);
                    }
                }
            }
            // Click Up in a Slot
            else if (mouse.transform.childCount == 1)
            {
                bool xp_up = false;

                if (selected_team_class == mouse_class && selected_team_level != 3)
                {
                    xp_up = true;
                }
                else if (selected_team_class != null && mouse_class != null)
                {
                    MoveSlot();
                }

                if (index_of_store != -1)
                {
                    GameObject creature = null;

                    if ((selected_team_class != mouse_class || things_in_team[team_slots.IndexOf(slot_team)] == null) && mouse_class != null) things_in_team[team_slots.IndexOf(slot_team)] = things_in_slots[index_of_store];
                    else creature = things_in_slots[index_of_store];

                    if (mouse_class != null)
                    {
                        things_in_slots[index_of_store].transform.SetParent(slot_team.transform, false);
                        things_in_slots[index_of_store].GetComponent<BasicCreatureScript>().item = false;
                    }
                    else
                    {
                        BasicItemScript item = creature.GetComponent<BasicItemScript>();
                        GameObject creature_to_equip = things_in_team[team_slots.IndexOf(slot_team)];

                        gold += item.AddAndGetDiscount(0);

                        if (item.is_equip == true)
                        {
                            item.DoEquipItem(creature_to_equip);
                            creature_to_equip.GetComponent<BasicCreatureScript>().SetItem(item.gameObject);
                        }
                        else
                        {
                            item.DoNoEquipItem(creature_to_equip);
                        }

                        item.transform.SetParent(null, false);
                    }

                    things_in_slots[index_of_store].GetComponent<SpriteRenderer>().color = Color.white;
                    things_in_slots[index_of_store] = null;
                    gold -= 3;

                    if (xp_up == true)
                    {
                        things_in_team[team_slots.IndexOf(slot_team)].GetComponent<BasicCreatureScript>().AddAndGetXp(creature.gameObject.GetComponent<BasicCreatureScript>().GetGiveXpAndDestroySelf());
                    }
                }

                if (index_of_team != -1)
                {
                    GameObject creature = null;
                    int new_index = team_slots.IndexOf(slot_team);

                    if (selected_team_class != mouse_class || things_in_team[new_index] == null) things_in_team[new_index] = mouse.transform.GetChild(0).gameObject;
                    else creature = mouse.transform.GetChild(0).gameObject;

                    things_in_team[new_index].transform.SetParent(team_slots[new_index].transform, false);

                    if (xp_up == true)
                    {
                        things_in_team[team_slots.IndexOf(slot_team)].GetComponent<BasicCreatureScript>().AddAndGetXp(creature.gameObject.GetComponent<BasicCreatureScript>().GetGiveXpAndDestroySelf());
                    }
                }
            }

            index_of_store = -1;
            index_of_team = -1;
        }

        if (mouse_class != null)
        {
            if (selected_team_class != null)
            {
                slot_cd += Time.deltaTime;
            }
            else
            {
                slot_cd = 0;
            }

            if (slot_cd >= 1)
            {
                MoveSlot();
            }
        }
        else
        {
            slot_cd = 0;
        }
    }

    void MoveSlot()
    {
        if (things_in_team[0] == null || things_in_team[1] == null || things_in_team[2] == null || things_in_team[3] == null || things_in_team[4] == null)
        {
            int index = team_slots.IndexOf(slot_team);

            switch (index)
            {
                case 0:
                    for (int i = 1; i < 5; i++)
                    {
                        if (things_in_team[i] == null)
                        {
                            for (int disloc = i - 1; disloc >= 0; disloc--)
                            {
                                things_in_team[disloc + 1] = things_in_team[disloc];
                                things_in_team[disloc].transform.SetParent(team_slots[disloc + 1].transform, false);
                                things_in_team[disloc] = null;
                            }

                            break;
                        }
                    }
                    break;

                case 1:
                    if (things_in_team[0] == null)
                    {
                        things_in_team[0] = things_in_team[1];
                        things_in_team[1].transform.SetParent(team_slots[0].transform, false);
                        things_in_team[1] = null;

                        break;
                    }
                    else
                    {
                        for (int i = 2; i < 5; i++)
                        {
                            if (things_in_team[i] == null)
                            {
                                for (int disloc = i - 1; disloc >= 1; disloc--)
                                {
                                    things_in_team[disloc + 1] = things_in_team[disloc];
                                    things_in_team[disloc].transform.SetParent(team_slots[disloc + 1].transform, false);
                                    things_in_team[disloc] = null;
                                }

                                break;
                            }
                        }

                        break;
                    }

                case 2:
                    if (things_in_team[3] == null)
                    {
                        things_in_team[3] = things_in_team[2];
                        things_in_team[2].transform.SetParent(team_slots[3].transform, false);
                        things_in_team[2] = null;

                        break;
                    }
                    else if (things_in_team[1] == null)
                    {
                        things_in_team[1] = things_in_team[2];
                        things_in_team[2].transform.SetParent(team_slots[1].transform, false);
                        things_in_team[2] = null;

                        break;
                    }
                    else if (things_in_team[4] == null)
                    {
                        things_in_team[4] = things_in_team[3];
                        things_in_team[3].transform.SetParent(team_slots[4].transform, false);
                        things_in_team[3] = null;

                        things_in_team[3] = things_in_team[2];
                        things_in_team[2].transform.SetParent(team_slots[3].transform, false);
                        things_in_team[2] = null;

                        break;
                    }
                    else
                    {
                        things_in_team[0] = things_in_team[1];
                        things_in_team[1].transform.SetParent(team_slots[0].transform, false);
                        things_in_team[1] = null;

                        things_in_team[1] = things_in_team[2];
                        things_in_team[2].transform.SetParent(team_slots[1].transform, false);
                        things_in_team[2] = null;

                        break;
                    }

                case 3:
                    if (things_in_team[4] == null)
                    {
                        things_in_team[4] = things_in_team[3];
                        things_in_team[3].transform.SetParent(team_slots[4].transform, false);
                        things_in_team[3] = null;

                        break;
                    }
                    else
                    {
                        for (int i = 3; i >= 1; i--)
                        {
                            if (things_in_team[i] == null)
                            {
                                for (int disloc = i + 1; disloc < 4; disloc++)
                                {
                                    things_in_team[disloc - 1] = things_in_team[disloc];
                                    things_in_team[disloc].transform.SetParent(team_slots[disloc - 1].transform, false);
                                    things_in_team[disloc] = null;
                                }

                                break;
                            }
                        }

                        break;
                    }

                case 4:
                    for (int i = 4; i >= 1; i--)
                    {
                        if (things_in_team[i] == null)
                        {
                            for (int disloc = i + 1; disloc < 5; disloc++)
                            {
                                things_in_team[disloc - 1] = things_in_team[disloc];
                                things_in_team[disloc].transform.SetParent(team_slots[disloc - 1].transform, false);
                                things_in_team[disloc] = null;
                            }

                            break;
                        }
                    }
                    break;
            }
        }
    }

    void CreateEnemies()
    {
        List<GameObject> enemies = new List<GameObject> { null, null, null, null, null };

        enemies[3] = rank1_creatures[0];
        enemies[4] = rank1_creatures[0];

        for (int i = 0; i < 5; i++)
        {
            if (enemies[i] != null)
            {
                GameObject new_enemy = Instantiate(enemies[i], enemy_slots[i].transform.position, Quaternion.identity, enemy_slots[i].transform);
                things_in_enemy[i] = new_enemy;
                new_enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                new_enemy.transform.localScale = new Vector3(new_enemy.transform.localScale.x * (-1), new_enemy.transform.localScale.y, new_enemy.transform.localScale.z);
            }
        }
    }

    bool DeadAnimationEnds()
    {
        bool team_ready = (team_count == 0 ? false : true);
        bool enemy_ready = (enemy_count == 0 ? false : true);

        if (team_count == 0) {
            foreach (GameObject creature in things_in_team)
            {
                if (creature != null)
                {
                    if (creature.GetComponent<BasicCreatureScript>().GetAnimationSpeed() == 0) team_ready = true;
                    else
                    {
                        team_ready = false;
                        break;
                    }
                }
            }
        }

        if (enemy_count == 0)
        {
            foreach (GameObject creature in things_in_enemy)
            {
                if (creature != null)
                {
                    if (creature.GetComponent<BasicCreatureScript>().GetAnimationSpeed() == 0) enemy_ready = true;
                    else
                    {
                        enemy_ready = false;
                        break;
                    }
                }
            }
        }

        return team_ready && enemy_ready;
    }

    public void RollButton()
    {
        bool empty_team = true;

        foreach (GameObject creature in things_in_team)
        {
            if (creature != null)
            {
                empty_team = false;
                break;
            }
        }

        if ((empty_team == false && gold > 0) || (empty_team == true && gold > 3))
        {
            gold--;
            RefreshStore();
        }
    }

    public void EndTurnButton()
    {
        team_count = 0;
        enemy_count = 0;

        foreach (GameObject team_creature in things_in_team)
        {
            if (team_creature != null) team_count++;
        }

        foreach (GameObject enemy_creature in things_in_enemy)
        {
            if (enemy_creature != null) enemy_count++;
        }

        if (team_count > 0)
        {
            state = "move_camera_up";
            ActiveTeamSlotSprites(false);
            ActiveEnemySlotSprites(false);
            ActiveStoreSlotsSprites(false);
        }
    }

    void ActiveStoreSlotsSprites(bool active)
    {
        for (int i = 0; i < store_slots.Count; i++)
        {
            store_slots[i].transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = (things_in_slots[i] != null ? active : false);
        }
    }

    void ActiveTeamSlotSprites(bool active)
    {
        for (int i = 0; i < team_slots.Count; i++)
        {
            team_slots[i].transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = active;
        }
    }

    void ActiveEnemySlotSprites(bool active)
    {
        for (int i = 0; i < enemy_slots.Count; i++)
        {
            enemy_slots[i].transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = (things_in_enemy[i] != null ? active : false);
        }
    }

    public string GetState()
    {
        return state;
    }

    public void SetState(string new_state)
    {
        state = new_state;
    }

    public void SetSlotStore(GameObject slot)
    {
        slot_store = slot;
    }

    public void SetSlotTeam(GameObject slot)
    {
        slot_team = slot;
    }

    public void SetSlotEnemy(GameObject slot)
    {
        slot_enemy = slot;
    }

    public void RemoveFromTeamCount(bool remove = true)
    {
        if (remove == true)
        {
            team_count--;
        }
        else
        {
            team_count++;
        }
    }

    public void RemoveFromEnemyCount(bool remove = true)
    {
        if (remove == true)
        {
            enemy_count--;
        }
        else
        {
            enemy_count++;
        }
    }

    public bool IsSomeTeamDead()
    {
        return (team_count == 0 || enemy_count == 0 ? true : false);
    }

    public void CreatureDoLevelUp()
    {
        GameObject new_thing = null;

        switch (rank)
        {
            case 1:
                new_thing = rank2_creatures[UnityEngine.Random.Range(0, rank2_creatures.Count)];
                break;

            case 2:
                new_thing = rank3_creatures[UnityEngine.Random.Range(0, rank3_creatures.Count)];
                break;

            case 3:
                new_thing = rank4_creatures[UnityEngine.Random.Range(0, rank4_creatures.Count)];
                break;

            case 4:
                new_thing = rank5_creatures[UnityEngine.Random.Range(0, rank5_creatures.Count)];
                break;

            default:
                new_thing = rank6_creatures[UnityEngine.Random.Range(0, rank6_creatures.Count)];
                break;
        }

        for (int i = 0; i < 7; i++)
        {
            if (store_slots[i].activeInHierarchy == true)
            {
                if (things_in_slots[i] == null)
                {
                    GameObject creature = Instantiate(new_thing, store_slots[i].transform.position, Quaternion.identity, store_slots[i].transform);

                    things_in_slots[i] = creature;
                    things_in_slots[i].transform.position = new Vector3(things_in_slots[i].transform.position.x, things_in_slots[i].transform.position.y, store_slots[i].transform.position.z - 1);
                    things_in_slots[i].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                    things_in_slots[i].GetComponent<BasicCreatureScript>().enemy = false;
                    things_in_slots[i].GetComponent<BasicCreatureScript>().item = true;
                    break;
                }
            }
            else
            {
                store_slots[i].SetActive(true);
                Destroy(things_in_slots[i]);

                GameObject creature = Instantiate(new_thing, store_slots[i].transform.position, Quaternion.identity, store_slots[i].transform);

                things_in_slots[i] = creature;
                things_in_slots[i].transform.position = new Vector3(things_in_slots[i].transform.position.x, things_in_slots[i].transform.position.y, store_slots[i].transform.position.z - 1);
                things_in_slots[i].GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                things_in_slots[i].GetComponent<BasicCreatureScript>().enemy = false;
                things_in_slots[i].GetComponent<BasicCreatureScript>().item = true;
                break;
            }
        }
    }

    public void AddToEffList(GameObject creature)
    {
        for (int i = 0; i < monsters_with_eff.Count; i++)
        {
            if (monsters_with_eff[i] == null)
            {
                monsters_with_eff[i] = creature;
                break;
            }
        }
    }

    public void RemoveFromEffList(GameObject creature)
    {
        int go_back = 10;

        for (int i = 0; i < monsters_with_eff.Count; i++)
        {
            if (monsters_with_eff[i] == creature)
            {
                go_back = i + 1;
                monsters_with_eff[i] = null;
            }

            if (i >= go_back)
            {
                monsters_with_eff[i - 1] = monsters_with_eff[i];
                monsters_with_eff[i] = null;
            }
        }
    }

    public int GetEffsToDoCont()
    {
        int count = 0;

        foreach (GameObject creature in monsters_with_eff)
        {
            if (creature != null)
            {
                count++;
            }
        }

        return count;
    }

    public GameObject GetNextListEff()
    {
        foreach (GameObject creature in monsters_with_eff)
        {
            if (creature != null)
            {
                return creature;
            }
        }

        return null;
    }

    public int AddAndGetGold(int add)
    {
        gold = math.max(0, gold + add);

        return gold;
    }

    public List<GameObject> GetStoreList()
    {
        return things_in_slots;
    }

    public List<GameObject> GetTeamList()
    {
        return things_in_team;
    }

    public List<GameObject> GetEnemyList()
    {
        return things_in_enemy;
    }

    public GameObject SwitchThingsInStore(int slot_number, GameObject new_thing)
    {
        if (things_in_slots[slot_number] != null)
        {
            Destroy(things_in_slots[slot_number]);
        }

        GameObject thing = Instantiate(new_thing, store_slots[slot_number].transform.position, Quaternion.identity, store_slots[slot_number].transform);

        things_in_slots[slot_number] = thing;

        if (thing.tag == "Friend" || thing.tag == "Enemy") thing.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        return thing;
    }
}