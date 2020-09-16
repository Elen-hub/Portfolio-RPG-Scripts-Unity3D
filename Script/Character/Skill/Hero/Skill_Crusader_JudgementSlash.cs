using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Crusader_JudgementSlash : BaseSkill
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
        Caster.Animator.Play("Shield_Skill_JudgementSlash");
        yield return new WaitForSeconds(SkillInfo.DurationTime*0.35f);
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_JudgementSlash", transform.position, transform.eulerAngles, 3f);
        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.15f);

        Vector3 pos = Caster.transform.position;
        EAllyType targetAlly = EAllyType.Hostile;
        if(Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        int casterID = Caster.UniqueID;
        EAttackType type;
        float damage = 0;

        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * 0.8f;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * 0.8f;
        }

        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, SkillInfo.Range, 180);
        for (int i = 0; i < characterList.Count; ++i)
        {
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                if (characterList[i].State == BaseCharacter.CharacterState.Death)
                    continue;

                if (transform.tag == "Player")
                {
                    int targetID = characterList[i].UniqueID;
                    NetworkMng.Instance.NotifyReceiveDamage(type, casterID, targetID, damage, 1.5f);
                }

                EffectMng.Instance.FindEffect("Skill/Effect_Crusader_JudgementSlashHit", characterList[i].AttachSystem.GetAttachPoint(EAttachPoint.UnderHead).position, Vector3.zero, 1);
                Vector3 nuckBackPos = (characterList[i].transform.position - transform.position).normalized;
                characterList[i].Nuckback(nuckBackPos, 0.3f, 1.5f);
            }
        }
    }
}
