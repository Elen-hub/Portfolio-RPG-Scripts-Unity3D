using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Rendering.PostProcessing;

public class CameraMng : TSingleton<CameraMng>
{ 
    [Flags] public enum CameraStyle
    {
        None = 0,
        Player = 1,
        World = 2,
        UI = 4,
        Character = 8,
        // Minimap=16,
    }

    Dictionary<CameraStyle, BaseCamera> m_cameraDic = new Dictionary<CameraStyle, BaseCamera>();
    CameraStyle m_currCamera = CameraStyle.None;

    public static BaseCamera CurrentCamera;
    PostProcessProfile m_postProcessing;
    Bloom m_bloom;
    AmbientOcclusion m_ambient;
    public bool UseBloom
    {
        set { m_bloom.enabled.value = value; }
    }
    public bool UseAmbient
    {
        set { m_ambient.enabled.value = value; }
    }

    public override void Init()
    {
        CreateCamera(CameraStyle.Player);
        CreateCamera(CameraStyle.UI);
        CreateCamera(CameraStyle.World);
        CreateCamera(CameraStyle.Character);
        // CreateCamera(CameraStyle.Minimap);

        PostProcessVolume volume = gameObject.AddComponent<PostProcessVolume>();
        volume.isGlobal = true;
        volume.profile = PostProcessProfile.CreateInstance<PostProcessProfile>();
        m_postProcessing = volume.profile;

        m_bloom = Bloom.CreateInstance<Bloom>();
        m_postProcessing.AddSettings(m_bloom);
        m_bloom.intensity.value = 15;
        m_bloom.intensity.overrideState = true;
        m_bloom.enabled.value = GameSystem.UseBloom;

        m_ambient = AmbientOcclusion.CreateInstance<AmbientOcclusion>();
        m_postProcessing.AddSettings(m_ambient);
        m_ambient.intensity.value = 0.6f;
        m_ambient.intensity.overrideState = true;
        m_ambient.thicknessModifier.value = 5;
        m_ambient.thicknessModifier.overrideState = true;
        m_ambient.enabled.value = GameSystem.UseAmbient;

        gameObject.layer = LayerMask.NameToLayer("PostProcessing");

        IsLoad = true;
    }

    public void SetCamera(CameraStyle style)
    {
        if (m_currCamera == style)
            return;

        if ((style & CameraStyle.UI) != 0)
        {
            m_cameraDic[CameraStyle.UI].Enabled = true;
            CurrentCamera = m_cameraDic[CameraStyle.UI];
        } 
        if ((style & CameraStyle.World) != 0)
        {
            m_cameraDic[CameraStyle.World].Enabled = true;
            CurrentCamera = m_cameraDic[CameraStyle.World];
        }
        else m_cameraDic[CameraStyle.World].Enabled = false;
        if ((style & CameraStyle.Player) != 0)
        {
            m_cameraDic[CameraStyle.Player].Enabled = true;
            CurrentCamera = m_cameraDic[CameraStyle.Player];
        }
        else m_cameraDic[CameraStyle.Player].Enabled = false;

        m_cameraDic[CameraStyle.Character].Enabled = (style & CameraStyle.Character) != 0;
        // m_cameraDic[CameraStyle.Minimap].Enabled = (style & CameraStyle.Minimap) != 0;
        m_currCamera = style;
    }

    public T GetCamera<T>(CameraStyle style) where T : BaseCamera
    {
        return m_cameraDic[style] as T;
    }

    public BaseCamera GetCamera(CameraStyle CamStyle)
    {
        return m_cameraDic[CamStyle];
    }
     
    public void CreateCamera(CameraStyle CamStyle)
    {
        BaseCamera cam = Instantiate(Resources.Load<GameObject>("Camera/" + CamStyle.ToString() + " Camera")).GetComponent<BaseCamera>();
        cam.Init();
        m_cameraDic.Add(CamStyle, cam);
        cam.transform.SetParent(transform);
        cam.Enabled = false;
    }

    #region ShockWave

