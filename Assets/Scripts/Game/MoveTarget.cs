using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTarget : MonoBehaviour
{
    public float speed;

    private Vector3 target;

    public void Init(Vector3 target)
    {
        this.target = target;
    }

    private void Update()
    {
        if (Mathf.Abs(target.x - transform.position.x) > 0.01f || Mathf.Abs(target.y - transform.position.y) > 0.01f)
        {
            transform.position = Vector2.Lerp(transform.position, target, speed * Time.deltaTime);
        }
        else
        {
            transform.position= target;
            Destroy(this.gameObject, 1.0f);
        }
    }


}
