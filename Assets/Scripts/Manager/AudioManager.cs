using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("�������")]
    public AudioSource bgmSource;

    [Header("ȿ����")]
    public AudioSource sfxSource;

    [Header("ȿ���� ����Ʈ")]
    public AudioClip clickSound;
    public AudioClip attackSound;
    public AudioClip hitSound;

    void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource == null || clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // ���� �Լ� ����
    public void PlayClickSound() => PlaySFX(clickSound);
    public void PlayAttackSound() => PlaySFX(attackSound);
    public void PlayHitSound() => PlaySFX(hitSound);
}
