using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psychic : Skill
{
    // protected PokemonSkillInfoData skillInfo;

    public override void Init(Pokemon owner, string skillName, int pokemonNumber)
    {
        base.Init(owner, skillName, pokemonNumber);

        skillEffectSound = PokemonDataManager.Instance.GetPokemonSkillEffectSound(skillName);
    }


    public override IEnumerator DoSkill(Pokemon target)
    {
        yield return StartCoroutine(base.DoSkill(target));

        Debug.Log(this.skillInfo.name);


        StartCoroutine(SkillEffect());

        yield return null;
    }


    private IEnumerator SkillEffect()
    {
        // 필터
        GameManager.Instance.filter.SetActive(true);
        // Meterial 적용
        GameManager.Instance.SetFilterMeterialByName("Psychic");


        // 스킬 사운드 재생
        GameManager.Instance.PlayEffectSound(skillEffectSound);

        // 사운드 재생이 끝날때까지 기다린다.
        yield return new WaitUntil(() => { return false == GameManager.Instance.isSoundEnd(); });
        GameManager.Instance.filter.SetActive(false);

        target.Damage(this.skillType, skillInfo.attack);


    }


}