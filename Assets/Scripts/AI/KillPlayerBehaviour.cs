/// <summary>
/// This is a behaviour that can be applied to an <see cref="EnemyControl"/>.
/// Using this behaviour, an enemy will walk straight to an ally in order to kill him.
/// </summary>
sealed public class KillPlayerBehaviour : AttackPlayerBehaviour {

    public override bool Choose(TargetMemory targetMemory, EnemyControl thisElement) {
        return targetMemory.health + targetMemory.armor < thisElement.attack;
    }

    public override int ChooseBest(TargetMemory enemy1, TargetMemory enemy2) {
        return enemy2.attack - enemy1.attack;
    }

    public override IntentType GetIntentType() => IntentType.KILL;

}