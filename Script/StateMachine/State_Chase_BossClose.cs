using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Chase_BossClose : BaseState
{
    new Enermy_Boss_CloseAttack m_targetCharacter;
    Transform transform;
    AttackSystem m_attackSystem;
    StatSystem m_statSystem;
    MoveSystem m_moveSystem;
    Animator m_animator;
    public State_Chase_BossClose(BaseCharacter target) : base(target)
    {
        transform = target.transform;
        m_targetCharacter = target as Enermy_Boss_CloseAttack;
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
        if (m_targetCharacter.Target == null)
        {
            if (m_targetCharacter.FindTarget() == null)
            {
                m_targetCharacter.State = BaseCharacter.CharacterState.Move;
                return;
            }
        }
        float distance = Vector3.Distance(transform.position, m_targetCharacter.Target.transform.position);
        if (distance > GameSystem.BossMonsterChaseRangeToTarget || m_targetCharacter.Target.State == BaseCharacter.CharacterState.Death)
        {
            m_targetCharacter.Target = null;
            return;
        }
        if (m_targetCharacter.CheckBattlePattern())
        {
            m_targetCharacter.State = BaseCharacter.CharacterState.Battle;
            return;
        }
        else
        {
            if (distance < m_attackSystem.NormalAttack.Range[m_attackSystem.AttackCount] * 0.7f)
            {
                m_targetCharacter.State = BaseCharacter.CharacterState.Battle;
                return;
            }
        }
        m_animator.SetInteger("State", 1);
        m_moveSystem.MoveSpeed = m_statSystem.GetMoveSpeed * (1 + m_statSystem.GetMoveSpeedPro);
        m_moveSystem.NextFrameChase();
    }
    public override void OnStateExit()
    {

    }
}
