using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine;

namespace Translations.Editor
{
    internal static partial class ToolGuiUtility
    {
        public static Texture ErrorIcon => EditorGUIUtility.IconContent("console.erroricon").image;
        public static Texture WarningIcon => EditorGUIUtility.IconContent("console.warnicon").image;
        public static Texture InfoIcon => EditorGUIUtility.IconContent("console.infoicon").image;
        public static Texture PlusIcon => EditorGUIUtility.IconContent("Toolbar Plus").image;
        public static Texture PlusIconMore => EditorGUIUtility.IconContent("Toolbar Plus More").image;
        public static Texture MinusIcon => EditorGUIUtility.IconContent("Toolbar Minus").image;
        public static Texture MinusIconMore => EditorGUIUtility.IconContent("Toolbar Minus More").image;


        public static Color BorderColor => EditorGUIUtility.isProSkin ? new Color(0.1372549019607843f, 0.1372549019607843f, 0.1372549019607843f) : new Color(0.6f, 0.6f, 0.6f);
        public static Texture2D BorderTexture => GenerateColorTexture(BorderColor);
        public static Color ButtonColor => EditorGUIUtility.isProSkin ?
                new Color(0.345098f, 0.345098f, 0.345098f) :
                new Color(0.8941177f, 0.8941177f, 0.8941177f);
        public static Texture2D ButtonColorTexture => GenerateColorTexture(ButtonColor);
        public static Color DarkBackgroundColor => EditorGUIUtility.isProSkin ?
                new Color(0.2078432f, 0.2078432f, 0.2078432f) :
                new Color(0.7137255f, 0.7137255f, 0.7137255f);
        public static Texture2D DarkBackgroundTexture => GenerateColorTexture(DarkBackgroundColor);

        public static Texture2D GenerateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        public static void VerticalLineLayout()
        {
            GUIStyle style = new GUIStyle()
            {
                fixedWidth = 1f,
                stretchHeight = true,
            };

            style.normal.background = BorderTexture;
            GUILayout.Box(GUIContent.none, style);
        }

        public static void HorizontalLineLayout()
        {
            GUIStyle style = new GUIStyle()
            {
                fixedHeight = 1f,
                stretchWidth = true,
            };

            style.normal.background = BorderTexture;
            GUILayout.Box(GUIContent.none, style);
        }

        public static void VerticalLine(Rect rect, float width = 1f)
        {
            GUIStyle style = new GUIStyle()
            {
                fixedWidth = width,
                stretchHeight = true,
            };

            style.normal.background = BorderTexture;

            if (Event.current.type == EventType.Repaint)
                style.Draw(rect, GUIContent.none, false, false, false, false);
        }

        public static void HorizontalLine(Rect rect, float height = 1f)
        {
            GUIStyle style = new GUIStyle()
            {
                fixedHeight = height,
                stretchWidth = true,
            };

            style.normal.background = BorderTexture;

            if (Event.current.type == EventType.Repaint)
                style.Draw(rect, GUIContent.none, false, false, false, false);
        }

        public static void BorderAround(Rect rect)
        {
            GUIStyle horizontalLine = new GUIStyle()
            {
                fixedHeight = 1f,
            };

            GUIStyle verticalLine = new GUIStyle()
            {
                fixedWidth = 1f,
            };

            horizontalLine.normal.background = BorderTexture;
            verticalLine.normal.background = BorderTexture;

            if (Event.current.type == EventType.Repaint)
            {
                verticalLine.Draw(rect.ResizeToLeft(0f), GUIContent.none, false, false, false, false);
                verticalLine.Draw(rect.ResizeToRight(0f), GUIContent.none, false, false, false, false);
                horizontalLine.Draw(rect.ResizeToTop(0f), GUIContent.none, false, false, false, false);
                horizontalLine.Draw(rect.ResizeToBottom(0f), GUIContent.none, false, false, false, false);
            }
        }

        public static void DrawColor(Rect rect, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUIStyle style = new GUIStyle()
                {
                    normal = new GUIStyleState()
                    {
                        background = GenerateColorTexture(color),
                    }
                };

                style.Draw(rect, GUIContent.none, false, false, false, false);
            }
        }

        /// <summary>Sorts a list using the search string</summary>
        /// <param name="list">List to sort</param>
        /// <param name="search">Search bar value</param>
        /// <returns>The sorted list</returns>
        public static IEnumerable<string> SortSearchList(IEnumerable<string> list, string search) =>
            SortSearchList(list, x => x, search);

        /// <summary>Sorts a list using the search string</summary>
        /// <param name="list">List to sort</param>
        /// <param name="func">Select the string</param>
        /// <param name="search">Search bar value</param>
        /// <returns>The sorted list</returns>
        public static IEnumerable<T> SortSearchList<T>(IEnumerable<T> list, Func<T, string> func, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return list;

            string[] keywords = search
                .ToLower()
                .Split(' ')
                .Where(s => s != string.Empty)
                .ToArray();

            return list
                .Select(x => new KeyValuePair<T, string>(x, func(x)))
                .GroupBy(x =>
                {
                    string s = x.Value.ToLower();
                    float percentage = 0f;
                    foreach (var keyword in keywords)
                    {
                        if (!s.Contains(keyword))
                            return 0f;

                        percentage += (float)s.Length / keyword.Length;
                        s = s.Replace(keyword, " ");
                    }

                    return percentage;
                })
                .Where(x => x.Key > 0f)
                .OrderBy(x => x.Key)
                .SelectMany(x => x)
                .Select(x => x.Key)
                .ToList();
        }
    }
}