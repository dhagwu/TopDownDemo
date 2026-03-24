using UnityEngine;
using UnityEngine.Audio;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string bgmVolumeParam = "BGMVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM")]
    [SerializeField] private AudioClip levelBgm;

    [Header("SFX")]
    [SerializeField] private AudioClip playerAttackClip;
    [SerializeField] private AudioClip playerHitClip;
    [SerializeField] private AudioClip enemyDeathClip;

    [Header("Default Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float bgmVolume01 = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume01 = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ApplyDefaultVolumes();

        if (levelBgm != null)
            PlayBgm(levelBgm);
    }

    public void PlayBgm(AudioClip clip)
    {
        if (bgmSource == null || clip == null)
            return;

        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBgm()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }

    public void PlayPlayerAttack()
    {
        PlaySfx(playerAttackClip);
    }

    public void PlayPlayerHit()
    {
        PlaySfx(playerHitClip);
    }

    public void PlayEnemyDeath()
    {
        PlaySfx(enemyDeathClip);
    }

    public void PlaySfx(AudioClip clip, float volumeScale = 1f)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip, volumeScale);
    }

    public void SetBgmVolume01(float value)
    {
        bgmVolume01 = Mathf.Clamp01(value);

        if (mixer != null)
            mixer.SetFloat(bgmVolumeParam, Linear01ToDb(bgmVolume01));
        else if (bgmSource != null)
            bgmSource.volume = bgmVolume01;
    }

    public void SetSfxVolume01(float value)
    {
        sfxVolume01 = Mathf.Clamp01(value);

        if (mixer != null)
            mixer.SetFloat(sfxVolumeParam, Linear01ToDb(sfxVolume01));
        else if (sfxSource != null)
            sfxSource.volume = sfxVolume01;
    }

    private void ApplyDefaultVolumes()
    {
        SetBgmVolume01(bgmVolume01);
        SetSfxVolume01(sfxVolume01);
    }

    private float Linear01ToDb(float value)
    {
        if (value <= 0.0001f)
            return -80f;

        return Mathf.Log10(value) * 20f;
    }
}