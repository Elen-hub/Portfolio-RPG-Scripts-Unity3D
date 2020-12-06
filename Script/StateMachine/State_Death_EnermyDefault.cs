using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Death_EnermyDefault : BaseState
{
    BuffSystem m_buffSystem;
    MoveSystem m_moveSystem;
    StatSystem m_statSystem;
    Animator m_animator;
    public State_Death_EnermyDefault(BaseCharacter target) : base(target)
    {
        m_animator = target.Animator;
        m_buffSystem = target.BuffSystem;
        m_moveSystem = target.MoveSystem;
        m_statSystem = target.StatSystem;
    }
    public override void OnStateEnter()
    {
        m_buffSystem.Disabled();
        m_targetCharacter.Target = null;
        m_moveSystem.Stop = true;
        m_statSystem.CurrHP = 0;
        m_animator.Play("Death");
        
        m_targetCharacter.StartCoroutine(DeathAction());
    }
    public override void OnStateStay(float deltaTime)
    {

    }
    public override void OnStateExit()
    {

    }
    protected IEnumerator DeathAction()
    {
        yield return new WaitForSeconds(3);
        m_targetCharacter.gameObject.SetActive(false);
    }
}

