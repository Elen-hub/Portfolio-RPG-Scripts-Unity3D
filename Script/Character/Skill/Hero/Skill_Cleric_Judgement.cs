using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Cleric_Judgement : BaseSkill
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

        
        StartCoroutine(Judgement());
    }

    IEnumerator Judgement()
    {
        Caster.Animator.Play("Skill_Cleric_Judgement");
        yield return new WaitForSeconds(SkillInfo.DurationTime*0.8f);
        EffectMng.Instance.FindEffect("Skill/Effect_Cleric_Judgement", transform.position, Vector3.zero, 3);
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(Caster.transform.position, SkillInfo.Range);
        EAllyType targetAlly = EAllyType.Friendly | EAllyType.Player;
        if (Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Hostile;

        int casterID = Caster.UniqueID;
        EAttackType type;
        float damage = 0;

        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * 2;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * 2;
        }
        float buffDurationTime = 15;

        for (int i = 0; i < characterList.Count; ++i)
        {
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                BaseEffect effect = EffectMng.Instance.FindEffect("Buff/Effect_Buff_WeakenDefence", characterList[i].transform, buffDurationTime);
                Buff buff = new Buff(Caster, characterList[i], EBuffOption.Single, EBuffType.WeakenDEF, Icon, buffDurationTime, 5 + Caster.StatSystem.WIS);
                characterList[i].BuffSystem.SetBuff(buff);

                if (Caster.tag == "Player")
                {
                    if (characterList[i].State == BaseCharacter.CharacterState.Death)
                        continue;

                    int targetID = characterList[i].UniqueID;
                    NetworkMng.Instance.NotifyReceiveDamage(type, casterID, targetID, damage, 0.3f);
                }
            }
        }
        yield return null;
    }
}
