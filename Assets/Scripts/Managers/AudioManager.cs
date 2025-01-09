using Ludo.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Ludo.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [SerializeField] private AudioDB audioDB;
        [SerializeField] private List<AudioSource> audioSources;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Debug.LogWarning("Multiple AudioManager instances detected. Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        public void Play(AudioName audioName)
        {
            var audio = audioDB.GetAudio(audioName);
            if (audio == null)
            {
                Debug.LogWarning($"Audio not found for AudioName: {audioName}");
                return;
            }

            var source = GetFreeOrNewAudioSource();
            if (source == null)
            {
                Debug.LogWarning("Failed to create or retrieve an AudioSource.");
                return;
            }

            source.clip = audio.Clip;
            source.volume = audio.Volume;

            source.Play();

            // Destroy the AudioSource after it finishes playing with a delay
            if (!audioSources.Contains(source)) // If dynamically created, schedule destruction
            {
                Destroy(source.gameObject, audio.Clip.length + 1f); // Add 1 second delay
            }
        }

        private AudioSource GetFreeOrNewAudioSource()
        {
            // Try to find a free AudioSource from the pool
            var freeSource = audioSources.Find(x => !x.isPlaying);
            if (freeSource != null)
            {
                return freeSource;
            }

            // No free AudioSource, create a new one dynamically
            //Debug.Log("No free AudioSource found. Creating a new one dynamically.");
            var newSource = new GameObject("AudioSource").AddComponent<AudioSource>();
            newSource.transform.SetParent(transform); // Optional: Attach to AudioManager for organization
            return newSource;
        }
    }
}
