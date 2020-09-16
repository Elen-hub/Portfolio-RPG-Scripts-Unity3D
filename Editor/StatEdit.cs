using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StatEdit : EditorWindow
{
    public Texture2D IconTexture;

    public Stat InputInformation(ref int posY, Stat stat, float windowSize, GUIStyle guiStyle, string path)
    {
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "Name", guiStyle);
        stat.Name = EditorGUI.TextField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.Name);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "Explanation", guiStyle);
        stat.Explanation = EditorGUI.TextField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.Explanation);
        posY += 20;
        // iconTexture
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "Icon", guiStyle);
        IconTexture = (Texture2D)EditorGUI.ObjectField(new Rect(0, posY + 20, 100, 100), IconTexture, typeof(Texture2D), false);
        if (IconTexture != null)
            stat.Icon = path + IconTexture.name;
        posY += 130;
        return stat;
    }
    public Stat InputHeroStat(ref int posY, Stat stat, float windowSize, GUIStyle guiStyle)
    {
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "Character Class", guiStyle);
        stat.Class = (ECharacterClass)EditorGUI.EnumPopup(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.Class);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "Awakening", guiStyle);
        stat.Awakening = EditorGUI.IntField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.Awakening);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "STR", guiStyle);
        stat.STR = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.STR);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "DEX", guiStyle);
        stat.DEX = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.DEX);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "INT", guiStyle);
        stat.INT = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.INT);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "WIS", guiStyle);
        stat.WIS = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.WIS);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "CON", guiStyle);
        stat.CON = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.CON);
        posY += 30;
        return stat;
    }
    public Stat InputStat(ref int posY, Stat stat, float windowSize, GUIStyle guiStyle)
    {
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "HP", guiStyle);
        stat.HP = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.HP);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "RecoveryHP", guiStyle);
        stat.RecoveryHP = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.RecoveryHP);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "MP", guiStyle);
        stat.MP = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.MP);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "RecoveryMP", guiStyle);
        stat.RecoveryMP = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.RecoveryMP);
        posY += 30;

        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "AttackDamage", guiStyle);
        stat.AttackDamage = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.AttackDamage);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "AttackDamagePro", guiStyle);
        stat.AttackDamagePro = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.AttackDamagePro);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "CriticalPro", guiStyle);
        stat.CriticalPro = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.CriticalPro);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "CriticalDamage", guiStyle);
        stat.CriticalDamage = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.CriticalDamage);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "SkillDamagePro", guiStyle);
        stat.SkillDamagePro = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.SkillDamagePro);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "CoolTime", guiStyle);
        stat.CoolTime = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.CoolTime);
        posY += 30;

        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "Defence", guiStyle);
        stat.Defence = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.Defence);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "DefencePro", guiStyle);
        stat.DefencePro = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.DefencePro);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "Resistance", guiStyle);
        stat.Resistance = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.Resistance);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "MoveSpeed", guiStyle);
        stat.MoveSpeed = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.MoveSpeed);
        posY += 20;
        EditorGUI.LabelField(new Rect(0, posY, windowSize / 2, 20), "MoveSpeedPro", guiStyle);
        stat.MoveSpeedPro = EditorGUI.FloatField(new Rect(windowSize / 2, posY, windowSize / 2, 20), stat.MoveSpeedPro);
        posY += 20;

        return stat;
    }
    public void Reset()
    {
        IconTexture = null;
    }
}
