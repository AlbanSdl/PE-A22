using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ExploreKnownTargetLocationBehaviour : Behaviour<EnemyControl> {

    public int HealthWeight = 5;
    public int AttackWeight = 1;

    public Intent? Compute(EnemyControl enemyControl) {
        List<TargetMemory> options = new List<TargetMemory>();
        foreach (TargetMemory target in enemyControl.Memory.Values)
            if (!target.IsAccurate && target.health + target.armor < enemyControl.attack)
                options.Add(target);
        if (options.Count > 0) {
            options.Sort((enemy1, enemy2) => (enemy1.Location - ((Vector2Int)enemyControl.GetCurrentTile().Location)).sqrMagnitude - (enemy2.Location - ((Vector2Int)enemyControl.GetCurrentTile().Location)).sqrMagnitude);
            TargetMemory choice = options.First();
            return new Intent((choice.Location - ((Vector2Int)enemyControl.GetCurrentTile().Location)).sqrMagnitude <= enemyControl.MovementRange * enemyControl.MovementRange ? IntentType.ATTACK : IntentType.MOVE, choice.Location);
        }
        return null;
    }

}