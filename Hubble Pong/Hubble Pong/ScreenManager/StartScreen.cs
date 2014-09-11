
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
using SpriteReader;

namespace Hubble_Pong.ScreenManager
{
    /// <summary>
    /// This class is used to draw the initial start screen of the game
    /// and also hold options for the user to select. Current options
    /// include singleplayer or multiplayer. Can easily add options for
    /// settings of volume, resolution, or various features to enable or
    /// disable within a settings screen option down the road.
    /// </summary>
    public class StartScreen : Screen
    {
        #region Class Member Variables

        // Holds background music. Used Sound instance, so I could
        // control volume
        private SoundEffectInstance _ambientSound;

        // These are the options displayed on the screen. I made
        // it an enumeration so more could be added down the road.
        private enum Options
        {
            ONEPLAYER,
            TWOPLAYER
        }

        // Active user selection
        private Options _selectedOption = Options.ONEPLAYER;

        // Used to draw satellite next to selection. Holds rect 
        // location of satellite in sprite sheet
        private SpriteReader.SpriteInfo _satelliteInfo;

        // Input related variables
        private KeyboardState _keyState;
        private KeyboardState _prevKeyState;
        private GamePadState _padState;
        private GamePadState _prevPadState;

        #endregion

        #region Constructor and overrides of inherited methods

        public StartScreen(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            _satelliteInfo = Resources.Instance.GetSpriteInfo("Satellite");
            _keyState = Keyboard.GetState();
            _prevKeyState = _keyState;
            _padState = GamePad.GetState(PlayerIndex.One);
            _prevPadState = _padState;
            Resources.Instance.GameOver = false;
        }

        /// <summary>
        /// Note to self: Could maybe break this method into some private
        /// methods for keyboard and gamepad input. 
        /// </summary>
        /// <param name="activeScreen">Reference used to change screen</param>
        public override void Update(ref Screen activeScreen)
        {
            // Put this here due to information was not available at the constructor. 
            // Turns the background sound on and sets its volume.
            if (_ambientSound == null)
            {
                _ambientSound = Resources.Instance.GetSound("MenuSound").CreateInstance();
                _ambientSound.IsLooped = true;
                _ambientSound.Volume = 0.5f;
                _ambientSound.Play();
            }

            // Tracks keyboard input and current user selection
            _keyState = Keyboard.GetState();
            if (_keyState != _prevKeyState)
            {
                if (_keyState.IsKeyDown(Keys.W) || _keyState.IsKeyDown(Keys.S) ||
                    _keyState.IsKeyDown(Keys.Up) || _keyState.IsKeyDown(Keys.Down))
                {
                    if (_selectedOption == Options.ONEPLAYER)
                        _selectedOption = Options.TWOPLAYER;
                    else
                        _selectedOption = Options.ONEPLAYER;
                    Resources.Instance.GetSound("SelectSound").Play();
                }
                if(_keyState.IsKeyDown(Keys.Enter))
                {
                    // stops music cause next screen has its own
                    _ambientSound.Stop();
                    if (_selectedOption == Options.ONEPLAYER)
                        activeScreen = new SingleplayerScreen(base._game, base._spriteBatch);
                    if (_selectedOption == Options.TWOPLAYER)
                        activeScreen = new MultiplayerScreen(base._game, base._spriteBatch);
                }
            }
            _prevKeyState = _keyState;

            // Tracks input and selection on gamepad
            _padState = GamePad.GetState(PlayerIndex.One);
            if (_padState != _prevPadState)
            {
                if (_padState.ThumbSticks.Left.Y > 0)
                {
                    _selectedOption = Options.ONEPLAYER;
                }
                if (_padState.ThumbSticks.Left.Y < 0)
                {
                    _selectedOption = Options.TWOPLAYER;
                }
                if (_padState.Buttons.A == ButtonState.Pressed)
                {
                    // Stops music since next screen has its own
                    _ambientSound.Stop();
                    if (_selectedOption == Options.ONEPLAYER)
                        activeScreen = new SingleplayerScreen(base._game, base._spriteBatch);
                    if (_selectedOption == Options.TWOPLAYER)
                        activeScreen = new MultiplayerScreen(base._game, base._spriteBatch);
                }
            }
            _prevPadState = _padState;
        }

        /// <summary>
        /// Draws UI of start screen. Pretty self-explanatory...
        /// TODO: Utitilize variable sizing for different screen 
        /// resolutions.
        /// </summary>
        public override void Draw()
        {
            _spriteBatch.Draw(Resources.Instance.GetTexture("Background"), new Rectangle(0, 0, base._game.GraphicsDevice.Viewport.Width, base._game.GraphicsDevice.Viewport.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
            _spriteBatch.DrawString(Resources.Instance.GetFont("TitleFont"), "HUBBLE PONG", new Vector2(60, 50), Color.Yellow, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            if (_selectedOption == Options.ONEPLAYER)
            {
                _spriteBatch.Draw(Resources.Instance.GetTexture("SpriteSheet"), new Rectangle(250, 250, 75, 20), _satelliteInfo.Position, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                _spriteBatch.DrawString(Resources.Instance.GetFont("OptionViewFont"), "SINGLEPLAYER", new Vector2(350, 250), Color.Yellow, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
                _spriteBatch.DrawString(Resources.Instance.GetFont("OptionViewFont"), "MULTIPLAYER", new Vector2(350, 300), Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }
            else
            {
                _spriteBatch.Draw(Resources.Instance.GetTexture("SpriteSheet"), new Rectangle(250, 300, 75, 20), _satelliteInfo.Position, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
                _spriteBatch.DrawString(Resources.Instance.GetFont("OptionViewFont"), "SINGLEPLAYER", new Vector2(350, 250), Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
                _spriteBatch.DrawString(Resources.Instance.GetFont("OptionViewFont"), "MULTIPLAYER", new Vector2(350, 300), Color.Yellow, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
            }
        }

        #endregion
    }
}
