using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Archer_SplitWind : BaseSkill
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
        EffectMng.Instance.FindEffect("Skill/Effect_Archer_SplitWindCharge", transform.position, transform.eulerAngles, 1);
        Caster.Animator.Play("Skill_Archer_SplitWind");
        // 대기시간
        Transform trs = EffectMng.Instance.FindEffect("Skill/Effect_Archer_SplitWindArrow", Caster.AttachSystem.GetAttachPoint(EAttachPoint.SubWeapon).position, transform.eulerAngles, 1).transform;
        trs.SetParent(Caster.AttachSystem.GetAttachPoint(EAttachPoint.SubWeapon));
        trs.localPosition = Vector3.zero;
        trs.localScale = Vector3.one;
        yield return new WaitForSeconds(SkillInfo.DurationTime*0.6f);
        EffectMng.Instance.FindEffect("Skill/Effect_Archer_SplitWindBlast", transform.position, transform.eulerAngles, 2);
        trs.SetParent(EffectMng.Instance.transform);
        yield return new WaitForSeconds(0.2f);
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
            damage = Caster.StatSystem.GetCriticalCalculateDamage * (3 + (Caster.StatSystem.GetDEX * 0.04f));
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * (3 + (Caster.StatSystem.GetDEX * 0.04f));
        }
    
        for (int i = 0; i < characterList.Count; ++i)
        {
            if (characterList[i].State == BaseCharacter.CharacterState.Death)
                continue;
            
            int targetID = characterList[i].UniqueID;
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                if (Caster.tag == "Player")
                    NetworkMng.Instance.NotifyReceiveDamage(type, casterID, targetID, damage, 1);

                EffectMng.Instance.FindEffect("Skill/Effect_Archer_SplitWindHit", characterList[i].AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, Vector3.zero, 2);
            }
        }
    }
}
