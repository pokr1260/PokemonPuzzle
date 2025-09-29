using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokemonDefine;

/*
 에러나는 스킬 목록 , 추후 데이터 수정 필요
  - 100번 포켓몬 스킬명 :  selfdestruct

 */

public abstract class Skill : MonoBehaviour
{
    protected PokemonSkillInfoData skillInfo;
    public PokemonType skillType;

    // 스킬사용시 포켓몬 등장 배경
    protected Object pokemonAppearancePrefab = null;

    //protected string appearancePrefabPath = "Prefab/Game/PokemonAppearanceUI";
    protected string appearancePrefabPath = "Prefab/Game/PokemonAppearance";


    // 포켓몬 번호
    protected int pokemonNumber = 0;

    protected GameObject pokemonAppearance = null;

    public float appearanceDelay = 2.0f;

    protected AudioClip skillEffectSound;

    protected Pokemon target;

    protected Pokemon owner;

    public virtual void Init(Pokemon owner, string skillName, int pokemonNumber)
    {
        this.skillInfo = PokemonDataManager.Instance.GetPokemonSkillData(skillName);
        skillType = PokemonDataManager.Instance.GetPokemonType(this.skillInfo.type);
        this.pokemonNumber = pokemonNumber;
        this.owner = owner;

        pokemonAppearancePrefab = Resources.Load(appearancePrefabPath);

        if (null == pokemonAppearancePrefab)
        {
            Debug.LogError("Skill.cs / pokemonAppearance is null, Resources.Load is Fail");
            return;
        }

    }

    public virtual IEnumerator DoSkill(Pokemon target)
    {
        if (null == target)
        {
            Debug.LogError("Skill.cs / target is null");
            yield return null;
        }
        else
        {
            this.target = target;
        }

        if (null == pokemonAppearance)
        {
            // 등장 씬 생성
            Transform uiCanvas = GameManager.Instance.uiCanvas.transform;
            GameObject obj = Instantiate(pokemonAppearancePrefab, new Vector3(), Quaternion.identity) as GameObject;
            pokemonAppearance = obj;
            pokemonAppearance.GetComponent<PokemonAppearance>().Init(pokemonNumber);
        }
        else
        {
            // 온오프를 한다.
            pokemonAppearance.SetActive(true);
        }

        yield return new WaitForSeconds(appearanceDelay);
        pokemonAppearance.SetActive(false);
    }

    public PokemonSkillInfoData GetSkillInfo()
    {
        return skillInfo;
    }




}
