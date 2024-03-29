﻿
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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using SpriteReader;

namespace Hubble_Pong
{
    /// <summary>
    /// This is a singleton class used to hold all of the globally
    /// accessible information for the game. All the resources are
    /// loaded upon startup. It also contains some other shared
    /// information that did not seem to fit properly or well into
    /// the game architecture. Any advice on how to handle instances
    /// such as the random variables or ball porting communication
    /// between class would be great.
    /// </summary>
    public class Resources
    {
        #region Class Member Variables

        private static Resources _active;

        private Dictionary<string, Texture2D> _textures;
        private Dictionary<string, SoundEffect> _sounds;
        private Dictionary<string, SpriteFont> _fonts;
        private SpriteReader.SpriteReader _reader;
        private SpriteManager.Sprite _ball;

        public int PlayerOneScore { get; set; }
        public int PlayerTwoScore { get; set; }
        public bool GameOver { get; set; }
        public int MAX_SCORE { get; private set; }
        public Random Rand { get; private set; }
        public bool PortBall { get; set; }

        #endregion

        #region Public Methods

        // I feel most of these are pretty self explanatory. If I saw 
        // a need for comments I put them in.

        public void AddTexture(string key, Texture2D texture)
        {
            _textures.Add(key, texture);
        }

        public Texture2D GetTexture(string key)
        {
            if (!_textures.ContainsKey(key))
                return null;
            return _textures[key];
        }

        public void AddSound(string key, SoundEffect sound)
        {
            _sounds.Add(key, sound);
        }

        public SoundEffect GetSound(string key)
        {
            if (!_sounds.ContainsKey(key))
                return null;
            return _sounds[key];
        }

        public void AddFont(string key, SpriteFont font)
        {
            _fonts.Add(key, font);
        }

        public SpriteFont GetFont(string key)
        {
            if (!_fonts.ContainsKey(key))
                return null;
            return _fonts[key];
        }

        /// <summary>
        /// The sprite reader is a class I made that interprets an xml file
        /// that describes a sprite sheet. It gives locations and sizes of
        /// images within the sheet.
        /// </summary>
        public void SetSpriteReader(SpriteReader.SpriteReader reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// This method returns the information for a given image within
        /// a sprite sheet
        /// </summary>
        /// <param name="key">The key of the image... AKA its string name</param>
        /// <returns>The sprite position within the sprite sheet</returns>
        public SpriteReader.SpriteInfo GetSpriteInfo(string key)
        {
            return _reader.getSpriteInfo(key);
        }

        public void SetBallPosition(Vector2 position, Vector2 direction)
        {
            _ball.Position = position;
            _ball.Direction = direction;
        }

        public void SetBall(SpriteManager.Sprite ball)
        {
            _ball = ball;
        }

        public void IncrementBallSpeed()
        {
            ((SpriteManager.BallSprite)_ball).IncrementBallSpeed();
        }

        /// <summary>
        /// Used to initialize this singleton class
        /// </summary>
        public static Resources Instance
        {
            get
            {
                if (_active == null)
                {
                    _active = new Resources();
                }
                return _active;
            }
        }

        #endregion

        #region Private Singleton Constructor

        private Resources()
        {
            _textures = new Dictionary<string, Texture2D>();
            _sounds = new Dictionary<string, SoundEffect>();
            _fonts = new Dictionary<string, SpriteFont>();
            Rand = new Random();
            MAX_SCORE = 3;
            PortBall = false;
        }

        #endregion
    }
}
