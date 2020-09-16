using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_ReturnSoul : BaseSkill
{
    bool m_isActive;
    float m_elapsedTime;
    Vector3[] m_pos;
    public override BaseSkill Init(BaseEnermy caster)
    {
        m_pos = new Vector3[4];
        Caster = caster;
        DurationTime = 3;
        CompleteTime = 3;
        CoolTime = 10;
        ElapsedTime = CoolTime;
        return this;
    }
    public override bool RangeCheck()
    {
        return true;
    }
    public override bool Using()
    {
        if (Caster.AttackSystem.HoldAttack || Caster.State == BaseCharacter.CharacterState.Death)
            return false;
        
        if (!PossibleSkill)
            return false;

        return true;
    }
    public override void Use()
    {
        PossibleSkill = false;
        ElapsedTime = 0;

        StartCoroutine(Skill());
    }
    IEnumerator Skill()
    {
        yield return null;
        Caster.Animator.Play("ReturnSoul");
        SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Main, "불사의 전사가 나타납니다.");
    }
    void ReturnSoulReady()
    {
        m_pos[0] = transform.position + Vector3.forward*5;
        m_pos[1] = transform.position + Vector3.right * 5;
        m_pos[2] = transform.position - Vector3.forward * 5;
        m_pos[3] = transform.position - Vector3.right*5;
        for(int i = 0; i<m_pos.Length; ++i)
            EffectMng.Instance.FindEffect("Enermy/Effect_Enermy_ReturnSoulReady", m_pos[i], Vector3.zero, 2);
    }
    void ReturnSoul()
    {
        if (PlayerMng.Instance.CurrParty != null && PlayerMng.Instance.CurrParty.PartyHost != PlayerMng.Instance.MainPlayer.HostID)
            return;

        for (int i = 0; i < m_pos.Length; ++i)
            EffectMng.Instance.FindEffect("Enermy/Effect_Enermy_ReturnSoulStart", m_pos[i], Vector3.zero, 2);

        m_elapsedTime = 0;
        m_isActive = true;
        for (int i = 0; i < m_pos.Length; ++i)
            NetworkMng.Instance.RequestEnermyInstantiate(1, m_pos[i], 90*i);
    }
    void SoulAttack()
    {

    }
    protected override void Update()
    {
        base.Update();

        if (m_isActive)
        {
            m_elapsedTime += Time.deltaTime;
            if (m_elapsedTime > 20)
            {
                SoulAttack();
                m_isActive = false;
            }
        }
    }
}
