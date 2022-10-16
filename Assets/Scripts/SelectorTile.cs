using System;
using UnityEngine;

public class SelectorTile : MonoBehaviour
{
    public Vector3Int Location { get; private set; }

    public bool ShowTile() {
        if (Location.z < 0) return false;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        return true;
    }

    public bool HideTile() {
        if (Location.z < 0) return false;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        return true;
    }

    public void InitializeLocation(Vector3Int location) {
        if (Location.x != 0 && Location.y != 0 && Location.z != 0) throw new System.Exception("La tile a déjà été positionnée");
        // Copy vector so that it won't be updated later on
        Location = new Vector3Int(location.x, location.y, location.z);
    }

    public bool CanAccessTo(SelectorTile OtherTile, bool ignoreCharacters = false) {
        return OtherTile.Location.z >= 0 && Math.Abs(OtherTile.Location.z - Location.z) < 2 && (ignoreCharacters || OtherTile.GetAllyOnTile() == null);
    }

    #nullable enable
    public AllyControl? GetAllyOnTile() {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");
        foreach (var character in characters) {
            AllyControl ally = character.GetComponent<AllyControl>();
            if (ally.GetCurrentTile() == this) return ally;
        }
        return null;
    }

}
