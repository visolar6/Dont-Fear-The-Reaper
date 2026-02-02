using System.Collections;
using UnityEngine;

namespace DontFearTheReaper.Mono.ReaperLeviathan
{
    internal class ReaperLeviathanModifications : MonoBehaviour
    {
        internal Creature? _reaperLeviathan;

        internal void Awake()
        {
            _reaperLeviathan = GetComponent<Creature>();
        }

        internal void Start()
        {
            if (_reaperLeviathan == null) return;

            // Add glowing red lights to both eyes
            var eyeNames = new[] { "eye_left", "eye_right" };
            foreach (var eyeName in eyeNames)
            {
                StartCoroutine(FindEyeTransform(eyeName, (eyeTransform) =>
                {
                    if (eyeTransform != null)
                    {
                        Plugin.Log?.LogInfo($"[ReaperLeviathanMeshLogger] Found {eyeName} transform, adding glow");
                        AddGlowToEyeTransform(eyeTransform);
                    }
                    else
                    {
                        Plugin.Log?.LogWarning($"[ReaperLeviathanMeshLogger] Could not find {eyeName} transform after multiple attempts");
                    }
                }));
            }
        }

        private IEnumerator FindEyeTransform(string eyeName, System.Action<Transform?> callback)
        {
            // Try to find the eye 10 times over 2 seconds
            Transform? eye = FindDeepChild(_reaperLeviathan!.transform, eyeName);
            if (eye != null)
            {
                callback(eye);
                yield break;
            }
            callback(null);
            yield return new WaitForSeconds(0.2f);
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

            // Log all material names and shaders under the head transform
            var head = eye.parent;
            if (head != null)
            {
                foreach (var renderer in head.GetComponentsInChildren<Renderer>(true))
                {
                    foreach (var mat in renderer.materials)
                    {
                        Plugin.Log?.LogInfo($"[ReaperLeviathanMeshLogger] Renderer: {renderer.name} | Material: {mat.name} | Shader: {mat.shader?.name}");
                    }
                }
            }
            else
            {
                Plugin.Log?.LogWarning($"[ReaperLeviathanMeshLogger] Could not find head transform for material logging");
            }
        }

        private Transform? FindDeepChild(Transform parent, string name)
        {
            if (parent.name == name)
                return parent;
            foreach (Transform child in parent)
            {
                var result = FindDeepChild(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}