using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ally", menuName = "Characters/Allies")] 
public class AlliesData : ScriptableObject {

    public string CharacterName;
    public Sprite Sprite;

    public int Initiative;
    public int Health;
    public int Armor;
    public int Attack;
    public int TempArmor;
    public int MovementRange = 5;
    public Weapon Weapon;
}
