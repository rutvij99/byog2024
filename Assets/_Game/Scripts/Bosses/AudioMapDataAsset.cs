using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioMap", menuName = "IRIS/AudioMap")]
public class AudioMapDataAsset : ScriptableObject
{
    [SerializeField]
    private SerializedDictionary<string, AudioClip> audioClips;

    public AudioClip GetClip(string key)
    {
        if (!audioClips.ContainsKey(key))
            return null;
        return audioClips[key];
    }
}
