using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Crusader_SkyChain : BaseSkill
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

        Caster.Animator.Play("Shield_Skill_SkyChain");

        yield return new WaitForSeconds(0.2f);
        BaseEffect effect = EffectMng.Instance.FindEffect("Skill/Effect_Crusdaer_SkyChain", transform, 8);

        WaitForSeconds wait = new WaitForSeconds(0.1f);

        EAllyType targetAlly = EAllyType.Friendly | EAllyType.Player;
        if (Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Hostile;

        int casterID = Caster.UniqueID;
        EAttackType type;
        float damage = 0;

        if (Caster.StatSystem.IsCritical)
        {
            type = EAttackType.Critical;
            damage = Caster.StatSystem.GetCriticalCalculateDamage;
        }
        else
        {
            type = EAttackType.Normal;
            damage = Caster.StatSystem.GetNormalCalculateDamage;
        }

        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(transform.position, SkillInfo.Range);
        for (int i = 0; i < characterList.Count; ++i)
        {
            Buff buff = new Buff(Caster, characterList[i], EBuffOption.Continue, EBuffType.WeakenReduction, Icon, 10, 0.25f);
            characterList[i].BuffSystem.SetBuff(buff);
        }
        int count = 0;
        while (IsKeyDown && count < 70)
        {
            for (int i = 0; i < characterList.Count; ++i)
            {
                if ((characterList[i].AllyType & targetAlly) != 0)
                {
                    if (Vector3.Distance(characterList[i].transform.position, transform.position) <= SkillInfo.Range)
                    {
                        if (Caster.tag == "Player" && count % 10 == 0)
                            NetworkMng.Instance.NotifyReceiveDamage(type, Caster.UniqueID, characterList[i].UniqueID, damage, 0);

                        characterList[i].Stun(0.3f);
                    }
                }
            }
            ++count;
            yield return wait;
        }

        IsKeyDown = false;
        effect.DisabledTime();
        Caster.Animator.Play("Shield_Skill_SkyChainEnd");
        Caster.AttackSystem.SetDurationTime = 0.4f;
        Caster.AttackSystem.SetCompleteTime = 0.4f;
    }
}
