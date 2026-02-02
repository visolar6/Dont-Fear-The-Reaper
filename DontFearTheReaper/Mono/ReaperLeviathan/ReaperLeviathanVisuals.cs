using System.Collections;
using UnityEngine;
using DontFearTheReaper.Utilities;

namespace DontFearTheReaper.Mono.ReaperLeviathan
{
    internal class ReaperLeviathanVisuals : MonoBehaviour
    {
        internal Creature? _reaperLeviathan;

        internal void Awake()
        {
            _reaperLeviathan = GetComponent<Creature>();
        }

        internal void Start()
        {
            if (_reaperLeviathan == null) return;

            if (Plugin.Options.GlowingEyes)
            {
                // Add glowing red lights to both eyes
                var eyeNames = new[] { "eye_left", "eye_right" };
                foreach (var eyeName in eyeNames)
                {
                    StartCoroutine(WaitForEyeTransform(eyeName, (eyeTransform) =>
                    {
                        if (eyeTransform != null)
                        {
                            Plugin.Logger?.LogInfo($"[ReaperLeviathanMeshLogger] Found {eyeName} transform, adding glow");
                            AddGlowToEyeTransform(eyeTransform);
                        }
                        else
                        {
                            Plugin.Logger?.LogWarning($"[ReaperLeviathanMeshLogger] Could not find {eyeName} transform after multiple attempts");
                        }
                    }));
                }
            }
        }

        private void AddGlowToEyeTransform(Transform eye)
        {
            var light = eye.gameObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = Color.red;
            light.intensity = 20f;
            light.range = 0.7f;
            light.shadows = LightShadows.None;
            light.renderMode = LightRenderMode.ForcePixel;
            light.cullingMask = -1;
            light.transform.localPosition += Vector3.forward * 0.05f;

            // Use ResourceHandler to load the custom emission texture
            var emissionTex = ResourceHandler.LoadTexture2DFromFile("Assets/Texture2D/Reaper_Leviathan_illum_eyeglow.png");
            if (emissionTex == null)
                Plugin.Logger?.LogWarning("[ReaperLeviathanMeshLogger] Failed to load emission texture using ResourceHandler");

            var root = _reaperLeviathan?.transform;
            if (emissionTex != null && root != null)
            {
                foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
                {
                    foreach (var mat in renderer.materials)
                    {
                        bool changed = false;
                        if (mat.HasProperty("_Illum"))
                        {
                            mat.SetTexture("_Illum", emissionTex);
                            changed = true;
                        }
                        if (mat.HasProperty("_GlowColor"))
                        {
                            mat.SetColor("_GlowColor", Color.red * 2f);
                            changed = true;
                        }
                        if (mat.HasProperty("_GlowStrength"))
                        {
                            mat.SetFloat("_GlowStrength", 2.0f);
                            changed = true;
                        }
                        if (mat.HasProperty("_GlowStrengthNight"))
                        {
                            mat.SetFloat("_GlowStrengthNight", 2.0f);
                            changed = true;
                        }
                        if (changed)
                        {
                            mat.EnableKeyword("MARMO_EMISSION");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds an eye transform by name, searching recursively.
        /// The reaper Leviathan's hierarchy may not be fully initialized at Start(), which is why this method uses a coroutine to retry.
        /// </summary>
        private IEnumerator WaitForEyeTransform(string eyeName, System.Action<Transform?> callback)
        {
            // Try to find the eye 10 times over 2 seconds
            Transform? eye = TransformHandler.FindDeepChild(_reaperLeviathan!.transform, eyeName);
            if (eye != null)
            {
                callback(eye);
                yield break;
            }
            callback(null);
            yield return new WaitForSeconds(0.2f);
        }
    }
}