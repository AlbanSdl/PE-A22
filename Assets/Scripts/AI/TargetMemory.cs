using UnityEngine;

/// <summary>
/// A TargetMemory is the memory the current Character has of another Character.
/// It includes the last known <see cref="TargetMemory.Location"/> of the Opponents,
/// their last known <see cref="TargetMemory.health"/>, <see cref="TargetMemory.armor"/>, etc
/// </summary>
public class TargetMemory {
    
    public TargetMemory(Vector2Int location, int health, int initiative, int armor, int attack) {
        this.IsAccurate = true;
        this.Location = location;
        this.health = health;
        this.initiative = initiative;
        this.armor = armor;
        this.attack = attack;
    }

    /// <summary>
    /// Indicates whether this <see cref="TargetMemory"/> is up to date
    /// </summary>
    public bool IsAccurate;
    /// <summary>
    /// The Location where the Opponent has been seen for the last time (by the current Character)
    /// </summary>
    public Vector2Int Location;
    /// <summary>
    /// The last known health of the Opponent
    /// </summary>
    public int health;
    /// <summary>
    /// The last known initiative of the Opponent
    /// </summary>
    public int initiative;
    /// <summary>
    /// The last known armor of the Opponent
    /// </summary>
    public int armor;
    /// <summary>
    /// The last known attack of the Opponent
    /// </summary>
    public int attack;
}