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
        if (Location.x != 0 && Location.y != 0 && Location.z != 0) throw new System.Exception("La tile a déjà été positionnée.");
        // Copy vector so that it won't be updated later on
        Location = new Vector3Int(location.x, location.y, location.z);
    }

    public bool CanAccessTo<O, T>(SelectorTile OtherTile, bool ignoreCharacters = false) where T : AbstractMovement<O, T> where O : AbstractMovement<T, O> {
        return OtherTile.Location.z >= 0 && Math.Abs(OtherTile.Location.z - Location.z) < 2 && OtherTile.GetCharacterOnTile<O, T>() == null && (ignoreCharacters || OtherTile.GetCharacterOnTile<T, O>() == null);
    }

    #nullable enable
    public T? GetCharacterOnTile<O, T>() where T: AbstractMovement<O, T> where O: AbstractMovement<T, O> {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");
        foreach (var character in characters) {
            T enemy = character.GetComponent<T>();
            if (enemy != null && enemy.GetCurrentTile() == this) return enemy;
        }
        return null;
    }

}
