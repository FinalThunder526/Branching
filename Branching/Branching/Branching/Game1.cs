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

namespace Branching
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Line> LineSet;
        List<RandomLineSet> rLineSetSet;
        
        int counter = 0;
        /// <summary>
        /// How often the program checks for a new line.
        /// </summary>
        int antiPrecision = 2;
        /// <summary>
        /// Total/Maximum number of lines in LineSet.
        /// NOTE: The length (in pixels) of the total line will be this*length
        /// </summary>
        int totalCount = 400;
        /// <summary>
        /// STYLE 1: Length of each individual line added.
        /// </summary>
        int length = 5;
        /// <summary>
        /// Style #
        /// </summary>
        int styleN = 1;
        bool hasReachedMax = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            LineSet = new List<Line>();
            rLineSetSet = new List<RandomLineSet>();
            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();
            IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            MouseState NewMState = Mouse.GetState();
            
            // Converting MouseStates to Vector2's.
            //Vector2 oldM = new Vector2(OldMouseState.X, OldMouseState.Y);
            Vector2 newM = new Vector2(NewMState.X, NewMState.Y);

            // every "antiPrecision" iterations, a new line is added to the existing set.
            if (counter % antiPrecision == 0)
            {
                
                // A line, of variable length, is added to LineSet regularly.
                #region Style 0
                if (styleN == 0)
                {
                    if (counter % antiPrecision == 0)
                    {
                        // At the beginning, the line starts at the initial mouse state
                        if (LineSet.Count == 0)
                            LineSet.Add(new Line(GraphicsDevice, 1f, Color.Gray, newM, newM));
                        // Otherwise, the line starts from the endpoint of the previous line.
                        else
                            LineSet.Add(new Line(GraphicsDevice, 1f, Color.Gray, LineSet.Last<Line>().pt2, newM));
                    }

                }
                #endregion
                // A line (or multiple lines) of set length is added based on the new position and the previous line's pt2.
                #region Style 1
                else if (styleN == 1)
                {
                    if (LineSet.Count == 0)
                        // At the beginning, the line starts at the initial mouse state
                        LineSet.Add(new Line(GraphicsDevice, 1f, Color.Gray, newM, newM));
                    else
                    {
                        int n = 0;
                        // Distance is the Vector2 that represents the distance between the current MouseState and the previous line's endpoint.
                        // I.E. Distance is the Vector2 that is the distance to be covered.
                        Vector2 Distance = newM - LineSet.Last<Line>().pt2;
                        // y is the linear distance that needs to be covered.
                        float y = Distance.Length();
                        // normalLine is the normalized vector of Distance
                        Vector2 normalLine = Distance;
                        normalLine.Normalize();
                        // deltaLine represents the length of each individual Line that is added.
                        Vector2 deltaLine = normalLine * length;
                        // n is the number of Lines that is added.
                        if (Distance.Length() != 0)
                        { n = (int)(y / deltaLine.Length()); }
                        for (int i = 0; i < n; i++)
                        {
                            LineSet.Add(new Line(GraphicsDevice, 1f, Color.Gray,
                                LineSet[LineSet.Count - 1].pt2, LineSet[LineSet.Count - 1].pt2 + deltaLine));
                        }
                    }
                }
                #endregion
            }
            
            Random r = new Random();
            // Every extraAntiPrecision, a new RandomLineSet is added to the end of the 
            if (counter % 7 == 0)
            {
                rLineSetSet.Add(new RandomLineSet(LineSet.Last<Line>().pt2, GraphicsDevice, 60, r.Next(360), 60, 10));
            }

            // Update each of the RandomLineSets
            foreach (RandomLineSet RR in rLineSetSet)
            {
                RR.Update(counter);
            }

            if (LineSet.Count > totalCount && !hasReachedMax)
                hasReachedMax = true;

            if (hasReachedMax)
                if (counter % 4 == 0)
                    LineSet.RemoveAt(0);


            counter++;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            // TODO: Add your drawing code here
            foreach (Line l in LineSet)
                l.DrawLine(spriteBatch);

            foreach (RandomLineSet rLineSet in rLineSetSet)
                rLineSet.Draw(spriteBatch);

            //rls.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public static float getAngleFromVector(Vector2 vec)
        {
            float angle = (float)Math.Atan(vec.Y/ vec.X);
            if (vec.X < 0)
            { angle += (float)Math.PI; }
            if (angle < 0)
            { angle += 2 * (float)Math.PI; }
            return angle;
        }
    }
}
