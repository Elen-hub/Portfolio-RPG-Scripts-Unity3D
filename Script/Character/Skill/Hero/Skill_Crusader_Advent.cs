using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Crusader_Advent : BaseSkill
{
    public override bool Using()
    {
        if (!PossibleSkill)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "아직 사용 할 수 없습니다.");
            return false;
        }
        if (Caster.StatSystem.CurrMP < SkillInfo.MP)
        {
            SystemMessage.Instance.PushMessage(SystemMessage.MessageType.Sub, "마나가 부족합니다.");
            return false;
        }
        return true;
    }
    public override void Use()
    {
        base.Use();
        StartCoroutine(Skill());
    }

    IEnumerator Skill()
    {
        Caster.AttackSystem.Invincibility = true;
        Caster.MoveSystem.Stop = true;
        Caster.StatSystem.CurrHP = Caster.StatSystem.GetHP;
        Caster.State = BaseCharacter.CharacterState.Death;
        Caster.Animator.Play("Death");
        yield return new WaitForSeconds(0.5f);
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_AdventCharge", transform.position, Vector3.zero, 3);
        yield return new WaitForSeconds(2f);
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_Advent", transform.position, Vector3.zero, 3);
        yield return new WaitForSeconds(0.3f);
        Caster.Revival(Caster.StatSystem.GetHP, Caster.StatSystem.GetMP);
        Buff InvincibilityArmor = Caster.BuffSystem.FindBuffType(EBuffType.InvincibilityArmor);
        if (InvincibilityArmor != null)
        {
            if (InvincibilityArmor.DurationTime - InvincibilityArmor.ElapsedTime < 3)
                InvincibilityArmor.Disabled();
        }
        InvincibilityArmor = new Buff(Caster, Caster, EBuffOption.Default, EBuffType.InvincibilityArmor, GameSystem.InvincibilityArmor, 3, 0);
        Caster.BuffSystem.SetBuff(InvincibilityArmor);
        yield return null;
    }
}
