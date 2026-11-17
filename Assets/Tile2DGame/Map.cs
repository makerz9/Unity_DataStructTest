using System.Linq;
using UnityEngine;

public enum TileTypes
{
    Empty = -1,
    //
    Grass = 15,
    Hills,
    Mountains,
    Towns,
    Castle,
    Monster,

}

public class Map 
{
    public int rows = 0;
    public int cols = 0;

    public Tile[] tiles;

    public Tile[] CoastTiles => tiles.Where(
        t => t.autoTileId >= 0 && t.autoTileId < (int)TileTypes.Grass).ToArray();

    public Tile[] LandTiles => tiles.Where(
        t => t.autoTileId >= (int)TileTypes.Grass).ToArray();

    public void Init(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;

        tiles = new Tile[rows * cols];
        for(int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new Tile
            {
                id = i
            };
        }

        for(int r = 0;r < rows; r++)
        {
            for (int c = 0; c < rows; c++)
            {
                int index = r * cols + c;
                var adjacents = tiles[index].adjacents;
                if((r - 1) >= 0)
                {
                    adjacents[(int)Sides.Top] = tiles[index - cols];
                }

                if((c + 1) < cols)
                {
                    adjacents[(int)Sides.Right] = tiles[index + 1];
                }

                if ((c - 1) >= 0)
                {
                    adjacents[(int)Sides.Left] = tiles[index - 1];
                }

                if ((r + 1) < rows)
                {
                    adjacents[(int)Sides.Bottom] = tiles[index + cols];
                }

            }
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].UpdateAutoTileId();
        }

    }


}
