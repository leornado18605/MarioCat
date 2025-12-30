using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip background;
    public AudioClip checkpoint;
    public AudioClip death;
    public AudioClip jump;
    public AudioClip hit; // khi nhận damage (chưa chết)

    private void Awake()
    {
        if (I && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic(background);
    }

    /* ===== MUSIC ===== */
    public void PlayMusic(AudioClip clip)
    {
        if (!musicSource || !clip) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    /* ===== SFX ===== */
    public void PlaySFX(AudioClip clip)
    {
        if (!sfxSource || !clip) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayJump()       => PlaySFX(jump);
    public void PlayCheckpoint() => PlaySFX(checkpoint);
    public void PlayDeath()      => PlaySFX(death);
    public void PlayHit()        => PlaySFX(hit);
}