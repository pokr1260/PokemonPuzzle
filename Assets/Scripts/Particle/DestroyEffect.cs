using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    public float destroyDelay = 1.0f;

    private void Start()
    {
        Destroy(this.gameObject, destroyDelay);
    }
}
