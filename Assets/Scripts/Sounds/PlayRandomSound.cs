using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Sounds
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayRandomSound : MonoBehaviour
    {
        [SerializeField] AudioSource _audioSource;
        [SerializeField] List<AudioClip> _audioClips;

        void OnEnable()
        {
            _audioSource.clip = _audioClips[Random.Range(0, _audioClips.Count)];
            _audioSource.Play();
        }
    }
}
