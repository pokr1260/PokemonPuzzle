using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tackle : Skill
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
        // ��ų ���� ���
        GameManager.Instance.PlayEffectSound(skillEffectSound);

        // ���� ����� ���������� ��ٸ���.
        yield return new WaitUntil(() => { return false == GameManager.Instance.isSoundEnd(); });



        target.Damage(this.skillType, skillInfo.attack);
    }
}