using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Move_Hero : BaseState
{
    Transform m_target;
    AttackSystem m_attackSystem;
    StatSystem m_statSystem;
    MoveSystem m_moveSystem;
    Animator m_animator;
    public State_Move_Hero(BaseCharacter target) : base(target)
    {
        m_target = target.transform;
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
        {
            m_targetCharacter.State = BaseCharacter.CharacterState.Idle;
            return;
        }

        m_animator.SetInteger("State", 1);
        if (m_targetCharacter.tag == "Player")
        {
            m_moveSystem.MoveSpeed = m_statSystem.GetMoveSpeed * (1 + m_statSystem.GetMoveSpeedPro);
            m_moveSystem.RotateAxis();
            m_moveSystem.MoveAxis();
            NetworkMng.Instance.NotifyCharacterState_Move(m_target.position, m_target.eulerAngles);
        }
    }
    public override void OnStateExit()
    {

    }
}
