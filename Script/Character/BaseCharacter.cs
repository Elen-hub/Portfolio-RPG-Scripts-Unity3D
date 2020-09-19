using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EAllyType
{
    Player =1,
    Friendly =2,
    Hostile =4,
    Neutral = 8,
}
public delegate void CharacterBehaviour();
public abstract class BaseCharacter : MonoBehaviour
{
    [System.Flags]
    public enum CharacterState
    {
        Idle= 1,
        Move =2,
        Chase =4,
        Battle=8,
        Death=16,
    }
    [Header("Info")]
    public int UniqueID;
    public EAllyType AllyType;
    [SerializeField] CharacterState m_state;

    BaseEffect m_stunEffect;
    public BaseCharacter Target;
    [HideInInspector] public ChatBox ChatBox;
    [HideInInspector] public Animator Animator;

    [HideInInspector] public AttackSystem AttackSystem;
    [HideInInspector] public StatSystem StatSystem;
    [HideInInspector] public BuffSystem BuffSystem;
    [HideInInspector] public MoveSystem MoveSystem;
    [HideInInspector] public AttachSystem AttachSystem;
    [HideInInspector] public Outline Outline;

    protected Dictionary<CharacterState, CharacterBehaviour> m_actionDic = new Dictionary<CharacterState, CharacterBehaviour>();

    [HideInInspector] public float m_recoveryElapsedTime;
    public virtual CharacterState State
    {
        get { return m_state; }
        set { if (m_state != CharacterState.Death) m_state = value; }
    }
    public virtual void SetAngle(float angle) { }
    protected abstract void Idle();
    protected abstract void Move();
    protected virtual void Chase() { }
    protected virtual void Death() { }
    public virtual void ReceiveAttack(SReceiveHandle handle) { }
    protected virtual void Battle() { }
    public virtual void Revival(float hp, float mp)
    {
        Animator.Play("Revive");
        StatSystem.CurrHP = hp;
        StatSystem.CurrMP = mp;
        m_state = CharacterState.Idle;
        MoveSystem.Stop = false;
    }
    [Header("Status")]
    public bool IsHit;
    public bool IsStun;
    public bool IsNuckback;
    Vector3 m_pivotPos;
    Vector3 m_nuckbackVector;
    float m_hitTime;
    float m_hitElapsedTime;
    float m_nuckbackTime;
    float m_nuckbackElapsedTime;
    float m_stunTime;
    float m_stunElapsedTime;
    public void SetHit(float time)
    {
        if (AttackSystem.SuperArmor || State == CharacterState.Death)
            return;

        MoveSystem.Stop = true;
        m_hitElapsedTime = 0;
        m_hitTime = time;
        IsHit = true;
    }
    public void Nuckback(Vector3 vector, float time, float distance)
    {
        if (AttackSystem.SuperArmor || AttackSystem.Invincibility || State == CharacterState.Death)
            return;

        m_nuckbackElapsedTime = 0;
        m_nuckbackTime = time;
        m_nuckbackVector = vector * distance;
        m_pivotPos = transform.position;
        IsNuckback = true;
    }
    public void Stun(float time)
    {
        if (AttackSystem.ImmunStun)
            return;
        if (IsStun && m_stunTime - m_stunElapsedTime > time)
            return;

        if (m_stunEffect)
            m_stunEffect.ResetTargetTime = time;
        else
            m_stunEffect = EffectMng.Instance.FindEffect("Buff/Effect_Buff_Stun", AttachSystem.GetAttachPoint(EAttachPoint.UnderHead), time);
        
        m_stunElapsedTime = 0;
        m_stunTime = time;
        IsStun = true;
    }
    protected virtual void Update()
    {
        if (State == CharacterState.Death)
            return;
        
        if(AttackSystem.ImmunStun && IsStun)
        {
            if(m_stunEffect)
            {
                m_stunEffect.Disabled();
                m_stunEffect = null;
            }
            m_stunTime = 0;
            IsStun = false;
        }
        if(AttackSystem.Invincibility || AttackSystem.SuperArmor)
        {
            if (AttackSystem.SuperArmor)
            {
                IsHit = false;
                IsNuckback = false;

                if (GameSystem.UseOutline)
                {
                    Outline.OutlineWidth = 2;
                    Outline.OutlineColor = Color.red;
                }
            }
            if (AttackSystem.Invincibility)
            {
                if (GameSystem.UseOutline)
                {
                    Outline.OutlineWidth = 2;
                    Outline.OutlineColor = Color.blue;
                }
            }
        }
        else
        {
            Outline.OutlineColor = Color.clear;
            Outline.OutlineWidth = 0;
        }
        if (IsHit)
        {
            m_hitElapsedTime += Time.deltaTime;
            if(m_hitElapsedTime > m_hitTime)
                IsHit = false;
        }
        if (IsNuckback)
        {
            m_nuckbackElapsedTime += Time.deltaTime;
            if (m_nuckbackElapsedTime > m_nuckbackTime)
                IsNuckback = false;
            else
                transform.position = Vector3.Lerp(m_pivotPos, m_pivotPos + m_nuckbackVector, m_nuckbackElapsedTime / m_nuckbackTime);
        }
        if(IsStun)
        {
            m_stunElapsedTime += Time.deltaTime;
            if (m_stunElapsedTime > m_stunTime)
            {
                m_stunEffect = null;
                IsStun = false;
            }
        }

        m_actionDic[State]();
    }

    public IEnumerator RevivalAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_AdventCharge", transform.position, Vector3.zero, 3);
        CameraMng.Infrared_OFF(1.5f);
        yield return new WaitForSeconds(1.5f);
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_Advent", transform.position, Vector3.zero, 3);
        Revival(1, 1);
        // UIMng.Instance.OPEN = UIMng.UIName.Game;
    }
}
