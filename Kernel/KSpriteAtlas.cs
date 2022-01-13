using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;

public class KSpriteAtlas
{
    private static List<SpriteAtlas> assets = new List<SpriteAtlas>();

    public static void Load()
    {
        Clear();

        var files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories).Where((s) => {
            if (s.Contains("Editor/")) return false;
            return s.EndsWith(".spriteatlas");
        });

        foreach (var file in files)
        {
            var path = "Assets" + file.Substring(Application.dataPath.Length).Replace("\\", "/");
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            if (atlas != null) assets.Add(atlas);
        }
    }

    public static SpriteAtlas GetSpriteAtlas(Sprite sprite)
    {
        foreach (var atlas in assets)
        {
            if (atlas.CanBindTo(sprite)) return atlas;
        }
        return null;
    }

    public static void Clear()
    {
        assets.Clear();
    }
}
