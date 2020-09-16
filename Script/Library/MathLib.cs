using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathLib
{
    public static Vector3 GetVector3AngleOfPosition(Vector3 Pivot, Vector3 Target, float angle)
    {
        Target -= Pivot;
        float r = Mathf.Deg2Rad * angle;

        float x = (Target.x) * Mathf.Cos(r) - (Target.z) * Mathf.Sin(r);
        float z = (Target.x) * Mathf.Sin(r) + (Target.z) * Mathf.Cos(r);

        return new Vector3(x, Target.y, z) + Pivot;
    }
    public static int IComparerSkillInfo(SkillInfo start, SkillInfo to)
    {
        if (start.Level == to.Level)
        {
            if (start.Handle > to.Handle)
                return 1;
            else
                return -1;
        }
        else
        {
            if (start.Level > to.Level)
                return 1;
            else
                return -1;
        }
    }
}
