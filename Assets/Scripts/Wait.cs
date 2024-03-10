using UnityEngine;
using System.Collections;

public class Wait : MonoBehaviour
{
    // inspiration from https://forum.unity.com/threads/how-to-pause-without-freezing.526166/

    float delay;
    System.Action action;

    public static Wait Start(float delay, System.Action action)
    {
        Wait waitObj = new GameObject("Wait").AddComponent<Wait>();
        waitObj.delay = delay;
        waitObj.action = action;
        return waitObj;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);
        action(); // runs the code block you wrote
        Destroy(gameObject);
    }
}