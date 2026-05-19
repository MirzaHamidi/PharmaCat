using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using PharmaCat.Scripts.World;
namespace PharmaCat.Scripts.Rendering
{
    internal static class YSortRenderer
    {
        public static void Draw(
            SpriteBatch spriteBatch,
            Player player,
            List<TiledIsoEntity> trees,
            List<TiledIsoEntity> bushes)
        {
            var renderList = new List<object>();

            renderList.Add(player);
            renderList.AddRange(trees);
            renderList.AddRange(bushes);

            foreach (var item in renderList.OrderBy(x =>
            {
                if (x is Player p)
                    return p.Position.Y;

                if (x is TiledIsoEntity e)
                    return e.Position.Y;

                return 0f;
            }))
            {
                if (item is Player p)
                    p.Draw(spriteBatch);

                if (item is TiledIsoEntity e)
                    e.Draw(spriteBatch);
            }
        }
    }
}