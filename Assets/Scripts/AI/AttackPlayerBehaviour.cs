using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AttackPlayerBehaviour : Behaviour<EnemyControl> {

    public int HealthWeight = 1;
    public int AttackWeight = 2;

    public Intent? Compute(EnemyControl enemyControl) {
        List<TargetMemory> options = new List<TargetMemory>();
        foreach (TargetMemory targetMemory in enemyControl.Memory.Values)
            if (
                targetMemory.IsAccurate &&
                (targetMemory.Location - ((Vector2Int)enemyControl.GetCurrentTile().Location)).sqrMagnitude <= enemyControl.MovementRange * enemyControl.MovementRange &&
                this.Choose(targetMemory, enemyControl)
            ) options.Add(targetMemory);
        if (options.Count > 0) {
            options.Sort(ChooseBest);
            return new Intent(
                this.GetIntentType(), options.First().Location
            );
        }
        return null;
    }

    public virtual bool Choose(TargetMemory targetMemory, EnemyControl thisElement) {
        return true;
    }

    public virtual int ChooseBest(TargetMemory enemy1, TargetMemory enemy2) {
        return (enemy2.attack - enemy1.attack) * AttackWeight + (enemy1.health - enemy2.health) * HealthWeight;
    }

    public virtual IntentType GetIntentType() => IntentType.ATTACK;

}