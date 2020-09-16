using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Archer_WindRain : BaseSkill
{
    new Hero_Archer Caster;
    public override BaseSkill Init(BaseCharacter caster, SkillInfo info)
    {
        base.Init(caster, info);
        Caster = GetComponent<Hero_Archer>();
        return this;
    }
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
        Caster.WindRainTargetList.Clear();

        EffectMng.Instance.FindEffect("Skill/Effect_Archer_WindRainReady", Caster.AttachSystem.GetAttachPoint(EAttachPoint.Weapon), 1);
        Caster.Animator.Play("Skill_Archer_WindRain");
        // 대기시간
        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.4f);
        EffectMng.Instance.FindEffect("Skill/Effect_Archer_WindRainShot", Caster.AttachSystem.GetAttachPoint(EAttachPoint.Weapon).position, Vector3.zero, 0.4f);

        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.4f);

        // 이펙트
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToRectangleRange(transform.position, transform.eulerAngles.y, 4, SkillInfo.Range);
        EAllyType targetAlly = EAllyType.Friendly | EAllyType.Player;
        if (Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Hostile;

        int casterID = Caster.UniqueID;
        EAttackType type;
        float damage = 0;
    
        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * (3 + (Caster.StatSystem.GetDEX * 0.05f));
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * (3 + (Caster.StatSystem.GetDEX * 0.05f));
        }

        Vector3 startPos = transform.position;
        startPos.y += 8;
        int count = 0;
    
        for (count = 0; count < characterList.Count; ++count)
        {
            if (count > 20)
                yield break;

            if (characterList[count].State == BaseCharacter.CharacterState.Death)
                continue;
            
            int targetID = characterList[count].UniqueID;
            if ((characterList[count].AllyType & targetAlly) != 0)
            {
                Vector3 targetPos = characterList[count].transform.position;
                WindRainMissile missile = EffectMng.Instance.FindMissile<WindRainMissile>("Missile_Archer_WindRainArrow", Vector3.Distance(characterList[count].transform.position, startPos) / 4);
                missile.Enabled(Caster, type, targetAlly, damage, 0.3f, startPos, targetPos);
            }
        }

        for(int i = count; i<20; ++i)
        {
            Vector3 targetPos = transform.position + transform.right * Random.Range(-2f, 2f) + transform.forward * Random.Range(1, SkillInfo.Range);
            WindRainMissile missile = EffectMng.Instance.FindMissile<WindRainMissile>("Missile_Archer_WindRainArrow", Vector3.Distance(targetPos, startPos) / Random.Range(1,8));
            missile.Enabled(Caster, type, targetAlly, damage, 0.3f, startPos, targetPos, HitAction);
        }
    }
    void HitAction(BaseCharacter character)
    {
        EffectMng.Instance.FindEffect("Skill/Effect_Archer_NormalDefaultHit", character.AttachSystem.GetAttachPoint(EAttachPoint.UnderHead).position, Vector3.zero, 1);
    }
}
