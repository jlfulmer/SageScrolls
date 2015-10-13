using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace sagescroll
{
    public class Level : IDisplayableItem
    {
        public bool actedOn { get; set; }
        #region Variables
        // Level content.        
        public ContentManager Content
        {
            get { return content; }
        }
        public Scorebar Score {
            get { return score; }
        }
        ContentManager content;
        public Byte GameEnded
        {
            get
            {
                if (gameFinished)
                {
                    if (gameCleared)
                        return 1;
                    else
                        return 2;
                }
                else
                    return 0;
            }
        }
        Boolean gameFinished;
        Boolean gameCleared;

        public SaveSlot saveOne;
        public SaveSlot saveTwo;
        public SaveSlot saveThree;
        public SaveSlot saveFour;

        List<IDisplayableItem> displayItems;
        List<IDisplayableItem> displayItemsToRemove;
        List<IDisplayableItem> displayItemsToAdd;
        List<IDisplayableItem> guiItems;
        Element saved_e;
        bool save_rdy = false;
        bool[] saveslots;
        bool timeBonus = false;
        double fadeCount;
        Timer timer;
        Scorebar score;
        Task task;
        Tutorial tutorial;
        public UserInput Input;
        public static bool CannotGrab;

        //HUD Stuff
        Texture2D background;
        Texture2D hudBg;
        SpriteFont hudFont;
        GraphicsDeviceManager graphics;
        Texture2D cursor;
        Texture2D cursor2;
        Vector2 cursorPosition;

        IDisplayableItem fire;
        IDisplayableItem water;
        IDisplayableItem wind;
        IDisplayableItem earth;

        public enum SFXNames
        {
            cardActivated = 0, cast, combineElement, fizzle, levelUp, resetElement, rowComplete, taskComplete,
            Count
        }
        SoundEffect[] sfx_list = new SoundEffect[(int)SFXNames.Count];
        Boolean[] sfx_playback = new Boolean[(int)SFXNames.Count];

        #endregion

        #region Loading
        //public Level(IServiceProvider serviceProvider)
        //{
        //    LoadState(serviceProvider);
        //    LoadHud();
        //    LoadElements(new Player("Default"));
        //}

        public Level(IServiceProvider serviceProvider, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            saveslots = new bool[4];
            saveslots[0] = false;
            LoadState(serviceProvider);
            LoadHud();
            LoadElements();
            LoadSFX();
            gameFinished = false;
            gameCleared = false;
            SageGame.gameSelectedIndex = 0;
        }

        private void LoadState(IServiceProvider serviceProvider)
        {
            Input = UserInput.GetUserInput();
            content = new ContentManager(serviceProvider, "Content");
            Element.ElementBuilder.OnBuild += Add;
        }

        private void LoadElements()
        {
            displayItems = new List<IDisplayableItem>();
            displayItemsToRemove = new List<IDisplayableItem>();
            displayItemsToAdd = new List<IDisplayableItem>();
            //tutorial = new Tutorial(this);
            //tutorial.StartTutorial(Tutorial.TutorialNumber.Tutorial_1);
            //displayItems.Add(new CardOrganizer(this));
            displayItems.Add(new WorkSpace(new Vector2(222, 250), this, 1));
            displayItems.Add(new WorkSpace(new Vector2(501, 250), this, 2));
            displayItems.Add(new WorkSpace(new Vector2(222, 390), this, 5));
            displayItems.Add(new WorkSpace(new Vector2(501, 390), this, 6));
            saveOne = new SaveSlot(new Vector2(50, 100), this, 8);
            saveTwo = new SaveSlot(new Vector2(120, 230), this, 9);
            saveThree = new SaveSlot(new Vector2(886, 100), this, 11);
            saveFour = new SaveSlot(new Vector2(816, 230), this, 10);
            //StartTutorial(Tutorial.TutorialNumber.Tutorial_1);
            fire = new ElementGenerator(new Vector2(116, 290), this, Element.Type.Fire);
            water = new ElementGenerator(new Vector2(800, 290), this, Element.Type.Water);
            earth = new ElementGenerator(new Vector2(800, 411), this, Element.Type.Earth);
            wind = new ElementGenerator(new Vector2(116, 411), this, Element.Type.Wind);
            displayItems.Add(fire);
            displayItems.Add(water);
            displayItems.Add(earth);
            displayItems.Add(wind);
        }

        private void LoadHud()
        {
            guiItems = new List<IDisplayableItem>();
            timer = new Timer(this, 45000, 745, 0);
            guiItems.Add(timer);// Prob need to define current allocated time from task.
            score = new Scorebar(this);
            guiItems.Add(score);
            background = Content.Load<Texture2D>("UI/game-backdrop");
            hudBg = Content.Load<Texture2D>("UI/hud-backdrop");
            hudFont = Content.Load<SpriteFont>("UI/Courier New");
#if !WINDOWS
            cursor = Content.Load<Texture2D>("Hand/hand-down");
            cursor2 = Content.Load<Texture2D>("Hand/hand-up");
#endif
        }

        private void LoadSFX()
        {
            sfx_list[(int)SFXNames.cardActivated] = content.Load<SoundEffect>("SFX/card activated");
            sfx_list[(int)SFXNames.cast] = content.Load<SoundEffect>("SFX/Cast");
            sfx_list[(int)SFXNames.combineElement] = content.Load<SoundEffect>("SFX/Combine Element");
            sfx_list[(int)SFXNames.fizzle] = content.Load<SoundEffect>("SFX/fizzle");
            sfx_list[(int)SFXNames.levelUp] = content.Load<SoundEffect>("SFX/Level Up");
            sfx_list[(int)SFXNames.resetElement] = content.Load<SoundEffect>("SFX/reset element");
            sfx_list[(int)SFXNames.rowComplete] = content.Load<SoundEffect>("SFX/Row Complete");
            sfx_list[(int)SFXNames.taskComplete] = content.Load<SoundEffect>("SFX/Task Complete");
            for (int i = 0; i < (int)SFXNames.Count; i++)
            {
                sfx_playback[i] = false;
            }
        }
        #endregion

        #region Update
        public void Update(GameTime gameTime)
        {
#if !WINDOWS
            bool changed = false;
            if (SageGame.MovedUp()) {
                SageGame.MoveGameLoc(false, false, false, this);
                changed = true;
            } else if (SageGame.MovedDown()) {
                SageGame.MoveGameLoc(false, true, false, this);
                changed = true;
            } else if (SageGame.MovedLeft()) {
                SageGame.MoveGameLoc(true, false, false, this);
                changed = true;
            } else if (SageGame.MovedRight()) {
                SageGame.MoveGameLoc(true, false, true, this);
                changed = true;
            }
            if (changed || cursorPosition == Vector2.Zero) {
                cursorPosition = SageGame.GetGameLoc();
            }
#endif

            if (saveslots[0]) { displayItems.Insert(0, saveOne); saveslots[0] = false;  saveOne.activate(); }
            if (saveslots[1]) { displayItems.Insert(0, saveTwo); saveslots[1] = false; saveTwo.activate(); }
            if (saveslots[2]) { displayItems.Insert(0, saveThree); saveslots[2] = false; saveThree.activate(); }
            if (saveslots[3]) { displayItems.Insert(0, saveFour); saveslots[3] = false; saveFour.activate(); }

            if (fadeCount > 0)
            {
                fadeCount -= gameTime.ElapsedGameTime.Milliseconds / 1000.0;
                if (fadeCount < 0)
                {
                    fadeCount = 0;
                }
            }

            if (timeBonus)
            {
                fadeCount = 1.5;
                timeBonus = false;
            }
            
            if (save_rdy)
            {
                displayItems.Add(saved_e);
                save_rdy = false;
            }



            if (SageGame.ButtonJustPressed(Buttons.A)) System.Diagnostics.Debug.WriteLine("---");
            CannotGrab = false;
            foreach (IDisplayableItem item in displayItems) {
                if (item is DraggableItem && ((DraggableItem)item).currState == DraggableItem.State.Dragging) {
                    CannotGrab = true;
                    break;
                }
            }
            

            foreach (IDisplayableItem item in displayItems)
            {
                item.Update(gameTime);
                if (item.actedOn) {
                    item.actedOn = false;
                    break;
                }
            }
            foreach (IDisplayableItem item in guiItems)
            {
                item.Update(gameTime);
            }
            CheckSFX();
            for (int i = 0; i < (int)SFXNames.Count; i++)
            {
                if (sfx_playback[i])
                    sfx_list[i].Play();
                sfx_playback[i] = false;
            }
            //this should be updated LAST
            Input.Update(gameTime);
            foreach (IDisplayableItem item in displayItemsToRemove)
            {
                displayItems.Remove(item);
            }
            foreach (IDisplayableItem item in displayItemsToAdd)
            {
                displayItems.Add(item);
            }

            

            displayItemsToRemove.Clear();
            displayItemsToAdd.Clear();

            
        }
        #endregion

        private void StartTutorial(Tutorial.TutorialNumber a_eTutorial)
        {
                Add(tutorial);
                tutorial.StartTutorial(a_eTutorial);
                timer.TotalTime = double.MaxValue;
                //adjust the time remaining so there is no change in happiness
                timer.TimeRemaining = timer.TotalTime;
                timer.help = true;
                task = new Task(tutorial, score.Prestige);
        }

        #region Functions

        public void updateTaskValue(int value)
        {
            score.updateCurrentTaskValue(timer, value);
        }

        public void TaskFinished()
        {
            int nLastPrestige = score.Prestige;

            if (timer.TimeRemaining <= 0)
                score.UpdateScore(timer, false);
            else
                score.UpdateScore(timer, true);


            if (nLastPrestige == 1 && score.Prestige == 2)
            {
                StartTutorial(Tutorial.TutorialNumber.Tutorial_2);
            }
            else if (!gameFinished)
            {
                task = new Task(score.Prestige);
                if (score.Prestige == 1)
                {
                    timer.TotalTime = 30000;
                }
                else
                {
                    Element.Type[] elements = task.Goals;
                    timer.TotalTime = 20000;
                    foreach (Element.Type e in elements)
                    {
                        if (e >= Element.Type.Spirit)
                            timer.TotalTime = timer.TotalTime + 24000;
                        else if (e >= Element.Type.Phoenix)
                            timer.TotalTime = timer.TotalTime + 18000;
                        else if (e >= Element.Type.Metal)
                            timer.TotalTime = timer.TotalTime + 12000;
                        else if (e >= Element.Type.Magma)
                            timer.TotalTime = timer.TotalTime + 8000;
                        else if (e >= Element.Type.Combustion)
                            timer.TotalTime = timer.TotalTime + 3000;
                    }
                }
                timer.Reset();
            }

            updateTaskValue(0);
        }

        public void updateTaskTime(int time)
        {
            timer.TimeRemaining = timer.TimeRemaining + (double)time;
            if (timer.TimeRemaining > timer.TotalTime)
                timer.TimeRemaining = timer.TotalTime;

            timeBonus = true;
        }

        public void GameEnd(Boolean win)
        {
            if (win)
            {
                gameFinished = true;
                gameCleared = true;
            }
            else
            {
                gameFinished = true;
                gameCleared = false;
            }
        }

        public void Remove(IDisplayableItem toRemove)
        {
            displayItemsToRemove.Add(toRemove);
        }

        public void Add(IDisplayableItem toAdd)
        {
            displayItemsToAdd.Add(toAdd);
        }

        public void AddSFX(SFXNames toAdd)
        {
            sfx_playback[(int)toAdd] = true;
        }

        public void CheckSFX()
        {
            if (sfx_playback[(int)SFXNames.taskComplete])
            {
                sfx_playback[(int)SFXNames.rowComplete] = false;
            }
        }

        public void Dispose()
        {
            timer = null;
            score = null;
            task = null;
            Element.ElementBuilder.OnBuild -= Add;
            if (!displayItems.Contains(tutorial))
                tutorial.Dispose();
            tutorial = null;
            foreach (IDisplayableItem item in displayItems)
            {
                item.Dispose();
            }
            displayItems = null;
            foreach (IDisplayableItem item in guiItems)
            {
                item.Dispose();
            }
            guiItems = null;
            foreach (IDisplayableItem item in displayItemsToAdd)
            {
                item.Dispose();
            }
            displayItemsToAdd = null;
            foreach (IDisplayableItem item in displayItemsToRemove)
            {
                item.Dispose();
            }
            displayItemsToRemove = null;
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, 1024, 768), Color.White);
            spriteBatch.End();
            foreach (IDisplayableItem item in displayItems)
            {
                item.Draw(gameTime, spriteBatch);
            }
            DrawHud(gameTime, spriteBatch);
        }

        public void DrawHud(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(hudBg, new Rectangle(0, 0, 1024, 70), Color.White);
            if (fadeCount > 0)
            {
                spriteBatch.DrawString(score.hudFont, "Time Bonus!", new Vector2(600, 400) + new Vector2(-(score.hudFont.MeasureString("Time Bonus!").X), -60 + (int)((fadeCount) * 20)), new Color(255, (int)(fadeCount * 255), 0, (int)(fadeCount * 255)));
            }
            spriteBatch.End();
            foreach (IDisplayableItem item in guiItems)
            {
                item.Draw(gameTime, spriteBatch);
            }
#if !WINDOWS
            spriteBatch.Begin();
            if (CannotGrab) {
                spriteBatch.Draw(cursor, cursorPosition + new Vector2(-200, -70), Color.White);
            } else {
                spriteBatch.Draw(cursor2, cursorPosition + new Vector2(-200, -70), Color.White);
            }
            spriteBatch.End();
#endif
        }

        public void remain(Element saved)
        {
            saved_e = saved;
            save_rdy = true;
        }

        public void addSave(int saveNum)
        {
            saveslots[saveNum] = true;
        }

        #endregion
    }
}
