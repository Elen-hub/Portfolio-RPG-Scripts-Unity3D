using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Archer_ForestShield : BaseSkill
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
        Caster.Animator.Play("Skill_Archer_ForestShield");
        yield return new WaitForSeconds(SkillInfo.DurationTime*0.2f);
        BaseEffect effect = EffectMng.Instance.FindEffect("Skill/Effect_Archer_ForestShield", transform.position, Vector3.zero, 5);

        EAllyType targetAlly = EAllyType.Hostile;
        if (Caster.AllyType != EAllyType.Hostile)
            targetAlly = EAllyType.Friendly | EAllyType.Player;

        yield return new WaitForSeconds(SkillInfo.DurationTime * 0.2f);
        Vector3 pos = Caster.transform.position;
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        for(int i =0; i<50; ++i)
        {
            List<BaseCharacter> characterList = CharacterMng.Instance.GetCharactersToDistance(pos, 4);
            for (int j = 0; j < characterList.Count; ++j)
            {
                if ((targetAlly & characterList[j].AllyType) != 0)
                {
                    if (characterList[j].State == BaseCharacter.CharacterState.Death)
                        continue;

                    Buff buff = new Buff(Caster, characterList[j], EBuffOption.Continue, EBuffType.StrengthReduction, Icon, 0.15f, 1);
                    characterList[j].BuffSystem.SetBuff(buff);
                }
            }
            yield return wait;
        }
    }
}
