using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Cleric_HolyLight : BaseSkill
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

        
        StartCoroutine(HolyLight());
    }

    IEnumerator HolyLight()
    {
        Caster.Animator.Play("Skill_Cleric_HolyLight");
        // 대기시간
        EffectMng.Instance.FindEffect("Skill/Effect_Cleric_HolyLightReady", transform.position, transform.eulerAngles, 0.6f);
        yield return new WaitForSeconds(SkillInfo.DurationTime*0.8f);
        EffectMng.Instance.FindEffect("Skill/Effect_Cleric_HolyLightWave", transform.position, transform.eulerAngles, 2);
        // 이펙트
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToRectangleRange(transform.position, transform.eulerAngles.y, 5, SkillInfo.Range);
        EAllyType targetAlly = EAllyType.Friendly | EAllyType.Player;
        if (Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Hostile;
        if (Caster.tag == "Player")
        { 
            int casterID = Caster.UniqueID;
            EAttackType type;
            float damage = 0;

            if (Caster.StatSystem.IsCritical)
            {
                type = EAttackType.Critical;
                damage = Caster.StatSystem.GetCriticalCalculateDamage * 4.5f;
            }
            else
            {
                type = EAttackType.Normal;
                damage = Caster.StatSystem.GetNormalCalculateDamage * 4.5f;
            }

            for (int i = 0; i < characterList.Count; ++i)
            {
                if (characterList[i].State == BaseCharacter.CharacterState.Death)
                    continue;

                int targetID = characterList[i].UniqueID;
                if ((characterList[i].AllyType & targetAlly) != 0)
                    NetworkMng.Instance.NotifyReceiveDamage(type, casterID, targetID, damage, 0.3f);
                else
                    NetworkMng.Instance.NotifyRecoveryHP(Caster.UniqueID, characterList[i].UniqueID, Caster.StatSystem.GetWIS * 4 , 0.3f);
            }

            //if(!characterList.Contains(Caster))
            //    NetworkMng.Instance.NotifyRecoveryHP(Caster.UniqueID, Caster.UniqueID, Caster.StatSystem.GetWIS * 4, 0.3f);
        }
        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.2f);
    }
}
