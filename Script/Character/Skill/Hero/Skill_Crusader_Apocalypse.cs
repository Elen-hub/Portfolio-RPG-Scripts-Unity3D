using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Crusader_Apocalypse : BaseSkill
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
        yield return null;
        Caster.Animator.Play("Shield_Skill_Apocalypse");
        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.1f);
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_ApocalypseReady", transform.position, Vector3.zero, 3f);
        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.35f);
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_ApocalypseFire", transform.position, Vector3.zero, 3f);

        Vector3 pos = Caster.transform.position;
        EAllyType targetAlly = EAllyType.Hostile;
        if(Caster.AllyType == EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        int casterID = Caster.UniqueID;
        EAttackType type;
        float damage = 0;

        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage * 0.8f;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage * 0.8f;
        }
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        for(int t =0; t<8; ++t)
        {
            List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(pos, SkillInfo.Range);
            for (int i = 0; i < characterList.Count; ++i)
            {
                if ((characterList[i].AllyType & targetAlly) != 0)
                {
                    if (characterList[i].State == BaseCharacter.CharacterState.Death)
                        continue;

                    if (transform.tag == "Player")
                    {
                        int targetID = characterList[i].UniqueID;
                        NetworkMng.Instance.NotifyReceiveDamage(type, casterID, targetID, damage, 0.3f);
                    }

                    EffectMng.Instance.FindEffect("Buff/Effect_Buff_Burn", characterList[i].transform, 1);
                }
            }
            yield return wait;
        }
    }
}
