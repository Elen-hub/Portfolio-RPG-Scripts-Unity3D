using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Cleric_Revelation : BaseSkill
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
        Caster.Animator.Play("Skill_Cleric_Revelation");
        // 대기시간
        // 이펙트
        BaseEffect readyEffect = EffectMng.Instance.FindEffect("Skill/Effect_Cleric_RevelationReady", transform.position, Vector3.zero, 2);
        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.3f);
        Caster.Animator.SetBool("Revelation", true);
        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.5f);
        BaseEffect startEffect = EffectMng.Instance.FindEffect("Skill/Effect_Cleric_RevelationStart", transform.position, Vector3.zero, 12);
        Caster.Animator.SetBool("Revelation", false);
        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.2f);
        Vector3 pos = transform.position;
        EAllyType targetAlly = EAllyType.Hostile;
        if(Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        WaitForSeconds wait = new WaitForSeconds(0.5f);
        for (int i =0; i<24; ++i)
        {
            List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(pos, SkillInfo.Range);
            for (int j = 0; j < characterList.Count; ++j)
            {
                int targetID = characterList[j].UniqueID;
                if ((characterList[j].AllyType & targetAlly) != 0)
                {
                    if (characterList[j].State == BaseCharacter.CharacterState.Death)
                        continue;

                    Buff buff = new Buff(Caster, characterList[j], EBuffOption.Continue, EParamsType.Revelation, Icon, 1, 0, 20 + Caster.StatSystem.GetWIS * 0.5f, 0.25f, 0.25f);
                    characterList[j].BuffSystem.SetBuff(buff);
                }
            }
            yield return wait;
        }
    }
}
