using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Chase_Hero : BaseState
{
    AttackSystem m_attackSystem;
    StatSystem m_statSystem;
    MoveSystem m_moveSystem;
    Animator m_animator;
    public State_Chase_Hero(BaseCharacter target) : base(target)
    {
        m_animator = target.Animator;
        m_moveSystem = target.MoveSystem;
        m_statSystem = target.StatSystem;
        m_attackSystem = target.AttackSystem;
    }
    public override void OnStateEnter()
    {

    }
    public override void OnStateStay(float deltaTime)
    {
        if (m_targetCharacter.IsStun || m_targetCharacter.IsHit || m_targetCharacter.IsNuckback || !m_attackSystem.CompleteAttack)
        {
            m_targetCharacter.State = BaseCharacter.CharacterState.Idle;
            return;
        }

        m_animator.SetInteger("State", 1);
        m_moveSystem.MoveSpeed = m_statSystem.GetMoveSpeed * (1 + m_statSystem.GetMoveSpeedPro);

        if (m_targetCharacter.tag == "Player")
        {
            m_moveSystem.NextFrameChase();
            NetworkMng.Instance.NotifyCharacterState_Chase(m_moveSystem.Target.position, m_moveSystem.ChaseDistance);
        }
        else
            m_moveSystem.MoveToPosition();
    }
    public override void OnStateExit()
    {

    }
}
