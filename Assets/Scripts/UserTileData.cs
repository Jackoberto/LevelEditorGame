using System.Collections.Generic;
using UnityEngine.Tilemaps;

[System.Serializable]
public class UserTileData
{
    public List<SerializableColor> colors;
    public List<string> editorNames;

    public UserTileData(List<Tile> tiles, List<string> editorNames)
    {
        colors = new List<SerializableColor>();
        foreach (var tile in tiles)
        {
            colors.Add(new SerializableColor(tile.color));
        }
        this.editorNames = editorNames;
    }
}