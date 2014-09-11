
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
    /// This is a player two computer controlled paddle. It
    /// is a simple and beatable AI that only follows the ball 
    /// on its return trip. It is not smart due to its lack of 
    /// trying to determine where the ball will end up after a 
    /// bounce. Made it only track and move on ball return trip 
    /// to make it look more realistic.
    /// </summary>
    class ComputerPlayerSprite : Sprite
    {
        #region Class Member Variables

        // Size of the paddle
        private const int HEIGHT = 100;
        private const int WIDTH = 30;

        // Texture of paddle
        private Texture2D _paddle;
        // Location of paddle texture within sprite sheet
        private Rectangle _spriteLocation;

        // Rect that represents paddle location
        private Rectangle _playerRect;

        // Used by AI to track ball
        private Vector2 _ballPos;
        private Vector2 _ballDir;

        #endregion

        #region Constructor and Overridden Methods

        public ComputerPlayerSprite(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            base.Name = "Paddle";
            base.Position = new Vector2(800, game.GraphicsDevice.Viewport.Height / 2);
            _paddle = Resources.Instance.GetTexture("SpriteSheet");
            _spriteLocation = Resources.Instance.GetSpriteInfo("Satellite").Position;
        }

        public override bool IsColliding(Sprite sprite)
        {
            // Uses simple rect intersection to determine if paddle is colliding with ball. I used this because
            // it is simple and all that was really needed.
            Rectangle playerRect = new Rectangle((int)(Position.X - HEIGHT / 2 - WIDTH / 2), (int)(Position.Y - HEIGHT / 2 - WIDTH / 2), WIDTH, HEIGHT);
            Rectangle ballRect = new Rectangle((int)(sprite.Position.X - sprite.Size.X / 2), (int)(sprite.Position.Y - sprite.Size.Y / 2), (int)sprite.Size.X, (int)sprite.Size.Y);
            _ballPos = sprite.Position;
            _ballDir = sprite.Direction;
            if (playerRect.Intersects(ballRect))
                return true;
            return false;
        }

        public override void HandleCollision(Sprite sprite)
        {
            // If ball collided that reflect it accross its X-axis
            sprite.Direction = Vector2.Reflect(sprite.Direction, Vector2.UnitX);
            sprite.Position = new Vector2((int)(Position.X - HEIGHT / 2 - WIDTH / 2) - sprite.Size.X / 2, sprite.Position.Y);
            Resources.Instance.GetSound("HitSound").Play();
        }

        public override void Update(GameTime gameTime)
        {
            // Tracks where ball it and moves towards its y position
            _playerRect = new Rectangle((int)(Position.X - HEIGHT / 2 - WIDTH / 2), (int)(Position.Y - HEIGHT / 2 - WIDTH / 2), WIDTH, HEIGHT);
            if (_ballDir.X > 0 && Position.Y - HEIGHT / 2 > 75 && _playerRect.Y + _playerRect.Height > _ballPos.Y)
            {
                Position = new Vector2(Position.X, Position.Y - 5);
            }
            if (_ballDir.X > 0 && Position.Y + HEIGHT / 2 < _game.GraphicsDevice.Viewport.Height - 45 && _playerRect.Y < _ballPos.Y)
            {
                Position = new Vector2(Position.X, Position.Y + 5);
            }
        }

        public override void Draw()
        {
            Rectangle dest = new Rectangle((int)Position.X - HEIGHT / 2, (int)Position.Y - WIDTH / 2, HEIGHT, WIDTH);
            _spriteBatch.Draw(_paddle, dest, _spriteLocation, Color.White, (float)(Math.PI / 2), new Vector2(_spriteLocation.Width / 2, _spriteLocation.Height / 2), SpriteEffects.None, 0.1f);
        }

        #endregion
    }
}

