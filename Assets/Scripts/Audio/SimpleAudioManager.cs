using UnityEngine;

public class SimpleAudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip _attackSound;
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private AudioClip _buildSound;
    [SerializeField] private AudioClip _abilitySound;
    [SerializeField] private AudioClip _backgroundMusic;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _musicSource;
    
    private void Start()
    {
        if (_backgroundMusic != null)
        {
            _musicSource.clip = _backgroundMusic;
            _musicSource.loop = true;
            _musicSource.Play();
        }
        
        EventManager.StartListening("UnitDamaged", OnUnitDamaged);
        EventManager.StartListening("UnitDied", OnUnitDied);
        EventManager.StartListening("BuildingCompleted", OnBuildingCompleted);
        EventManager.StartListening("AbilityUsed", OnAbilityUsed);
    }
    
    private void OnDestroy()
    {
        EventManager.StopListening("UnitDamaged", OnUnitDamaged);
        EventManager.StopListening("UnitDied", OnUnitDied);
        EventManager.StopListening("BuildingCompleted", OnBuildingCompleted);
        EventManager.StopListening("AbilityUsed", OnAbilityUsed);
    }
    
    private void OnUnitDamaged(object data)
    {
        PlaySFX(_attackSound);
    }
    
    private void OnUnitDied(object data)
    {
        PlaySFX(_deathSound);
    }
    
    private void OnBuildingCompleted(object data)
    {
        PlaySFX(_buildSound);
    }
    private void OnAbilityUsed(object data)
    {
        PlaySFX(_abilitySound);
    }
    
    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && _sfxSource != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
    }
}
