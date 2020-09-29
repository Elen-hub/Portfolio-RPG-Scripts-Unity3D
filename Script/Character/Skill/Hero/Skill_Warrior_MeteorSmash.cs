using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Warrior_MeteorSmash : BaseSkill
{
    List<BaseCharacter> m_hitList = new List<BaseCharacter>();
    Vector3 m_targetPos;
    Vector3 m_lastEffectPos;
    public override bool Using()
    {
        if (base.Using())
            return true;
        return false;
    }
    public override void Use()
    {
        base.Use();

        Caster.AttackSystem.SuperArmor = true;
        Caster.Animator.Play("Skill_Warrior_MeteorSmash");
        m_targetPos = transform.position + transform.forward * 2.5f;
        m_lastEffectPos = m_targetPos + transform.forward * 2;
    }
    void OnMeteorEffect01()
    {
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_MeteorUpperSlash", transform.position, transform.eulerAngles, 3f);
    }
    void OnMeteorAir()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(1.1f);
        Vector3 targetPos = transform.position + transform.forward * 1.5f;
        targetPos.y = transform.position.y + 1.5f;
        Caster.MoveSystem.EnabledNavMeshAgent = false;
        StartCoroutine(IEAirControl(targetPos, 0.35f, false));
    }
    void OnMeteorDamage01()
    {
        m_hitList.Clear();
        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToRectangleRange(transform.position, transform.eulerAngles.y, 5, SkillInfo.Range);
        for (int i = 0; i < characterList.Count; ++i)
        {
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                if (characterList[i].State == BaseCharacter.CharacterState.Death)
                    continue;

                m_hitList.Add(characterList[i]);
                EffectMng.Instance.FindEffect("Skill/Effect_Warrior_MeteorUpperHitEffect", characterList[i].AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, new Vector3(270 + Random.Range(-30, 30), 0, 0), 1);
            }
        }

        EAttackType type;
        int casterID = Caster.UniqueID;
        float damage = 0;
        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * 1.5f;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * 1.5f;
        }

        for (int i =0; i<m_hitList.Count; ++i)
        {
            m_hitList[i].Nuckback(transform.forward, 0.3f, 3);
            if (transform.tag == "Player")
            {
                int targetID = m_hitList[i].UniqueID;
                NetworkMng.Instance.NotifyReceiveDamage(type, casterID, targetID, damage, 1);
            }
        }
    }
    void OnMeteorDown()
    {
        StartCoroutine(IEAirControl(m_targetPos, 0.2f, true));
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_MeteorDownSmash", transform.position, transform.eulerAngles, 3f);
    }
    void OnMeteorDamage02()
    {
        m_hitList.Clear();
        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToRectangleRange(transform.position, transform.eulerAngles.y, 5, SkillInfo.Range*1.5f);
        for (int i = 0; i < characterList.Count; ++i)
        {
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                if (characterList[i].State == BaseCharacter.CharacterState.Death)
                    continue;

                EffectMng.Instance.FindEffect("Skill/Effect_Warrior_MeteorSlashHitEffect", characterList[i].AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, new Vector3(270 + Random.Range(-30, 30), 0, 0), 1);
                m_hitList.Add(characterList[i]);
            }
        }

        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.8f);
        CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 1.5f, 150, 5);
        EAttackType type;
        int casterID = Caster.UniqueID;
        float damage = 0;
        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * 3.5f;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * 3.5f;
        }

        for (int i = 0; i < m_hitList.Count; ++i)
        {
            m_hitList[i].Stun(2f);
            m_hitList[i].Nuckback((m_hitList[i].transform.position - transform.position).normalized, 0.5f, 3.5f);
            if (transform.tag == "Player")
            {
                int targetID = m_hitList[i].UniqueID;
                NetworkMng.Instance.NotifyReceiveDamage(type, casterID, targetID, damage, 1.5f);
            }
        }
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_MeteorSmash", m_lastEffectPos, transform.eulerAngles, 2);
        Caster.AttackSystem.SuperArmor = false;
    }
    IEnumerator IEAirControl(Vector3 targetPos, float time, bool navEnabled)
    {
        yield return null;
        Vector3 currentPos = transform.position;
        float elapsedTime = 0;
        while(true)
        {
            yield return null;
            if (elapsedTime > time)
            {
                if (navEnabled)
                    Caster.MoveSystem.EnabledNavMeshAgent = true;

                yield break;
            }

            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(currentPos, targetPos, elapsedTime / time);
        }
    }
}
