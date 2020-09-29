using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Warrior_Crescent : BaseSkill
{
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
        Caster.Animator.Play("Skill_Warrior_Crescent");
    }
    void OnCrescentMoveEvent()
    {
        StartCoroutine(IEMovingToFoward(0.2f, SkillInfo.Range * 0.25f));
    }
    void OnCrescentEffectEvent()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.85f);
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_Crescent", transform.position, transform.eulerAngles, 3f);
    }
    void OnCrescentDamageEvent()
    {
        Caster.AttackSystem.SuperArmor = false;
        EffectMng.Instance.FindEffect("Skill/Effect_Warrior_CrescentHit", transform.position, transform.eulerAngles, 1);

        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        int casterID = Caster.UniqueID;
        EAttackType type;
        float damage = 0;

        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * 2.5f;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * 2.5f;
        }

        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(transform.position, SkillInfo.Range * 0.75f);
        for (int i = 0; i < characterList.Count; ++i)
        {
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                if (characterList[i].State == BaseCharacter.CharacterState.Death)
                    continue;

                if (transform.tag == "Player")
                {
                    int targetID = characterList[i].UniqueID;
                    NetworkMng.Instance.NotifyReceiveDamage(type, casterID, targetID, damage, 1.5f);
                }

                Vector3 nuckBackPos = (characterList[i].transform.position - transform.position).normalized;
                characterList[i].Nuckback(nuckBackPos, 0.6f, 2.5f);
                characterList[i].SetHit(1.5f);
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
