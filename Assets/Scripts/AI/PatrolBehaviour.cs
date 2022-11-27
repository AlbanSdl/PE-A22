using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This is a behaviour that can be applied to an <see cref="EnemyControl"/>.
/// Using this behaviour, an enemy will move randomly. This is the last behaviour you should use,
/// only when all the other behaviours failed to give an <see cref="Intent"/>.
/// </summary>
public class PatrolBehaviour : Behaviour<EnemyControl> {

    public Intent? Compute(EnemyControl enemyControl) {
        List<Vector2Int> movements = new List<Vector2Int>(enemyControl.GetPossibleMovements());
        if (movements.Count < 1) return null;
        return new Intent(IntentType.MOVE, movements[Random.Range(0, movements.Count)]);
    }

}