using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;

[CreateAssetMenu(fileName = "Creature")]

public class Creature : ScriptableObject
{
    //Enemy or Not
    public bool enemy = true;

    //Stats
    public string creature_class = "";
    public int creature_hp = 1;
    public int creature_atk = 1;
    public int creature_special = 1;
    public int creature_int = 0;
    public int creature_jump_force = 400;

    //Sprites
    public string creature_idle_sprite = "";
    public string creature_hurt_sprite = "";
    public string creature_death_sprite = "";
    public string creature_atk_1_sprite = "";
    public string creature_atk_2_sprite = "";
    public string creature_jump_sprite = "";

    //Fly Creatures
    public bool fly = false;
    public float ground_distance = 0;
}