using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager instance;
    
    private AudioSource[] audioSource;

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
        
        audioSource = GetComponents<AudioSource>();

    }

    public void PlayPickSfx(AudioClip clip)
    {
        audioSource[0].PlayOneShot(clip);
    }
}
