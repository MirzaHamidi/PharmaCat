using System;
using System.Collections.Generic;
using PharmaCat.Scripts.World;
namespace PharmaCat.Scripts
{
    internal static class JungleEntityUpdater
    {
        public static void UpdateEntityAlpha(
            Player player,
            List<TiledIsoEntity> trees,
            List<TiledIsoEntity> bushes)
        {
            foreach (var tree in trees)
            {
                bool playerBehindTree = player.Position.Y < tree.Position.Y;
                bool playerCloseX = Math.Abs(player.Position.X - tree.Position.X) < 90f;
                bool playerCloseY = Math.Abs(player.Position.Y - tree.Position.Y) < 140f;

                tree.Alpha = playerBehindTree && playerCloseX && playerCloseY
                    ? 0.45f
                    : 1f;
            }

            foreach (var bush in bushes)
            {
                bool playerBehindBush = player.Position.Y < bush.Position.Y;
                bool playerCloseX = Math.Abs(player.Position.X - bush.Position.X) < 70f;
                bool playerCloseY = Math.Abs(player.Position.Y - bush.Position.Y) < 80f;

                bush.Alpha = playerBehindBush && playerCloseX && playerCloseY
                    ? 0.65f
                    : 1f;
            }
        }

        public static TiledIsoEntity TryCollectBush(
            Player player,
            List<TiledIsoEntity> bushes)
        {
            foreach (var bush in bushes)
            {
                if (!bush.Collected &&
                    Microsoft.Xna.Framework.Vector2.Distance(player.Position, bush.Position) < 80f)
                {
                    bush.Collected = true;
                    return bush;
                }
            }

            return null;
        }
    }
}