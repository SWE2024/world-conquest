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
    public static int Roll() => GameController.Random.Next(1, 7);
}