    public static void Shockwave(Camera camera, float x, float y)
    {
        CameraPlay_Shockwave CP = camera.gameObject.AddComponent<CameraPlay_Shockwave>() as CameraPlay_Shockwave;
        CP.PosX = x;
        CP.PosY = y;
        CP.Duration = 1;
    }

    public static void Shockwave(Camera camera, Vector3 Pos)
    {
        CameraPlay_Shockwave CP = camera.gameObject.AddComponent<CameraPlay_Shockwave>() as CameraPlay_Shockwave;
        CP.PosX = Pos.x;
        CP.PosY = Pos.y;
        CP.Duration = 1.5f;
    }

    public static void Shockwave(Camera camera, float x, float y, float time)
    {
        CameraPlay_Shockwave CP = camera.gameObject.AddComponent<CameraPlay_Shockwave>() as CameraPlay_Shockwave;
        CP.PosX = x;
        CP.PosY = y;
        CP.Duration = time;
    }

    public static void Shockwave(float x, float y, float time, float size)
    {
        CameraPlay_Shockwave CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Shockwave>() as CameraPlay_Shockwave;
        CP.PosX = x;
        CP.PosY = y;
        CP.Duration = time;
        CP.Size = size;
    }

    #endregion

    #region Hit

    public static void Hit(float Time)
    {
        CameraPlay_Hit CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Hit>() as CameraPlay_Hit;
        CP.Duration = Time;
        CP.HitColor = Color.red;
    }

    public static void Hit(float Time, Camera cam)
    {
        CameraPlay_Hit CP = cam.gameObject.AddComponent<CameraPlay_Hit>() as CameraPlay_Hit;
        CP.Duration = Time;
        CP.HitColor = Color.red;
    }

    public void PingPongHit(float Time, float IntervalTime, float ContinuousTime)
    {
        StartCoroutine(IEPingPongHit(Time, IntervalTime, ContinuousTime));
    }

    public IEnumerator IEPingPongHit(float Time, float IntervalTime, float ContinuousTime)
    {
        bool isEnd = false;
        float elapsedTime = 0;

        while (!isEnd)
        {
            Hit(Time);
            elapsedTime += IntervalTime;

            if (ContinuousTime <= elapsedTime)
            {
                isEnd = true;
                yield return null;
            }

            yield return new WaitForSeconds(IntervalTime);
        }

        yield return null;
    }

    public static void Hit(Color col, float Time)
    {
        CameraPlay_Hit CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Hit>() as CameraPlay_Hit;
        CP.Duration = Time;
        CP.HitColor = col;
    }

    public static void Hit(Color col)
    {
        CameraPlay_Hit CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Hit>() as CameraPlay_Hit;
        CP.Duration = 1;
        CP.HitColor = col;
    }

    #endregion

    #region Earth  Quake

    public static void EarthQuakeShake(Camera camera, float duration, float Speed, float Size)
    {
        CameraPlay_Shake CP = camera.gameObject.AddComponent<CameraPlay_Shake>() as CameraPlay_Shake;
        CP.Duration = duration;
        CP.Speed = Speed;
        CP.Size = Size;
    }

    #endregion

    #region Bullet

    public static void BulletHole(Vector3 Pos, float Time)
    {
        CameraPlay_BulletHole CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_BulletHole>() as CameraPlay_BulletHole;
        CP.Duration = Time;
        CP.PosX = Pos.x;
        CP.PosY = Pos.y;
        CP.Distortion = 4;
    }

    public static void BulletHole(float Time)
    {
        CameraPlay_BulletHole CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_BulletHole>() as CameraPlay_BulletHole;
        CP.Duration = Time;
        CP.PosX = Random.Range(0.1f, 0.9f);
        CP.PosY = Random.Range(0.1f, 0.9f);
        CP.Distortion = 1;
    }

    public static void BulletHole()
    {
        CameraPlay_BulletHole CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_BulletHole>() as CameraPlay_BulletHole;
        CP.Duration = 4;
        CP.PosX = Random.Range(0.1f, 0.9f);
        CP.PosY = Random.Range(0.1f, 0.9f);
        CP.Distortion = 1;
    }

