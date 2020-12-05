using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Idle_Hero : BaseState
{
    Animator m_animator;
    public State_Idle_Hero(BaseCharacter target) : base(target)
    {
        m_animator = target.Animator;
    }
    public override void OnStateEnter()
    {

    }
    public override void OnStateStay(float deltaTime)
    {
        m_animator.SetInteger("State", 0);
    }
    public override void OnStateExit()
    {

    }
}
