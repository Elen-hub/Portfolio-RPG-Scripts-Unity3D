using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_HammerStrike : BaseSkill
{
    float m_range = 2;
    float m_angle = 180;
    public override BaseSkill Init(BaseEnermy caster)
    {
        Caster = caster;
        DurationTime = 3.5f;
        CompleteTime = 3.5f;
        CoolTime = 12;
        ElapsedTime = CoolTime;
        return this;
    }
    public override bool RangeCheck()
    {
        float successDistance = 2 + 1.5f;
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
        Caster.Animator.Play("HammerStrike");
    }
    void OnHammerStrikeReady()
    {

    }
    void OnHammerStrikeDamage()
    {
        Vector3 pos = transform.position + transform.forward * 2;

        EffectMng.Instance.FindEffect("Enermy/Effect_Enermy_HammerStrike", pos, transform.eulerAngles, 2);
        CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 1, 75, 5);

        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(pos, m_range);
        for (int i = 0; i < characterList.Count; ++i)
        {
            BaseCharacter character = characterList[i];
            if ((character.AllyType & targetAlly) == 0)
                continue;
            if (character.State == BaseCharacter.CharacterState.Death)
                continue;

            EffectMng.Instance.FindEffect("Effect_DefaultHit_Red", character.AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, character.transform.eulerAngles, 2);
            characterList[i].Nuckback((character.transform.position - pos).normalized, 0.3f, 3);
            characterList[i].Stun(3);

            if (character.tag == "Player")
                NetworkMng.Instance.NotifyReceiveDamage(EAttackType.Critical, Caster.UniqueID, character.UniqueID, character.StatSystem.GetHP * 0.2f, 0.5f);
        }
    }
}