    public static void BulletHole(float Time, float dist)
    {
        CameraPlay_BulletHole CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_BulletHole>() as CameraPlay_BulletHole;
        CP.Duration = Time;
        CP.PosX = Random.Range(0.1f, 0.9f);
        CP.PosY = Random.Range(0.1f, 0.9f);
        CP.Distortion = dist;
    }

    public static void BulletHole(float sx, float sy, float Time, float dist)
    {
        CameraPlay_BulletHole CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_BulletHole>() as CameraPlay_BulletHole;
        CP.Duration = Time;
        CP.PosX = sx;
        CP.PosY = sy;
        CP.Distortion = dist;
    }

    public static void BulletHole(float sx, float sy, float Time)
    {
        CameraPlay_BulletHole CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_BulletHole>() as CameraPlay_BulletHole;
        CP.Duration = Time;
        CP.PosX = sx;
        CP.PosY = sy;
        CP.Distortion = 1;
    }

    #endregion

    #region Plash

    public static void MangaFlash(float Time)
    {
        CameraPlay_MangaFlash CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_MangaFlash>() as CameraPlay_MangaFlash;
        CP.Duration = Time;
    }

    public static void MangaFlash()
    {
        CameraPlay_MangaFlash CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_MangaFlash>() as CameraPlay_MangaFlash;
        CP.PosX = 0.5f;
        CP.PosY = 0.5f;
        CP.Duration = 4;
    }

    public static void MangaFlash(float sx, float sy, float Time, int SpeedFPS, Color color)
    {
        CameraPlay_MangaFlash CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_MangaFlash>() as CameraPlay_MangaFlash;
        CP.Duration = Time;
        CP.PosX = sx;
        CP.PosY = sy;
        CP.Speed = SpeedFPS;
        CP.Color = color;
    }

    public static void MangaFlash(Vector3 Pos, float Time)
    {
        CameraPlay_MangaFlash CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_MangaFlash>() as CameraPlay_MangaFlash;
        CP.Duration = Time;

        Vector3 ScreenPos = PosScreen(Pos);
        CP.PosX = ScreenPos.x;
        CP.PosY = ScreenPos.y;
    }

    public static void MangaFlash(Vector3 Pos)
    {
        CameraPlay_MangaFlash CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_MangaFlash>() as CameraPlay_MangaFlash;
        CP.Duration = 4;

        Vector3 ScreenPos = PosScreen(Pos);
        CP.PosX = ScreenPos.x;
        CP.PosY = ScreenPos.y;
        CP.Speed = 5;
        CP.Color = Color.white;
    }

    #endregion

    #region Pitch

    public static void Pitch(float Time)
    {
        CameraPlay_Pitch CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Pitch>() as CameraPlay_Pitch;
        CP.Duration = Time;
        CP.PosX = 0.5f;
        CP.PosY = 0.5f;
        CP.Distortion = 1;
    }

    public static void Pitch()
    {
        CameraPlay_Pitch CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Pitch>() as CameraPlay_Pitch;
        CP.Duration = 4;
        CP.PosX = 0.5f;
        CP.PosY = 0.5f;
        CP.Distortion = 1;
    }

    public static void Pitch(float Time, float dist)
    {
        CameraPlay_Pitch CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Pitch>() as CameraPlay_Pitch;
        CP.Duration = Time;
        CP.PosX = 0.5f;
        CP.PosY = 0.5f;
        CP.Distortion = dist;
    }

    public static void Pitch(Vector3 Pos, float Time)
    {
        CameraPlay_Pitch CP = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Pitch>() as CameraPlay_Pitch;
        CP.Duration = Time;

        Vector3 ScreenPos = PosScreen(Pos);
        CP.PosX = ScreenPos.x;
        CP.PosY = ScreenPos.y;
        CP.Distortion = 1;
    }

    #endregion

    #region WaterDrop

    private static CameraPlay_RainDrop CamRainDrop;

