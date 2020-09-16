using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Crusader_Judgement : BaseSkill
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
        Caster.Animator.Play("Shield_Skill_Judgement");
        // 대기시간
        // 이펙트
        yield return new WaitForSeconds(0.9f);
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_Judgement", transform.position, transform.eulerAngles, 3f);
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(Caster.transform.position, SkillInfo.Range);
        EAllyType targetAlly = EAllyType.Hostile;
        if(Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        int casterID = Caster.UniqueID;
        EAttackType type;
        float damage = 0;

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

        for (int i = 0; i < characterList.Count; ++i)
        {
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                if (characterList[i].State == BaseCharacter.CharacterState.Death)
                    continue;
                 
                int targetID = characterList[i].UniqueID;
                if (transform.tag == "Player") 
                    NetworkMng.Instance.NotifyReceiveDamage(type, casterID, targetID, damage, 0.3f);

                characterList[i].Stun(1.2f);
            }
        }

        yield return null;
    }
}
