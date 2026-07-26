using UnityEngine;

public static class Chance
{
    public static bool Roll(float probability)
    {
        if (probability <= 0f)
            return false;

        if (probability >= 1f)
            return true;

        return Random.value < probability;
    }
}
