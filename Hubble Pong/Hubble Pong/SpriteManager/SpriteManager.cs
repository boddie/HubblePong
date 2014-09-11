
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
    /// This is the sprite manager (Component) that is used to hold all sprites
    /// and update and draw them. This uses a customized version of
    /// the observor pattern to pass information along for collisions. 
    /// The other classes are notified if a collision occurs and then
    /// they are responsible to handle it. I did it this way to make it 
    /// more modular and easily expandable.
    /// </summary>
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Class Member Variables

        // Holds a list of all active sprites. This is  
        // iterrated through continuously to draw, update,
        // and check for collisions.
        private List<Sprite> _sprites;

        private GamePadState _padStateOne;
        private GamePadState _prevPadStateOne;
        private GamePadState _padStateTwo;
        private GamePadState _prevPadStateTwo;
        private bool _paused = false;

        #endregion

        #region Constructor and Game Component Overridden Methods

        public SpriteManager(Game game) 
            : base(game) 
        {
            _sprites = new List<Sprite>();
        }

        public override void Update(GameTime gameTime)
        {
            // If the game gets paused with the gamepad start button then pause all
            // updates of sprites and collisions.
            _padStateOne = GamePad.GetState(PlayerIndex.One);
            if (_prevPadStateOne != _padStateOne && _padStateOne.Buttons.Start == ButtonState.Pressed)
            {
                _paused = _paused ? false : true;
            }
            _prevPadStateOne = _padStateOne;

            _padStateTwo = GamePad.GetState(PlayerIndex.Two);
            if (_prevPadStateTwo != _padStateTwo && _padStateTwo.Buttons.Start == ButtonState.Pressed)
            {
                _paused = _paused ? false : true;
            }
            _prevPadStateTwo = _padStateTwo;

            // As long as the game is not over or paused then process the sprite information,
            // and check and notify classes of collisions.
            if (!Resources.Instance.GameOver && !_paused)
            {
                foreach (Sprite sprite in _sprites)
                {
                    sprite.Update(gameTime);
                    if (sprite.Name == "Ball")
                    {
                        foreach (Sprite check in _sprites)
                        {
                            if (check.IsColliding(sprite))
                            {
                                check.HandleCollision(sprite);
                            }
                        }
                    }
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (Sprite sprite in _sprites)
            {
                // Calls each sprites draw method
                sprite.Draw();
            }
            base.Draw(gameTime);
        }

        #endregion

        #region Other Public Methods

        // This is used to add a sprite to be monitored and notified to the 
        // active sprites.
        public void AddSprite(Sprite sprite)
        {
            _sprites.Add(sprite);
        }

        #endregion
    }
}
