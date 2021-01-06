using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SaveEdit : MonoBehaviour {

    public Tilemap map;
    public List<int> xPos, yPos;
    public List<string> tileId;
    public Vector3Int origin;
    public string path;
    public static string pathToLoad;
    public InputField inputField;
    public ConfirmationPanel confirmationPanel;

    public string Path
    {
        get => path;
        set => path = value;
    }
    
    public MapData MapData { get; set; }

    private void Awake()
    {
        inputField.SetTextWithoutNotify(pathToLoad);
        Path = pathToLoad;
        LoadTileData();
    }

    public void Save (UserTileData tileData)
    {
        origin = map.origin;
        for (var i = 0; i < map.size.x; i++)
        {
            for (var e = 0; e < map.size.y; e++)
            {
                TileBase tile = map.GetTile(new Vector3Int(origin.x + i, origin.y + e, 0));
                if (tile == null) continue;
                tileId.Add(tile.name);
                xPos.Add(i);
                yPos.Add(e);
            }
        }
        SaveSystem.SaveMap(this, tileData);
	}

    public void LoadTileData()
    {
        MapData = SaveSystem.LoadMap(path);
        if (MapData == null) return;
        FindObjectOfType<Editor>().TileData = MapData.tileData;
    }

    public void LoadTileMap(Editor editor)
    {
        map.ClearAllTiles();
        for (var i = 0; i < MapData.tileId.Length; i++)
        {  
            for (var e = 0; e < editor.tileName.Count; e++)
            {
                if (MapData.tileId[i] == editor.tileName[e])
                {
                    editor.map.SetTile(new Vector3Int(MapData.originX + MapData.xPos[i], MapData.originY + MapData.yPos[i], 0), editor.tile[e]);
                }
            }
        }
    }

    public void ResetMap()
    {
        map.ClearAllTiles();
    }

    public void ResetToLastSave()
    {
        MapData = SaveSystem.LoadMap(path);
        LoadTileMap(FindObjectOfType<Editor>());
    }

    public static void OpenSaveFolder()
    {
        Application.OpenURL(SaveSystem.directoryPath);
    }

    public void SpawnConfirmationPanel()
    {
        var instance = Instantiate(confirmationPanel, FindObjectOfType<Canvas>().transform);
        instance.SetUp("Are You Sure You Want To Exit To Main Menu? \n All Unsaved Changes Will Be Lost");
        instance.OnConfirm += LoadMainMenu;
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("Load");
    }
}
