using UnityEngine;

public class TargetMemory {
    
    public TargetMemory(Vector2Int location, int health, int initiative, int armor, int attack) {
        this.IsAccurate = true;
        this.Location = location;
        this.health = health;
        this.initiative = initiative;
        this.armor = armor;
        this.attack = attack;
    }

    public bool IsAccurate;
    public Vector2Int Location;
    public int health;
    public int initiative;
    public int armor;
    public int attack;
}