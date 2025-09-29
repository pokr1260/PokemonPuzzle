using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpProgressHandle : MonoBehaviour
{
    public GameObject progressImage;
    private Image handleImage;

    public float jumpHeight = 25.0f;
    public float jumpSpeed = 5.0f;

    [Range(0.0f,1.0f)]
    public float alpha = 1.0f;

    private void Start()
    {
        if (null == progressImage)
        {
            Debug.LogError("JumpProgressHandle.cs / progressImage is null");
            return;
        }   


    }

    private void Update()
    {
        float height = Mathf.Abs(jumpHeight * Mathf.Sin(Time.time * jumpSpeed));
        Vector3 pos = progressImage.transform.localPosition;
        pos.y = height;

        progressImage.transform.localPosition = pos;
    }
}
