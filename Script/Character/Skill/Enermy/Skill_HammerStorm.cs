using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_HammerStorm : BaseSkill
{
    float m_range = 3;
    float m_angle = 180;
    public override BaseSkill Init(BaseEnermy caster)
    {
        Caster = caster;
        DurationTime = 4;
        CompleteTime = 4;
        CoolTime = 20;
        ElapsedTime = CoolTime;
        return this;
    }
    public override bool RangeCheck()
    {
        float successDistance = 2.5f;
        if (Vector3.Distance(Caster.transform.position, Caster.Target.transform.position) > successDistance)
        {
            Caster.MoveSystem.SetMoveToTarget(Caster.Target.transform, successDistance);
            return false;
        }
        return true;
    }
    public override bool Using()
    {
        if (Caster.AttackSystem.HoldAttack || Caster.State == BaseCharacter.CharacterState.Death)
            return false;
        
        if (!PossibleSkill)
            return false;

        return true;
    }
    public override void Use()
    {
        PossibleSkill = false;
        ElapsedTime = 0;

        transform.LookAt(Caster.Target.transform);
        Caster.Animator.Play("HammerStorm");
    }
    void OnHammerStormReady()
    {

    }
    void OnHammerStormFront01()
    {
        EffectMng.Instance.FindEffect("Enermy/Effect_Enermy_HammerStormFirst", transform.position, transform.eulerAngles, 2);
        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, m_range, 180);
        for (int i = 0; i < characterList.Count; ++i)
        {
            BaseCharacter character = characterList[i];
            if ((character.AllyType & targetAlly) == 0)
                continue;
            if (character.State == BaseCharacter.CharacterState.Death)
                continue;

            EffectMng.Instance.FindEffect("Effect_DefaultHit_Red", character.AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, character.transform.eulerAngles, 2);
            characterList[i].Nuckback((character.transform.position - transform.position).normalized, 0.1f, 2);
            characterList[i].Stun(1);

            if (character.tag == "Player")
            {
                NetworkMng.Instance.NotifyReceiveDamage(EAttackType.Absolutely, Caster.UniqueID, character.UniqueID, Caster.StatSystem.GetAttackDamage * 2.5f, 0.5f);
                CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 0.5f, 100, 5);
            }
        }
    }
    void OnHammerStormBack()
    {
        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, m_range, -180);
        for (int i = 0; i < characterList.Count; ++i)
        {
            BaseCharacter character = characterList[i];
            if ((character.AllyType & targetAlly) == 0)
                continue;
            if (character.State == BaseCharacter.CharacterState.Death)
                continue;

            EffectMng.Instance.FindEffect("Effect_DefaultHit_Red", character.AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, character.transform.eulerAngles, 2);
            characterList[i].Nuckback((character.transform.position - transform.position).normalized, 0.1f, 2);
            characterList[i].Stun(1);

            if (character.tag == "Player")
            {
                NetworkMng.Instance.NotifyReceiveDamage(EAttackType.Absolutely, Caster.UniqueID, character.UniqueID, Caster.StatSystem.GetAttackDamage * 2.5f, 0.5f);
                CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 0.5f, 100, 5);
            }
        }
    }
    void OnHammerStormFront02()
    {
        EffectMng.Instance.FindEffect("Enermy/Effect_Enermy_HammerStormSecond", transform.position, transform.eulerAngles, 2);
        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, m_range+1, 180);
        for (int i = 0; i < characterList.Count; ++i)
        {
            BaseCharacter character = characterList[i];
            if ((character.AllyType & targetAlly) == 0)
                continue;
            if (character.State == BaseCharacter.CharacterState.Death)
                continue;

            EffectMng.Instance.FindEffect("Effect_DefaultHit_Red", character.AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, character.transform.eulerAngles, 2);
            characterList[i].Nuckback((character.transform.position - transform.position).normalized, 0.3f, 5);
            characterList[i].Stun(1.5f);

            if (character.tag == "Player")
            {
                NetworkMng.Instance.NotifyReceiveDamage(EAttackType.Critical, Caster.UniqueID, character.UniqueID, character.StatSystem.GetHP * 0.33f, 1);
                CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 1, 150, 5);
            }
        }
    }
}
