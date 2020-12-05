using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enermy_Boss_CloseAttack : BaseEnermy
{
    public BaseCharacter FindTarget()
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
        m_stateDic[CharacterState.Chase] = new State_Chase_BossClose(this);
        m_collider.radius = GameSystem.NormalMonsterColliderRange;
        AttackSystem.SuperArmor = true;
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
