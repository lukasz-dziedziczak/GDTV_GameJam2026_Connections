using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] musicClips;

    int currentClipIndex = 0;

    private void Start()
    {
        if (musicClips.Length > 0)
        {
            audioSource.clip = musicClips[currentClipIndex];
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (!audioSource.isPlaying && musicClips.Length > 0)
        {
            currentClipIndex = (currentClipIndex + 1) % musicClips.Length;
            audioSource.clip = musicClips[currentClipIndex];
            audioSource.Play();
        }
    }
}
