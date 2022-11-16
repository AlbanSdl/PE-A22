using UnityEngine;
using System.Collections.Generic;

public class PatrolBehaviour : Behaviour<EnemyControl> {

    public Intent? Compute(EnemyControl enemyControl) {
        List<Vector2Int> movements = new List<Vector2Int>(enemyControl.GetPossibleMovements());
        if (movements.Count < 1) return null;
        return new Intent(IntentType.MOVE, movements[Random.Range(0, movements.Count)]);
    }

}