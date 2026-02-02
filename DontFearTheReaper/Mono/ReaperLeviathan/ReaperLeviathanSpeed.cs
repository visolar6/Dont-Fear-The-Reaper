using UnityEngine;

namespace DontFearTheReaper.Mono.ReaperLeviathan
{
    internal class ReaperLeviathanSpeed : MonoBehaviour
    {
        private float Multiplier => Plugin.Options.SpeedMultiplier;
        private Creature? _reaperLeviathan;

        internal void Awake()
        {
            _reaperLeviathan = GetComponent<Creature>();
        }

        internal void Start()
        {
            if (_reaperLeviathan == null) return;

            var allComponents = _reaperLeviathan.GetComponents<Component>();
            string[] speedFields = ["swimVelocity", "maxVelocity", "maxAcceleration"];
            string[] allowedTypes = ["Locomotion", "SwimRandom", "FleeOnDamage", "AttackLastTarget", "AttackCyclops"];
            foreach (var comp in allComponents)
            {
                var compType = comp.GetType();
                if (!System.Array.Exists(allowedTypes, n => compType.Name == n))
                    continue;

                foreach (var field in compType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
                {
                    if (field.FieldType == typeof(float) && System.Array.Exists(speedFields, n => field.Name == n))
                    {
                        try
                        {
                            float value = (float)field.GetValue(comp);
                            float newValue = value * Multiplier;
                            field.SetValue(comp, newValue);
                        }
                        catch
                        {
                            Plugin.Logger?.LogWarning($"[ReaperLeviathanSpeedMod] Failed to set {compType.Name}.{field.Name}");
                        }
                    }
                }
            }
        }
    }
}
