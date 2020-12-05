using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Idle_EnermyDefault : BaseState
{
    Animator m_animator;
    public State_Idle_EnermyDefault(BaseCharacter target) : base(target)
    {
        m_animator = target.Animator;
    }
    public override void OnStateEnter()
    {

    }
    public override void OnStateStay(float deltaTime)
    {
        m_animator.SetInteger("State", 0);
        if (m_targetCharacter.Target != null)
            m_targetCharacter.State = BaseCharacter.CharacterState.Chase;
    }
    public override void OnStateExit()
    {

    }
}
