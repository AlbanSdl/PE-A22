using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyControl : MonoBehaviour
{

    public AlliesData AllyData;

    public Sprite sprite;
    public new string name;
    public int health;
    public int initiative;
    public int attack;
    public int armor;
    public void Start()
    {
        if (AllyData != null) {
            LoadData(AllyData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadData (AlliesData data) {
        sprite = data.Sprite;
        health = data.Health;
        initiative = data.Initiative;
        attack = data.Attack;
        armor = data.Armor;
        name = data.CharacterName;
    }
}
