[System.Serializable]
public class MapData {

    public int[] xPos, yPos;
    public string[] tileId;
    public int originX, originY;
    public UserTileData tileData;

    public MapData(SaveEdit map, UserTileData tileData)
    {
        xPos = map.xPos.ToArray();
        yPos = map.yPos.ToArray();
        tileId = map.tileId.ToArray();
        originX = map.origin.x;
        originY = map.origin.y;
        this.tileData = tileData;
    }
}
