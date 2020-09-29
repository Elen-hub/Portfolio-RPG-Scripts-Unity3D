using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Warrior_FullMoomSlash : BaseSkill
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

        Caster.Animator.Play("Skill_Warrior_FullMoomSlash");
    }
    void OnFullMoonMoveEvent()
    {
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.9f);
        Caster.Nuckback(transform.forward, 0.15f, SkillInfo.Range * 0.3f);
    }
    void OnFullMoonEffectEvent()
    {
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_JudgementSlash", transform.position, transform.eulerAngles, 3f);
    }
    void OnFullMoonDamageEvent()
    {
        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        int casterID = Caster.UniqueID;
        EAttackType type;
        float damage = 0;

        type = EAttackType.Critical;
        damage = Caster.StatSystem.GetCriticalCalculateDamage * 1.5f;

        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharacterToDot(transform, SkillInfo.Range * 0.7f, 180);
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

                EffectMng.Instance.FindEffect("Skill/Effect_Crusader_JudgementSlashHit", characterList[i].AttachSystem.GetAttachPoint(EAttachPoint.UnderHead).position, Vector3.zero, 1);
                Vector3 nuckBackPos = (characterList[i].transform.position - transform.position).normalized;
                characterList[i].Nuckback(nuckBackPos, 0.4f, 2);
                characterList[i].SetHit(1);
            }
        }
    }
}
