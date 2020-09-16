using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Cleric_Heal : BaseSkill
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

        
        StartCoroutine(Heal());
    }

    IEnumerator Heal()
    {
        Caster.Animator.Play("Skill_Cleric_Heal");
        // 대기시간
        // 이펙트
        yield return new WaitForSeconds(SkillInfo.DurationTime*0.5f);
        EffectMng.Instance.FindEffect("Skill/Effect_Cleric_Heal", transform.position, Vector3.zero, 3f);
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(Caster.transform.position, SkillInfo.Range);
        EAllyType targetAlly = EAllyType.Hostile;
        if(Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        int casterID = Caster.UniqueID;
        EAttackType type;
        float damage = 0;
        int target = -1;

        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * 3;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * 3;
        }
        List<BaseCharacter> enermyCharacter = new List<BaseCharacter>();
        float buffDurationTime = 20;
        for (int i = 0; i < characterList.Count; ++i)
        {
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                if (characterList[i].State == BaseCharacter.CharacterState.Death)
                    continue;

                NetworkMng.Instance.NotifyRecoveryHP(Caster.UniqueID, characterList[i].UniqueID, Caster.StatSystem.GetWIS * 2, 0.1f);
                BaseEffect effect = EffectMng.Instance.FindEffect("Buff/Effect_Buff_Heal", characterList[i].transform, buffDurationTime);
                Buff buff = new Buff(Caster, characterList[i], EBuffOption.Single | EBuffOption.Interval, EBuffType.RecoveryHPPer, Icon, buffDurationTime, 0.02f, 1);
                characterList[i].BuffSystem.SetBuff(buff, effect);
                target += 1;
            }
            else
            {
                enermyCharacter.Add(characterList[i]);
            }
        }
        damage -= damage * target * 0.5f;
        if (damage > 0)
        {
            for (int i = 0; i < enermyCharacter.Count; ++i)
            {
                if (enermyCharacter[i].State == BaseCharacter.CharacterState.Death)
                    continue;

                int targetID = enermyCharacter[i].UniqueID;
                NetworkMng.Instance.NotifyReceiveDamage(type, casterID, targetID, damage, 0.3f);
            }
        }
        yield return null;
    }
}
