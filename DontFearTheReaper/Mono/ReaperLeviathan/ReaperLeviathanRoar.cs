using UnityEngine;

namespace DontFearTheReaper.Mono.ReaperLeviathan
{
    public class ReaperLeviathanRoar : MonoBehaviour
    {
        private float FrequencyMultiplier => Plugin.Options.RoarIntensity / 2f;
        private float VolumeMultiplier => Plugin.Options.RoarIntensity;
        private const float baseRoarInterval = 30f; // Default interval in seconds

        private AudioSource? roarAudioSource;
        private float roarTimer = 0f;
        private float baseRoarVolume = 1f;

        void Start()
        {
            StartCoroutine(WaitForRoarAudioSource());
        }

        void Update()
        {
            if (roarAudioSource == null)
                return;

            float interval = baseRoarInterval / Mathf.Max(FrequencyMultiplier, 0.01f);
            roarTimer += Time.deltaTime;
            if (roarTimer >= interval)
            {
                roarTimer = 0f;
                PlayRoar();
            }
        }

        private void PlayRoar()
        {
            if (roarAudioSource != null)
            {
                roarAudioSource.volume = baseRoarVolume * VolumeMultiplier;
                roarAudioSource.Play();
            }
        }

        private AudioSource? FindRoarAudioSource()
        {
            // Try to find the roar AudioSource by name or other heuristic
            var sources = GetComponentsInChildren<AudioSource>(true);
            foreach (var src in sources)
            {
                if (src.clip != null && src.clip.name.ToLower().Contains("roar"))
                {
                    baseRoarVolume = src.volume;
                    return src;
                }
            }
            return null;
        }

        private System.Collections.IEnumerator WaitForRoarAudioSource()
        {
            while (roarAudioSource == null)
            {
                roarAudioSource = FindRoarAudioSource();

                if (roarAudioSource != null)
                    yield break;

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
