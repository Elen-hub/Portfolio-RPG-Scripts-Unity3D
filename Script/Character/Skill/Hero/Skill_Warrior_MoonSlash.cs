using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Warrior_MoonSlash : BaseSkill
{
    EAllyType m_targetAlly;
    public override bool Using()
    {
        if (base.Using())
            return true;
        return false;
    }
    public override void Use()
    {
        base.Use();

        m_targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            m_targetAlly = EAllyType.Friendly | EAllyType.Player;

        Caster.AttackSystem.SuperArmor = true;
        Caster.Animator.SetBool("MoonSlash", true);
        Caster.Animator.Play("Skill_Warrior_MoonSlash");
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_MoonSlashReady", Caster.AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, transform.eulerAngles, 1.5f);
    }
    void OnMoonSlashEffectEvent01()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.85f);
        CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 0.15f, 75, 1);
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_MoonSlash1", transform.position, transform.eulerAngles, 3f);
    }
    void OnMoonSlashDamageEvent01()
    {
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, SkillInfo.Range, 180);
        SetDamage(1.5f, 0.3f, characterList);
    }
    void OnMoonSlashEffectEvent02()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.85f);
        CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 0.15f, 75, 1);
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_MoonSlash2", transform.position, transform.eulerAngles, 3f);
    }
    void OnMoonSlashDamageEvent02()
    {
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, SkillInfo.Range, 180);
        SetDamage(1.5f, 0.3f, characterList);
    }
    void OnMoonSlashEffectEvent03()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.85f);
        CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 0.15f, 75, 1);
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_MoonSlash3", transform.position, transform.eulerAngles, 3f);
    }
    void OnMoonSlashDamageEvent03()
    {
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, SkillInfo.Range, 180);
        SetDamage(2, 0.3f, characterList);
    }
    void OnMoonSlashEffectEvent04()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.75f);
        CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 0.75f, 100, 1.5f);
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_MoonSlash4", transform.position, transform.eulerAngles, 3f);
    }
    void OnMoonSlashDamageEvent04()
    {
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, SkillInfo.Range, 180);
        SetDamage(3, 1.5f, characterList, true, 0.5f, 2);
    }
    void OnMoonSlashEndEvent()
    {
        Caster.Animator.SetBool("MoonSlash", false);
        Caster.AttackSystem.SuperArmor = false;
    }
    public override void EndKeydown()
    {
        base.EndKeydown();

        Caster.Animator.SetBool("MoonSlash", false);
        Caster.AttackSystem.HoldAttack = false;
        Caster.AttackSystem.CompleteAttack = true;
        Caster.AttackSystem.SuperArmor = false;
    }
    void SetDamage(float damagePercent, float hitTime, List<BaseCharacter> characterList, bool useNuckBack = false, float nuckBackTime = 0, float nuckBackForce = 0)
    {
        EAttackType type;
        float damage = 0;

        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * damagePercent;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * damagePercent;
        }

        for (int i = 0; i < characterList.Count; ++i)
        {
            if ((characterList[i].AllyType & m_targetAlly) != 0)
            {
                if (characterList[i].State == BaseCharacter.CharacterState.Death)
                    continue;

                if (transform.tag == "Player")
                {
                    int targetID = characterList[i].UniqueID;
                    NetworkMng.Instance.NotifyReceiveDamage(type, Caster.UniqueID, targetID, damage, 1.5f);
                }

                EffectMng.Instance.FindEffect("Skill/Effect_Warrior_MoonSlashHit", characterList[i].AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, Vector3.zero, 1);
                Vector3 nuckBackPos = (characterList[i].transform.position - transform.position).normalized;
                if(useNuckBack)
                    characterList[i].Nuckback(nuckBackPos, nuckBackTime, nuckBackForce);

                characterList[i].SetHit(hitTime);
            }
        }
    }
}
