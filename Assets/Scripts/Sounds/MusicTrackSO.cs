using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrackSO", menuName = "Sounds/MusicTrackSO")]
public class MusicTrackSO : ScriptableObject
{

    public string musicName;

    public AudioClip musicClip;

    [Range(0, 1)]
    public float musicVolume = 1f;
}
