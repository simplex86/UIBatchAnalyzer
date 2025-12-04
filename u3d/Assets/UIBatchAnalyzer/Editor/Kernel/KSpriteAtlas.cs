using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;

namespace SimpleX
{
    /// <summary>
    /// 图集
    /// </summary>
    public class KSpriteAtlas
    {
        /// <summary>
        /// 图集列表
        /// </summary>
        private static List<SpriteAtlas> list = new List<SpriteAtlas>(50);

        /// <summary>
        /// 加载
        /// </summary>
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

        /// <summary>
        /// 获取包含指定图片的图集
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public static SpriteAtlas GetSpriteAtlas(Sprite sprite)
        {
            foreach (var atlas in list)
            {
                if (atlas.CanBindTo(sprite)) return atlas;
            }
            return null;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public static void Clear()
        {
            list.Clear();
        }
    }
}