using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Warrior_DrawingSword : BaseSkill
{
    EAllyType m_targetAlly;
    List<BaseCharacter> m_hitList = new List<BaseCharacter>();
    Vector3 m_effectPos;
    Vector3 m_effectAngle;
    public override bool Using()
    {
        if (base.Using())
            return true;
        return false;
    }
    public override void Use()
    {
        base.Use();

        m_hitList.Clear();
        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        m_effectPos = transform.position;
        m_effectAngle = transform.eulerAngles;
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToRectangleRange(transform.position, transform.eulerAngles.y, 3, SkillInfo.Range);
        for (int i = 0; i < characterList.Count; ++i)
        {
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                if (characterList[i].State == BaseCharacter.CharacterState.Death)
                    continue;

                m_hitList.Add(characterList[i]);
                characterList[i].Stun(1f);
            }
        }
        Caster.Animator.Play("Skill_Warrior_DrawingSword");
    }
    void OnDrawingMoveEvent()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.85f);
        Caster.transform.position += transform.forward * 4;
    }
    void OnDrawingSlashEffect()
    {
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_DrwaingSword", m_effectPos, m_effectAngle, 3f);
    }
    void OnDrawingDamageEvent()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.8f);
        CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 0.75f, 50, 1);

        int casterID = Caster.UniqueID;
        EAttackType type;
        float damage = 0;

        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * 4;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * 4;
        }

        for (int i = 0; i < m_hitList.Count; ++i)
        {
            if (m_hitList[i].State == BaseCharacter.CharacterState.Death)
                continue;

            if (transform.tag == "Player")
            {
                int targetID = m_hitList[i].UniqueID;
                NetworkMng.Instance.NotifyReceiveDamage(type, casterID, targetID, damage, 2);
            }
            EffectMng.Instance.FindEffect("Skill/Effect_Warrior_DrawingSwordHit", m_hitList[i].AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, m_hitList[i].transform.eulerAngles, 2);
        }
    }
}
