using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This is a behaviour that can be applied to an <see cref="EnemyControl"/>.
/// Using this behaviour, an enemy will follow other enemies to attack and kill players, even if the so-called
/// players are not in the range of this enemy.
/// </summary>
public class FollowPackBehaviour : Behaviour<EnemyControl> {

    public int HealthWeight = 1;
    public int AttackWeight = 2;

    public Intent? Compute(EnemyControl enemyControl) {
        HashSet<EnemyControl> packMembers = enemyControl.Pack;
        if (packMembers.Count >= 3) return null;
        List<TargetMemory> options = new List<TargetMemory>();
        foreach (EnemyControl packMember in packMembers)
            if (packMember.Intent?.Type == IntentType.ATTACK && (packMember.Intent.Value.Location - ((Vector2Int)enemyControl.GetCurrentTile().Location)).sqrMagnitude <= enemyControl.MovementRange * enemyControl.MovementRange)
                options.Add(new List<TargetMemory>(packMember.Memory.Values).Find((target) => target.Location == packMember.Intent.Value.Location));
        if (options.Count > 0) {
            options.Sort((enemy1, enemy2) => (enemy2.attack - enemy1.attack) * AttackWeight + (enemy1.health - enemy2.health) * HealthWeight);
            return new Intent(IntentType.ATTACK, options.First().Location);
        }
        return null;
    }

}