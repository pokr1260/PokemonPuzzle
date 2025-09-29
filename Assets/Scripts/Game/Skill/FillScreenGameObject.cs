using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillScreenGameObject : MonoBehaviour
{
    [Tooltip("-1인 경우 가득 채운다.")]
    public Vector2 targetSize;

    protected float worldScreenHeight;
    protected float worldScreenWidth;

    protected Vector2 result;

    public virtual void Init()
    {
        this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        float width = 1.0f;
        float height = 1.0f;

        worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        result = new Vector2(worldScreenWidth / width, worldScreenHeight / height);

        this.transform.localScale = result;
    }
}
