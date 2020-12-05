using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Move_EnermyDefault : BaseState
{
    new BaseEnermy m_targetCharacter;
    Collider m_collider;
    AttackSystem m_attackSystem;
    StatSystem m_statSystem;
    MoveSystem m_moveSystem;
    Animator m_animator;
    public State_Move_EnermyDefault(BaseCharacter target, Collider m_agroCollider) : base(target)
    {
        m_targetCharacter = target as BaseEnermy;
        m_animator = target.Animator;
        m_moveSystem = target.MoveSystem;
        m_statSystem = target.StatSystem;
        m_attackSystem = target.AttackSystem;
        m_collider = m_agroCollider;
    }
    public override void OnStateEnter()
    {
        m_collider.enabled = false;
        m_moveSystem.Stop = true;
    }
    public override void OnStateStay(float deltaTime)
    {
        if (m_targetCharacter.IsStun || m_targetCharacter.IsHit || m_targetCharacter.IsNuckback || !m_attackSystem.CompleteAttack)
            return;

        m_animator.SetInteger("State", 1);
        m_statSystem.RecoveryHP(m_statSystem.GetHP * deltaTime * 0.2f);
        if(m_moveSystem.MoveToPosition(m_targetCharacter.InitPosition, 0))
            m_targetCharacter.State = BaseCharacter.CharacterState.Idle;
    }
    public override void OnStateExit()
    {
        m_targetCharacter.RESET();
        m_statSystem.RecoveryHP(m_statSystem.GetHP);
        m_collider.enabled = true;
    }
}
