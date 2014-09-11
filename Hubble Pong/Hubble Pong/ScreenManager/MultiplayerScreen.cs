
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

namespace Hubble_Pong.ScreenManager
{
    /// <summary>
    /// This class is used for drawing and added in components for 
    /// multiplayer. This differs from singleplayer because it has
    /// two player controlled paddles instean of one AI one.
    /// </summary>
    class MultiplayerScreen : Screen
    {
        #region Class Member Variables

        // Used soud instance to control volume
        private SoundEffectInstance _ambientSound;
        private KeyboardState _keyState;
        private KeyboardState _prevKeyState;
        private GamePadState _padState;
        private SpriteManager.SpriteManager _manager;

        #endregion

        #region Overridden Inherited Methods and Constructor

        public MultiplayerScreen(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            Resources.Instance.PlayerOneScore = 0;
            Resources.Instance.PlayerTwoScore = 0;
            Resources.Instance.GameOver = false;
            Resources.Instance.PortBall = false;

            SpriteManager.Sprite ball = new SpriteManager.BallSprite(game, spriteBatch);

            // The ball is added here because I needed to find a way to 
            // to transfer the ball location from one black hole to another.
            // It was hard to do this without a reference in my singleton
            // that made it easy to get the information and set it. I would 
            // appreciate any better input on how to do this.
            Resources.Instance.SetBall(ball);

            // Adds in all components used throughout a singleplayer game
            _manager = new SpriteManager.SpriteManager(game);
            _manager.AddSprite(ball);
            _manager.AddSprite(new SpriteManager.FrameSprite(game, spriteBatch));
            _manager.AddSprite(new SpriteManager.PlayerOneSprite(game, spriteBatch));
            _manager.AddSprite(new SpriteManager.PlayerTwoSprite(game, spriteBatch));
            _manager.AddSprite(new SpriteManager.AsteroidSprite(game, spriteBatch));
            _manager.AddSprite(new SpriteManager.AsteroidSprite(game, spriteBatch));
            _manager.AddSprite(new SpriteManager.AsteroidSprite(game, spriteBatch));
            _manager.AddSprite(new SpriteManager.HoleSprite(game, spriteBatch));
            _manager.AddSprite(new SpriteManager.HoleSprite(game, spriteBatch));
            _manager.AddSprite(new SpriteManager.WeaknessSprite(game, spriteBatch, new Vector2(8, game.GraphicsDevice.Viewport.Height / 2), 1));
            _manager.AddSprite(new SpriteManager.WeaknessSprite(game, spriteBatch, new Vector2(game.GraphicsDevice.Viewport.Width - 8, game.GraphicsDevice.Viewport.Height / 2), 2));
            game.Components.Add(_manager);
        }

        /// <summary>
        /// This contains the information for controlling user input that 
        /// changes to start screen after game is over or to exit game in general.
        ///     XBOX - Controller (Player one or two)
        ///         Start - pause the game
        ///         Back - leave the game
        ///         A - exit game after gameover
        ///     Keyboard
        ///         ESC - leave game
        ///         Enter - exit game after game over
        /// TODO: Split method up into private methods for cleaner code
        /// </summary>
        /// <param name="activeScreen">Reference to active screen for changing it</param>
        public override void Update(ref Screen activeScreen)
        {
            // Plays and initializes sound here since was not ready 
            // in constructor
            if (_ambientSound == null)
            {
                _ambientSound = Resources.Instance.GetSound("GameSound").CreateInstance();
                _ambientSound.IsLooped = true;
                _ambientSound.Volume = 0.2f;
                _ambientSound.Play();
            }

            // Lets users return to start screen when game is over
            if (Resources.Instance.GameOver == false && Resources.Instance.PlayerOneScore == Resources.Instance.MAX_SCORE)
            {
                _ambientSound.Stop();
                Resources.Instance.GameOver = true;
                Resources.Instance.GetSound("GameOverSound").Play();
            }
            if (Resources.Instance.GameOver == false && Resources.Instance.PlayerTwoScore == Resources.Instance.MAX_SCORE)
            {
                _ambientSound.Stop();
                Resources.Instance.GameOver = true;
                Resources.Instance.GetSound("GameOverSound").Play();
            }

            // Allows users to leave or pause the game
            // TODO: Add pausing capabilities with keyboard
            _keyState = Keyboard.GetState();
            if (_keyState != _prevKeyState)
            {
                if (Resources.Instance.GameOver && _keyState.IsKeyDown(Keys.Enter))
                {
                    activeScreen = new StartScreen(base._game, base._spriteBatch);
                    _game.Components.Remove(_manager);
                }
                if (_keyState.IsKeyDown(Keys.Escape))
                {
                    _ambientSound.Stop();
                    activeScreen = new StartScreen(base._game, base._spriteBatch);
                    _game.Components.Remove(_manager);
                }
            }
            _prevKeyState = _keyState;

            _padState = GamePad.GetState(PlayerIndex.One);
            if (_padState.Buttons.B == ButtonState.Pressed)
            {
                _ambientSound.Stop();
                activeScreen = new StartScreen(base._game, base._spriteBatch);
                _game.Components.Remove(_manager);
            }
            if (Resources.Instance.GameOver && _padState.Buttons.A == ButtonState.Pressed)
            {
                activeScreen = new StartScreen(base._game, base._spriteBatch);
                _game.Components.Remove(_manager);
            }

            _padState = GamePad.GetState(PlayerIndex.Two);
            if (_padState.Buttons.B == ButtonState.Pressed)
            {
                _ambientSound.Stop();
                activeScreen = new StartScreen(base._game, base._spriteBatch);
                _game.Components.Remove(_manager);
            }
            if (Resources.Instance.GameOver && _padState.Buttons.A == ButtonState.Pressed)
            {
                activeScreen = new StartScreen(base._game, base._spriteBatch);
                _game.Components.Remove(_manager);
            }
        }

        public override void Draw()
        {
            // Draws HUD information during gameplay
            _spriteBatch.Draw(Resources.Instance.GetTexture("Background"), new Rectangle(0, 0, base._game.GraphicsDevice.Viewport.Width, base._game.GraphicsDevice.Viewport.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            _spriteBatch.DrawString(Resources.Instance.GetFont("OptionViewFont"), "PLAYER 1", new Vector2(195, 435), Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            _spriteBatch.DrawString(Resources.Instance.GetFont("OptionViewFont"), "PLAYER 2", new Vector2(475, 435), Color.Green, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            _spriteBatch.DrawString(Resources.Instance.GetFont("OptionViewFont"), Resources.Instance.PlayerOneScore.ToString(), new Vector2(250, 12), Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            _spriteBatch.DrawString(Resources.Instance.GetFont("OptionViewFont"), Resources.Instance.PlayerTwoScore.ToString(), new Vector2(530, 12), Color.Green, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            // Draws the winner information after a game has finished
            if (Resources.Instance.PlayerOneScore == Resources.Instance.MAX_SCORE)
            {
                _spriteBatch.DrawString(Resources.Instance.GetFont("TitleFont"), "PLAYER 1 WINS!", new Vector2(40, 90), Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            if (Resources.Instance.PlayerTwoScore == Resources.Instance.MAX_SCORE)
            {
                _spriteBatch.DrawString(Resources.Instance.GetFont("TitleFont"), "PLAYER 2 WINS!", new Vector2(40, 90), Color.Green, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
        }

        #endregion
    }
}
