/// <summary>
/// This is a behaviour that can be applied to an <see cref="EnemyControl"/>.
/// In order to determine what an <see cref="EnemyControl"/> does, behaviours are run one ather the others
/// until one behaviour appears to be benefic to its <see cref="EnemyControl"/>.
/// </summary>
public interface Behaviour<in Data> {

    /// <summary>
    /// Creates the <see cref="Intent"/> of the behaviour. If the behaviour should neither be selected
    /// nor run, return a null value. Otherwise, generate the intent with the movement goal attached as
    /// a position and an <see cref="IntentType"/>
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public Intent? Compute(Data data);
}