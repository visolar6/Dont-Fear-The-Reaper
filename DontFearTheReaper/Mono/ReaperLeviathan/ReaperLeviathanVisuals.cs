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

            // if (Plugin.Options.ShadowAura)
            // {
            //     Plugin.Logger?.LogInfo("[ReaperLeviathanMeshLogger] Adding shadow aura to Reaper Leviathan");
            //     AddShadowAura();
            // }

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

        private void AddShadowAura()
        {
            // Create a new GameObject for the particle system
            var auraObj = new GameObject("ShadowAura");
            auraObj.transform.SetParent(_reaperLeviathan!.transform, false);
            auraObj.transform.localPosition = Vector3.zero;

            var ps = auraObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(0, 0, 0, 0.005f), new Color(0, 0, 0, 0.01f));
            main.startSize = new ParticleSystem.MinMaxCurve(2f, 4f);
            main.startLifetime = 0.5f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 30;
            main.loop = true;
            main.duration = 1f;
            main.startSpeed = 0.01f;

            var emission = ps.emission;
            emission.rateOverTime = 20f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.2f;
            shape.radiusThickness = 0.05f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new();
            grad.SetKeys(
                [new GradientColorKey(new Color(0, 0, 0, 0.01f), 0.0f), new GradientColorKey(new Color(0, 0, 0, 0.005f), 1.0f)],
                [new GradientAlphaKey(0.01f, 0.0f), new GradientAlphaKey(0.0f, 1.0f)]
            );
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(grad);

            var renderer = auraObj.GetComponent<ParticleSystemRenderer>();
            Shader shader = Shader.Find("Legacy Shaders/Particles/Multiply");
            Material mat;
            if (shader != null)
            {
                mat = new Material(shader);
                // Try to load the custom soft-edged shadow texture
                var shadowTex = ResourceHandler.LoadTexture2DFromFile("Assets/Texture2D/soft_shadow_particle.png");
                if (shadowTex != null)
                {
                    mat.mainTexture = shadowTex;
                    Plugin.Logger?.LogInfo("[ReaperLeviathanVisuals] Loaded custom soft shadow particle texture.");
                }
                else
                {
                    Plugin.Logger?.LogWarning("[ReaperLeviathanVisuals] Could not load custom soft shadow particle texture, using default.");
                }
            }
            else
            {
                Plugin.Logger?.LogWarning("[ReaperLeviathanVisuals] Could not find 'Legacy Shaders/Particles/Multiply', using default particle material.");
                mat = renderer.sharedMaterial; // fallback
            }
            renderer.material = mat;
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.sortingOrder = 1;
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