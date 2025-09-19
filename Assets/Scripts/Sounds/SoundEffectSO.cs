using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffectSO", menuName = "Sounds/SoundEffectSO")]
public class SoundEffectSO : ScriptableObject
{
    public string soundEffectName;

    public GameObject soundPrefab;

    public AudioClip soundEffectClip;

    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMin = 0.8f;

    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMax = 1.2f;

    [Range(0f, 1f)]
    public float soundEffectVolume = 1f;
}
