
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
    /// Black hole sprite that sucks in the ball and sends it 
    /// out into a another black hole to be shot out at a random
    /// direction.
    /// </summary>
    public class HoleSprite : Sprite
    {
        #region Class Member Variables

        // Size of Hole for both length and width of sprite
        private const int LENGTH = 100;
        // Length of sprite update for movement in ms
        private const int FRAME_LENGTH = 10;
        // If a ball just ported wait this long in ms to port again
        private const int PORT_WAIT = 250;

        // Direction that hole is moving to
        private Vector2 _destination;
        // Used to generate random numbers
        private Random _random;
        // Boundary that hole can move within
        private Rectangle _moveBoundary;
        // Location of sprite within sprite sheet
        private Rectangle _spriteLocation;

        // used to time events
        private int _lastFrame;

        // Used to track who ported ball and timing to port again
        private int _lastPortFrame = 0;
        private bool _portedBall = false;
        private bool _portWait = false;

        #endregion

        #region Constructor and Overridden Methods

        public HoleSprite(Game game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            Name = "BlackHole";
            _random = Resources.Instance.Rand;
            _moveBoundary = new Rectangle(100, 100, game.GraphicsDevice.Viewport.Width - 200, game.GraphicsDevice.Viewport.Height - 200);
            _spriteLocation = Resources.Instance.GetSpriteInfo("BlackHole").Position;
            int x_location = _random.Next(_moveBoundary.X, _moveBoundary.X + _moveBoundary.Width);
            int y_location = _random.Next(_moveBoundary.Y, _moveBoundary.Y + _moveBoundary.Height);
            Position = new Vector2(x_location, y_location);
            GenerateRandomLocation();
        }

        public override bool IsColliding(Hubble_Pong.SpriteManager.Sprite sprite)
        {
            // Determines if ball is within pull of black hole using simple 
            // distance formula of their centers. The dot product of the directions
            // determines if the ball was just ported into one because its direction would 
            // be outwards. If just ported then ignore this collision.
            Vector2 dir = Vector2.Normalize(sprite.Position - Position);
            if (!_portWait && Distance(Position, sprite.Position) < 40 && Vector2.Dot(dir, sprite.Direction) <= 0)
                return true;
            return false;
        }

        public override void HandleCollision(Sprite sprite)
        {
            // Notify other hole that this ball should be moved to its center
            if (!Resources.Instance.PortBall)
            {
                Resources.Instance.GetSound("BlackHoleSound").Play();
                _portedBall = true;
                Resources.Instance.PortBall = true;
            }
            else
            {
                Resources.Instance.PortBall = false;
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // If another hole ported tha ball than pick it up and move it here
            if (!_portedBall && Resources.Instance.PortBall)
            {
                Resources.Instance.SetBallPosition(Position + Direction*2, GenerateRandomDirection());
                Resources.Instance.PortBall = false;
                _portWait = true;
            }
            // If we ported ball than ignore it
            if (_portedBall && !Resources.Instance.PortBall)
            {
                _portedBall = false;
            }

            // Time tracking to wait to port the ball in between ports
            if (_portWait)
            {
                _lastPortFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (_lastPortFrame > PORT_WAIT)
                {
                    _portWait = false;
                    _lastPortFrame = 0;
                }
            }

            // Move the black hole to its temporary destination
            // TODO: maybe break this method up into helper methods
            _lastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (_lastFrame > FRAME_LENGTH)
            {
                _lastFrame = 0;
                Position += Direction * 0.5f;
                if (Distance(Position, _destination) < 5)
                {
                    GenerateRandomLocation();
                }
            }
        }

        public override void Draw()
        {
            Rectangle pos = new Rectangle((int)Position.X - LENGTH / 2, (int)Position.Y - LENGTH / 2, LENGTH, LENGTH);
            _spriteBatch.Draw(Resources.Instance.GetTexture("SpriteSheet"), pos, _spriteLocation, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.8f);
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Generated a random location within the boundary of the hole to move toward
        /// </summary>
        private void GenerateRandomLocation()
        {
            int x_location = _random.Next(_moveBoundary.X, _moveBoundary.X + _moveBoundary.Width);
            int y_location = _random.Next(_moveBoundary.Y, _moveBoundary.Y + _moveBoundary.Height);
            _destination = new Vector2(x_location, y_location);
            Direction = Vector2.Normalize(_destination - Position);
        }

        /// <summary>
        /// Basic distance formula calculator
        /// </summary>
        /// <param name="pos1">First location</param>
        /// <param name="pos2">Second Location</param>
        /// <returns>Distance between two locations</returns>
        private float Distance(Vector2 pos1, Vector2 pos2)
        {
            return (float)Math.Sqrt(Math.Pow((pos1.X - pos2.X), 2) + Math.Pow((pos1.Y - pos2.Y), 2));
        }

        /// <summary>
        /// Generates a random direction to shoot the ball out of. I try to
        /// minimize the possibility of it going straight up and down or close to that
        /// since it is annoying.
        /// </summary>
        /// <returns></returns>
        private Vector2 GenerateRandomDirection()
        {
            int first = Math.Max(_random.Next(1, 101), 50);
            int second = _random.Next(1, 101);
            if (_random.Next(0, 2) == 1)
                first *= -1;
            if (_random.Next(0, 2) == 1)
                second *= -1;
            Vector2 dir = new Vector2(first, second);
            return Vector2.Normalize(dir);
        }

        #endregion
    }
}
