using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This is a behaviour that can be applied to an <see cref="EnemyControl"/>.
/// Using this behaviour, an enemy will walk straight to an ally in order to attack him.
/// </summary>
public class AttackPlayerBehaviour : Behaviour<EnemyControl> {

    /// <summary>
    /// Determines the weight of the ally's heath in the decision of wether using this behaviour
    /// </summary>
    public int HealthWeight = 1;
    /// <summary>
    /// Determines the weight of the ally's attack in the decision of wether using this behaviour
    /// </summary>
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

    /// <summary>
    /// Indicates whether this Behaviour can be used on the current <see cref="TargetMemory"/>
    /// </summary>
    /// <param name="targetMemory"></param>
    /// <param name="thisElement"></param>
    /// <returns></returns>
    public virtual bool Choose(TargetMemory targetMemory, EnemyControl thisElement) {
        return true;
    }

    /// <summary>
    /// Indicates which <see cref="TargetMemory"/> will give the best result for the behaviour.
    /// </summary>
    /// <param name="enemy1"></param>
    /// <param name="enemy2"></param>
    /// <returns></returns>
    public virtual int ChooseBest(TargetMemory enemy1, TargetMemory enemy2) {
        return (enemy2.attack - enemy1.attack) * AttackWeight + (enemy1.health - enemy2.health) * HealthWeight;
    }

    /// <summary>
    /// Returns the <see cref="IntentType"/> of the current behaviour
    /// </summary>
    /// <returns></returns>
    public virtual IntentType GetIntentType() => IntentType.ATTACK;

}