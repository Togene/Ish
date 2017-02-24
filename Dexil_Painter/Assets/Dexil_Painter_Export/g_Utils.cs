using UnityEngine;
using System.Collections;

public static class g_Utils
{
    public static float normalize(float val, float min, float max)
    {
        return (val - min) / (max - min);
    }

    public static float lerp(float norm, float min, float max)
    {
        return (max - min) * norm + min;
    }

    public static float map(float val, float sourceMin, float sourceMax, float destMin, float destMax)
    {
        return lerp(normalize(val, sourceMin, sourceMax), destMin, destMax);
    }

    public static float randomRange(float min, float max)
    {
        return min + Random.value * (max - min);
    }

    public static bool InRange(float value, float min, float max)
    {
        return value >= Mathf.Min(min, max) && value <= Mathf.Max(min, max);
    }

    public static bool pointInRect(float x, float y, Vector3 p1, Vector3 p2)
    {
        return InRange(x, p1.x, p2.x) && InRange(y, p1.y, p2.y);
    }

    public static bool pointInRectExcludeBorder(float x, float y, Vector3 p1, Vector3 p2)
    {
        return InRange(x, p1.x, p2.x) && 
               InRange(y, p1.y, p2.y) &&
               x != p1.x && x != p2.x && 
               y != p1.y && y != p2.y;
    }

    public static bool pointInCube(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return InRange(p0.x, p1.x, p2.x) && InRange(p0.y, p1.y, p2.y) && InRange(p0.z, p1.z, p2.z);
    }

    public static int roundNearest(float value, float nearest)
    {
        return (int)(Mathf.Round(value / nearest) * nearest);
    }

    public static Vector3 roundNearestVector(Vector3 value, Vector3 nearest)
    {
        return new Vector3((Mathf.Round(value.x / nearest.x) * nearest.x),
                           (Mathf.Round(value.y / nearest.y) * nearest.y),
                           (Mathf.Round(value.z / nearest.z) * nearest.z));
    }

    public static bool rangeIntersect(float min0, float max0, float min1, float max1)
    {
        return Mathf.Max(min0, max0) >= Mathf.Min(min1, max1) &&
               Mathf.Min(min0, max0) <= Mathf.Max(min1, max1);
    }

    public static bool rectIntersect(Quad left, Quad right)
    {
        return rangeIntersect(left.vertexPoints[2].vertice.x, left.vertexPoints[3].vertice.x, right.vertexPoints[2].vertice.x, right.vertexPoints[3].vertice.x)
            && rangeIntersect(left.vertexPoints[0].vertice.y, left.vertexPoints[2].vertice.y, right.vertexPoints[0].vertice.y, right.vertexPoints[2].vertice.y);

        //return rangeIntersect(tile.x1, tile.x2, box.topLeft.x, box.topRight.x)
        //    && rangeIntersect(tile.y1, tile.y4, box.bottomLeft.y, box.topLeft.y);
    }

    public static Color RandomColor()
    {
        return new Color(randomRange(0, 1.0f), randomRange(0, 1.0f), randomRange(0, 1.0f), 1.0f);
    }

    public static Sprite getSprite(Texture2D _set, int i, int j, int tileWidth, int tileHeight)
    {

        Sprite spr = Sprite.Create(_set, 
                                    new Rect(tileWidth * i,
                                             tileHeight * j,
                                    tileWidth, tileHeight), 
                                    new Vector2(0f, 0f),
                                    1,
                                    1, 
                                    SpriteMeshType.FullRect, 
                                    new Vector4(1, 1, 1, 1));
        return (Sprite)spr;
    }

}