    public static void RainDrop_ON(float Time)
    {
        if (CamRainDrop != null) return;
        if (CurrentCamera.RainDrop_Switch) return;
        CamRainDrop = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_RainDrop>() as CameraPlay_RainDrop;
        if (CamRainDrop.CamTurnOff) return;
        CurrentCamera.RainDrop_Switch = true;
        CamRainDrop.Duration = Time;
        CamRainDrop.Zoom = 0.2f;
        CamRainDrop.Distortion = 1;
    }

    public static void RainDrop_ON()
    {
        if (CamRainDrop != null) return;
        if (CurrentCamera.RainDrop_Switch) return;
        CamRainDrop = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_RainDrop>() as CameraPlay_RainDrop;
        if (CamRainDrop.CamTurnOff) return;
        CurrentCamera.RainDrop_Switch = true;
        CamRainDrop.Zoom = 0.2f;
        CamRainDrop.Distortion = 1;
        CamRainDrop.Duration = 1;
    }

    public static void RainDrop_OFF(float Time)
    {
        if (CamRainDrop == null) return;
        if (!CurrentCamera.RainDrop_Switch) return;
        CamRainDrop.Duration = Time;
        CamRainDrop.CamTurnOff = true;
    }

    public static void RainDrop_OFF()
    {
        if (CamRainDrop == null) return;
        if (!CurrentCamera.RainDrop_Switch) return;
        CamRainDrop.Duration = 1;
        CamRainDrop.flashy = 1;
        CamRainDrop.CamTurnOff = true;
    }

    #endregion

    #region Fade IO

    public static CameraPlay_Fade CamFade;
    public static CameraPlay_Fade UICamFade;

    public static void Fade_ON()
    {
        if (CamFade != null) return;
        if (CurrentCamera.Fade_Switch) return;
        PlayerCamera cam = CurrentCamera as PlayerCamera;
        if (cam != null)
        {
            CameraPlay_Fade fade = cam.UICamera.gameObject.AddComponent<CameraPlay_Fade>();
            fade.Duration = 0;
        }
        CamFade = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Fade>() as CameraPlay_Fade;
        UICamFade = Instance.m_cameraDic[CameraStyle.UI].gameObject.AddComponent<CameraPlay_Fade>() as CameraPlay_Fade;
        if (CamFade.CamTurnOff) return;
        CurrentCamera.Fade_Switch = true;
        CamFade.Duration = 0;
        UICamFade.Duration = 0;
    }

    public static void Fade_ON(float time)
    {
        if (CamFade != null) return;
        if (CurrentCamera.Fade_Switch) return;
        PlayerCamera cam = CurrentCamera as PlayerCamera;
        if (cam != null)
        {
            CameraPlay_Fade fade = cam.UICamera.gameObject.AddComponent<CameraPlay_Fade>();
            fade.Duration = time;
        }
        CamFade = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Fade>() as CameraPlay_Fade;
        UICamFade = Instance.m_cameraDic[CameraStyle.UI].gameObject.AddComponent<CameraPlay_Fade>() as CameraPlay_Fade;
        if (CamFade.CamTurnOff) return;
        CurrentCamera.Fade_Switch = true;
        CamFade.Duration = time;
        UICamFade.Duration = time;
    }

    public static void Fade_ON(Color col, float time)
    {
        if (CamFade != null) return;
        if (CurrentCamera.Fade_Switch) return;
        PlayerCamera cam = CurrentCamera as PlayerCamera;
        if (cam != null)
        {
            CameraPlay_Fade fade = cam.UICamera.gameObject.AddComponent<CameraPlay_Fade>();
            fade.Duration = time;
            fade.ColorFade = col;
        }
        CamFade = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Fade>() as CameraPlay_Fade;
        if (CamFade.CamTurnOff) return;
        CurrentCamera.Fade_Switch = true;
        CamFade.ColorFade = col;
        CamFade.Duration = time;
    }

    public static void Fade_ON(Color col)
    {
        if (CamFade != null) return;
        if (CurrentCamera.Fade_Switch) return;
        PlayerCamera cam = CurrentCamera as PlayerCamera;
        if (cam != null)
        {
            CameraPlay_Fade fade = cam.UICamera.gameObject.AddComponent<CameraPlay_Fade>();
            fade.Duration = 1;
            fade.ColorFade = col;
        }
        CamFade = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Fade>() as CameraPlay_Fade;
        if (CamFade.CamTurnOff) return;
        CurrentCamera.Fade_Switch = true;
        CamFade.ColorFade = col;
        CamFade.Duration = 1;
    }

