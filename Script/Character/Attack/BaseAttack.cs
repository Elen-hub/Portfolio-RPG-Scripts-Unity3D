using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack
{
    public int Handle;
    public int Count;
    public float[] DurationTime;
    public float[] CompleteTime;
    public float[] DamagePro;
    public virtual void Init(int count, float[] durationTime, float[] completeTime, float[] damagePro)
    {

    }
    public virtual BaseCharacter[] GetTargets()
    {
        return new BaseCharacter[1] { GameObject.Find("Player").GetComponent<BaseCharacter>() };
    }
    public virtual void SendDamage(EAttackType type, int uniqueID, float damage, int count)
    {
        float Damage = damage * (1 + DamagePro[count]);
        SReceiveHandle Handle = new SReceiveHandle(type, uniqueID, Damage, 0.3f);

        BaseCharacter[] character = GetTargets();
        for (int i = 0; i < character.Length; ++i)
            character[i].ReceiveAttack(Handle);
    }
}
