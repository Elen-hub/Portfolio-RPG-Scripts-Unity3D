using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Battle_Hero : BaseState
{
    AttackSystem m_attackSystem;
    MoveSystem m_moveSystem;
    Animator m_animator;
    public State_Battle_Hero(BaseCharacter target) : base(target)
    {
        m_animator = target.Animator;
        m_moveSystem = target.MoveSystem;
        m_attackSystem = target.AttackSystem;
    }
    public override void OnStateEnter()
    {
        m_moveSystem.Stop = true;
        m_animator.SetInteger("State", 3);
    }
    public override void OnStateStay(float deltaTime)
    {
        if(m_attackSystem.CompleteAttack)
            m_targetCharacter.State = BaseCharacter.CharacterState.Idle;
    }
    public override void OnStateExit()
    {

    }
}
