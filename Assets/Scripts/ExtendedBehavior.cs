using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedBehavior : MonoBehaviour
{
    public void Wait(float seconds, Action action)
    {
        StartCoroutine(_wait(seconds, action));
    }

    IEnumerator _wait(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        if (null != callback)
        {
            callback();
        }
    }
}
