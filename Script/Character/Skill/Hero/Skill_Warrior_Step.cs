using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Warrior_Step : BaseSkill
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

        Caster.Animator.Play("Skill_Warrior_Step");
    }
    void OnStepMoveEvent()
    {
        StartCoroutine(IEMovingToFoward(0.25f, SkillInfo.Range));
        CameraMng.Instance.GetCamera<PlayerCamera>(CameraMng.CameraStyle.Player).CameraAction_Look(0.95f);
    }
    IEnumerator IEMovingToFoward(float time, float distance)
    {
        yield return null;
        float elapsedTime = 0;
        while (true)
        {
            yield return null;
            if (Caster.IsHit || Caster.IsNuckback || Caster.IsStun)
                yield break;
            if (elapsedTime > time)
                yield break;

            elapsedTime += Time.deltaTime;
            transform.position += transform.forward * distance / time * Time.deltaTime;
        }
    }
}
