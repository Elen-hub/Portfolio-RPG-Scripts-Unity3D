using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Battle_EnermyDefault : BaseState
{
    new BaseEnermy m_targetCharacter;
    Transform transform;
    AttackSystem m_attackSystem;
    StatSystem m_statSystem;
    MoveSystem m_moveSystem;
    Animator m_animator;
    public State_Battle_EnermyDefault(BaseCharacter target) : base(target)
    {
        transform = target.transform;
        m_targetCharacter = target as BaseEnermy;
        m_animator = target.Animator;
        m_moveSystem = target.MoveSystem;
        m_statSystem = target.StatSystem;
        m_attackSystem = target.AttackSystem;
    }
    public override void OnStateEnter()
    {
        m_moveSystem.Stop = true;
    }
    public override void OnStateStay(float deltaTime)
    {
        if (m_targetCharacter.IsStun || m_targetCharacter.IsHit || m_targetCharacter.IsNuckback || !m_attackSystem.CompleteAttack)
            return;

        if(m_targetCharacter.Target == null || m_targetCharacter.Target.State == BaseCharacter.CharacterState.Death)
        {
            m_targetCharacter.Target = null;
            m_targetCharacter.State = BaseCharacter.CharacterState.Move;
            return;
        }
        // 패턴을 사용하지 않는다면 기본공격.
        if(!m_targetCharacter.UseBattlePattern())
        {
            float distance = Vector3.Distance(transform.position, m_targetCharacter.Target.transform.position);
            if (distance > m_attackSystem.NormalAttack.Range[m_attackSystem.AttackCount] * 0.7f)
            {
                m_moveSystem.SetMoveToTarget(m_targetCharacter.Target.transform, m_attackSystem.NormalAttack.Range[m_attackSystem.AttackCount] * 0.7f);
                return;
            }
            m_animator.SetInteger("State", 0);
            m_animator.Play("Attack");
            transform.LookAt(m_targetCharacter.Target.transform);
            m_attackSystem.UseAttack(m_targetCharacter.UniqueID, m_statSystem.GetAttackDamage);
        }
    }
    public override void OnStateExit()
    {

    }
}
