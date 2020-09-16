using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
public class BuildMenu
{
    [MenuItem("Build/Occlusion")]
    static void BakeOcclusion()
    {
        GameObject[] objs = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        for (int i = 0; i < objs.Length; ++i)
        {
            if (objs[i].layer == LayerMask.NameToLayer("Object") || objs[i].layer == LayerMask.NameToLayer("Particle") || objs[i].layer == LayerMask.NameToLayer("Road"))
            {
                StaticEditorFlags flag = GameObjectUtility.GetStaticEditorFlags(objs[i]);
                GameObjectUtility.SetStaticEditorFlags(objs[i], flag | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.BatchingStatic);
            }
        }

        StaticOcclusionCulling.smallestOccluder = 1.5f;
        StaticOcclusionCulling.smallestHole = 0.15f;
        StaticOcclusionCulling.backfaceThreshold = 100;
        StaticOcclusionCulling.Compute();
    }
    [MenuItem("Build/NavMesh")]
    static void BakeNavMesh()
    {
        Transform trs = GameObject.Find("NotWalkAbledObject").transform;
        for (int i = 0; i < trs.childCount; ++i)
        {
            GameObject obj = trs.GetChild(i).gameObject;
            StaticEditorFlags flag = GameObjectUtility.GetStaticEditorFlags(obj);
            GameObjectUtility.SetStaticEditorFlags(obj, flag | StaticEditorFlags.NavigationStatic);
            GameObjectUtility.SetNavMeshArea(obj, 1);
            for (int j = 0; j < obj.transform.childCount; ++j)
            {
                GameObject obj2 = obj.transform.GetChild(j).gameObject;
                flag = GameObjectUtility.GetStaticEditorFlags(obj2);
                GameObjectUtility.SetStaticEditorFlags(obj2, flag | StaticEditorFlags.NavigationStatic);
                GameObjectUtility.SetNavMeshArea(obj2, 1);
            }
        }
        trs = GameObject.Find("WalkAbledObject").transform;
        for (int i = 0; i < trs.childCount; ++i)
        {
            GameObject obj = trs.GetChild(i).gameObject;
            StaticEditorFlags flag = GameObjectUtility.GetStaticEditorFlags(obj);
            GameObjectUtility.SetStaticEditorFlags(obj, flag | StaticEditorFlags.NavigationStatic | StaticEditorFlags.OffMeshLinkGeneration);
            GameObjectUtility.SetNavMeshArea(obj, 0);
        }

        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
    }
    [MenuItem("Build/LightMap")]
    static void BakeLightMap()
    {
        GameObject[] objs = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];

        for (int i = 0; i < objs.Length; ++i)
        {
            if (objs[i].layer == LayerMask.NameToLayer("Object") || objs[i].layer == LayerMask.NameToLayer("Road"))
            {
                StaticEditorFlags flag = GameObjectUtility.GetStaticEditorFlags(objs[i]);
                GameObjectUtility.SetStaticEditorFlags(objs[i], flag | StaticEditorFlags.ContributeGI | StaticEditorFlags.ReflectionProbeStatic);
            }
        }

        UnityEditor.Lightmapping.realtimeGI = false;
        UnityEditor.Lightmapping.bakedGI = true;
        UnityEditor.LightmapEditorSettings.mixedBakeMode = MixedLightingMode.Subtractive;

        UnityEditor.LightmapEditorSettings.lightmapper = LightmapEditorSettings.Lightmapper.ProgressiveGPU;
        UnityEditor.LightmapEditorSettings.prioritizeView = true;
        UnityEditor.LightmapEditorSettings.directSampleCount = 10;
        UnityEditor.LightmapEditorSettings.indirectSampleCount = 100;
        UnityEditor.LightmapEditorSettings.bounces = 2;
        UnityEditor.LightmapEditorSettings.filteringMode = LightmapEditorSettings.FilterMode.Auto;

        UnityEditor.LightmapEditorSettings.bakeResolution = 20;
        UnityEditor.LightmapEditorSettings.padding = 2;
        UnityEditor.LightmapEditorSettings.maxAtlasSize = 256;
        UnityEditor.LightmapEditorSettings.textureCompression = true;
        UnityEditor.LightmapEditorSettings.enableAmbientOcclusion = false;

        UnityEditor.Lightmapping.BakeAsync();
    }
    [MenuItem("Build/AssetBundle")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundle_BuildComplete";
        if (!Directory.Exists(assetBundleDirectory))
            Directory.CreateDirectory(assetBundleDirectory);

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.Android);
    }
}
