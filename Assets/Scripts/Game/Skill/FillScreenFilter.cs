using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillScreenFilter : FillScreenGameObject
{
    public override void Init()
    {
        base.Init();

        // 캐릭터 화면 가운데로 이동
        float height = Screen.height;
        float UIHeight = GameManager.Instance.rightTopScreenPosition.y - GameManager.Instance.leftBottomScreenPosition.y;
        float puzzleZoneHeight = (height * GameManager.Instance.GetPuzzleZoneBG().GetTotalHeight()) / UIHeight;

        float center = ((Screen.height - puzzleZoneHeight) / 2.0f) + puzzleZoneHeight;

        Vector3 posResult = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, center, 0.0f));
        posResult.x = 0.0f;

        this.transform.position = posResult;

        // 크기 조절
        // (Screen.height - puzzleZoneHeight) : Screen.height == x : Camera.main.orthographicSize * 2.0f;
        // 외항의 곱은 내항의 곱과 같다 를 이용
        // Screen.height * x == (Screen.height - puzzleZoneHeight) * Camera.main.orthographicSize * 2.0f
        // x == (Screen.height - puzzleZoneHeight) * Camera.main.orthographicSize * 2.0f / Screen.height

        float resultScaleY = (Screen.height - puzzleZoneHeight) * Camera.main.orthographicSize * 2.0f / Screen.height;
        Debug.Log(resultScaleY);

        Vector3 resultScale = this.transform.localScale;
        resultScale.y = resultScaleY;

        this.transform.localScale = resultScale;


        // Active off
        this.gameObject.SetActive(false);

    }


}
