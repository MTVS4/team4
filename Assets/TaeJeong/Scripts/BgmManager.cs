using UnityEngine;
using UnityEngine.UI;

public class BgmManager : MonoBehaviour
{
    public static BgmManager instance;
    private AudioSource audioSource;
    [SerializeField] private Slider volumeSlider;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
           
        }
        else
        {
            Destroy(gameObject);
        }
        
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = BgmManager.instance.GetVolume(); // 시작값 초기화
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }
    
    private void OnVolumeChanged(float value)
    {
        BgmManager.instance.SetVolume(value);
    }
    
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public float GetVolume()
    {
        return audioSource.volume;
    }

}
