using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_FireWall : BaseSkill
{
    Vector3 m_pivot;
    Vector3[] m_pos;
    public override BaseSkill Init(BaseEnermy caster)
    {
        Caster = caster;
        m_pos = new Vector3[4];
        DurationTime = 2.5f;
        CompleteTime = 2.5f;
        CoolTime = 5;
        ElapsedTime = CoolTime;
        return this;
    }
    public override bool RangeCheck()
    {
        if (Vector3.Distance(Caster.transform.position, Caster.Target.transform.position) > 3)
        {
            Caster.MoveSystem.SetMoveToTarget(Caster.Target.transform, 3);
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

        StartCoroutine(Skill());
    }
    IEnumerator Skill()
    {
        yield return null;
        Caster.Animator.Play("FireWall");
        m_pivot = transform.position;
        m_pos = new Vector3[4];
        m_pos[0] = new Vector3(m_pivot.x - 5, m_pivot.y, m_pivot.z+5);
        m_pos[1] = new Vector3(m_pivot.x + 5, m_pivot.y, m_pivot.z + 5);
        m_pos[2] = new Vector3(m_pivot.x + 5, m_pivot.y, m_pivot.z - 5);
        m_pos[3] = new Vector3(m_pivot.x - 5, m_pivot.y, m_pivot.z - 5);
        float time = 0;
        WaitForSeconds second = new WaitForSeconds(0.5f);

        EffectMng.Instance.FindEffect("Enermy/Effect_Enermy_FireWall", m_pivot, Vector3.zero, 20);
        while (time<15)
        {
            time += 0.5f;

            BaseCharacter character = PlayerMng.Instance.MainPlayer.Character;
            if (character.State == BaseCharacter.CharacterState.Death)
                yield return second;

            bool isDamage =
                CharacterMng.Instance.CheckToRectangleRange(character.transform.position, m_pos[0], 90, 2, 10) ||
                 CharacterMng.Instance.CheckToRectangleRange(character.transform.position, m_pos[1], 180, 2, 10) ||
                  CharacterMng.Instance.CheckToRectangleRange(character.transform.position, m_pos[2], 270, 2, 10) ||
                   CharacterMng.Instance.CheckToRectangleRange(character.transform.position, m_pos[3], 0, 2, 10);

            if (isDamage)
                NetworkMng.Instance.NotifyReceiveDamage(EAttackType.Normal, Caster.UniqueID, character.UniqueID, character.StatSystem.GetHP * 0.2f, 0.2f);

            yield return second;
        }
    }
    void FireWallReady()
    {

    }
}
