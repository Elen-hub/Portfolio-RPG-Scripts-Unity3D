using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Crusader_AssultDevilHunt : BaseSkill
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

        StartCoroutine(Skill());
    }
    IEnumerator Skill()
    {
        Caster.State = BaseCharacter.CharacterState.Idle;
        Caster.MoveSystem.EnabledNavMeshAgent = false;
        Caster.Animator.Play("Shield_Skill_AssultDevilHuntJump");
        WaitForSeconds wait = new WaitForSeconds(0.022f);
        Vector3 upVector = transform.up;
        Vector3 forwardVector = transform.forward;
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_AssultDevilHuntTail", Caster.AttachSystem.GetAttachPoint(EAttachPoint.Foot), 1);
        for (int i =0;  i<15; ++i)
        {
            transform.position += upVector * 0.05f + forwardVector * 0.025f;
            transform.eulerAngles += new Vector3(1, 0, 0);
            yield return wait;
        }
        yield return new WaitForSeconds(0.15f);
        Caster.Animator.Play("Shield_Skill_AssultDevilHunt");
        for (int i = 0; i<15; ++i)
        {
            transform.position += -upVector * 0.05f + forwardVector * 0.275f;
            yield return wait;
        }
        transform.eulerAngles -= new Vector3(15, 0, 0);
        Caster.MoveSystem.EnabledNavMeshAgent = true;
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_AssultDevilHunt", transform.position, transform.eulerAngles, 1.5f);
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(transform.position, 2);
        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

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
        for (int i = 0; i < characterList.Count; ++i)
        {
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                if (characterList[i].State == BaseCharacter.CharacterState.Death)
                    continue;

                int targetID = characterList[i].UniqueID;
                characterList[i].Stun(1f);
                if (transform.tag == "Player")
                    NetworkMng.Instance.NotifyReceiveDamage(type, casterID, targetID, damage, 0.5f);
            }
        }
        yield return null;
    }
}
