using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Archer_Heal : BaseSkill
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
        Caster.Animator.Play("Skill_Archer_Heal");
        // 대기시간
        // 이펙트
        yield return new WaitForSeconds(SkillInfo.DurationTime*0.5f);
        EffectMng.Instance.FindEffect("Skill/Effect_Archer_Heal", transform.position, Vector3.zero, 3f);
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(Caster.transform.position, SkillInfo.Range);
        EAllyType targetAlly = EAllyType.Hostile;
        if(Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        float buffDurationTime = 20;
        for (int i = 0; i < characterList.Count; ++i)
        {
            int targetID = characterList[i].UniqueID;
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                if (characterList[i].State == BaseCharacter.CharacterState.Death)
                    continue;

                NetworkMng.Instance.NotifyRecoveryHP(Caster.UniqueID, characterList[i].UniqueID, Caster.StatSystem.GetWIS * 2, 0.1f);
                BaseEffect effect = EffectMng.Instance.FindEffect("Buff/Effect_Buff_ArcherHeal", characterList[i].transform, buffDurationTime);
                Buff hpBuff = new Buff(Caster, characterList[i], EBuffOption.Single | EBuffOption.Interval, EBuffType.RecoveryHPPer, Icon, buffDurationTime, (20 + Caster.StatSystem.GetWIS * 0.5f)*0.02f, 1);
                characterList[i].BuffSystem.SetBuff(hpBuff, effect);
                Buff mpBuff = new Buff(Caster, characterList[i], EBuffOption.Single | EBuffOption.Interval, EBuffType.RecoveryMPPer, Icon, buffDurationTime, 0.02f, 1);
                characterList[i].BuffSystem.SetBuff(mpBuff, null);
            }
        }
        yield return null;
    }
}
