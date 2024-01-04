using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    BGM = 0,
    block_break = 1,
    block_knock = 2,
    character_voice_01 = 3,
    character_voice_02 = 4,
    chatacter_death = 5,
    fever_voice_01 = 6,
    fever_voice_02 = 7,
    fever_voice_03 = 8,
    item = 9,
    wall_knock = 10,
}

public class SoundMgr : SingletonComponentBase<SoundMgr>
{
    public static bool isMute = false;

    public AudioClip defultBgm;
    public List<AudioClip> bgmClipList = new List<AudioClip>();
    public List<AudioClip> fxClipList = new List<AudioClip>();

    private Dictionary<string, AudioClip> bgmDictionay = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> fxDictionay = new Dictionary<string, AudioClip>();

    private Transform bgmContainer;
    private Transform fxContainer;

    private List<AudioSource> playSoundList = new List<AudioSource>();

    public SoundObj SoundComponent;

    protected override void InitializeSingleton(){}
    private void Awake()
    {
        BgmInit();
        FxInit();

        PlayDefultBgm();
    }

    public void BgmInit()
    {
        bgmContainer = new GameObject().transform;
        bgmContainer.name = "BgmContainer";
        bgmContainer.SetParent(transform);

        AudioSource bgm = bgmContainer.gameObject.AddComponent<AudioSource>();
        bgm.loop = true;
        bgm.volume = isMute == true ? 0.0f : 0.5f; ;

        foreach (var bgmClip in bgmClipList)
        {
            bgmDictionay.Add(bgmClip.name, bgmClip);
        }
    }
    public void FxInit()
    {
        fxContainer = new GameObject().transform;
        fxContainer.name = "FxContainer";
        fxContainer.SetParent(transform);

        foreach (var fxClip in fxClipList)
        {
            fxDictionay.Add(fxClip.name, fxClip);
        }
    }

    public void PlayDefultBgm()
    {
        AudioSource bgm = bgmContainer.GetComponent<AudioSource>();
        playSoundList.Add(bgm);

        bgm.clip = defultBgm;
        bgm.Play();
    }
    public void ChangeBgm(string bgmName)
    {
        if (bgmDictionay.TryGetValue(bgmName, out var clip))
        {
            AudioSource bgm = bgmContainer.GetComponent<AudioSource>();

            bgm.clip = clip;
            bgm.Play();
        }
    }
    public void ChangeBgm(SoundType bgmType)
    {
        string name = SoundTypeToName(bgmType);
        ChangeBgm(name);
    }

    public void PlayFx(string fxName, bool isLoop = false)
    {
        SoundObj fxObj = ObjectPoolMgr.Instance.Load<SoundObj>(PoolObjectType.Effect, "SoundComponent");
        fxObj.name = fxName;
        fxObj.transform.SetParent(fxContainer);

        playSoundList.Add(fxObj.audioSource);

        if(fxDictionay.TryGetValue (fxName, out var clip))
        {
            fxObj.Init(clip, isLoop);
            fxObj.Play();
        }
    }
    public void PlayFx(SoundType fxType, bool isLoop = false)
    {
        PlayFx(SoundTypeToName(fxType), isLoop);
    }

    public void SetMute(bool show)
    {
        isMute = show;

        if (show)
        {
            foreach(var audio in playSoundList)
            {
                audio.volume = 0.0f;
            }
        }
        else
        {
            foreach (var audio in playSoundList)
            {
                audio.volume = 0.5f;
            }
        }
    }

    public string SoundTypeToName(SoundType type)
    {
        switch (type)
        {
            case SoundType.BGM: return "BGM";
            case SoundType.block_break: return "block_break";
            case SoundType.block_knock: return "block_knock";
            case SoundType.character_voice_01: return "character_voice_01";
            case SoundType.chatacter_death: return "chatacter_death";
            case SoundType.fever_voice_01: return "fever_voice_01";
            case SoundType.fever_voice_02: return "fever_voice_02";
            case SoundType.fever_voice_03: return "fever_voice_03";
            case SoundType.item: return "item";
            case SoundType.wall_knock: return "wall_knock";
            default: return string.Empty;
        }
    }
}
