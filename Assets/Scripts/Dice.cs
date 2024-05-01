/// <summary>
/// <c>Dice</c> simulates a physical dice.
/// </summary>
public class Dice
{
    /// <summary>
    /// <c>Roll</c> rolls the virtual dice.
    /// </summary>
    /// <returns>
    /// A random number that is between 1 and 6.
    /// </returns>
    public static int Roll() => UnityEngine.Random.Range(1, 7); // int is exclusive, float is inclusive
}
