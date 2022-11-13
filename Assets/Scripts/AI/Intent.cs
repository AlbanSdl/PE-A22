using UnityEngine;

public struct Intent {
    public Intent(IntentType Type, Vector2Int Location) {
        this.Type = Type;
        this.Location = Location;
    }

    public IntentType Type;
    public Vector2Int Location;
}

public enum IntentType {
    MOVE, ATTACK, KILL
}