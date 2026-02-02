using UnityEngine;

namespace DontFearTheReaper.Mono.ReaperLeviathan
{
    internal class ReaperLeviathanAggression : MonoBehaviour
    {
        private float Multiplier => Plugin.Options.AggressionMultiplier; // Placeholder, replace with aggression option if needed
        private Creature? _reaperLeviathan;

        internal void Awake()
        {
            _reaperLeviathan = GetComponent<Creature>();
        }

        internal void Start()
        {
            if (_reaperLeviathan == null) return;

            var allComponents = _reaperLeviathan.GetComponents<Component>();
            // Table of field logic: fieldName => ("multiply" or "divide")
            var aggressionFieldLogic = new System.Collections.Generic.Dictionary<string, string> {
                {"aggressionThreshold", "divide"},
                {"attackAggressionThreshold", "divide"},
                {"aggressPerSecond", "multiply"},
                {"aggressionPerSecond", "multiply"},
                {"pauseInterval", "divide"},
                {"attackPause", "divide"},
                {"rememberTargetTime", "multiply"},
                {"aggressionDamageScalar", "multiply"}
            };
            string[] allowedTypes = {
                "AttackLastTarget", "AttackCyclops", "AggressiveWhenSeeTarget", "AggressiveOnDamage"
            };
            foreach (var comp in allComponents)
            {
                var compType = comp.GetType();
                if (!System.Array.Exists(allowedTypes, n => compType.Name == n))
                    continue;
                foreach (var field in compType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
                {
                    if (field.FieldType == typeof(float) && aggressionFieldLogic.ContainsKey(field.Name))
                    {
                        try
                        {
                            float value = (float)field.GetValue(comp);
                            float newValue = value;
                            string logic = aggressionFieldLogic[field.Name];
                            if (logic == "multiply")
                                newValue = value * Multiplier;
                            else if (logic == "divide" && Multiplier != 0f)
                                newValue = value / Multiplier;
                            field.SetValue(comp, newValue);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
