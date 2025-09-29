using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleZoneBG : MonoBehaviour
{
    public Camera mainCamera;

    public Camera uiCamera;

    public RectTransform canvasRect;

    public RectTransform skillPanelRectTransform;

    private float height;
    
    public void SetHeight(Rect rect)
    {
        Vector3 pos = mainCamera.WorldToViewportPoint(new Vector3(0.0f,rect.y+rect.height,0.0f));

        //Debug.Log(pos);

        pos = uiCamera.ViewportToWorldPoint(pos);

        //Debug.Log(pos);

        pos = transform.InverseTransformPoint(pos);

        //Debug.Log(pos);

        height = pos.y;

        GetComponent<RectTransform>().sizeDelta = new Vector2(0.0f, height);
    }

    public float GetHeight()
    {
        return GetComponent<RectTransform>().rect.height;
    }
    public float GetSkillPanelHeight()
    { 
        return skillPanelRectTransform.rect.height;
    }

    public float GetTotalHeight()
    {
        return GetHeight() + GetSkillPanelHeight();
    }


}
