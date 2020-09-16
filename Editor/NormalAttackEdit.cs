using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NormalAttackEdit : EditorWindow
{
    public NormalAttack InputNormalAttack(ref int posY, NormalAttack stat, float windowSize, GUIStyle guiStyle)
    {
        GUI.Label(new Rect(0, posY, windowSize/2, 20), "AttackCount:");
        stat.Count = EditorGUI.IntField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.Count);

        if (stat.Count == 0)
            return stat;

        if(stat.DurationTime==null|| stat.Count != stat.DurationTime.Length)
        {
            stat.DurationTime = new float[stat.Count];
            stat.CompleteTime = new float[stat.Count];
            stat.DamagePro = new float[stat.Count];
            stat.HitTime = new float[stat.Count];
            stat.Type = new EFindCharacterType[stat.Count];
            stat.Range = new float[stat.Count];
            stat.Angle = new float[stat.Count];
            stat.Width = new float[stat.Count];
            stat.MissileHandle = new int[stat.Count];
        }

        posY += 30;
        GUI.Label(new Rect(0, posY, windowSize/(stat.Count + 1), 20), "DurationTime:");
        for (int i =0; i<stat.Count; ++i)
            stat.DurationTime[i] = EditorGUI.FloatField(new Rect((i+1)*windowSize / (stat.Count+1), posY, windowSize / (stat.Count + 1), 20), stat.DurationTime[i]);
        posY += 20;
        GUI.Label(new Rect(0, posY, windowSize / (stat.Count + 1), 20), "CompleteTime:");
        for (int i = 0; i < stat.Count; ++i)
            stat.CompleteTime[i] = EditorGUI.FloatField(new Rect((i + 1) * windowSize / (stat.Count + 1), posY, windowSize / (stat.Count + 1), 20), stat.CompleteTime[i]);
        posY += 20;
        GUI.Label(new Rect(0, posY, windowSize / (stat.Count + 1), 20), "DamagePro:");
        for (int i = 0; i < stat.Count; ++i)
            stat.DamagePro[i] = EditorGUI.FloatField(new Rect((i + 1) * windowSize / (stat.Count + 1), posY, windowSize / (stat.Count + 1), 20), stat.DamagePro[i]);
        posY += 20;
        GUI.Label(new Rect(0, posY, windowSize / (stat.Count + 1), 20), "HitTime:");
        for (int i = 0; i < stat.Count; ++i)
            stat.HitTime[i] = EditorGUI.FloatField(new Rect((i + 1) * windowSize / (stat.Count + 1), posY, windowSize / (stat.Count + 1), 20), stat.HitTime[i]);
        posY += 20;
        GUI.Label(new Rect(0, posY, windowSize / (stat.Count + 1), 20), "FindType:");
        for (int i = 0; i < stat.Count; ++i)
            stat.Type[i] = (EFindCharacterType)EditorGUI.EnumPopup(new Rect((i + 1) * windowSize / (stat.Count + 1), posY, windowSize / (stat.Count + 1), 20), stat.Type[i]);
        posY += 20;
        GUI.Label(new Rect(0, posY, windowSize / (stat.Count + 1), 20), "Range:");
        for (int i = 0; i < stat.Count; ++i)
            stat.Range[i] = EditorGUI.FloatField(new Rect((i + 1) * windowSize / (stat.Count + 1), posY, windowSize / (stat.Count + 1), 20), stat.Range[i]);
        posY += 20;
        GUI.Label(new Rect(0, posY + 20, windowSize / (stat.Count + 1), 20), "Angle:");
        GUI.Label(new Rect(0, posY + 40, windowSize / (stat.Count + 1), 20), "MissileHandle:");
        GUI.Label(new Rect(0, posY + 60, windowSize / (stat.Count + 1), 20), "Width:");
        for (int i =0; i<stat.Count; ++i)
        {
            switch (stat.Type[i])
            {
                case EFindCharacterType.Sector:
                    stat.Angle[i] = EditorGUI.FloatField(new Rect((i + 1) * windowSize / (stat.Count + 1), posY + 20, windowSize / (stat.Count + 1), 20), stat.Angle[i]);
                    break;
                case EFindCharacterType.Missile:
                    stat.MissileHandle[i] = EditorGUI.IntField(new Rect((i + 1) * windowSize / (stat.Count + 1), posY + 40, windowSize / (stat.Count + 1), 20), stat.MissileHandle[i]);
                    break;
                case EFindCharacterType.Rectangle:
                    stat.Width[i] = EditorGUI.FloatField(new Rect((i + 1) * windowSize / (stat.Count + 1), posY + 60, windowSize / (stat.Count + 1), 20), stat.Width[i]);
                    break;
            }
        }

        return stat;
    }

    public void Reset()
    {

    }
}
