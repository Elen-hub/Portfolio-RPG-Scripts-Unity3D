using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMng : TSingleton<SoundMng>
{
    bool m_isFade;
    AudioSource m_mainSource;
    AudioClip m_currBGM;
    Dictionary<string, AudioClip> m_musicDic = new Dictionary<string, AudioClip>();
    public override void Init()
    {
        m_mainSource = gameObject.AddComponent<AudioSource>();
        m_mainSource.loop = true;
        m_mainSource.playOnAwake = false;
        IsLoad = true;
    }
    public void PlayMainMusic(string music)
    {
        if (!GameSystem.UseMusic)
            return;

        if (!m_musicDic.ContainsKey(music))
            m_musicDic.Add(music, Resources.Load<AudioClip>("Sound/BGM/" + music));

        if (m_currBGM != m_musicDic[music])
        {
            m_currBGM = m_musicDic[music];
            m_mainSource.clip = m_musicDic[music];
            m_mainSource.Play();
        }

    }
    public void PlayMainMusic(string music, float FadeTime)
    {
        if (!GameSystem.UseMusic)
            return;

        if (!m_musicDic.ContainsKey(music))
            m_musicDic.Add(music, Resources.Load<AudioClip>("Sound/BGM/" + music));

        if (m_currBGM != m_musicDic[music])
        {
            m_currBGM = m_musicDic[music];
            m_mainSource.volume = 0f;
            m_mainSource.clip = m_musicDic[music];
            m_mainSource.Play();
            StartCoroutine(FadeAudio(FadeTime, true));
        }
    }
    public void StopMainMusic()
    {
        m_mainSource.Stop();
    }
    public void StopMainMusic(float FadeTime)
    {
        StartCoroutine(FadeAudio(FadeTime, false));
    }
    private void LateUpdate()
    {
        m_mainSource.mute = !GameSystem.UseMusic;
        if(!m_isFade)
            m_mainSource.volume = GameSystem.MusicVolume;
    }
    IEnumerator FadeAudio(float Time, bool isStart)
    {
        m_isFade = true;
        if (isStart)
        {
            for (int i = 1; i < Time * 51; ++i)
            {
                m_mainSource.volume = Mathf.Lerp(0, GameSystem.MusicVolume, i / (Time*51));
                yield return new WaitForSeconds(0.02f);
            }

            m_mainSource.volume = GameSystem.MusicVolume;
        }
        else
        {
            for (int i = 0; i < Time * 51; ++i)
            {
                m_mainSource.volume = Mathf.Lerp(GameSystem.MusicVolume, 0, i / (Time * 51));
                yield return new WaitForSeconds(0.02f);
            }

            m_mainSource.Stop();
            m_mainSource.volume = GameSystem.MusicVolume;
        }
        m_isFade = false;
        yield return null;
    }
}
