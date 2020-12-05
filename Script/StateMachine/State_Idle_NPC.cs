using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Idle_NPC : BaseState
{
    Animator m_animator;
    float m_alterElapsedTime;
    public State_Idle_NPC(BaseCharacter target) : base(target)
    {
        m_animator = target.Animator;
    }
    public override void OnStateEnter()
    {
        m_alterElapsedTime = 0;
    }
    public override void OnStateStay(float deltaTime)
    {
        m_alterElapsedTime += deltaTime;
        if (m_alterElapsedTime > 1)
        {
            m_animator.SetInteger("Alter", Random.Range(0, 101));
            m_alterElapsedTime = 0;
        }

        m_animator.SetInteger("State", 0);
    }
    public override void OnStateExit()
    {

    }
}
