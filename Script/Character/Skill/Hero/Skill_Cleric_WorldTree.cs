using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Cleric_WorldTree : BaseSkill
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
        Caster.Animator.Play("Skill_Cleric_WorldTree");
        BaseEffect effect = EffectMng.Instance.FindEffect("Skill/Effect_Cleric_WorldTree", transform.position, Vector3.zero, 5);
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        Caster.Animator.SetBool("WorldTree", true);
        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        int count = 0;
        while (IsKeyDown && count < 50)
        {
            List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(Caster.transform.position, SkillInfo.Range);
            for(int i =0; i<characterList.Count; ++i)
            {
                if((targetAlly & characterList[i].AllyType) != 0)
                {
                    if (characterList[i].State == BaseCharacter.CharacterState.Death)
                        continue;

                    Buff buff = new Buff(Caster, characterList[i], EBuffOption.Continue, EBuffType.InvincibilityArmor, Icon, 0.15f, 0);
                    characterList[i].BuffSystem.SetBuff(buff);
                }
            }
            ++count;
            yield return wait;
        }
        Caster.Animator.SetBool("WorldTree", false);
        effect.DisabledTime();
        Caster.AttackSystem.HoldAttack = false;
        Caster.AttackSystem.CompleteAttack = true;
        yield return null;
    }
}
