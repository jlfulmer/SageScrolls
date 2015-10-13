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
    public class Scorebar : IDisplayableItem
    {
        public bool actedOn { get; set; }
        #region Fields
        public double Score
        {
            get { return score; }
            set { score = value; }
        }
        double score;

        public int Happiness
        {
            get { return happiness; }
            set { happiness = value; }
        }
        int happiness = 50;

        public int Prestige {
            get { return prestige; }
        }
        int prestige;

        Level level;
        private static int s_Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        private static int s_Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        public SpriteFont hudFont;
        private static Color fontColor = new Color(107, 81, 48);
        private Texture2D multiBack;
        private Texture2D multiTrans;
        private Texture2D scoreBack;
        private Texture2D happinessBack;
        private Texture2D happinessIcon;
        private Texture2D prestigeBack;
        private static Vector2 scorePos = new Vector2(47, 2);
        private static Vector2 scoreCenterOffset = new Vector2(147, 30);
        private static Vector2 multiPos = new Vector2(280, 15);
        private static Vector2 multiCenterOffset = new Vector2(41, 36);
        private static Vector2 happinessPos = new Vector2(360, 2);
        private static Vector2 happinessCenterOffset = new Vector2(60, 30);
        private static Vector2 happinessIconOffset = new Vector2(25, 14);
        private static Vector2 prestigePos = new Vector2(499, 2);
        private static Vector2 prestigeCenterOffset = new Vector2(116, 30);
        private static Vector2 center_s = new Vector2(s_Width / 2, s_Height / 5);
        private double fadeCount;
        public bool newTask = true;
        bool[] saves = new bool[4];


        public bool NeedsAdvancedNotification
        {
            get { return needsAdvancedNotification; }
            set { needsAdvancedNotification = value; }
        }
        bool needsAdvancedNotification = false;
        public bool SawAdvancedNotification
        {
            get { return sawAdvancedNotification; }
            set { sawAdvancedNotification = value; }
        }
        bool sawAdvancedNotification = false;
        public int NumAdvancedNotification
        {
            get { return numAdvancedNotification; }
            set { numAdvancedNotification = value; }
        }
        int numAdvancedNotification = 1;

        int multiplierChange;
        int happinessChange;
        int prestigeChange;
        int currentTaskValue;
        Boolean updated;
        private Texture2D timerBack;
        private Texture2D timerFill;

        #endregion

        #region Loading
        public Scorebar(Level l)
        {
            level = l;
            updated = false;
            UpdatePrestige();

            for (int i = 0; i < saves.Length; i++)
                saves[i] = false;

            score = 10000;
            LoadContent();
        }

        public void LoadContent()
        {
            hudFont = level.Content.Load<SpriteFont>("UI/Lithos Pro Regular Score");
            multiBack = level.Content.Load<Texture2D>("UI/multiplier-backdrop");
            multiTrans = level.Content.Load<Texture2D>("UI/multiplier-trans");
            scoreBack = level.Content.Load<Texture2D>("UI/score-backdrop");
            happinessBack = level.Content.Load<Texture2D>("UI/happiness-backdrop");
            happinessIcon = level.Content.Load<Texture2D>("UI/happiness-icon");
            prestigeBack = level.Content.Load<Texture2D>("UI/prestige-backdrop");
            timerBack = level.Content.Load<Texture2D>("UI/timer-backdrop");
            timerFill = level.Content.Load<Texture2D>("UI/timer-fill");
        }

        public void Dispose()
        {
            level = null;
        }
        #endregion

        #region Functions
        public void UpdateScore(Timer timer, bool taskFinished)
        {
            // Obtain the ratio of time remaining to total time
            //we want the player to be able to get 100% points & happiness 
            //for the first 15% of time
            double ratio = Math.Min(1, timer.TimeRemaining / (timer.TotalTime * 0.85));

            // Update the Score
            if (taskFinished)
                score = score + currentTaskValue + (currentTaskValue / 2);
            else
                score = score + currentTaskValue;

            // Update the Happiness and Multiplier
            if (!timer.help)
            {
                int currMulti = getMultiplier();
                happinessChange = (int)(ratio * 30 - 15);
                if (prestige == 1 && happinessChange < 0)
                {
                    if (ratio != 0.0)
                        happinessChange = 0;
                    else
                        happinessChange = -5;
                }
                happiness += happinessChange;
                happiness = Math.Max(Math.Min(happiness, 100), 0);
                multiplierChange = getMultiplier() - currMulti;
            }
            else
            {
                timer.help = false;
                happinessChange = 0;
            }



            // Update the Prestige
            int currPres = prestige;
            UpdatePrestige();
            prestigeChange = prestige - currPres;
            if (prestigeChange != 0)
            {
                level.AddSFX(Level.SFXNames.levelUp);
                if (prestige == 2)
                {
                    needsAdvancedNotification = true;
                    numAdvancedNotification = 0;
                }
                else if (prestige == 4)
                {
                    needsAdvancedNotification = true;
                    numAdvancedNotification = 1;
                }
                else if (prestige == 3)
                {
                    needsAdvancedNotification = true;
                    numAdvancedNotification = 2;
                }
            }
            
            // Check win/lose conditions
            if (prestige == 10)
            {
                level.GameEnd(true);
            }
            else if (happiness <= 0)
            {
                level.GameEnd(false);
            }

            updated = true;
        }

        public void updateCurrentTaskValue(Timer timer, int value)
        {
            if (newTask)
            {
                currentTaskValue = 0;
                newTask = false;
            }
            else
            {
                double ratio = Math.Min(1, timer.TimeRemaining / (timer.TotalTime * 0.85));

                value = (int)(value * ratio);
                value = value * getMultiplier();

                currentTaskValue += value;
            }

        }

        public int getMultiplier()
        {
            if (happiness == 100)
            {
                return 4;
            }
            else if (happiness > 75)
            {
                return 3;
            }
            else if (happiness > 50)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        public static string PrestigeNumberToTitle(int a_nPrestige)
        {
            string[] sarrPrestigeTitles = new string[]{
                "Beginner",
                "Student",
                "Novice",
                "Apprentice",
                "Adept",
                "Sage",
                "Specialist",
                "Teacher",
                "Expert",
                "Master"};

            return sarrPrestigeTitles[a_nPrestige - 1];
        }

        public void UpdatePrestige() {
            
            //if (score > 190000) {
            //    prestige = 10;
            //} else if (score > 140000) {
            //    level.addSave(3);
            //    prestige = 9;
            //} else if (score > 100000) {
            //    prestige = 8;
            //} else if (score > 55000) {
            //    level.addSave(2);
            //    prestige = 7;
            //} else if (score > 35000) {
            //    prestige = 6;
            //} else if (score > 20000) {
            //    level.addSave(1);
            //    prestige = 5;
            //} else if (score > 10000) {
            //    prestige = 4;
            //} else if (score > 5000) {
            //    level.addSave(0);
            //    prestige = 3;
            //} else if (score > 1000) {
            //    prestige = 2;
            //} else {
            //    prestige = 1;
            //}
            prestige = 9;
        }

        public double getNextLevel(int prestige)
        {
            switch (prestige)
            {
                case 1:
                    return 1000;
                case 2:
                    return 5000;
                case 3:
                    return 10000;
                case 4:
                    return 20000;
                case 5:
                    return 35000;
                case 6:
                    return 55000;
                case 7:
                    return 100000;
                case 8:
                    return 140000;
                case 9:
                    return 190000;
                default:
                    return 1;
            }
        }

        #endregion

        #region Update
        public void Update(GameTime gameTime)
        {
            if (fadeCount > 0)
            {
                fadeCount -= gameTime.ElapsedGameTime.Milliseconds / 1000.0;
                if (fadeCount < 0)
                {
                    fadeCount = 0;
                }
            }
            if (updated)
            {
                fadeCount = 1.0;
                updated = false;
            }
        }

        #endregion

        #region Draw
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(scoreBack, scorePos, Color.White);

            //draw score section
            string sNumber = score.ToString("N0", System.Threading.Thread.CurrentThread.CurrentCulture);
            spriteBatch.DrawString(hudFont, sNumber, scorePos + scoreCenterOffset -hudFont.MeasureString(sNumber)/2, fontColor);
            spriteBatch.Draw(multiBack, multiPos, Color.White);
            string sMultiplier = "x" + getMultiplier();
            spriteBatch.DrawString(hudFont, sMultiplier, multiPos + multiCenterOffset, Color.White, -.25f, hudFont.MeasureString(sMultiplier)/2 , .85f, SpriteEffects.None, .5f);

            //draw happiness section
            spriteBatch.Draw(happinessBack, happinessPos, Color.White);
            sNumber = happiness.ToString();
            spriteBatch.DrawString(hudFont, sNumber, happinessPos + happinessCenterOffset - hudFont.MeasureString(sNumber)/2, fontColor);
            spriteBatch.Draw(happinessIcon, happinessPos + happinessIconOffset, null, Color.White, 0.0f, new Vector2(happinessIcon.Width, happinessIcon.Height)/2, 1, SpriteEffects.None, .5f);

            //draw prestige section
            spriteBatch.Draw(prestigeBack, prestigePos, Color.White);
            sNumber = PrestigeNumberToTitle(prestige);
            spriteBatch.DrawString(hudFont, sNumber, prestigePos + prestigeCenterOffset - hudFont.MeasureString(sNumber)/2, fontColor);   
            spriteBatch.Draw(timerFill, new Rectangle((int)prestigePos.X + 33, (int)prestigePos.Y + 45, (int)(timerFill.Width * ((score - getNextLevel(prestige - 1)) / (getNextLevel(prestige) - getNextLevel(prestige-1)))), timerFill.Height),
                new Rectangle(0, 0, (int)(timerFill.Width * ((score - getNextLevel(prestige - 1)) / (getNextLevel(prestige) - getNextLevel(prestige - 1)))), timerFill.Height), Color.White);

            //draw score etc. changes
            if (fadeCount > 0)
            {
                if (currentTaskValue > 0)
                {
                    spriteBatch.DrawString(hudFont, "Score +" + currentTaskValue, center_s + new Vector2(-(hudFont.MeasureString("Score +" + currentTaskValue).X), -60 + (int)((fadeCount) * 20)), new Color(0, (int)(fadeCount * 255), 0, (int)(fadeCount * 255)));
                    newTask = true;
                }
                if (multiplierChange != 0)
                {
                    spriteBatch.Draw(multiTrans, multiPos, new Color((int)(fadeCount * 255), (int)(fadeCount * 255), (int)(fadeCount * 255), (int)(fadeCount * 255)));
                }
                if (happinessChange > 0)
                {
                    spriteBatch.DrawString(hudFont, "Health +" + happinessChange, center_s + new Vector2(-(hudFont.MeasureString("Health +" + happinessChange).X), -30 + (int)((fadeCount) * 20)), new Color(0, (int)(fadeCount * 255), 0, (int)(fadeCount * 255)));
                }
                else if (happinessChange < 0)
                {
                    spriteBatch.DrawString(hudFont, "Health " + happinessChange, center_s + new Vector2(-(hudFont.MeasureString("Health " + happinessChange).X), -30+ (int)((fadeCount) * 20)), new Color((int)(fadeCount * 255), 0, 0, (int)(fadeCount * 255)));
                }
                if (prestigeChange > 0)
                {
                    spriteBatch.DrawString(hudFont, sNumber, prestigePos + prestigeCenterOffset - hudFont.MeasureString(sNumber) / 2, new Color(0, (int)(fadeCount * 255), 0, (int)(fadeCount * 255)));
                    //spriteBatch.DrawString(hudFont, "+" + prestigeChange, prestigePos + new Vector2(-(hudFont.MeasureString("+" + prestigeChange).X), (int)((1 - fadeCount) * 20)), new Color(0, (int)(fadeCount * 255), 0, (int)(fadeCount * 255)));
                }
            }

            spriteBatch.End();
        }

        #endregion
    }
}
