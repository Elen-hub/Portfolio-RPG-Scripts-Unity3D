using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Death_Hero : BaseState
{
    Collider m_collider;
    BuffSystem m_buffSystem;
    MoveSystem m_moveSystem;
    StatSystem m_statSystem;
    Animator m_animator;
    public State_Death_Hero(BaseCharacter target) : base(target)
    {
        m_collider = target.GetComponent<Collider>();
        m_animator = target.Animator;
        m_buffSystem = target.BuffSystem;
        m_moveSystem = target.MoveSystem;
        m_statSystem = target.StatSystem;
    }
    public override void OnStateEnter()
    {
        m_buffSystem.Disabled();
        m_collider.enabled = false;
        m_moveSystem.Stop = true;
        m_statSystem.CurrHP = 0;
        m_animator.Play("Death");
        if (m_targetCharacter.tag == "Player")
            m_targetCharacter.StartCoroutine(DeathAction());
    }
    public override void OnStateStay(float deltaTime)
    {

    }
    public override void OnStateExit()
    {
        m_animator.Play("Revive");
        m_collider.enabled = true;
        m_moveSystem.Stop = false;
    }
    protected IEnumerator DeathAction()
    {
        CameraMng.Infrared_ON(3);
        yield return new WaitForSeconds(2f);
        UIMng.Instance.Open<SelectPopup>(UIMng.UIName.SelectPopup).RevivePopup.Enabled();
    }
}

