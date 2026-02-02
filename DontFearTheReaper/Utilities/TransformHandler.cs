using UnityEngine;

namespace DontFearTheReaper.Utilities
{
    public static class TransformHandler
    {
        public static Transform? FindDeepChild(Transform parent, string name)
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