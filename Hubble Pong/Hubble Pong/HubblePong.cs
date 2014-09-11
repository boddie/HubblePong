
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
using Hubble_Pong.ScreenManager;
using SpriteReader;

namespace Hubble_Pong
{
    /// <summary>
    /// This is the main Hubble Pong game class. It is used to set
    /// the game resolution, add screen manager component, build 
    /// the resources, and initialize the sprite batch used.
    /// </summary>
    public class HubblePong : Microsoft.Xna.Framework.Game
    {
        #region Class Member Variables

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private ScreenManager.ScreenManager screenManager;

        #endregion

        #region Constructor and Game Inherited Methods

        public HubblePong()
        {
            // Initialize graphics and screen resolution
            graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 480;
            this.graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            Resources.Instance.SetSpriteReader(new SpriteReader.SpriteReader(Content.RootDirectory + @"\", "SpriteSheet.xml"));
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.Services.AddService(typeof(SpriteBatch), spriteBatch);
            
            // Add the screen manager component used to control the 
            // flow of screens throughout the game.
            screenManager = new ScreenManager.ScreenManager(this);
            this.Components.Add(screenManager);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // I add all of the game resources here, so they only have to load at one time. They are
            // then located within a singleton class that can be accessed at any time by all
            // other classes.
            Resources.Instance.AddTexture("Background", Content.Load<Texture2D>(@"Textures/Background"));
            Resources.Instance.AddTexture("Frame", Content.Load<Texture2D>(@"Textures/Frame"));
            Resources.Instance.AddTexture("SpriteSheet", Content.Load<Texture2D>(@"Textures/SpriteSheet"));

            Resources.Instance.AddSound("SelectSound", Content.Load<SoundEffect>(@"Sounds/SelectSound"));
            Resources.Instance.AddSound("BlackHoleSound", Content.Load<SoundEffect>(@"Sounds/BlackHoleSound"));
            Resources.Instance.AddSound("GameOverSound", Content.Load<SoundEffect>(@"Sounds/GameOverSound"));
            Resources.Instance.AddSound("GameSound", Content.Load<SoundEffect>(@"Sounds/GameSound"));
            Resources.Instance.AddSound("HitSound", Content.Load<SoundEffect>(@"Sounds/HitSound"));
            Resources.Instance.AddSound("MenuSound", Content.Load<SoundEffect>(@"Sounds/MenuSound"));
            Resources.Instance.AddSound("ScoreSound", Content.Load<SoundEffect>(@"Sounds/ScoreSound"));

            Resources.Instance.AddFont("TitleFont", Content.Load<SpriteFont>(@"Fonts/TitleFont"));
            Resources.Instance.AddFont("OptionViewFont", Content.Load<SpriteFont>(@"Fonts/OptionViewFont"));

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            // If back is pressed on the controller the game will exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Left this as cornflower blue because I thought it was 
            // funny. Coincidentally it is not shown, so it really does not
            // matter.
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.BackToFront, null);
            base.Draw(gameTime);
            spriteBatch.End();
        }

        #endregion
    }
}
