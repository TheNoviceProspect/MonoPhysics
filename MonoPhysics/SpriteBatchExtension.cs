using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoPhysics
{
    internal static class SpriteBatchExtensions
    {
        #region Fields

        private static Texture2D _pixel;

        #endregion Fields

        #region Public Methods

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness = 1f)
        {
            Vector2 delta = end - start;
            float angle = (float)Math.Atan2(delta.Y, delta.X);
            float length = delta.Length();
            spriteBatch.Begin();
            spriteBatch.Draw(
                GetPixelTexture(spriteBatch.GraphicsDevice),
                start,
                null,
                color,
                angle,
                Vector2.Zero,
                new Vector2(length, thickness),
                SpriteEffects.None,
                0f
            );
            spriteBatch.End();
        }

        #endregion Public Methods

        #region Private Methods

        private static Texture2D GetPixelTexture(GraphicsDevice graphics)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(graphics, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }
            return _pixel;
        }

        #endregion Private Methods
    }
}