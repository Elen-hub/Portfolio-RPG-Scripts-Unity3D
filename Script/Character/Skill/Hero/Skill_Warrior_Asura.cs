using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Warrior_Asura : BaseSkill
{
    EAllyType m_targetAlly;
    public override bool Using()
    {
        if (base.Using())
            return true;
        return false;
    }
    public override void Use()
    {
        base.Use();

        m_targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            m_targetAlly = EAllyType.Friendly | EAllyType.Player;

        Caster.AttackSystem.SuperArmor = true;
        Caster.Animator.Play("Skill_Warrior_Asura");
    }
    void OnAsuraEffect01()
    {
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_Asura01", transform.position, transform.eulerAngles, 3f);
        StartCoroutine(IEMovingToFoward(0.15f, 0.2f));
    }
    void OnAsuraDamage01()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.9f);
        CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 0.1f, 75, 1);
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, SkillInfo.Range, 180);
        SetDamage(1.5f, 0.75f, characterList, true, 0.1f, 0.2f);
    }
    void OnAsuraEffect02()
    {
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_Asura02", transform.position, transform.eulerAngles, 3f);
        StartCoroutine(IEMovingToFoward(0.15f, 0.2f));
    }
    void OnAsuraDamage02()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.9f);
        CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 0.1f, 75, 1);
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, SkillInfo.Range, 180);
        SetDamage(1.5f, 0.75f, characterList, true, 0.1f, 0.2f);
    }
    void OnAsuraEffect03()
    {
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_Asura03", transform.position, transform.eulerAngles, 3f);
        StartCoroutine(IEMovingToFoward(0.1f, 0.2f));
    }
    void OnAsuraDamage03()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.85f);
        CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 0.1f, 75, 1);
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, SkillInfo.Range, 180);
        SetDamage(2, 0.75f, characterList, true, 0.1f, 0.3f);
    }
    void OnAsuraEffect04()
    {
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_Asura04", transform.position, transform.eulerAngles, 3f);
        StartCoroutine(IEMovingToFoward(0.15f, 0.3f));
    }
    void OnAsuraDamage04()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.85f);
        CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 0.1f, 75, 1);
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, SkillInfo.Range, 180);
        SetDamage(2, 0.75f, characterList, true, 0.1f, 0.5f);
    }
    void OnAsuraEffect05()
    {
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_Asura05", transform.position, transform.eulerAngles, 3f);
        StartCoroutine(IEMovingToFoward(0.3f, 0.5f));
    }
    void OnAsuraDamage05()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.8f);
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, SkillInfo.Range*1.25f, 180);
        SetDamage(3, 0.75f, characterList, true, 0.2f, 0.75f);
    }
    void OnAsuraEffect06()
    {
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_Asura06", transform.position, transform.eulerAngles, 3f);
        StartCoroutine(IEMovingToFoward(0.3f, 0.75f));
    }
    void OnAsuraDamage06()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.75f);
        CameraMng.EarthQuakeShake(CameraMng.Instance.GetCamera(CameraMng.CameraStyle.Player).camera, 1, 150, 2);
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, SkillInfo.Range * 1.5f, 180);
        SetDamage(5, 1.5f, characterList, true, 0.25f, 1.5f);
    }
    void OnAsuraEnd()
    {
        Caster.AttackSystem.SuperArmor = false;
    }
    void SetDamage(float damagePercent, float hitTime, List<BaseCharacter> characterList, bool useNuckBack = false, float nuckBackTime = 0, float nuckBackForce = 0)
    {
        EAttackType type;
        float damage = 0;

        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * damagePercent;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * damagePercent;
        }

        for (int i = 0; i < characterList.Count; ++i)
        {
            if ((characterList[i].AllyType & m_targetAlly) != 0)
            {
                if (characterList[i].State == BaseCharacter.CharacterState.Death)
                    continue;

                if (transform.tag == "Player")
                {
                    int targetID = characterList[i].UniqueID;
                    NetworkMng.Instance.NotifyReceiveDamage(type, Caster.UniqueID, targetID, damage, 1.5f);
                }

                EffectMng.Instance.FindEffect("Skill/Effect_Warrior_AsuraHit", characterList[i].AttachSystem.GetAttachPoint(EAttachPoint.Chest).position, Vector3.zero, 1);
                Vector3 nuckBackPos = (characterList[i].transform.position - transform.position).normalized;
                if(useNuckBack)
                    characterList[i].Nuckback(nuckBackPos, nuckBackTime, nuckBackForce);

                characterList[i].SetHit(hitTime);
            }
        }
    }
    IEnumerator IEMovingToFoward(float time, float distance)
    {
        yield return null;
        float elapsedTime = 0;
        while (true)
        {
            yield return null;
            if (elapsedTime > time)
                yield break;

            elapsedTime += Time.deltaTime;
            transform.position += transform.forward * distance / time * Time.deltaTime;
        }
    }
}
