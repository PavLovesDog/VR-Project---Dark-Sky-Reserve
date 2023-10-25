using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static Transform FindChildWithTag(this Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child;
            }

            Transform found = child.FindChildWithTag(tag);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    public static Transform[] FindChildrenWithTag(this Transform parent, string tag)
    {
        List<Transform> results = new List<Transform>();

        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag(tag))
            {
                results.Add(child);
            }
        }

        return results.ToArray();
    }

}
