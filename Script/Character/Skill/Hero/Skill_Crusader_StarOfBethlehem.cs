using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Crusader_StarOfBethlehem : BaseSkill
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
        Caster.Animator.Play("Shield_Skill_Boost2");
        // 대기시간
        // 이펙트
        Vector3 pos = transform.position;
        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.3f);
        BaseEffect readyEffect = EffectMng.Instance.FindEffect("Skill/Effect_Crusader_Bethlehem", transform.position, Vector3.zero, 12);
        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.2f);
        EAllyType targetAlly = EAllyType.Hostile;
        if(Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        WaitForSeconds wait = new WaitForSeconds(1);
        for (int i =0; i<12; ++i)
        {
            List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(pos, SkillInfo.Range);
            for (int j = 0; j < characterList.Count; ++j)
            {
                if ((characterList[j].AllyType & targetAlly) != 0)
                {
                    if (characterList[j].State == BaseCharacter.CharacterState.Death)
                        continue;

                    Buff buff = new Buff(Caster, characterList[j], EBuffOption.Continue, EParamsType.Bethlehem, Icon, 1.5f, 0, 0.5f, 0.5f);
                    characterList[j].BuffSystem.SetBuff(buff);
                    NetworkMng.Instance.NotifyRecoveryHP(Caster.UniqueID, characterList[i].UniqueID, Caster.StatSystem.CON * 5, 0);
                }
            }
            yield return wait;
        }
    }
}
