using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Dash : BaseSkill
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

        Caster.State = BaseCharacter.CharacterState.Battle;
        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        WaitForSeconds wait = new WaitForSeconds(0.02f);
        Vector3 dashPoint = transform.forward * 0.2f;
        for(int i =0; i<20; ++i)
        {
            if (Caster.IsHit || Caster.IsNuckback || Caster.IsStun)
                yield break;

            transform.position += dashPoint;
            yield return wait;
        }
        Caster.State = BaseCharacter.CharacterState.Idle;
        yield return null;
    }
}
