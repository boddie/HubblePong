
/**********************************************************/
/**                                                      **/
/**                Author: James Boddie                  **/
/**                Date: 2/2/2014                        **/
/**                                                      **/
/**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Hubble_Pong.SpriteManager
{
    /// <summary>
    /// Represents the frame along the top and bottom boundaries
    /// of the game that the ball bounces against
    /// </summary>
    class FrameSprite : Sprite
    {
        #region Class Member Variables

        // The distance off the top and bottom of the screen the frame sits.
        // Used to bounce ball off at that distance.
        private int Y_OFFSET = 84;

        #endregion

        #region Constructor and Overridden Methods

        public FrameSprite(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            base.Name = "Frame";
            base.Position = new Vector2(0, Y_OFFSET);
        }

        public override bool IsColliding(Sprite sprite)
        {
            // Checks if ball is over or under offset. If it is then it is colliding
            if (sprite.Position.Y + sprite.Size.Y / 2 < Position.Y 
                || sprite.Position.Y - sprite.Size.Y / 2 > _game.GraphicsDevice.Viewport.Height - Position.Y)
            {
                return true;
            }
            return false;
        }

        public override void HandleCollision(Sprite sprite)
        {
            // Determine which wall it is bouncing on than reflect the ball accross its Y
            if (sprite.Position.Y + sprite.Size.Y / 2 < Position.Y)
            {
                sprite.Direction = Vector2.Reflect(sprite.Direction, Vector2.UnitY);
                sprite.Position = new Vector2(sprite.Position.X, Position.Y - sprite.Size.Y / 2);
                Resources.Instance.GetSound("HitSound").Play();
            }
            if (sprite.Position.Y - sprite.Size.Y / 2 > _game.GraphicsDevice.Viewport.Height - Position.Y)
            {
                sprite.Direction = Vector2.Reflect(sprite.Direction, Vector2.UnitY);
                sprite.Position = new Vector2(sprite.Position.X, _game.GraphicsDevice.Viewport.Height - Position.Y + sprite.Size.Y / 2);
                Resources.Instance.GetSound("HitSound").Play();
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Not used for this sprite
        }

        public override void Draw()
        {
            _spriteBatch.Draw(Resources.Instance.GetTexture("Frame"), new Rectangle(0, 0, base._game.GraphicsDevice.Viewport.Width, base._game.GraphicsDevice.Viewport.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
        }

        #endregion
    }
}
