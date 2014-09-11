
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
    /// Used to represent the player on the left side of the game board. 
    /// This is always active in any game.
    /// 
    /// TODO: Merge with PlayerTwoSprite since the functionality is mostly 
    /// just copied.
    /// </summary>
    class PlayerOneSprite : Sprite
    {
        #region Class Member Variables

        // Size of the paddle
        private const int HEIGHT = 100;
        private const int WIDTH = 30;

        // paddle texture
        private Texture2D _paddle;
        // Location of texture within sprite sheet
        private Rectangle _spriteLocation;

        // used to track user input
        private KeyboardState _keyState;
        private GamePadState _padState;

        #endregion

        #region Constructor and Overridden Methods

        public PlayerOneSprite(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            base.Name = "Paddle";
            base.Position = new Vector2(100, game.GraphicsDevice.Viewport.Height / 2);
            _paddle = Resources.Instance.GetTexture("SpriteSheet");
            _spriteLocation = Resources.Instance.GetSpriteInfo("Satellite").Position;
        }

        public override bool IsColliding(Sprite sprite)
        {
            // Usees basic box collisions between the ball and paddle due to its
            // simplicity and since it is all that is needed.
            Rectangle playerRect = new Rectangle((int)(Position.X - HEIGHT / 2 - WIDTH / 2), (int)(Position.Y - HEIGHT / 2 - WIDTH / 2), WIDTH, HEIGHT);
            Rectangle ballRect = new Rectangle((int)(sprite.Position.X - sprite.Size.X / 2), (int)(sprite.Position.Y - sprite.Size.Y / 2), (int)sprite.Size.X, (int)sprite.Size.Y);
            if (playerRect.Intersects(ballRect))
                return true;
            return false;
        }

        public override void HandleCollision(Sprite sprite)
        {
            // Increments speed of ball everytime hit and reflects the ball accross the paddles x-axis
            // where it intersected with the ball
            Resources.Instance.IncrementBallSpeed();
            sprite.Direction = Vector2.Reflect(sprite.Direction, Vector2.UnitX);
            sprite.Position = new Vector2((int)(Position.X - HEIGHT / 2 - WIDTH / 2) + WIDTH + sprite.Size.X / 2, sprite.Position.Y);
            Resources.Instance.GetSound("HitSound").Play();
        }

        public override void Update(GameTime gameTime)
        {
            // Player one uses W and S to control movement of paddle with keyboard
            _keyState = Keyboard.GetState();
            if(Position.Y - HEIGHT / 2 > 75 && _keyState.IsKeyDown(Keys.W))
            {
                Position = new Vector2(Position.X, Position.Y - 5);
            }
            if (Position.Y + HEIGHT / 2 < _game.GraphicsDevice.Viewport.Height - 45 && _keyState.IsKeyDown(Keys.S))
            {
                Position = new Vector2(Position.X, Position.Y + 5);
            }

            // Player one controls paddle with Y-axis of left thumb stick of XBOX controller
            _padState = GamePad.GetState(PlayerIndex.One);
            if (Position.Y - HEIGHT / 2 > 75 && _padState.ThumbSticks.Left.Y > 0)
            {
                Position = new Vector2(Position.X, Position.Y - 5);
            }
            if (Position.Y + HEIGHT / 2 < _game.GraphicsDevice.Viewport.Height - 45 && _padState.ThumbSticks.Left.Y < 0)
            {
                Position = new Vector2(Position.X, Position.Y + 5);
            }
        }

        public override void Draw()
        {
            Rectangle dest = new Rectangle((int)Position.X - HEIGHT / 2, (int)Position.Y - WIDTH / 2, HEIGHT, WIDTH);
            _spriteBatch.Draw(_paddle, dest, _spriteLocation, Color.White, (float)(Math.PI / -2), new Vector2(_spriteLocation.Width / 2, _spriteLocation.Height / 2), SpriteEffects.None, 0.1f);
        }

        #endregion
    }
}
