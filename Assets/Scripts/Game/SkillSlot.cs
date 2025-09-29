using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokemonDefine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    [Header("AssetLoad Info")]
    public string assetBundleName;
    public string fileName;

    [Header("Slots Info")]
    public GameObject[] slots = new GameObject[2];

    [Tooltip("스킬 슬롯 번호 (0,1,2...)")]
    public int skillSlotNumber;

    [Tooltip("플레이어 포켓몬 슬롯 번호 (0,1,2...)")]
    public int pokemonSlotNumber;

    [Tooltip("포켓몬 번호")]
    public int pokemonNumber;

    [Tooltip("스킬 게이지 슬라이더")]
    public Slider slider;

    [Tooltip("블록 하나당 채울 게이지 값")]
    [Range(0.0f,1.0f)]
    public float blockPerGauge;

    [Header("Transform")]
    public Camera uiCamera;
    public RectTransform gaugeHandleRectTransform;

    private Button[] skillButtons;

    public void Init(int pokemonSlot ,int pokemonNumber, Skill[] pokemonSkillInfoDatas)
    {
        this.pokemonNumber = pokemonNumber;
        this.pokemonSlotNumber = pokemonSlot;

        for (int i = 0; i < pokemonSkillInfoDatas.Length; i++)
        {
            SpriteAtlas spriteAtlas = AssetBundlesManager.Instance.LoadAssetBundlesAtLocal(assetBundleName, fileName) as SpriteAtlas;

            if (null == spriteAtlas)
            {
                Debug.LogError("SkillSlot.cs / atlas is not found");
                return;
            }

            Sprite sprite = spriteAtlas.GetSprite(pokemonSkillInfoDatas[i].GetSkillInfo().enName);
            if (null == sprite)
            {
                Debug.LogError("SkillSlot.cs / sprite is not found");
                return;
            }

            Image image = slots[i].GetComponent<Image>();
            image.sprite = sprite;
            image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            slots[i].GetComponentInChildren<Text>().text = pokemonSkillInfoDatas[i].GetSkillInfo().name;

        }

        skillButtons = gameObject.GetComponentsInChildren<Button>();

        //SetGaugeValue(0.0f);
        //EnableSkillButton(false);

        // 디버그용
        SetGaugeValue(1.0f);
        EnableSkillButton(true);
    }

    public Vector3 GetGaugeHandleWorldPosition()
    {
        Vector3 result = gaugeHandleRectTransform.position;

        // 먼저 World를 Screen으로 변환한다. ( 스크린 좌표로 변환 )
        result = uiCamera.WorldToScreenPoint(result);

        // 만일 offset등등이 필요할 경우 여기에 추가한다.
        { }

        // 다시 World로 변환한다.
        result = Camera.main.ScreenToWorldPoint(result);

        //Debug.Log(result);

        return result;
    }

    public float GetGaugeValue()
    {
        return slider.value;
    }

    public void SetGaugeValue(float value)
    {
        slider.value = value;
        if (1.0f <= slider.value)
        {
            slider.value = 1.0f;
            EnableSkillButton(true);
        }
        else if (slider.value <= 0.0f)
        {
            slider.value = 0.0f;
            EnableSkillButton(false);
        }
    }

    public void AddGauge()
    {
        AddGaugeValue(blockPerGauge);
    }

    public void AddGaugeValue(float addValue)
    {
        slider.value += addValue;
        if (1.0f <= slider.value)
        {
            slider.value = 1.0f;
            EnableSkillButton(true);
        }
        else if (slider.value <= 0.0f)
        {
            slider.value = 0.0f;
            EnableSkillButton(false);
        }
    }

    public void EnableSkillButton(bool value)
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            skillButtons[i].interactable = value;
        }
    }

    public void SkillButton(int skillSlotIndex)
    {
        if (GameManager.Instance.GetGameState() == EGameState.WAITINPUT)
        {
            Pokemon pokemon = GameManager.Instance.GetPlayerPokemon(pokemonSlotNumber);
            if (null == pokemon)
            {
                Debug.LogError("SkillSlot.cs / GetPlayerPokemon is Fail");
                return;
            }

            pokemon.DoSkill(skillSlotIndex);

            EnableSkillButton(false);
            SetGaugeValue(0.0f);
        }
    }

}
