using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Cleric_Salvation : BaseSkill
{
    public override bool Using()
    {
        if (base.Using())
            return true;
        return false;
    }
    public override void Use()
    {
        base.Use();

        
        StartCoroutine(Skill());
    }

    IEnumerator Skill()
    {
        Caster.Animator.Play("Skill_Cleric_Salavation");
        BaseEffect readyEffectCaster = EffectMng.Instance.FindEffect("Skill/Effect_Cleric_SalvationCaster", transform.position, Vector3.zero, 3);
        yield return new WaitForSeconds(0.4f);

        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(Caster.transform.position, 5);
        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        float range = 0;
        BaseCharacter target = null;
        for (int i = 0; i < characterList.Count; ++i)
        {
            int targetID = characterList[i].UniqueID;
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                if (characterList[i].State != BaseCharacter.CharacterState.Death)
                    continue;

                if (target == null)
                {
                    target = characterList[i];
                    range = Vector3.Distance(Caster.transform.position, characterList[i].transform.position);
                    continue;
                }
                float distance = Vector3.Distance(Caster.transform.position, characterList[i].transform.position);
                if (range > distance)
                {
                    target = characterList[i];
                    range = distance;
                }
            }
        }

        if(target == null)
        {
            ElapsedTime = CoolTime - 1;
            yield return null;
        }

        BaseEffect readyEffectTarget = EffectMng.Instance.FindEffect("Skill/Effect_Cleric_SalvationCaster", target.transform.position, Vector3.zero, 3);
        Caster.Animator.SetBool("Salavation", true);
        yield return new WaitForSeconds(SkillInfo.DurationTime-0.4f);
        Caster.Animator.SetBool("Salavation", false);
        if (target != null)
        {
            target.Revival(target.StatSystem.GetHP * Caster.StatSystem.Level * 0.01f, target.StatSystem.CurrMP + target.StatSystem.GetMP * Caster.StatSystem.Level);
        }
        else
        {
            ElapsedTime = CoolTime - 1;
        }

        BaseEffect startEffect = EffectMng.Instance.FindEffect("Skill/Effect_Cleric_SalvationCaster", target.transform.position, Vector3.zero, 3);

        yield return null;
    }
}
