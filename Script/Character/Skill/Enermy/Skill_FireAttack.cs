using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_FireAttack : BaseSkill
{
    Enermy_SkeletonKing Enermy;
    int m_width = 3;
    int m_range = 8;
    public override BaseSkill Init(BaseEnermy caster)
    {
        Enermy = caster as Enermy_SkeletonKing;
        DurationTime = 3;
        CompleteTime = 3;
        CoolTime = 12;
        ElapsedTime = CoolTime;
        return this;
    }
    public override bool RangeCheck()
    {
        if (Vector3.Distance(Enermy.transform.position, Enermy.Target.transform.position) > 5)
        {
            Enermy.MoveSystem.SetMoveToTarget(Enermy.Target.transform, 5);
            return false;
        }
        return true;
    }
    public override bool Using()
    {
        if (Enermy.AttackSystem.HoldAttack || Enermy.State == BaseCharacter.CharacterState.Death)
            return false;
        
        if (!PossibleSkill)
            return false;

        return true;
    }
    public override void Use()
    {
        PossibleSkill = false;
        ElapsedTime = 0;

        StartCoroutine(Skill());
    }
    IEnumerator Skill()
    {
        yield return null;
        transform.LookAt(Enermy.Target.transform);

        Enermy.Animator.Play("FireAttack");
    }
    void FireAttackReady()
    {
        EffectMng.Instance.FindEffect("Enermy/Effect_Enermy_FireSwordCharge", Enermy.AttachSystem.GetAttachPoint(EAttachPoint.Weapon), 1.5f);
    }
    void FireAttack()
    {
        EffectMng.Instance.FindEffect("Enermy/Effect_Enermy_FireSwordCleave", transform.position, transform.eulerAngles, 2);
    }
    void FireAttackDamage()
    {
        EAllyType targetAlly = EAllyType.Hostile;
        if (Enermy.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        //BaseCharacter character = PlayerMng.Instance.MainPlayer.Character;
        //if (CharacterMng.Instance.CheckToRectangleRange(character.transform.position, transform.position, transform.eulerAngles.y, m_width, m_range + 2))
        //{
        //    if ((character.AllyType & targetAlly) == 0)
        //        return;
        //    if (character.State == BaseCharacter.CharacterState.Death)
        //        return;

        //    NetworkMng.Instance.NotifyReceiveDamage(EAttackType.Absolutely, Enermy.UniqueID, character.UniqueID, character.StatSystem.GetHP * 0.25f, 1);
        //}

        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToRectangleRange(transform.position, transform.eulerAngles.y, m_width, m_range);
        for (int i = 0; i < characterList.Count; ++i)
        {
            BaseCharacter character = characterList[i];
            if ((character.AllyType & targetAlly) == 0)
                continue;
            if (character.State == BaseCharacter.CharacterState.Death)
                continue;

            EffectMng.Instance.FindEffect("Enermy/Effect_Enermy_FireSwordHit", character.AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, character.transform.eulerAngles, 2);
            if (character.tag == "Player")
                NetworkMng.Instance.NotifyReceiveDamage(EAttackType.Absolutely, Enermy.UniqueID, character.UniqueID, character.StatSystem.GetHP * 0.35f, 1);
        }
    }
}
