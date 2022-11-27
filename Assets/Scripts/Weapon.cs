using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")] 
public class Weapon : ScriptableObject {
    public new string name;
    public float DamageMultiplier = 1f;
    public float ExtraDamageMultiplier = 1.2f;
    public float TerrainDamageMultiplier = 1.1f;
    public Weapon ExtraDamageForWeapon;
}
