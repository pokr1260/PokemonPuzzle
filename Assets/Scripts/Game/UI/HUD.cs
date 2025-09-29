using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text name;
    public Slider hpSlider;
    public Image fill;

    public Text level;

    public Text currentHpText;
    public Text totalHpText;

    public Gradient gradient;

    public float hpSlideSpeed = 0.01f;

    public bool isAnimationFinish = false;

    private float totalHp;
    private float currentHp;
    

    public void Start()
    {
        if (null == name)
        {
            Debug.LogError("HUD.cs / name is null");
            return;
        }

        if (null == hpSlider)
        {
            Debug.LogError("HUD.cs / name is null");
            return;
        }

        if (null == level)
        {
            Debug.LogError("HUD.cs / name is null");
            return;
        }
    }

    public void Init(int levelValue, float currentHp, float totalHp, string pokemonName, Vector3 pos)
    {
        SetLevel(levelValue);
        SetHP(currentHp, totalHp);
        SetName(pokemonName);

        InitPosition(pos);

    }

    public void SetLevel(int levelValue)
    {
        level.text = levelValue.ToString();
    }

    public void SetHP(float targetHp, float totalHp)
    {
        this.currentHp = targetHp;
        this.totalHp = totalHp;

        float hpRatio = this.currentHp / this.totalHp;

        currentHpText.text = currentHp.ToString();
        totalHpText.text = totalHp.ToString();
        hpSlider.value = (hpRatio);
        fill.color = gradient.Evaluate(hpRatio);
    }

    public void SetAnimateHP(float targetHp, float totalHp)
    {
        StartCoroutine(AnimationSetHpbar(targetHp, totalHp));
    }

    private IEnumerator AnimationSetHpbar(float targetHp, float totalHp)
    {
        float value = this.currentHp;
        float targetValue = targetHp;
        float hpRatio = this.currentHp / this.totalHp;

        while (true)
        {
            value = Mathf.Lerp(value, targetValue, (Time.time * hpSlideSpeed));

            hpRatio = value / totalHp;

            hpSlider.value = (hpRatio);
            fill.color = gradient.Evaluate(hpRatio);
            currentHpText.text = ((int)value).ToString();
            totalHpText.text = ((int)totalHp).ToString();

            if (Mathf.Abs(value - targetValue) < 0.9f)
            {
                SetHP((int)targetHp, (int)totalHp);
                break;
            }

            yield return null;
        }

        isAnimationFinish = true;

        yield return null;
    }

    public void SetName(string pokemonName)
    {
        name.text = pokemonName;
    }

    private void InitPosition(Vector3 pos)
    {
        //Debug.Log(pos);

        // 포켓몬 스프라이트보다 아래에 두어야한다.
        pos.y = pos.y - 1.0f;

        //InverseTransformPoint : 월드 공간에서 로컬 공간으로 위치를 변경.
        this.transform.localPosition = transform.parent.InverseTransformPoint(pos);
    }

}
