using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Header("BGM")]
    public AudioClip[] bgm;
    public float bgmVolume;
    AudioSource bgmPlayer;
    
    [Header("SFX")]
    public AudioClip[] sfx;
    public float sfxVolume;
    AudioSource sfxPlayer;



    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(instance);
        
    }
}
