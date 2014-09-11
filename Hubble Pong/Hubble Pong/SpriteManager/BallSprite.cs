
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
    /// A ball class that has a trail that is what the player
    /// hits during the pong game.
    /// </summary>
    class BallSprite : Sprite
    {
        #region Class Member Variables

        // Size of the ball sprite
        private const int WIDTH = 25;
        private const int HEIGHT = 25;

        // Amount to increment ball during collisions
        private const float SPEED_INC = 2;

        // used to determine random direction on start of each throw
        private Random _random;
        // Information of each ball image in sprite sheet
        private List<SpriteReader.SpriteInfo> _ballInfo;
        // Texture that contains ball images
        private Texture2D _ballTexture;

        // used to animate and move ball at certain speed
        private int _frameSpeed = 50;
        private int _lastFrame;
        private int _frameIndex = 0;
        private float _ballSpeed = 15f;

        // Tracks previous positions of ball to draw trail
        private List<Vector2> _prevPositions;

        #endregion

        #region Constructor and Overridden Methods

        public BallSprite(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            base.Name = "Ball";
            base.Size = new Vector2(WIDTH, HEIGHT);
            _random = Resources.Instance.Rand;
            base.Position = new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);

            _ballTexture = Resources.Instance.GetTexture("SpriteSheet");
            _prevPositions = new List<Vector2>();

            GenerateRandomDirection();

            // Gets all of the information of each sprite from the sprite sheet
            _ballInfo = new List<SpriteReader.SpriteInfo>();
            _ballInfo.Add(Resources.Instance.GetSpriteInfo("Ball1"));
            _ballInfo.Add(Resources.Instance.GetSpriteInfo("Ball2"));
            _ballInfo.Add(Resources.Instance.GetSpriteInfo("Ball3"));
            _ballInfo.Add(Resources.Instance.GetSpriteInfo("Ball4"));
            _ballInfo.Add(Resources.Instance.GetSpriteInfo("Ball5"));
        }

        public override bool IsColliding(Sprite sprite)
        {
            // Not used here. Each item it collides with is responsible for 
            // collision since how it is handles is different based on what 
            // it collides with
            return false;
        }

        public override void HandleCollision(Sprite sprite)
        {
            // Not used here
        }

        public override void Update(GameTime gameTime)
        {
            // Moves the ball in its current direction
            _lastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (_lastFrame > _frameSpeed)
            {
                _lastFrame = 0;
                _frameIndex++;
                if (_frameIndex >= _ballInfo.Count)
                {
                    _frameIndex = 0;
                }
                if (_prevPositions.Count >= 5)
                    _prevPositions.RemoveAt(0);
                _prevPositions.Add(Position);
                Position += Direction * _ballSpeed;
            }

            // If ball goes off left or right side of screen give the players a point
            if (Position.X + WIDTH / 2 < 0)
            {
                Position = new Vector2(_game.GraphicsDevice.Viewport.Width / 2, _game.GraphicsDevice.Viewport.Height / 2);
                GenerateRandomDirection();
                Resources.Instance.PlayerTwoScore++;
                Resources.Instance.GetSound("ScoreSound").Play();
                _ballSpeed = 15f;
            }
            if (Position.X - WIDTH / 2 > _game.GraphicsDevice.Viewport.Width)
            {
                Position = new Vector2(_game.GraphicsDevice.Viewport.Width / 2, _game.GraphicsDevice.Viewport.Height / 2);
                GenerateRandomDirection();
                Resources.Instance.PlayerOneScore++;
                Resources.Instance.GetSound("ScoreSound").Play();
                _ballSpeed = 15f;
            }

            // If the game is overmove the ball to center to avoid problems and stop it from moving
            // NOTE: May not be necessary
            if (Resources.Instance.GameOver)
            {
                Position = new Vector2(_game.GraphicsDevice.Viewport.Width / 2, _game.GraphicsDevice.Viewport.Height / 2);
                Direction = Vector2.Zero;
            }
        }

        public override void Draw()
        {
            DrawTrail();
            Rectangle dest = new Rectangle((int)Position.X - WIDTH / 2, (int)Position.Y - HEIGHT / 2, WIDTH, HEIGHT);
            Rectangle source = _ballInfo[_frameIndex].Position;
            base._spriteBatch.Draw(_ballTexture, dest, source, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.1f);
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// Generated a random direction for ball to travel in. This
        /// implementatoin tries to avail going perfectly vertical or
        /// near vertical since it is annoying.
        /// </summary>
        private void GenerateRandomDirection()
        {
            int first = Math.Max(_random.Next(1, 101), 50);
            int second = _random.Next(1, 101);
            if (_random.Next(0, 2) == 1)
                first *= -1;
            if (_random.Next(0, 2) == 1)
                second *= -1;
            Direction = new Vector2(first, second);
            Direction = Vector2.Normalize(Direction);
        }

        /// <summary>
        /// Uses previous positions of the ball to scale a transparent "shadow" and to put
        /// the "shadow" at those locations drawing a "trail" like effect.
        /// </summary>
        private void DrawTrail()
        {
            if (_prevPositions.Count >= 1)
            {
                Rectangle dest = new Rectangle((int)_prevPositions[_prevPositions.Count - 1].X - (int)(WIDTH * 0.8) / 2, (int)_prevPositions[_prevPositions.Count - 1].Y - (int)(HEIGHT * 0.8) / 2, (int)(WIDTH * 0.8), (int)(HEIGHT * 0.8));
                Rectangle source = Resources.Instance.GetSpriteInfo("Shadow").Position;
                base._spriteBatch.Draw(_ballTexture, dest, source, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.3f);
            }
            if (_prevPositions.Count >= 2)
            {
                Rectangle dest = new Rectangle((int)_prevPositions[_prevPositions.Count - 2].X - (int)(WIDTH * 0.6) / 2, (int)_prevPositions[_prevPositions.Count - 2].Y - (int)(HEIGHT * 0.6) / 2, (int)(WIDTH * 0.6), (int)(HEIGHT * 0.6));
                Rectangle source = Resources.Instance.GetSpriteInfo("Shadow").Position;
                base._spriteBatch.Draw(_ballTexture, dest, source, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.3f);
            }
            if (_prevPositions.Count >= 3)
            {
                Rectangle dest = new Rectangle((int)_prevPositions[_prevPositions.Count - 3].X - (int)(WIDTH * 0.4) / 2, (int)_prevPositions[_prevPositions.Count - 3].Y - (int)(HEIGHT * 0.4) / 2, (int)(WIDTH * 0.4), (int)(HEIGHT * 0.4));
                Rectangle source = Resources.Instance.GetSpriteInfo("Shadow").Position;
                base._spriteBatch.Draw(_ballTexture, dest, source, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.3f);
            }
            if (_prevPositions.Count >= 4)
            {
                Rectangle dest = new Rectangle((int)_prevPositions[_prevPositions.Count - 4].X - (int)(WIDTH * 0.2) / 2, (int)_prevPositions[_prevPositions.Count - 4].Y - (int)(HEIGHT * 0.2) / 2, (int)(WIDTH * 0.2), (int)(HEIGHT * 0.2));
                Rectangle source = Resources.Instance.GetSpriteInfo("Shadow").Position;
                base._spriteBatch.Draw(_ballTexture, dest, source, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.3f);
            }
        }

        #endregion

        #region Other Public Methods

        /// <summary>
        /// I did not like doing it this way but I needed a way to set the ball
        /// speed from other classes. The main contact point where this is called
        /// is within the resources class. Any input on how to better do this 
        /// would be great. The speed is changed during contact with asteroids and
        /// the paddles.
        /// </summary>
        public void IncrementBallSpeed()
        {
            _ballSpeed += SPEED_INC;
        }

        #endregion
    }
}
