using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;

public class KSpriteAtlas
{
    private static List<SpriteAtlas> list = new List<SpriteAtlas>(50);

    public static void Load()
    {
        Clear();

        var guids = AssetDatabase.FindAssets("t:spriteatlas");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            list.Add(atlas);
        }
    }

    public static SpriteAtlas GetSpriteAtlas(Sprite sprite)
    {
        foreach (var atlas in list)
        {
            if (atlas.CanBindTo(sprite)) return atlas;
        }
        return null;
    }

    public static void Clear()
    {
        list.Clear();
    }
}
