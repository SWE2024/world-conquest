using UnityEngine;
using System.Collections;

/// <summary>
/// <c>Wait</c> controls running functions after a time delay.
/// Inspired by: https://forum.unity.com/threads/how-to-pause-without-freezing.526166/
/// </summary>
public class Wait : MonoBehaviour
{
    float delay;
    System.Action action;

    /// <summary>
    /// <c>Start</c> creates a timer that will run a function after <c>delay</c> seconds.
    /// </summary>
    /// <param name="delay">The time until a function is ran (in seconds).</param>
    /// <param name="action">The function you would like to call after <c>delay</c>.</param>
    /// <returns>
    /// <c>GameObject</c> of type <c>Wait</c> which runs <c>action</c> and then destroys itself.
    /// </returns>
    public static Wait Start(float delay, System.Action action)
    {
        Wait waitObj = new GameObject("Wait").AddComponent<Wait>(); // creates a new Wait GameObject
        waitObj.delay = delay;
        waitObj.action = action;
        return waitObj;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay); // waits for delay seconds
        action(); // runs the code block you wrote
        Destroy(gameObject);
    }
}