    public static void Fade_OFF()
    {
        if (CamFade == null) return;
        if (!CurrentCamera.Fade_Switch) return;
        PlayerCamera cam = CurrentCamera as PlayerCamera;
        if (cam != null)
        {
            CameraPlay_Fade fade = cam.UICamera.GetComponent<CameraPlay_Fade>();
            fade.Duration = 1;
            fade.CamTurnOff = true;
        }
        UICamFade.Duration = 1;
        UICamFade.CamTurnOff = true;
        CamFade.Duration = 1;
        CamFade.CamTurnOff = true;
    }

    public static void Fade_OFF(float time)
    {
        if (CamFade == null) return;
        if (!CurrentCamera.Fade_Switch) return;
        PlayerCamera cam = CurrentCamera as PlayerCamera;
        if (cam != null)
        {
            CameraPlay_Fade fade = cam.UICamera.GetComponent<CameraPlay_Fade>();
            fade.Duration = time;
            fade.CamTurnOff = true;
        }
        UICamFade.Duration = time;
        UICamFade.CamTurnOff = true;
        CamFade.Duration = time;
        CamFade.CamTurnOff = true;
    }

    public static void Fade_OFF(Camera cam, float time)
    {
        if (CamFade == null) return;
        if (!CurrentCamera.Fade_Switch) return;
        PlayerCamera cam2 = cam.GetComponent<PlayerCamera>();
        if (cam != null)
        {
            CameraPlay_Fade fade = cam2.UICamera.GetComponent<CameraPlay_Fade>();
            fade.Duration = time;
            fade.CamTurnOff = true;
        }
        CamFade = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Fade>() as CameraPlay_Fade;
        CamFade.Duration = time;
        CamFade.CamTurnOff = true;
    }

    #endregion

    #region PosTransformation

    public static Vector3 PosScreen(Vector3 Pos)
    {
        Pos = CurrentCamera.camera.WorldToScreenPoint(Pos);

        float x = Pos.x / Screen.width;
        float y = Pos.y / Screen.height;

        return new Vector3(x, y, 0);
    }

    public static float PosScreenX(Vector3 Pos)
    {
        Pos = CurrentCamera.camera.WorldToScreenPoint(Pos);
        float x = Pos.x / Screen.width;
        return x;
    }

    public static float PosScreenY(Vector3 Pos)
    {
        Pos = CurrentCamera.camera.WorldToScreenPoint(Pos);
        float y = Pos.y / Screen.height;
        return y;
    }

    #endregion

    #region Infrared

    private static CameraPlay_Infrared CamInfrared;

    public static void Infrared_ON()
    {
        if (CamInfrared != null) return;
        if (CurrentCamera.Infrared_Switch) return;
        CamInfrared = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Infrared>() as CameraPlay_Infrared;
        if (CamInfrared.CamTurnOff) return;
        CurrentCamera.Infrared_Switch = true;
        CamInfrared.Duration = 1;
    }

    public static void Infrared_ON(float time)
    {
        if (CamInfrared != null) return;
        if (CurrentCamera.Infrared_Switch) return;
        CamInfrared = CurrentCamera.camera.gameObject.AddComponent<CameraPlay_Infrared>() as CameraPlay_Infrared;
        if (CamInfrared.CamTurnOff) return;
        CurrentCamera.Infrared_Switch = true;
        CamInfrared.Duration = time;
    }

    public static void Infrared_OFF()
    {
        if (CamInfrared == null) return;
        if (!CurrentCamera.Infrared_Switch) return;
        CamInfrared.Duration = 1;
        CamInfrared.CamTurnOff = true;
    }

    public static void Infrared_OFF(float time)
    {
        if (CamInfrared == null) return;
        if (!CurrentCamera.Infrared_Switch) return;
        CamInfrared.Duration = time;
        CamInfrared.CamTurnOff = true;
    }

    #endregion
}
