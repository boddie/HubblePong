
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
    /// This class is for putting spinning asteroids that 
    /// the ball bounces off of and also increases the ball
    /// speed after the collision occurs
    /// </summary>
    public class AsteroidSprite : Sprite
    {
        #region Class Member Variables

        // Length of frames for spinning animation
        private const int FRAME_LENGTH = 30;
        // Max size of asteroid
        private const int MAX_DIMENSION = 80;

        // Diameter of asteroid
        private int _diameter = 0;
        // Used for tracking previous frame time in ms
        private int _lastFrame;
        // Boundary that the asteroid can spawn in
        private Rectangle _spawnBoundary;
        // Location of sprite in sprite sheet
        private Rectangle _spriteLocation;
        // Used for calulating random spawn position
        private Random _random;
        // Rotation of sprite
        private float _rotation = 0;
        // Bool to wait until next spawn 
        private bool _wait = true;
        // Time to wait (random so not all asteroids always spawn at same time)
        private int _waitTime;

        #endregion

        #region Constructor and Overridden Methods

        public AsteroidSprite(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            base.Name = "Asteroid";
            _random = Resources.Instance.Rand;
            _spriteLocation = Resources.Instance.GetSpriteInfo("Asteroid").Position;
            _spawnBoundary = new Rectangle(150, 150, game.GraphicsDevice.Viewport.Width - 300, game.GraphicsDevice.Viewport.Height - 300);
            GenerateRandomLocation();
        }

        public override bool IsColliding(Sprite sprite)
        {
            // Checks collision based off of circular areas. Adds ball diameter to this then 
            // can use that to tell by equation of a circle if ball is colliding. The circle 
            // collision is rough estimate but looks good for the small size of the sprite. If 
            // The asteroid is modified to be larger than a new method of collision detection
            // should be used to make it look better.
            if (_diameter < 20) 
                return false;
            float offset = _diameter * 0.1f;
            int newDiameter = (int)(_diameter * 0.8);
            Vector2 center = new Vector2(Position.X - offset, Position.Y - offset);
            int radius = (newDiameter + (int)sprite.Size.X) / 2;
            if (Math.Sqrt(Math.Pow((center.X - sprite.Position.X), 2) + Math.Pow((center.Y - sprite.Position.Y), 2)) < radius)
                return true;
            return false;
        }

        public override void HandleCollision(Sprite sprite)
        {
            // If the ball collides that reflect it accross the 
            // asteroids normal. Move the ball out so it does not instantly collide
            // again and cause problems. Also increase the speed of the
            // ball.
            Resources.Instance.GetSound("HitSound").Play();
            Resources.Instance.IncrementBallSpeed();
            float offset = _diameter * 0.1f;
            int newDiameter = (int)(_diameter * 0.8);
            Vector2 center = new Vector2(Position.X - offset, Position.Y - offset);
            Vector2 normal = Vector2.Normalize(sprite.Position - center);
            int radius = (newDiameter + 2 + (int)sprite.Size.X) / 2;
            sprite.Direction = Vector2.Reflect(sprite.Direction, normal);
            while (Math.Sqrt(Math.Pow((center.X - sprite.Position.X), 2) + Math.Pow((center.Y - sprite.Position.Y), 2)) < radius)
                sprite.Position += sprite.Direction;
        }

        public override void Update(GameTime gameTime)
        {
            // Wait to spawn
            _lastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (_wait)
            {
                if (_lastFrame > _waitTime)
                {
                    _wait = false;
                    _diameter = 20;
                }
            }
            // Animate the ball by rotating it and making it bigger
            else if (_lastFrame > FRAME_LENGTH)
            {
                _lastFrame = 0;
                _diameter += 1;
                _rotation += 0.2f;
                if (_diameter >= MAX_DIMENSION)
                {
                    _diameter = 0;
                    GenerateRandomLocation();

                }
            }
        }

        public override void Draw()
        {
            Rectangle location = new Rectangle((int)Position.X, (int)Position.Y, _diameter, _diameter);
            Vector2 origin = new Vector2(_spriteLocation.X + _spriteLocation.Width / 2, _spriteLocation.Y + _spriteLocation.Height / 2);
            _spriteBatch.Draw(Resources.Instance.GetTexture("SpriteSheet"), location, _spriteLocation, Color.White, _rotation, origin,
                SpriteEffects.None, 0.7f);
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Used to generate a random location for the asteroid to spawn
        /// </summary>
        private void GenerateRandomLocation()
        {
            int x_location = _random.Next(_spawnBoundary.X, _spawnBoundary.X + _spawnBoundary.Width);
            int y_location = _random.Next(_spawnBoundary.Y, _spawnBoundary.Y + _spawnBoundary.Height);
            _waitTime = _random.Next(0, 5000);
            _wait = true;
            Position = new Vector2(x_location, y_location);
        }

        #endregion
    }
}
