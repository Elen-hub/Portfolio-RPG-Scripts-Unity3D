using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Move_NPC : BaseState
{
    new BaseNPC m_targetCharacter;
    MoveSystem m_moveSystem;
    Animator m_animator;
    public State_Move_NPC(BaseCharacter target) : base(target)
    {
        m_targetCharacter = target as BaseNPC;
        m_animator = target.Animator;
        m_moveSystem = target.MoveSystem;
    }
    public override void OnStateEnter()
    {
        m_moveSystem.Stop = true;
    }
    public override void OnStateStay(float deltaTime)
    {
        m_animator.SetInteger("State", 1);

        if (m_moveSystem.MoveToPosition(m_targetCharacter.InitPosition, 0))
            m_targetCharacter.State = BaseCharacter.CharacterState.Idle;
    }
    public override void OnStateExit()
    {

    }
}
