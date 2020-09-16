using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Crusader_SwordOfLightwave : BaseSkill
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
        yield return null;
        Caster.Animator.Play("Shield_Skill_SwordOfLightWave");
        // 대기시간
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_SwordOfLightWaveReady", transform.position, Vector3.zero, 2);
        yield return new WaitForSeconds(SkillInfo.DurationTime*0.55f);
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_SwordOfLightWaveShot", transform.position, transform.eulerAngles, 2);
        // 이펙트
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToRectangleRange(transform.position, transform.eulerAngles.y, 3, SkillInfo.Range);
        EAllyType targetAlly = EAllyType.Friendly | EAllyType.Player;
        if (Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Hostile;

        int casterID = Caster.UniqueID;
        EAttackType type;
        float damage = 0;

        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * 5.5f;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * 5.5f;
        }

        for (int i = 0; i < characterList.Count; ++i)
        {
            if (characterList[i].State == BaseCharacter.CharacterState.Death)
                continue;

            int targetID = characterList[i].UniqueID;
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                characterList[i].Stun(1);
                if (Caster.tag == "Player")
                    NetworkMng.Instance.NotifyReceiveDamage(type, casterID, targetID, damage, 0.3f);
            }

        //if(!characterList.Contains(Caster))
        //    NetworkMng.Instance.NotifyRecoveryHP(Caster.UniqueID, Caster.UniqueID, Caster.StatSystem.GetWIS * 4, 0.3f);
        }
    }
}
