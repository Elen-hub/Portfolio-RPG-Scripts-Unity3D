using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Cleric_Bless : BaseSkill
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

        
        StartCoroutine(Bless());
    }

    IEnumerator Bless()
    {
        // 대기시간
        // 이펙트
        Caster.Animator.Play("Skill_Cleric_Heal");
        yield return new WaitForSeconds(SkillInfo.DurationTime*0.6f);
        EffectMng.Instance.FindEffect("Skill/Effect_Cleric_Bless", transform.position, Vector3.zero, 3);
        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.4f);
        List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(Caster.transform.position, SkillInfo.Range);
        EAllyType targetAlly = EAllyType.Hostile;
        if(Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        float buffDurationTime = 40;
        float attackDamageValue = 5 + Caster.StatSystem.GetWIS * 0.5f;
        float defenceValue = 3 + Caster.StatSystem.GetWIS * 0.3f;
        for (int i = 0; i < characterList.Count; ++i)
        {
            int targetID = characterList[i].UniqueID;
            if ((characterList[i].AllyType & targetAlly) != 0)
            {
                if (characterList[i].State == BaseCharacter.CharacterState.Death)
                    continue;

                BaseEffect effect = EffectMng.Instance.FindEffect("Buff/Effect_Buff_Bless", characterList[i].AttachSystem.GetAttachPoint(EAttachPoint.Weapon), buffDurationTime);
                Buff buff = new Buff(Caster, characterList[i], EBuffOption.Single, EParamsType.Bless, Icon, buffDurationTime, 0, attackDamageValue, defenceValue, 0.5f);
                characterList[i].BuffSystem.SetBuff(buff, effect);
            }
        }
        yield return null;
    }
}
