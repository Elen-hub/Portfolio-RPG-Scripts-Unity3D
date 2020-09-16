using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Flags]
public enum ESkillType
{
    Default = 0,
    Keydown = 1,
    Link =2,
    Channeling = 4,
    NotLearn = 8,
    Passive = 16,
}
public class SkillInfo
{
    public SkillInfo()
    {
        Name = "Empty Skill";
    }
    public int Handle;
    public ECharacterClass CharacterClass;
    public int CharacterAwakening;
    public int Level;
    public int SkillPoint;
    public string Name;
    public string Information;
    public string Explanation;
    public string Icon;
    public float DurationTime;
    public float CompleteTime;
    public float MP;
    public float CoolTime;

    public bool IsAutoAim = true;
    public float Range;

    public ESkillType Type;
    public int LinkHandle;
    public float LinkPossibleTime;
    public float KeydownTime;
    public float ChannelingTime;
}
public class BaseSkill : MonoBehaviour, IQuickSlotable
{
    public SkillInfo SkillInfo;
    public float ElapsedTime { get; set; }
    public float CoolTime { get; set; }
    public string Icon { get; set; }
    public bool IsChanneling;
    float m_channelingElapsedTime;

    public bool IsKeyDown;
    float m_keydownElapsedTime;

    public bool IsLink;
    public BaseSkill LinkSkill;
    float m_linkElapsedTime;

    public BaseCharacter Caster;

    public float DurationTime = 0;
    public float CompleteTime = 0;

    public bool PossibleSkill;
    public float GetCoolTimePercentage { get { return 1 - ElapsedTime / SkillInfo.CoolTime; } }

    public virtual BaseSkill Init(BaseEnermy caster)
    {
        return this;
    }
    public virtual BaseSkill Init(BaseCharacter caster, SkillInfo info)
    {
        SkillInfo = info;
        Caster = caster;
        Icon = SkillInfo.Icon;
        CoolTime = SkillInfo.CoolTime;
        ElapsedTime = SkillInfo.CoolTime;
        DurationTime = info.DurationTime;
        CompleteTime = info.CompleteTime;
        if ((SkillInfo.Type & ESkillType.Link) != 0)
        {
            if (SkillInfo.LinkHandle != 0)
                LinkSkill = CharacterMng.Instance.AddSkill(caster, SkillInfo.LinkHandle);
        }

        return this;
    }
    public virtual bool RangeCheck()
    {
        if (SkillInfo.IsAutoAim)
        {
            switch (GameSystem.AutoAim)
            {
                // Optional AutoAiming
                case 1:
                    if (Caster.Target)
                    {
                        if (Caster.Target.State == BaseCharacter.CharacterState.Death)
                            return true;

                        return AutoAIM();
                    }
                    return true;
                // Always AutoAiming 
                case 2:
                    if (Caster.Target)
                    {
                        if (Caster.Target.State != BaseCharacter.CharacterState.Death)
                            return AutoAIM();
                    }
                    else
                    {
                        BaseCharacter target = CharacterMng.Instance.GetFilterClosestCharacter(transform.position, EAllyType.Hostile);
                        if (target != null)
                        {
                            UIMng.Instance.GetUI<Game>(UIMng.UIName.Game).InfoWindow.Enabled(target);
                            return AutoAIM();
                        }
                    }
                    return true;
            }
        }
        return true;
    }
    public virtual bool Using()
    {
        if (Caster.IsStun || Caster.IsHit || Caster.IsNuckback)
            return false;

        if (Caster.AttackSystem.HoldAttack)
            return false;

        if (!PossibleSkill)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "아직 사용 할 수 없습니다.");
            return false;
        }
        if (Caster.StatSystem.CurrMP < SkillInfo.MP)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "마나가 부족합니다.");
            return false;
        }
        return RangeCheck();
    }
    bool AutoAIM()
    {
        bool rangeCheck = Vector3.Distance(Caster.transform.position, Caster.Target.transform.position) < SkillInfo.Range;
        if (rangeCheck)
        {
            // 타겟이 거리조건을 만족한다면, 스킬 사용각도 설정
            float angle = Vector3.Angle(Vector3.forward, Caster.Target.transform.position - Caster.transform.position);
            if (Caster.Target.transform.position.x - transform.position.x < 0)
                angle *= -1;
            Caster.MoveSystem.SetAxis = angle;
        }
        else
        {
            // 만족하지 않는다면, 스킬사용거리까지 이동 후 스킬재사용 요청
            Caster.MoveSystem.SetMoveToTarget(Caster.Target.transform, SkillInfo.Range, () =>
            {
                Caster.State = BaseCharacter.CharacterState.Idle;
                float angle = Vector3.Angle(Vector3.forward, Caster.Target.transform.position - transform.position);
                if (Caster.Target.transform.position.x - transform.position.x < 0)
                    angle *= -1;
                Caster.MoveSystem.SetAxis = angle;
                NetworkMng.Instance.NotifyCharacterState_Skill(Caster.MoveSystem.GetCurrAxis, SkillInfo.Handle);
            });
        }
        return rangeCheck;
    }
    public virtual void Use()
    {
        PossibleSkill = false;
        ElapsedTime = 0;
        Caster.StatSystem.CurrMP -= SkillInfo.MP;

        if((SkillInfo.Type & ESkillType.Keydown) != 0)
        {
            m_keydownElapsedTime = 0;
            IsKeyDown = true;
        }
        if ((SkillInfo.Type & ESkillType.Link) != 0)
        {
            m_linkElapsedTime = 0;
            IsLink = true;
        }
        if ((SkillInfo.Type & ESkillType.Channeling) != 0)
        {
            m_channelingElapsedTime = 0;
            IsChanneling = true;
        }

        Caster.MoveSystem.Stop = true;
        Caster.State = BaseCharacter.CharacterState.Idle;
    }
    protected virtual void ChannelingAction()
    {

    }
    public virtual void EndChanneling()
    {
        IsChanneling = false;
        m_channelingElapsedTime = 0;
    }
    public virtual void EndLink()
    {
        IsLink = false;
        m_linkElapsedTime = 0;
    }
    public virtual void EndKeydown()
    {
        IsKeyDown = false;
        m_keydownElapsedTime = 0;
    }
    protected virtual void Update()
    {
        if (IsKeyDown)
        {
            m_keydownElapsedTime += Time.deltaTime;
            if(m_keydownElapsedTime > SkillInfo.KeydownTime)
                EndKeydown();
        }
        if (IsLink)
        {
            m_linkElapsedTime += Time.deltaTime;
            if(m_linkElapsedTime > SkillInfo.LinkPossibleTime)
                EndLink();
        }
        if (IsChanneling)
        {
            ChannelingAction();
            m_channelingElapsedTime += Time.deltaTime;
            if(m_channelingElapsedTime > SkillInfo.ChannelingTime)
                EndChanneling();
        }

        if (PossibleSkill)
            return;

        ElapsedTime += Time.deltaTime;

        if (ElapsedTime> CoolTime)
            PossibleSkill = true;
    }
}
