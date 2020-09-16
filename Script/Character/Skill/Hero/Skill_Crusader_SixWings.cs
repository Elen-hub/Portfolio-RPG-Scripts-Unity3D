using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Crusader_SixWings : BaseSkill
{
    public Hero_Crusader Crusader;

    public override bool Using()
    {
        if (base.Using())
            return true;
        return false;
    }
    public override void Use()
    {
        base.Use();
        if (Crusader == null)
            Crusader = GetComponent<Hero_Crusader>();

        StartCoroutine(Skill());
    }

    IEnumerator Skill()
    {
        yield return null;
        Crusader.Wing.SetActive(true);
        Buff buff = new Buff(Caster, Caster, EBuffOption.Default, EParamsType.SixWings, Icon, 20, 0, 0.25f, 0.25f);
        BaseEffect effect = EffectMng.Instance.FindEffect("Buff/Effect_Buff_SixWings", Crusader.Wing.transform.position, Vector3.zero, 20);
        Caster.BuffSystem.SetBuff(buff, effect);
        effect.transform.SetParent(Crusader.transform);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localEulerAngles = Vector3.zero;
        EffectMng.Instance.FindEffect("Skill/Effect_Crusader_SixWings", Crusader.Wing.transform.position, Vector3.zero, 20);

        Buff superArmor = Caster.BuffSystem.FindBuffType(EBuffType.SuperArmor);
        if (superArmor != null)
            if (superArmor.DurationTime - superArmor.ElapsedTime < 20)
                superArmor.Disabled();

        superArmor = new Buff(Caster, Caster, EBuffOption.Default, EBuffType.SuperArmor, GameSystem.SuperArmorIcon, 20, 0);
        Caster.BuffSystem.SetBuff(superArmor);

        yield return new WaitForSeconds(20);

        Crusader.Wing.SetActive(false);
        effect.transform.SetParent(EffectMng.Instance.transform);
    }

}
