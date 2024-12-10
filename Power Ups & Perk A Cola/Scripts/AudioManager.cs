using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audioSource;
    public AudioClip gunShot;
    public AudioClip roundStart;
    public AudioClip zombieAproach;

    void Awake()
    {
        // Đảm bảo chỉ có một instance của AudioManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ AudioManager tồn tại qua các scene
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Phương thức để phát âm thanh
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioClip không được gán!");
        }
    }
    
    public void PlayGunShotSound()
    {
        if (gunShot != null)
        {
            audioSource.PlayOneShot(gunShot);
        }
        else
        {
            Debug.LogWarning("AudioClip không được gán!");
        }
    }

    public void PlayRoundStartSound()
    {
        if (roundStart != null)
        {
            audioSource.PlayOneShot(roundStart);
        }
        else
        {
            Debug.LogWarning("AudioClip không được gán!");
        }
    }
}
