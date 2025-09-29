using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    https://usroom.tistory.com/entry/Unity-%ED%95%B4%EC%83%81%EB%8F%84-%EC%84%A4%EC%A0%95
    https://smilejsu.tistory.com/1618
    https://answers.unity.com/questions/620699/scaling-my-background-sprite-to-fill-screen-2d-1.html?_ga=2.44852391.228112900.1586504428-1928946603.1577076231
*/

public class FillScreenSprite : MonoBehaviour
{
    [Tooltip("-1인 경우 가득 채운다.")]
    public Vector2 targetSize;
    public GameObject bg;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        SpriteRenderer bgSpriteRenderer = bg.GetComponent<SpriteRenderer>();
        Transform bgTransform = bg.transform;

        bgTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        float width = 1.0f;
        float height = 1.0f;
        if (null != bgSpriteRenderer)
        {
            width = bgSpriteRenderer.bounds.size.x;
            height = bgSpriteRenderer.bounds.size.y;
        }

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        Vector2 result = new Vector2(worldScreenWidth / width, worldScreenHeight / height);

        if (-1 != targetSize.x)
        {
            result.x = targetSize.x;
        }

        if (-1 != targetSize.y)
        {
            result.y = targetSize.y;
        }

        bgTransform.localScale = result;
    }





}
