
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
    /// This sprite represents a weakness that exists on both sides 
    /// of player regions. I got the idea from Star Wars. The Death Star
    /// had one vulnerable area in which if it was shot at it blew up.
    /// If the ball collides exactly with this (ball center fully within 
    /// this rect) than that player loses. A simple glide by or touch is 
    /// not enough.
    /// </summary>
    public class WeaknessSprite : Sprite
    {
        #region Class Member Variables

        // Length and width of the rect
        private const int LENGTH = 16;

        // Where the sprite is drawn on screen
        private Rectangle _drawRect;
        // Location of sprite within sprite sheet
        private Rectangle _spriteLocation;
        // Texture for sprite (Sprite sheet in this case)
        private Texture2D _texture;
        // Is this the weakness for first or second player
        private int _player;

        #endregion

        #region Constructor and Overridden Methods

        public WeaknessSprite(Game game, SpriteBatch spriteBatch, Vector2 location, int player)
            : base(game, spriteBatch)
        {
            _drawRect = new Rectangle((int)location.X - LENGTH / 2, (int)location.Y - LENGTH / 2, LENGTH, LENGTH);
            _spriteLocation = Resources.Instance.GetSpriteInfo("RedBox").Position;
            _texture = Resources.Instance.GetTexture("SpriteSheet");
            _player = player;
            Position = location;
            Name = "Weakness";
        }

        public override bool IsColliding(Sprite sprite)
        {
            // Simple rect contains for checking that center of ball
            // is fully within this weakness zone. Did it this way to make
            // it harder and more unlikely to win this way.
            if (_drawRect.Contains((int)sprite.Position.X, (int)sprite.Position.Y))
                return true;
            return false;
        }

        public override void HandleCollision(Sprite sprite)
        {
            // This being hit is worth the game
            Resources.Instance.GetSound("ScoreSound").Play();
            if (_player == 1)
                Resources.Instance.PlayerTwoScore = Resources.Instance.MAX_SCORE;
            else
                Resources.Instance.PlayerOneScore = Resources.Instance.MAX_SCORE;
        }

        public override void Update(GameTime gameTime)
        {
            // Not used with this sprite
        }

        public override void Draw()
        {
            _spriteBatch.Draw(_texture, _drawRect, _spriteLocation, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.2f);
        }

        #endregion
    }
}
