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
    #region Default Var
    public SkillInfo SkillInfo;

    // Action target, Animation Time
    public BaseCharacter Caster;
    public float DurationTime = 0;
    public float CompleteTime = 0;

    public float ElapsedTime { get; set; }
    public float CoolTime { get; set; }
    public string Icon { get; set; }
    public bool PossibleSkill;
    public float GetCoolTimePercentage { get { return 1 - ElapsedTime / SkillInfo.CoolTime; } }
    #endregion

    #region Flag Var
    public bool IsChanneling;
    float m_channelingElapsedTime;

    public bool IsKeyDown;
    float m_keydownElapsedTime;

    public bool IsLink;
    public BaseSkill LinkSkill;
    float m_linkElapsedTime;
    #endregion

    public virtual BaseSkill Init(BaseEnermy caster)
    {
        return this;
    } // 몬스터 스킬 초기화
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
    } // 플레이어블 스킬 초기화
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
    } // 이 스킬이 사용 가능한지 쿨타임, 마나, 캐릭터 상태 체크.
    public virtual void Use()
    {
        PossibleSkill = false;
        ElapsedTime = 0;
        Caster.StatSystem.CurrMP -= SkillInfo.MP;

        if ((SkillInfo.Type & ESkillType.Keydown) != 0)
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
    } // 스킬을 사용함에 있어서 공통적인 소모값, 상태변경. 가시적인 효과는 오버라이딩하여 구현
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
    } // Auto Aming 기능 사용시 거리, 타겟체크
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
    } // 체크가 완료됬다면 실행
    protected virtual void ChannelingAction()
    {

    } // 채널링 지속 효과
    public virtual void EndChanneling()
    {
        IsChanneling = false;
        m_channelingElapsedTime = 0;
    } // 채널링효과 종료
    public virtual void EndLink()
    {
        IsLink = false;
        m_linkElapsedTime = 0;
    } // 링크스킬 활성화 시간 만료
    public virtual void EndKeydown()
    {
        IsKeyDown = false;
        m_keydownElapsedTime = 0;
    } // 키다운 스킬 비활성화
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
    }  // 쿨타임, 애니메이션, 특수 플래그 시간 채크
}
