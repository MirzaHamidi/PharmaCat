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
            int tilesetColumns,
            int mapPixelWidth,
            int mapPixelHeight)
        {
            string[] lines = File.ReadAllLines(csvPath);

            // Count actual data rows and columns from the CSV
            int csvRows = 0;
            int csvCols = 0;
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                csvRows++;
                int colCount = line.Split(',').Length;
                if (colCount > csvCols)
                    csvCols = colCount;
            }

            // Calculate tile dimensions from map image size and CSV grid
            // Staggered iso: width = cols * tileW + tileW/2 = (cols + 0.5) * tileW
            // Staggered iso: height = (rows - 1) * (tileH/2) + tileH = (rows + 1) * tileH / 2
            float mapTileWidth = mapPixelWidth / (csvCols + 0.5f);
            float mapTileHeight = mapPixelHeight * 2f / (csvRows + 1f);

            for (int y = 0; y < lines.Length; y++)
            {
                if (string.IsNullOrWhiteSpace(lines[y]))
                    continue;

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
                            isBush,
                            localId
                        )
                    );
                }
            }
        }
    }
}