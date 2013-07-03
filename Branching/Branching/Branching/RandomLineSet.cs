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
    /// A LineSet that randomly generates Lines and attaches them to the end whenever Update() is called.
    /// </summary>
    class RandomLineSet
    {
        Vector2 Origin;
        GraphicsDevice gd;

        List<Line> LineSet;
        /// <summary>
        /// Length of the lineset at any given time.
        /// </summary>
        int length = 10;
        /// <summary>
        /// This is the original base angle for the very first line.
        /// </summary>
        int baseAngleDegree;
        /// <summary>
        /// This is the range (in degrees) in which new lines can be added to the previous line.
        /// </summary>
        int degreeRange;
        /// <summary>
        /// Once the LineSet has hit this number (including all the ones that have been removed),
        /// the LineSet will not add any more lines.
        /// </summary>
        int maxNOfLines;
        int nOfLines = 0;
        /// <summary>
        /// This is the maximum number of lines that the LineSet can have on-screen at any given time.
        /// </summary>
        int tempMax;
        int antiPrecision = 4;
        Random r;
        Color c;

        public RandomLineSet(Vector2 newOrigin, GraphicsDevice newGD, int newDegreeRange, int newBaseAngleD, int newMax, int newTempMax, Color newC)
        {
            LineSet = new List<Line>();
            Origin = newOrigin;
            gd = newGD;
            degreeRange = newDegreeRange;
            baseAngleDegree = newBaseAngleD;
            maxNOfLines = newMax;
            tempMax = newTempMax;
            r = new Random();
            c = newC;
        }

        public void Initialize()
        {
            if (LineSet.Count == 0)
            {
                // In this method, I have it set the original to be random with the degreeRange.
                //LineSet.Add(new Line(gd, Origin, createRandomFinishPoint(Origin, 45)));

                // Here, this is set to the baseAngleDegree in the beginning.
                LineSet.Add(new Line(gd, 3f, c, Origin, Origin + length * new Vector2((float)Math.Cos(baseAngleDegree), (float)Math.Sin(baseAngleDegree))));
            }
        }
        /// <summary>
        /// Adds a Line, randomly, to the LineSet.
        /// </summary>
        public void Update(int counter)
        {
            if (nOfLines == 0)
                Initialize();
            else if (LineSet.Count > tempMax)
                LineSet.RemoveAt(0);

            if (counter % antiPrecision == 0)
            {
                // As long as the number of lines hasn't reached the max total number of lines, keep adding.
                if (LineSet.Count != 0 && nOfLines < maxNOfLines)
                {
                    LineSet.Add(new Line(gd, 3f, c, LineSet.Last<Line>().pt2,
                        createRandomFinishPoint(LineSet.Last<Line>().pt2, (int)LineSet.Last<Line>().angle)));
                    nOfLines++;
                }

                if (nOfLines >= maxNOfLines && LineSet.Count > 0)
                {
                    LineSet.RemoveAt(0);
                }
            }

        }
        /// <summary>
        /// Creates a random finish point based on the origin.
        /// </summary>
        /// <param name="OriginPoint">The original point.</param>
        /// <returns>A random finish point, based on the class' initial- and finalDegrees.</returns>
        private Vector2 createRandomFinishPoint(Vector2 OriginPoint, int oldAngle)
        {
            int angle = r.Next(degreeRange) + oldAngle - degreeRange/2;
            float rad = MathHelper.ToRadians(angle);
            Vector2 delta = length * new Vector2((float)Math.Cos(rad), (float)Math.Sin(rad));
            return OriginPoint + delta;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Line l in LineSet)
            {
                l.DrawLine(spriteBatch);
            }
        }
    }
}
