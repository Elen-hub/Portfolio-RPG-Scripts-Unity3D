using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enermy_SkeletonKing : Enermy_Boss_CloseAttack
{
    public PSMeshRendererUpdater MeshEffect_FireSword;
    public override void Init(int uniqueID, EAllyType allyType, NormalAttack attack, Stat stat)
    {
        base.Init(uniqueID, allyType, attack, stat);
        m_collider.radius = GameSystem.BossMonsterColliderRange;
        AttackSystem.SuperArmor = true;
        if (MeshEffect_FireSword == null)
            MeshEffect_FireSword = EffectMng.Instance.FindMeshEffect(AttachSystem.GetAttachPoint(EAttachPoint.Weapon), EMeshEffectType.Fire);

        MeshEffect_FireSword.IsActive = true;
    }
}
