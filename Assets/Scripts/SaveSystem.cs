using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Random = System.Random;

public static class SaveSystem
{
    public static string directoryPath = Application.persistentDataPath + "/Maps/";
    private static DirectoryInfo directorySelected = new DirectoryInfo(directoryPath);
    public static void SaveMap(SaveEdit map, UserTileData tileData)
    {
        var path = directoryPath + map.path +".json";
        var sw = new StreamWriter(path);
        var data = JsonUtility.ToJson(new MapData(map,tileData));
        sw.WriteLine(data);
        sw.Close();
        Compress(directorySelected);
        File.Delete(path);
    }

    public static MapData LoadMap(string path)
    {
        var realPath = directoryPath + path +".json";
        if (File.Exists(realPath+".gz"))
            Decompress(new FileInfo(realPath+".gz"));
        if (File.Exists(realPath))
        {
            var sr = new StreamReader(realPath);
            var data = sr.ReadLine();
            var json = JsonUtility.FromJson<MapData>(data);
            sr.Close();
            File.Delete(realPath);
            return json;
        }
        else
        {
            Debug.LogError("User_map not found in " + realPath);
            return null;
        }
    }

    public static string[] GetAllMapNames()
    {
        var paths = Directory.GetFiles(directoryPath);
        var pathsThatEndsWithNumber = new List<string>();
        for (var i = 0; i < paths.Length; i++)
        {
            paths[i] = paths[i].Remove(0,directoryPath.Length);
            paths[i] = paths[i].Remove(paths[i].Length - ".json.gz".Length, ".json.gz".Length);
            if (int.TryParse(paths[i].Substring(paths[i].Length - 1, 1), out var number))
            {
                Debug.Log(paths[i] + " ends with a number");
                pathsThatEndsWithNumber.Add(paths[i]);
            }
        }
        var sortedPaths = paths.ToList();
        sortedPaths.Sort();
        sortedPaths.Sort(new StringSorter());
        foreach (var path in sortedPaths)
        {
            Debug.Log(path);
        }

        return sortedPaths.ToArray();
    }

    public static string LoadMapAsString(string path)
    {
        var realPath = directoryPath + path +".json";
        Decompress(new FileInfo(realPath+".gz"));
        if (File.Exists(realPath))
        {
            var sr = new StreamReader(realPath);
            var data = sr.ReadLine();
            sr.Close();
            File.Delete(realPath);
            return data;
        }
        else
        {
            Debug.LogError("User_map not found in " + realPath);
            return null;
        }
    }
    
    /*public static void SaveUserTileData(Editor editor)
    {
        var formatter = new BinaryFormatter();
        var path = Application.persistentDataPath + "/"+ "UserTileData";
        var stream = new FileStream(path, FileMode.Create);

        var data = new UserTileData(editor.tile, editor.editorTileNames);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static UserTileData LoadUserTileData()
    {
        var realPath = Application.persistentDataPath + "/" + "UserTileData";
        if (File.Exists(realPath))
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(realPath, FileMode.Open);
            var data = formatter.Deserialize(stream) as UserTileData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("UserTileData not found in " + realPath);
            return null;
        }
    }*/
    
    public static void Compress(DirectoryInfo directorySelected)
    {
        foreach (FileInfo fileToCompress in directorySelected.GetFiles())
        {
            using (FileStream originalFileStream = fileToCompress.OpenRead())
            {
                if ((File.GetAttributes(fileToCompress.FullName) &
                     FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                {
                    using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                    {
                        using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                            CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressionStream);
                        }
                    }
                }
            }
        }
    }
    
    public static void Decompress(FileInfo fileToDecompress)
    {
        using (FileStream originalFileStream = fileToDecompress.OpenRead())
        {
            string currentFileName = fileToDecompress.FullName;
            string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

            using (FileStream decompressedFileStream = File.Create(newFileName))
            {
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(decompressedFileStream);
                }
            }
        }
    }
    
    public class StringSorter : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var xNum = NumSubstring(x);
            var yNum = NumSubstring(y);
            if (yNum == -1 || xNum == -1)
                return string.CompareOrdinal(x, y);
            return xNum.CompareTo(yNum);
        }

        public int NumSubstring(string stringToGetNum)
        {
            var number = -1;
            for (var j = stringToGetNum.Length - 1; j > 0; j--)
            {
                if (int.TryParse(stringToGetNum.Substring(j, stringToGetNum.Length - j), out var newNumber))
                {
                    number = newNumber;
                }
                else break;
            }
            Debug.Log(stringToGetNum +" " +number);

            return number;
        }
    }
}



