using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enermy_Boss_CloseAttack : BaseEnermy
{
    BaseCharacter FindTarget()
    {
        Target = null;
        List<BaseCharacter> characters = CharacterMng.Instance.GetCharactersToDistance(transform.position, GameSystem.BossMonsterChaseRangeToTarget);
        float chaseRange = GameSystem.BossMonsterChaseRangeToTarget;
        for (int i = 0; i < characters.Count; ++i)
        {
            if (characters[i].tag == "Player" || characters[i].tag == "Ally")
            {
                float deltaDistance = Vector3.Distance(transform.position, characters[i].transform.position);
                if (chaseRange > deltaDistance)
                {
                    chaseRange = deltaDistance;
                    Target = characters[i];
                }
            }
        }

        return Target;
    }
    public override void Init(int uniqueID, EAllyType allyType, NormalAttack attack, Stat stat)
    {
        base.Init(uniqueID, allyType, attack, stat);
        m_collider.radius = GameSystem.NormalMonsterColliderRange;
        AttackSystem.SuperArmor = true;
    }
    protected override void Chase()
    {
        if(Target == null)
        {
            if (FindTarget() == null)
            {
                m_collider.enabled = false;
                State = CharacterState.Move;
                return;
            }
        }

        // 후퇴거리 체크
        float distance = Vector3.Distance(transform.position, Target.transform.position);
        if (distance > GameSystem.BossMonsterChaseRangeToTarget || Target.State == CharacterState.Death)
        {
            Target = null;
            return;
        }

        // 패턴사용 체크
        bool pattern = false;
        for (int i = 0; i < m_patternList.Count; ++i)
        {
            if (m_patternList[i].IsUse())
            {
                pattern = true;
                if (m_patternList[i].RangeCheck())
                {
                    State = CharacterState.Battle;
                    return;
                }
            }
        }
        // 일반공격 체크
        if (!pattern)
        {
            if (distance < AttackSystem.NormalAttack.Range[AttackSystem.AttackCount] * 0.7f)
            {
                State = CharacterState.Battle;
                return;
            }
        }

        Animator.SetInteger("State", 1);
        MoveSystem.MoveSpeed = StatSystem.GetMoveSpeed * (1 + StatSystem.GetMoveSpeedPro);
        MoveSystem.NextFrameChase();
    }
    public void AttackEvent(int count)
    {
        EAttackType type;
        float damage;
        if (StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = StatSystem.GetCriticalCalculateDamage;
        }
        else
        {
            type = EAttackType.Normal;
            damage = StatSystem.GetNormalCalculateDamage;
        }
        AttackSystem.SendDamage(count, UniqueID, AllyType, type, damage * AttackSystem.NormalAttack.DamagePro[count]);
    }
}
