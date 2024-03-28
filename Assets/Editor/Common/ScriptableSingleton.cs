
using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor.Common
{
    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject 
    {
        private static T s_Instance;

        public static T instance
        {
            get
            {
                if ((UnityEngine.Object)s_Instance == (UnityEngine.Object)null)
                {
                    CreateAndLoad();
                }

                return s_Instance;
            }
        }

        private static void CreateAndLoad()
        {
            string filePath = GetFilePath();
            if (!string.IsNullOrEmpty(filePath))
            {
                var asset = AssetDatabase.LoadAssetAtPath<T>(filePath);
                if (asset == null)
                {
                    var directoryName = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);
                    asset = ScriptableObject.CreateInstance<T>();
                    AssetDatabase.CreateAsset(asset, filePath);
                    AssetDatabase.SaveAssets();
                    //UnityEngine.Object[] objArr = new T[1] { asset };
                    //InternalEditorUtility.SaveToSerializedFileAndForget(objArr, filePath, true);
                    //AssetDatabase.Refresh();
                }
                s_Instance = asset;
            }
            else
            {
                Debug.LogError($"{nameof(ScriptableSingleton<T>)}: did not use FilePath Attribute!");
            }

        }

        protected static string GetFilePath()
        {
            Type typeFromHandle = typeof(T);
            object[] customAttributes = typeFromHandle.GetCustomAttributes(inherit: true);
            foreach (object obj in customAttributes)
            {
                if (obj is FilePathAttribute)
                {
                    FilePathAttribute filePathAttribute = obj as FilePathAttribute;
                    return filePathAttribute.filePath;
                }
            }
            return string.Empty;
        }

    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FilePathAttribute : Attribute
    {
        internal readonly string filePath;

        /// <summary>
        /// 单例存放路径
        /// </summary>
        /// <param name="path">相对 Project 路径</param>
        public FilePathAttribute(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Invalid relative path (it is empty)");
            }

            if (path[0] == '/')
            {
                path = path.Substring(1);
            }

            filePath = path;
        }
    }

}