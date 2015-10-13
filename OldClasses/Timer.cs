using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace sagescroll
{
    public class Timer : IDisplayableItem
    {
        public bool actedOn { get; set; }
        public double TimeRemaining
        {
            get { return timeRemaining; }
            set { timeRemaining = value; }
        }
        double timeRemaining;
        public double TotalTime
        {
            get { return totalTime; }
            set { totalTime = value; }
        }

        Level level;
        Texture2D timerBack;
        Texture2D timerFill;
        private double totalTime;
        private int xPos;
        private int yPos;
        public bool help = false;
        bool pauseTimer = true;
        bool fadeIn = false;
        bool fadeOut = false;
        bool init = false;
        double fadeTime = 0;
        double currentFade = 0;
        double timeCount = 0;
        Color timerColor = Color.White;
        Color timerFillColor = Color.White;

        public Timer(Level level, int totalTime, int xPos, int yPos)
        {
            this.level = level;
            this.totalTime = totalTime;
            timeRemaining = this.totalTime;
            this.xPos = xPos;
            this.yPos = yPos;

            LoadContent();
        }

        public void LoadContent()
        {
            timerBack = level.Content.Load<Texture2D>("UI/timer-backdrop");
            timerFill = level.Content.Load<Texture2D>("UI/timer-fill");
        }

        public void Dispose()
        {
            level = null;
        }

        public void Update(GameTime gameTime)
        {
            if (!pauseTimer)
            {
                //Keep the timer going if there is still time remaining.
                if (timeRemaining > 0)
                {
                    //timeRemaining -= gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }
            else
            {
                if (timeCount <= 0)
                {
                    timeCount = 0;
                    pauseTimer = false;
                }
                else
                    timeCount = timeCount - (gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
            }

            //There is no more time remaining.
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                level.TaskFinished();
            }

            //Timer is now below 100% of total time and above 50%.
            if (timeRemaining > totalTime * 0.50)
            {
                fadeTime = 0;
                timerColor = Color.White;
            }
            //Timer is now below 50% of total time and above 40%.
            else if (timeRemaining > totalTime * 0.40)
            {
                timerColor = Color.White;
                timerFillColor = Color.White;
                fadeTime = 1.5;
            }
            //Timer is now below 40% of total time and above 30%.
            else if (timeRemaining > totalTime * 0.30)
            {
                timerColor = Color.White;
                timerFillColor = Color.White;
                fadeTime = 1.0;
            }
            //Timer is now below 30% of total time and above 20%.
            else if (timeRemaining > totalTime * 0.20)
            {
                timerColor = Color.White;
                timerFillColor = Color.White;
                fadeTime = 0.8;
            }
            //Timer is now below 20% of total time and above 10%.
            else if (timeRemaining > totalTime * 0.10)
            {
                timerColor = Color.White;
                timerFillColor = Color.White;
                fadeTime = 0.6;
            }
            else
            {
                timerColor = new Color(255, 0, 0);
                timerFillColor = Color.White;
            }

            Warning(gameTime);
        }

        public void Warning(GameTime gameTime)
        {
            if (fadeTime > 0)
            {

             if (!init)
                {
                    currentFade = 0;
                    fadeIn = true;
                    init = true;
                }

                if (fadeIn)
                {
                    if (currentFade >= fadeTime)
                    {
                        fadeIn = false;
                        currentFade = fadeTime;
                        fadeOut = true;
                    }
                    else
                        currentFade += gameTime.ElapsedGameTime.TotalMilliseconds / (fadeTime * 1000);
                }

                if (fadeOut)
                {
                    if (currentFade <= 0)
                    {
                        fadeOut = false;
                        currentFade = 0;
                        fadeIn = true;
                    }
                    else
                        currentFade -= gameTime.ElapsedGameTime.TotalMilliseconds / (fadeTime * 1000);
                }
            }
            else
            {
                fadeIn = false;
                fadeOut = false;
                init = false;
            }
        }

        public void Reset()
        {
            timeRemaining = totalTime;
            pauseTimer = true;
            timeCount = 1.0;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
           
            if (fadeIn || fadeOut)
                spriteBatch.Draw(timerBack, new Vector2(xPos, yPos), new Color(255,0,0,(int)(currentFade * 255)));
            else
                spriteBatch.Draw(timerBack, new Vector2(xPos, yPos), Color.White);

            spriteBatch.Draw(timerFill, new Rectangle(xPos+38, yPos+24, (int)(timerFill.Width * (timeRemaining / totalTime)), timerFill.Height), new Rectangle(0, 0, (int)(timerFill.Width * (timeRemaining / totalTime)), timerFill.Height), timerFillColor);
            spriteBatch.End();
        }
    }
}
