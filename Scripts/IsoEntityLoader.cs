using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PharmaCat.Scripts.World
{
    internal static class IsoEntityLoader
    {
        public static void LoadEntityLayerFromCsv(
            string csvPath,
            Texture2D tilesetTexture,
            List<TiledIsoEntity> targetList,
            bool isBush,
            int firstGid,
            int tileWidth,
            int tileHeight,
            int tilesetColumns)
        {
            string[] lines = File.ReadAllLines(csvPath);

            int mapTileWidth = 110;
            int mapTileHeight = 70;

            for (int y = 0; y < lines.Length; y++)
            {
                string[] values = lines[y].Split(',');

                for (int x = 0; x < values.Length; x++)
                {
                    if (!int.TryParse(values[x], out int gid))
                        continue;

                    if (gid < 0)
                        continue;

                    int localId = gid - firstGid;

                    if (localId < 0)
                        continue;

                    int sourceX = localId % tilesetColumns;
                    int sourceY = localId / tilesetColumns;

                    Rectangle source = new Rectangle(
                        sourceX * tileWidth,
                        sourceY * tileHeight,
                        tileWidth,
                        tileHeight
                    );

                    float worldX = x * mapTileWidth;

                    if (y % 2 == 1)
                        worldX += mapTileWidth / 2f;

                    float worldY = y * (mapTileHeight / 2f);

                    Vector2 basePosition = new Vector2(
                        worldX + mapTileWidth / 2f,
                        worldY + mapTileHeight
                    );

                    targetList.Add(
                        new TiledIsoEntity(
                            tilesetTexture,
                            source,
                            basePosition,
                            isBush
                        )
                    );
                }
            }
        }
    }
}