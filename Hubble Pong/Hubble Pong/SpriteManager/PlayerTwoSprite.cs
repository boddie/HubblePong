
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
    /// This class represents a non-AI controlled player that 
    /// can play on the right side. 
    /// 
    /// TODO: Merge with PlayerOneSprite and just call it Player
    /// or user controlled player... something like that. There is 
    /// too much copied and shared information.
    /// </summary>
    class PlayerTwoSprite : Sprite
    {
        #region Class Member Variables

        // Size of the paddle
        private const int HEIGHT = 100;
        private const int WIDTH = 30;

        // Texture of paddle
        private Texture2D _paddle;
        // Location of paddle image within sprite sheet
        private Rectangle _spriteLocation;

        // Used to grab user input
        private KeyboardState _keyState;
        private GamePadState _padState;

        #endregion

        #region Constructor and Overridden Methods

        public PlayerTwoSprite(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            base.Name = "Paddle";
            base.Position = new Vector2(800, game.GraphicsDevice.Viewport.Height / 2);
            _paddle = Resources.Instance.GetTexture("SpriteSheet");
            _spriteLocation = Resources.Instance.GetSpriteInfo("Satellite").Position;
        }

        public override bool IsColliding(Sprite sprite)
        {
            // Represents ball with a rect and checks if that rect intersects the rect for the paddle. Did it this way
            // due to its simplicity and because it was all that was needed.
            Rectangle playerRect = new Rectangle((int)(Position.X - HEIGHT / 2 - WIDTH / 2), (int)(Position.Y - HEIGHT / 2 - WIDTH / 2), WIDTH, HEIGHT);
            Rectangle ballRect = new Rectangle((int)(sprite.Position.X - sprite.Size.X / 2), (int)(sprite.Position.Y - sprite.Size.Y / 2), (int)sprite.Size.X, (int)sprite.Size.Y);
            if (playerRect.Intersects(ballRect))
                return true;
            return false;
        }

        public override void HandleCollision(Sprite sprite)
        {
            // Everytime the ball is hit increase its speed and reflect its direction accross the paddle
            Resources.Instance.IncrementBallSpeed();
            sprite.Direction = Vector2.Reflect(sprite.Direction, Vector2.UnitX);
            sprite.Position = new Vector2((int)(Position.X - HEIGHT / 2 - WIDTH / 2) - sprite.Size.X / 2, sprite.Position.Y);
            Resources.Instance.GetSound("HitSound").Play();
        }

        public override void Update(GameTime gameTime)
        {
            // Keyboard controls
            //    Player two uses keyboard up and down arrows
            _keyState = Keyboard.GetState();
            if (Position.Y - HEIGHT / 2 > 75 && _keyState.IsKeyDown(Keys.Up))
            {
                Position = new Vector2(Position.X, Position.Y - 5);
            }
            if (Position.Y + HEIGHT / 2 < _game.GraphicsDevice.Viewport.Height - 45 && _keyState.IsKeyDown(Keys.Down))
            {
                Position = new Vector2(Position.X, Position.Y + 5);
            }

            // Gamepad controls
            //    Player two uses left joystick and only Y direction matters
            _padState = GamePad.GetState(PlayerIndex.Two);
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
            _spriteBatch.Draw(_paddle, dest, _spriteLocation, Color.White, (float)(Math.PI / 2), new Vector2(_spriteLocation.Width / 2, _spriteLocation.Height / 2), SpriteEffects.None, 0.1f);
        }

        #endregion
    }
}
