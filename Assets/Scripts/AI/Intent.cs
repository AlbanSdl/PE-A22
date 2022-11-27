using UnityEngine;

/// <summary>
/// An Intent is the result of a positive behaviour. It includes the position where the character should go to
/// and the reason why it should go to this position (ie. the goal of the movement).
/// </summary>
public struct Intent {
    public Intent(IntentType Type, Vector2Int Location) {
        this.Type = Type;
        this.Location = Location;
    }

    /// <summary>
    /// The goal of this Intent
    /// </summary>
    public IntentType Type;
    /// <summary>
    /// The position this Intent targets
    /// </summary>
    public Vector2Int Location;
}

/// <summary>
/// This is the goal of an <see cref="Intent"/>. It can only take one value at a time, so always use the
/// most significative one. (eg. don't use <see cref="IntentType.MOVE"/> if your goal is to <see cref="IntentType.ATTACK"/> a character)
/// </summary>
/// <example>
/// An Intent created to kill an Enemy should be using the <see cref="IntentType.KILL"/> IntentType.
/// </example>
public enum IntentType {
    MOVE, ATTACK, KILL
}