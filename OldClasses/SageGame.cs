#define GRAPHIC_FIX

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

#if GRAPHIC_FIX
//using fbDeprofiler;
#endif

namespace sagescroll
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SageGame : Microsoft.Xna.Framework.Game
    {
        public delegate void SageGameEvent();
        public static SageGameEvent OnPause;
        public static SageGameEvent OnUnpause;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Input State Checkers
        UserInput input;

        // Gamestate Management Variables
        enum GameState { MainMenu, Instructions, Credits, InGame, GameClear, GameOver, Introduction };
        GameState currentGameState = GameState.Introduction;
        bool playedIntroduction = false;

        public bool IsPaused
        {
            get { return isPaused; }
            set 
            { 
                isPaused = value;
                if (isPaused && OnPause != null) {
                    currentPauseMenuIndex = -1;
                    OnPause();
                } else if (!isPaused && OnUnpause != null) {
                    OnUnpause();
                }
            }
        }
        /// <summary>
        /// DO NOT ASSIGN VALUES TO THIS VARIABLE, 
        /// ALWAYS USE IsPaused PROPERTY WHEN ASSIGNING A VALUE
        /// </summary>
        bool isPaused;
        bool isGameOver = false;

        // Menu Items
        MenuSelection[] mainMenuItems;
        MenuSelection[] pauseMenuItems;
        int currentMainMenuIndex = -1;
        int currentPauseMenuIndex = -1;
        Texture2D pauseOverlay;
        Texture2D pauseBanner;
        Texture2D mainMenuBackground;
        TextScroll instructionsScreen;
        TextScroll creditsScreen;
        NotificationScroll gameOverScreen;
        NotificationScroll gameCompleteScreen;

        Texture2D pauseButtonDim;
        Texture2D pauseButtonLit;
        List<UIButton> buttons;

        Texture2D mainMenuIconDimL;
        Texture2D mainMenuIconDimR;
        Texture2D mainMenuIconDimC;
        Texture2D mainMenuIconLitL;
        Texture2D mainMenuIconLitR;
        Texture2D mainMenuIconLitC;

        Texture2D pauseMenuIconDimL;
        Texture2D pauseMenuIconDimR;
        Texture2D pauseMenuIconDimC;
        Texture2D pauseMenuIconLitL;
        Texture2D pauseMenuIconLitR;
        Texture2D pauseMenuIconLitC;

        int centerIconWidth;    //Determines how wide icons should be.

        double fadeTimer = 5.0;
        int fadeIncrement = 3;
        int alphaValue = 0;

        //Music
        protected Song mainMenuSong;
        protected Song gameSong;
        protected Song creditsSong;
        bool gameSongStart = false;
        bool mainMenuSongStart = false;
        bool creditsSongStart = false;
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue mainMenuCue;
        Cue inGameCue;
        Cue creditsCue;

        //Level
        Level level;
        Texture2D advancedHelpIndicator0;
        Texture2D advancedHelpIndicator1;

        //Fonts
        SpriteFont lithosProBlack;
        SpriteFont lithosProMenu;
        SpriteFont bradleyHand;

        //Sounds
        SoundEffect sfx_menuSelection;
        SoundEffect sfx_gameWin;
        SoundEffect sfx_endGame;

        //Introduction Sequence
        Texture2D finalIntroFrame;
        Texture2D cloud1;
        Texture2D cloud2;
        Texture2D cloud3;
        Texture2D cloud4;
        Texture2D forest1;
        Texture2D forest2;
        Texture2D forest3;
        Texture2D forest4;
        Texture2D ground;
        Texture2D logo;
        Texture2D mountains;
        Texture2D sea;
        Texture2D sky;
        Texture2D volcano;

        Vector2 cloud1Position = new Vector2(95, -850);
        Vector2 cloud2Position = new Vector2(-50, -700);
        Vector2 cloud3Position = new Vector2(600, -900);
        Vector2 cloud4Position = new Vector2(700, -720);
        Vector2 forest1Position = new Vector2(0, -800);
        Vector2 forest2Position = new Vector2(0, -790);
        Vector2 forest3Position = new Vector2(0, -780);
        Vector2 forest4Position = new Vector2(0, -775);
        Vector2 groundPosition = new Vector2(0, 658);
        Vector2 logoPosition = new Vector2(95, -350);
        Vector2 mountainsPosition= new Vector2(0, 925);
        Vector2 seaPosition = new Vector2(0, 800);
        Vector2 skyPosition = new Vector2(0, 1232);
        Vector2 volcanoPosition = new Vector2(0, 1100);

        double introTime = 7.5;
        private Texture2D advancedHelpIndicator2;

        public static PlayerIndex selectedIndex;
        bool PlayerIndexChosen = false;

        public static int gameSelectedIndex = 0;

        public static GamePadState prevGamePadState;

        public SageGame()
        {
            #if GRAPHIC_FIX
            //DeProfiler.Run();
            #endif
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }

        public void Pause_OnClick(UIButton source)
        {
            IsPaused = true;
            //System.Diagnostics.Debug.WriteLine("Pausing game.");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsPaused = false;
            this.IsMouseVisible = true;
            input = UserInput.GetUserInput();
            Window.Title = "Sage Scrolls";

            //A new level will be created in the ManageMainMenu method.
            //level = new Level(Services);
          
            mainMenuItems = new MenuSelection[4];            
            currentMainMenuIndex = 0;          
            
            pauseMenuItems = new MenuSelection[4];                        
            currentPauseMenuIndex = 0;

            centerIconWidth = 350;

            buttons = new List<UIButton>();

            prevGamePadState = GamePad.GetState(selectedIndex);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Songs
            mainMenuSong = Content.Load<Song>("Sound/Know Thyself");
            gameSong = Content.Load<Song>("Sound/MainGameMusic");
            creditsSong = Content.Load<Song>("Sound/Credits");
            MediaPlayer.IsRepeating = true;

            audioEngine = new AudioEngine("Content/Sound/GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, "Content/Sound/Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content/Sound/Sound Bank.xsb");
            mainMenuCue = soundBank.GetCue("KnowThyself");
            inGameCue = soundBank.GetCue("InGameMusic");
            creditsCue = soundBank.GetCue("Credits");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Fonts
            lithosProBlack = Content.Load<SpriteFont>("UI/Lithos Pro Regular");
            lithosProMenu = Content.Load<SpriteFont>("UI/Lithos Pro Regular Menu");
            bradleyHand = Content.Load<SpriteFont>("UI/Bradley Hand ITC TT");

            // Load Menu information.
            pauseOverlay = Content.Load<Texture2D>("Menus/pauseMenuOverlay");
            pauseBanner = Content.Load<Texture2D>("UI/paused-banner");
            mainMenuBackground = Content.Load<Texture2D>("Menus/backdrop-no-animation");
            instructionsScreen = new TextScroll(Content, input, "UI/dialog-instructions");
            creditsScreen = new TextScroll(Content, input, "UI/dialog-credits");
            gameOverScreen = new NotificationScroll(Content, input, "UI/dialog-backdrop", "Game Over");
            gameCompleteScreen = new NotificationScroll(Content, input, "UI/dialog-backdrop", "You Have Won!");

            mainMenuIconDimL = Content.Load<Texture2D>("UI/button-dim-left");
            mainMenuIconDimR = Content.Load<Texture2D>("UI/button-dim-right");
            mainMenuIconDimC = Content.Load<Texture2D>("UI/button-dim-middle");
            mainMenuIconLitL = Content.Load<Texture2D>("UI/button-lit-left");
            mainMenuIconLitR = Content.Load<Texture2D>("UI/button-lit-right");
            mainMenuIconLitC = Content.Load<Texture2D>("UI/button-lit-middle");

            pauseMenuIconDimL = Content.Load<Texture2D>("UI/button-dim-left");
            pauseMenuIconDimR = Content.Load<Texture2D>("UI/button-dim-right");
            pauseMenuIconDimC = Content.Load<Texture2D>("UI/button-dim-middle");
            pauseMenuIconLitL = Content.Load<Texture2D>("UI/button-lit-left");
            pauseMenuIconLitR = Content.Load<Texture2D>("UI/button-lit-right");
            pauseMenuIconLitC = Content.Load<Texture2D>("UI/button-lit-middle");

            // UI buttons
            pauseButtonDim = Content.Load<Texture2D>("UI/pause-dim");
            pauseButtonLit = Content.Load<Texture2D>("UI/pause-lit");
            UIButton pause = new UIButton(new Vector2(950, 700), this.Content, UIButton.Button.pauseButton, true, -1);
            pause.OnClick += Pause_OnClick;
            buttons.Add(pause);

            int initialX = (graphics.GraphicsDevice.Viewport.Width / 2) - ((mainMenuIconDimL.Width + mainMenuIconDimR.Width + centerIconWidth)/ 2);
            int initialY = (graphics.GraphicsDevice.Viewport.Height / 4) + 75;

            // Set up Main Menu.   
            mainMenuItems[0] = new MenuSelection("Play", pauseMenuIconDimL, pauseMenuIconDimR, pauseMenuIconDimC,
                pauseMenuIconLitL, pauseMenuIconLitR, pauseMenuIconLitC, initialX, initialY, centerIconWidth, lithosProMenu);
            mainMenuItems[1] = new MenuSelection("Instructions", pauseMenuIconDimL, pauseMenuIconDimR, pauseMenuIconDimC,
                pauseMenuIconLitL, pauseMenuIconLitR, pauseMenuIconLitC, initialX, initialY + 90, centerIconWidth, lithosProMenu);
            mainMenuItems[2] = new MenuSelection("Credits", pauseMenuIconDimL, pauseMenuIconDimR, pauseMenuIconDimC,
                pauseMenuIconLitL, pauseMenuIconLitR, pauseMenuIconLitC, initialX, initialY + 180, centerIconWidth, lithosProMenu);
            mainMenuItems[3] = new MenuSelection("Exit", pauseMenuIconDimL, pauseMenuIconDimR, pauseMenuIconDimC,
                pauseMenuIconLitL, pauseMenuIconLitR, pauseMenuIconLitC, initialX, initialY + 270, centerIconWidth, lithosProMenu);
            
            // Set up Pause Menu.
            initialY = (graphics.GraphicsDevice.Viewport.Height / 4) + 25;
            pauseMenuItems[0] = new MenuSelection("Resume Game", mainMenuIconDimL, mainMenuIconDimR, mainMenuIconDimC,
                mainMenuIconLitL, mainMenuIconLitR, mainMenuIconLitC, initialX, initialY, centerIconWidth, lithosProMenu);
            pauseMenuItems[1] = new MenuSelection("Help", mainMenuIconDimL, mainMenuIconDimR, mainMenuIconDimC,
                mainMenuIconLitL, mainMenuIconLitR, mainMenuIconLitC, initialX, initialY + 90, centerIconWidth, lithosProMenu);
            pauseMenuItems[2] = new MenuSelection("Return to Title", mainMenuIconDimL, mainMenuIconDimR, mainMenuIconDimC,
                mainMenuIconLitL, mainMenuIconLitR, mainMenuIconLitC, initialX, initialY + 180, centerIconWidth, lithosProMenu);
            pauseMenuItems[3] = new MenuSelection("Exit", mainMenuIconDimL, mainMenuIconDimR, mainMenuIconDimC,
                mainMenuIconLitL, mainMenuIconLitR, mainMenuIconLitC, initialX, initialY + 270, centerIconWidth, lithosProMenu);

            // Misc
            advancedHelpIndicator0 = Content.Load<Texture2D>("UI/LvL2_PopUp");
            advancedHelpIndicator1 = Content.Load<Texture2D>("UI/Help");
            advancedHelpIndicator2 = Content.Load<Texture2D>("UI/Tutorial3");

            // SFX
            sfx_menuSelection = Content.Load<SoundEffect>("SFX/menu selection");
            sfx_gameWin = Content.Load<SoundEffect>("SFX/Game Win");
            sfx_endGame = Content.Load<SoundEffect>("SFX/End Game");

            //Introduction Sequence.
            finalIntroFrame = Content.Load<Texture2D>("IntroSequence/intro-final-frame");
            cloud1 = Content.Load<Texture2D>("IntroSequence/title-cloud1");
            cloud2 = Content.Load<Texture2D>("IntroSequence/title-cloud2");
            cloud3 = Content.Load<Texture2D>("IntroSequence/title-cloud3");
            cloud4 = Content.Load<Texture2D>("IntroSequence/title-cloud4");
            forest1 = Content.Load<Texture2D>("IntroSequence/title-forest1");
            forest2 = Content.Load<Texture2D>("IntroSequence/title-forest2");
            forest3 = Content.Load<Texture2D>("IntroSequence/title-forest3");
            forest4 = Content.Load<Texture2D>("IntroSequence/title-forest4");
            ground = Content.Load<Texture2D>("IntroSequence/title-ground");
            logo = Content.Load<Texture2D>("IntroSequence/title-logo");
            mountains = Content.Load<Texture2D>("IntroSequence/title-mountains");
            sea = Content.Load<Texture2D>("IntroSequence/title-sea");
            sky = Content.Load<Texture2D>("IntroSequence/title-sky");
            volcano = Content.Load<Texture2D>("IntroSequence/title-volcano");
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
            // TODO: Add your update logic here
            if (input.KeyJustPressed(Keys.F11))
                graphics.ToggleFullScreen();

            if (PlayerIndexChosen && !GamePad.GetState((PlayerIndex)selectedIndex).IsConnected) {
                PlayerIndexChosen = false;
            }

            if (!PlayerIndexChosen) {
                for (int i = (int)PlayerIndex.One; i < (int)PlayerIndex.Four; i++) {
                    if (GamePad.GetState((PlayerIndex)i).Buttons.A == ButtonState.Pressed ||
                        GamePad.GetState((PlayerIndex)i).Buttons.B == ButtonState.Pressed ||
                        GamePad.GetState((PlayerIndex)i).Buttons.Back == ButtonState.Pressed ||
                        GamePad.GetState((PlayerIndex)i).Buttons.BigButton == ButtonState.Pressed ||
                        GamePad.GetState((PlayerIndex)i).Buttons.LeftShoulder == ButtonState.Pressed ||
                        GamePad.GetState((PlayerIndex)i).Buttons.LeftStick == ButtonState.Pressed ||
                        GamePad.GetState((PlayerIndex)i).Buttons.RightShoulder == ButtonState.Pressed ||
                        GamePad.GetState((PlayerIndex)i).Buttons.RightStick == ButtonState.Pressed ||
                        GamePad.GetState((PlayerIndex)i).Buttons.Start == ButtonState.Pressed ||
                        GamePad.GetState((PlayerIndex)i).Buttons.X == ButtonState.Pressed ||
                        GamePad.GetState((PlayerIndex)i).Buttons.Y == ButtonState.Pressed) {
                        selectedIndex = (PlayerIndex)i;
                        PlayerIndexChosen = true;
                        break;
                    }
                }
            }

            switch (currentGameState)
            {
                case GameState.MainMenu:
                    if (input.KeyJustPressed(Keys.Escape) || ButtonJustPressed(Buttons.Back))
                        this.Exit();                    

                    if(!mainMenuSongStart)
                    {
                        try
                        {
                            gameSongStart = false;
                            creditsSongStart = false;
                            mainMenuCue = soundBank.GetCue("KnowThyself");
                            mainMenuCue.Play();
                            mainMenuSongStart = true;
                        }
                        catch (InvalidOperationException e) { }
                    }
                     
                    ManageMainMenu();                   
                    break;

                case GameState.Instructions:
                    if (input.KeyJustPressed(Keys.Escape) || instructionsScreen.IsMouseSelectedAndClicked
                        || PressedAnything())
                    {
                        sfx_menuSelection.Play();
                        if (isPaused)
                        {
                            currentGameState = GameState.InGame;
                            instructionsScreen.ResetPosition();
                        }
                        else
                        {
                            currentGameState = GameState.MainMenu;
                            instructionsScreen.ResetPosition();
                        }
                    }
                    instructionsScreen.Update(gameTime);
                    break;

                case GameState.Credits:
                    if (input.KeyJustPressed(Keys.Escape) || creditsScreen.IsMouseSelectedAndClicked
                        || PressedAnything())
                    {
                        sfx_menuSelection.Play();
                        currentGameState = GameState.MainMenu;
                        creditsScreen.ResetPosition();
                        creditsCue.Stop(AudioStopOptions.Immediate);
                        creditsCue = soundBank.GetCue("Credits");
                    }
                    creditsScreen.Update(gameTime);
                    if (!creditsSongStart)
                    {
                        mainMenuSongStart = false;
                        gameSongStart = false;
                        creditsCue.Play();
                        creditsSongStart = true;
                    }
                    break;

                case GameState.InGame:
                      UpdateGame(gameTime);
                      if (level != null && level.GameEnded != 0)
                      {
                          MediaPlayer.Stop();
                          byte endGame = level.GameEnded;
                          level.Dispose();
                          level = null;
                          if (endGame == 1)
                          {
                              sfx_gameWin.Play();
                              currentGameState = GameState.GameClear;
                          }
                          else if (endGame == 2)
                          {
                              sfx_endGame.Play();
                              currentGameState = GameState.GameOver;
                          }
                    }                
                    if (!gameSongStart)
                    {
                        mainMenuSongStart = false;
                        creditsSongStart = false;
                        inGameCue = soundBank.GetCue("InGameMusic");
                        inGameCue.Play();
                        gameSongStart = true;
                    }

                    break;

                case GameState.GameClear:
                    if (input.KeyJustPressed(Keys.Escape) || gameCompleteScreen.IsMouseSelectedAndClicked2 || PressedAnything())
                    {
                        sfx_menuSelection.Play();
                        currentGameState = GameState.MainMenu;
                        gameCompleteScreen = new NotificationScroll(Content, input, "UI/dialog-backdrop", "You Have Won!");
                    }
                    else if (gameCompleteScreen.IsMouseSelectedAndClicked1)
                    {
                        sfx_menuSelection.Play();
                        level = new Level(Services, graphics);
                        currentGameState = GameState.InGame;
                        gameCompleteScreen = new NotificationScroll(Content, input, "UI/dialog-backdrop", "You Have Won!");
                    }
                    gameCompleteScreen.Update(gameTime);
                    break;

                case GameState.GameOver:
                    if (input.KeyJustPressed(Keys.Escape) || gameOverScreen.IsMouseSelectedAndClicked2 || PressedAnything())
                    {
                        sfx_menuSelection.Play();
                        currentGameState = GameState.MainMenu;
                        gameOverScreen = new NotificationScroll(Content, input, "UI/dialog-backdrop", "Game Over");
                    }
                    else if (gameOverScreen.IsMouseSelectedAndClicked1)
                    {
                        sfx_menuSelection.Play();
                        level = new Level(Services, graphics);
                        currentGameState = GameState.InGame;
                        gameOverScreen = new NotificationScroll(Content, input, "UI/dialog-backdrop", "Game Over");
                    }
                    gameOverScreen.Update(gameTime);
                    break;
                case GameState.Introduction:
                    if (input.KeyJustPressed(Keys.Escape) || input.KeyJustPressed(Keys.Space) || input.ButtonJustPressed(UserInput.MouseButton.Left)
                        || PressedAnything())
                    {
                        logoPosition = new Vector2(95, 39);
                        introTime = -1;
                    }

                    if (introTime <= 0)
                        currentGameState = GameState.MainMenu;
                    else
                    {
                        introTime -= gameTime.ElapsedGameTime.TotalSeconds;
                        //System.Diagnostics.Debug.WriteLine("Current Time Remaining: " + introTime);

                        //Increase visibility of buttons.
                        fadeTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                        if (fadeTimer < 0)
                        {
                            alphaValue += fadeIncrement;
                        }

                        //Scroll up sky background
                        if (skyPosition.Y >= 0)
                            skyPosition -= new Vector2(0, 10);

                        //Move ground position.
                        if (groundPosition.Y >= -768)
                            groundPosition -= new Vector2(0, 5);
                        if(groundPosition.Y < 300)
                            groundPosition -= new Vector2(0, 1);

                        //Move mountains.
                        if (mountainsPosition.Y >= -768)
                            mountainsPosition -= new Vector2(0, 5);
                        
                        //Sea position.
                        if (seaPosition.Y >= -768)
                            seaPosition -= new Vector2(0, 5);
                        
                        //Move volcano.
                        if (volcanoPosition.Y >= -768)
                            volcanoPosition -= new Vector2(0, 5);

                        //Move forest into position.
                        if (forest1Position.Y < 768)
                            forest1Position += new Vector2(0, 5);
                        if (forest2Position.Y < 768)
                            forest2Position += new Vector2(0, 5);
                        if (forest3Position.Y < 768)
                            forest3Position += new Vector2(0, 5);
                        if (forest4Position.Y < 768)
                            forest4Position += new Vector2(0, 5);

                        //Move clouds into position.
                        if (cloud1Position.Y < 768)
                            cloud1Position += new Vector2(0, 4);
                        if (cloud2Position.Y < 768)
                            cloud2Position += new Vector2(0, 4);
                        if (cloud3Position.Y < 768)
                            cloud3Position += new Vector2(0, 4);
                        if (cloud4Position.Y < 768)
                            cloud4Position += new Vector2(0, 4);

                        //Logo position.
                        if (logoPosition.Y < 39)
                            logoPosition += new Vector2(0, 1);
                        else
                            logoPosition = new Vector2(95, 39);
                    }

                    if (!mainMenuSongStart)
                    {
                        try
                        {
                            gameSongStart = false;
                            creditsSongStart = false;
                            mainMenuCue.Play();
                            mainMenuSongStart = true;
                        }
                        catch (InvalidOperationException e) { }
                    }
                    break;
            }

            input.Update(gameTime);
            
            base.Update(gameTime);
            prevGamePadState = GamePad.GetState(selectedIndex);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    spriteBatch.Begin();
                    if (playedIntroduction)
                        spriteBatch.Draw(mainMenuBackground, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                    else
                    {
                        spriteBatch.Draw(finalIntroFrame, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                        spriteBatch.Draw(logo, new Rectangle((int)logoPosition.X, (int)logoPosition.Y, logo.Width, logo.Height), Color.White);
                    }

                    for (int i = 0; i < mainMenuItems.Length; i++)
                    {
                        if (i == currentMainMenuIndex)
                            mainMenuItems[i].Draw(gameTime, spriteBatch, true);
                        else
                            mainMenuItems[i].Draw(gameTime, spriteBatch, false);
                    }
                    spriteBatch.End();
                    break;

                case GameState.Instructions:                    
                    if (isPaused)
                    {
                        level.Draw(gameTime, spriteBatch);
                        spriteBatch.Begin();
                        spriteBatch.Draw(pauseOverlay, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), new Color(255, 255, 255, 200));
                        spriteBatch.End();
                        instructionsScreen.Draw(gameTime, spriteBatch);
                    }
                    else
                    {
                        spriteBatch.Begin();
                        if (playedIntroduction)
                            spriteBatch.Draw(mainMenuBackground, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                        else
                        {
                            spriteBatch.Draw(finalIntroFrame, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                        }
                        spriteBatch.Draw(pauseOverlay, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), new Color(255, 255, 255, 200));
                        spriteBatch.End();
                        instructionsScreen.Draw(gameTime, spriteBatch);
                    }                    
                    break;

                case GameState.Credits:
                    spriteBatch.Begin();
                    if (playedIntroduction)
                        spriteBatch.Draw(mainMenuBackground, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                    else
                    {
                        spriteBatch.Draw(finalIntroFrame, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                    }
                    spriteBatch.Draw(pauseOverlay, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), new Color(255, 255, 255, 200));
                    spriteBatch.End();
                    creditsScreen.Draw(gameTime, spriteBatch);
                    break;

                case GameState.InGame:
                    level.Draw(gameTime, spriteBatch);

                    foreach(UIButton a in buttons)
                    {
                        a.Draw(gameTime, spriteBatch);
                    }


                    spriteBatch.Begin();
                    if (level.Score.NeedsAdvancedNotification)
                    {
                        if (level.Score.NumAdvancedNotification == 0)
                            spriteBatch.Draw(advancedHelpIndicator0, new Rectangle(512 - (advancedHelpIndicator0.Width / 2), 384 - (advancedHelpIndicator0.Height / 2), advancedHelpIndicator0.Width, advancedHelpIndicator0.Height), Color.White);
                        else if (level.Score.NumAdvancedNotification == 1) 
                            spriteBatch.Draw(advancedHelpIndicator1, new Rectangle(512 - (advancedHelpIndicator1.Width / 2), 384 - (advancedHelpIndicator1.Height / 2), advancedHelpIndicator1.Width, advancedHelpIndicator1.Height), Color.White);
                        else if (level.Score.NumAdvancedNotification == 2)
                            spriteBatch.Draw(advancedHelpIndicator2, new Rectangle(512 - (advancedHelpIndicator2.Width / 2), 384 - (advancedHelpIndicator2.Height / 2), advancedHelpIndicator2.Width, advancedHelpIndicator2.Height), Color.White);
                            
                    }

                    if (isPaused)
                    {                        
                        spriteBatch.Draw(pauseOverlay, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), new Color(255, 255, 255, 200));
                        spriteBatch.Draw(pauseBanner, new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) - (pauseBanner.Width/2), 50, pauseBanner.Width, pauseBanner.Height), Color.White);
                        for (int i = 0; i < pauseMenuItems.Length; i++)
                        {
                            if (i == currentPauseMenuIndex)
                                pauseMenuItems[i].Draw(gameTime, spriteBatch, true);
                            else
                                pauseMenuItems[i].Draw(gameTime, spriteBatch, false);
                        }                     
                    }
                    spriteBatch.End();
                    break;

                case GameState.GameClear:
                    spriteBatch.Begin();
                    spriteBatch.Draw(mainMenuBackground, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                    spriteBatch.Draw(pauseOverlay, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), new Color(255, 255, 255, 200));
                    spriteBatch.End();
                    gameCompleteScreen.Draw(gameTime, spriteBatch);                    
                    break;

                case GameState.GameOver:
                    spriteBatch.Begin();
                    spriteBatch.Draw(mainMenuBackground, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                    spriteBatch.Draw(pauseOverlay, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), new Color(255, 255, 255, 200));
                    spriteBatch.End();
                    gameOverScreen.Draw(gameTime, spriteBatch);
                    break;
                case GameState.Introduction:
                    spriteBatch.Begin();
                    spriteBatch.Draw(sky, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height),
                        new Rectangle(0, (int)skyPosition.Y, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                    spriteBatch.Draw(mountains, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height),
                        new Rectangle((int)mountainsPosition.X, (int)mountainsPosition.Y, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                    spriteBatch.Draw(sea, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height),
                        new Rectangle((int)seaPosition.X, (int)seaPosition.Y, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                    spriteBatch.Draw(volcano, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height),
                        new Rectangle((int)volcanoPosition.X, (int)volcanoPosition.Y, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);
                    
                    //Draw forests.    
                    spriteBatch.Draw(forest1, new Rectangle(0, (int)forest1Position.Y, forest1.Width, forest1.Height), Color.White);
                    spriteBatch.Draw(forest2, new Rectangle(0, (int)forest2Position.Y, forest2.Width, forest2.Height), Color.White);
                    spriteBatch.Draw(forest3, new Rectangle(0, (int)forest3Position.Y, forest3.Width, forest3.Height), Color.White);
                    spriteBatch.Draw(forest4, new Rectangle(0, (int)forest4Position.Y, forest4.Width, forest4.Height), Color.White);

                    //Draw clouds.
                    spriteBatch.Draw(cloud1, new Rectangle((int)cloud1Position.X, (int)cloud1Position.Y, cloud1.Width, cloud1.Height), Color.White);
                    spriteBatch.Draw(cloud2, new Rectangle((int)cloud2Position.X, (int)cloud2Position.Y, cloud2.Width, cloud2.Height), Color.White);
                    spriteBatch.Draw(cloud3, new Rectangle((int)cloud3Position.X, (int)cloud3Position.Y, cloud3.Width, cloud3.Height), Color.White);
                    spriteBatch.Draw(cloud4, new Rectangle((int)cloud4Position.X, (int)cloud4Position.Y, cloud4.Width, cloud4.Height), Color.White);

                    //Draw the ground.
                    spriteBatch.Draw(ground, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height),
                        new Rectangle(0, (int)groundPosition.Y, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height), Color.White);

                    //Draw the logo.
                    spriteBatch.Draw(logo, new Rectangle((int)logoPosition.X, (int)logoPosition.Y, logo.Width, logo.Height), Color.White);
                    spriteBatch.End();

                    if (fadeTimer < 0)
                    {
                        spriteBatch.Begin();
                        foreach (MenuSelection a in mainMenuItems)
                        {                            
                            a.Draw(gameTime, spriteBatch, alphaValue);                            
                        }
                        spriteBatch.End();
                    }
                    break;
            }

            base.Draw(gameTime);
        }

        public void ManageMainMenu()
        {
            //Find out where the mouse currently is at, change selection accordingly

#if !WINDOWS
            if (currentMainMenuIndex == -1) currentMainMenuIndex = 0;
            if (ButtonJustPressed(Buttons.DPadDown) ||
                GamePad.GetState(selectedIndex).ThumbSticks.Left.Y < -0.3f && ThumbstickJustTilted(false) ||
                GamePad.GetState(selectedIndex).ThumbSticks.Right.Y < -0.3f && ThumbstickJustTilted(true)) {
                currentMainMenuIndex = (currentMainMenuIndex + 1) % mainMenuItems.Length;
            }
            if (ButtonJustPressed(Buttons.DPadUp) ||
                GamePad.GetState(selectedIndex).ThumbSticks.Left.Y > 0.3f && ThumbstickJustTilted(false) ||
                GamePad.GetState(selectedIndex).ThumbSticks.Right.Y > 0.3f && ThumbstickJustTilted(true)) {
                currentMainMenuIndex = (currentMainMenuIndex + mainMenuItems.Length - 1) % mainMenuItems.Length;
            }
#else

            bool hitMenu = false;
            for (int i = 0; i < mainMenuItems.Length; i++) {
                MenuSelection z = mainMenuItems[i];
                MouseState currentMouseState = Mouse.GetState();

                if (currentMouseState.Y < z.GetMouseSelectionArea().Bottom
                    && currentMouseState.Y > z.GetMouseSelectionArea().Top
                    && currentMouseState.X < z.GetMouseSelectionArea().Right
                    && currentMouseState.X > z.GetMouseSelectionArea().Left) {
                    currentMainMenuIndex = i;
                    hitMenu = true;
                    break;
                }
            }
            if (!hitMenu) currentMainMenuIndex = -1;
#endif

            //When the mouse clicks, pick the selection.
            if (input.ButtonJustPressed(UserInput.MouseButton.Left) || ButtonJustPressed(Buttons.A) && currentMainMenuIndex != -1)
            {
                sfx_menuSelection.Play();
                string selection = mainMenuItems[currentMainMenuIndex].GetTitle();
                if (selection == "Play")
                {
                    mainMenuCue.Stop(AudioStopOptions.Immediate);
                    mainMenuSongStart = false;
                    level = new Level(Services, graphics);
                    if (!playedIntroduction)
                        playedIntroduction = true;
                    currentGameState = GameState.InGame;
                }
                else if (selection == "Instructions")
                {
                    currentGameState = GameState.Instructions;
                }
                else if (selection == "Exit")
                {
                    this.Exit();
                }
                else
                {
                    mainMenuCue.Stop(AudioStopOptions.Immediate);
                    mainMenuSongStart = false;
                    currentGameState = GameState.Credits;
                }
            }
        }

        private void ManageUI(GameTime gameTime)
        {
            foreach (UIButton a in buttons)
            {
                a.Update(gameTime);                                             
            }
        }
        
        private void UpdateGame(GameTime gameTime)
        {
            if (input.KeyJustPressed(Keys.Escape) || ButtonJustPressed(Buttons.Back) || ButtonJustPressed(Buttons.Start))
                IsPaused = !IsPaused;

            if (isPaused)
            {
                //Find out where the mouse currently is at, change selection accordingly

#if !WINDOWS
                if (currentPauseMenuIndex == -1) currentPauseMenuIndex = 0;
                if (ButtonJustPressed(Buttons.DPadDown) ||
                    GamePad.GetState(selectedIndex).ThumbSticks.Left.Y < -0.3f && ThumbstickJustTilted(false) ||
                    GamePad.GetState(selectedIndex).ThumbSticks.Right.Y < -0.3f && ThumbstickJustTilted(true)) {
                        currentPauseMenuIndex = (currentPauseMenuIndex + 1) % pauseMenuItems.Length;
                }
                if (ButtonJustPressed(Buttons.DPadUp) ||
                    GamePad.GetState(selectedIndex).ThumbSticks.Left.Y > 0.3f && ThumbstickJustTilted(false) ||
                    GamePad.GetState(selectedIndex).ThumbSticks.Right.Y > 0.3f && ThumbstickJustTilted(true)) {
                        currentPauseMenuIndex = (currentPauseMenuIndex + pauseMenuItems.Length - 1) % pauseMenuItems.Length;
                }
#else


                bool hitMenu = false;
                for (int i = 0; i < pauseMenuItems.Length; i++)
                {
                    MenuSelection z = pauseMenuItems[i];
                    MouseState currentMouseState = Mouse.GetState();

                    if (currentMouseState.Y < z.GetMouseSelectionArea().Bottom
                        && currentMouseState.Y > z.GetMouseSelectionArea().Top
                        && currentMouseState.X < z.GetMouseSelectionArea().Right
                        && currentMouseState.X > z.GetMouseSelectionArea().Left)
                    {
                        currentPauseMenuIndex = i;
                        hitMenu = true;
                    }
                }
                if (!hitMenu) currentPauseMenuIndex = -1;
#endif
                //When the mouse clicks, pick the selection.
                if ((input.ButtonJustPressed(UserInput.MouseButton.Left) || ButtonJustPressed(Buttons.A)) && currentPauseMenuIndex != -1)
                {
                    sfx_menuSelection.Play();
                    string selection = pauseMenuItems[currentPauseMenuIndex].GetTitle();
                    if (selection == "Resume Game")
                    {
                        IsPaused = false;
                    }
                    else if (selection == "Help")
                    {
                        currentGameState = GameState.Instructions;
                    }
                    else if (selection == "Return to Title")
                    {
                        level.Dispose();
                        level = null;
                        currentGameState = GameState.MainMenu;
                        IsPaused = false;
                        inGameCue.Stop(AudioStopOptions.Immediate);
                    }
                    else
                    {
                        this.Exit();
                    }
                }
            }
            else if (level.Score.NeedsAdvancedNotification)
            {
                if (input.ButtonJustPressed(UserInput.MouseButton.Left) || SageGame.PressedAnything())
                {
                    level.Score.SawAdvancedNotification = true;
                    level.Score.NeedsAdvancedNotification = false;
                    //System.Diagnostics.Debug.WriteLine("Stuff should now be not drawn");
                }
            }
            else
            {
                ManageUI(gameTime);
                if (level != null)
                {
                    level.Update(gameTime);
                }
            }
        }

        public static bool PressedAnything() {
            if (ButtonJustPressed(Buttons.A)
                        || ButtonJustPressed(Buttons.B)
                        || ButtonJustPressed(Buttons.Back)
                        || ButtonJustPressed(Buttons.BigButton)
                        || ButtonJustPressed(Buttons.DPadDown)
                        || ButtonJustPressed(Buttons.DPadLeft)
                        || ButtonJustPressed(Buttons.DPadRight)
                        || ButtonJustPressed(Buttons.DPadUp)
                        || ButtonJustPressed(Buttons.LeftShoulder)
                        || ButtonJustPressed(Buttons.LeftStick)
                        || ButtonJustPressed(Buttons.LeftTrigger)
                        || ButtonJustPressed(Buttons.RightShoulder)
                        || ButtonJustPressed(Buttons.RightStick)
                        || ButtonJustPressed(Buttons.Start)
                        || ButtonJustPressed(Buttons.X)
                        || ButtonJustPressed(Buttons.Y)) {
                return true;
            }
            return false;
        }

        public static bool ButtonJustPressed(Buttons button) {
            return prevGamePadState.IsButtonUp(button) && GamePad.GetState(selectedIndex).IsButtonDown(button);
        }

        public static bool ButtonJustReleased(Buttons button) {
            return prevGamePadState.IsButtonDown(button) && GamePad.GetState(selectedIndex).IsButtonUp(button);
        }

        public static Vector2 GetGameLoc() {
            Vector2 cursorPosition = Vector2.Zero;
            if (gameSelectedIndex < 4) {
                cursorPosition.Y = 482;
                if (gameSelectedIndex == 0) {
                    cursorPosition.X = 171;
                } else if (gameSelectedIndex == 1) {
                    cursorPosition.X = 347;
                } else if (gameSelectedIndex == 2) {
                    cursorPosition.X = 626;
                } else if (gameSelectedIndex == 3) {
                    cursorPosition.X = 855;
                }
            } else if (gameSelectedIndex < 8) {
                cursorPosition.Y = 603;
                if (gameSelectedIndex == 4) {
                    cursorPosition.X = 171;
                } else if (gameSelectedIndex == 5) {
                    cursorPosition.X = 347;
                } else if (gameSelectedIndex == 6) {
                    cursorPosition.X = 626;
                } else if (gameSelectedIndex == 7) {
                    cursorPosition.X = 855;
                }
            } else {
                if (gameSelectedIndex == 8) {
                    cursorPosition.X = 95;
                    cursorPosition.Y = 145;
                } else if (gameSelectedIndex == 9) {
                    cursorPosition.X = 165;
                    cursorPosition.Y = 275;
                } else if (gameSelectedIndex == 10) {
                    cursorPosition.X = 861;
                    cursorPosition.Y = 275;
                } else if (gameSelectedIndex == 11) {
                    cursorPosition.X = 931;
                    cursorPosition.Y = 145;
                }
            }
            return cursorPosition;
        }

        public static void MoveGameLoc(bool horiz, bool down, bool right, Level level) {
            if (!horiz) {
                if (down) {
                    gameSelectedIndex = (gameSelectedIndex + 4) % 12;
                } else {
                    gameSelectedIndex = (gameSelectedIndex + 8) % 12;
                }
            } else {
                if (right) {
                    gameSelectedIndex = (gameSelectedIndex + 1) % 4 + gameSelectedIndex / 4*4;
                } else {
                    gameSelectedIndex = (gameSelectedIndex + 3) % 4 + gameSelectedIndex / 4*4;
                }
            }
            if (gameSelectedIndex >= 8) {
                int index = gameSelectedIndex - 8;
                bool miss = false;
                if (index == 0 && !level.saveOne.active) {
                    miss = true;
                } else if (index == 1 && !level.saveTwo.active) {
                    miss = true;
                } else if (index == 2 && !level.saveFour.active) {
                    miss = true;
                } else if (index == 3 && !level.saveThree.active) {
                    miss = true;
                }
                while (miss) {
                    if (!horiz) {
                        if (down) {
                            gameSelectedIndex = (gameSelectedIndex + 4) % 12;
                        } else {
                            gameSelectedIndex = (gameSelectedIndex + 8) % 12;
                        }
                    } else {
                        if (right) {
                            gameSelectedIndex = (gameSelectedIndex + 1) % 4 + gameSelectedIndex / 4 * 4;
                        } else {
                            gameSelectedIndex = (gameSelectedIndex + 3) % 4 + gameSelectedIndex / 4 * 4;
                        }
                    }

                    if (gameSelectedIndex < 8) {
                        miss = false;
                    } else {
                        index = gameSelectedIndex - 8;

                        miss = false;
                        if (index == 0 && !level.saveOne.active) {
                            miss = true;
                        } else if (index == 1 && !level.saveTwo.active) {
                            miss = true;
                        } else if (index == 2 && !level.saveFour.active) {
                            miss = true;
                        } else if (index == 3 && !level.saveThree.active) {
                            miss = true;
                        }
                    }
                    
                }

            }
        }

        public static bool MovedDown() {
            if (ButtonJustPressed(Buttons.DPadDown) ||
                    GamePad.GetState(selectedIndex).ThumbSticks.Left.Y < -0.3f && ThumbstickJustTilted(false) ||
                    GamePad.GetState(selectedIndex).ThumbSticks.Right.Y < -0.3f && ThumbstickJustTilted(true)) {
                return true;
            }
            return false;
        }

        public static bool MovedUp() {
            if (ButtonJustPressed(Buttons.DPadUp) ||
                    GamePad.GetState(selectedIndex).ThumbSticks.Left.Y > 0.3f && ThumbstickJustTilted(false) ||
                    GamePad.GetState(selectedIndex).ThumbSticks.Right.Y > 0.3f && ThumbstickJustTilted(true)) {
                return true;
            }
            return false;
        }

        public static bool MovedLeft() {
            if (ButtonJustPressed(Buttons.DPadLeft) ||
                    GamePad.GetState(selectedIndex).ThumbSticks.Left.X < -0.3f && ThumbstickJustTilted(false) ||
                    GamePad.GetState(selectedIndex).ThumbSticks.Right.X < -0.3f && ThumbstickJustTilted(true)) {
                return true;
            }
            return false;
        }

        public static bool MovedRight() {
            if (ButtonJustPressed(Buttons.DPadRight) ||
                    GamePad.GetState(selectedIndex).ThumbSticks.Left.X > 0.3f && ThumbstickJustTilted(false) ||
                    GamePad.GetState(selectedIndex).ThumbSticks.Right.X > 0.3f && ThumbstickJustTilted(true)) {
                return true;
            }
            return false;
        }

        public static bool ThumbstickJustTilted(bool right) {
            if (right) {
                return (Math.Abs(prevGamePadState.ThumbSticks.Right.X) <= 0.3f && Math.Abs(GamePad.GetState(selectedIndex).ThumbSticks.Right.X) > 0.3f ||
                    Math.Abs(prevGamePadState.ThumbSticks.Right.Y) <= 0.3f && Math.Abs(GamePad.GetState(selectedIndex).ThumbSticks.Right.Y) > 0.3f);
            } else {
                return (Math.Abs(prevGamePadState.ThumbSticks.Left.X) <= 0.3f && Math.Abs(GamePad.GetState(selectedIndex).ThumbSticks.Left.X) > 0.3f ||
                    Math.Abs(prevGamePadState.ThumbSticks.Left.Y) <= 0.3f && Math.Abs(GamePad.GetState(selectedIndex).ThumbSticks.Left.Y) > 0.3f);
            }
        }
    }


    class MenuSelection
    {
        private string title;
        private Texture2D iconTexture;
        private Rectangle mouseSelectionBox;
        private Vector2 position;
        private int centerTextureWidth;
        private SpriteFont textFont;

        Texture2D iconTextureDimL;
        Texture2D iconTextureDimR;
        Texture2D iconTextureDimC;
        Texture2D iconTextureLitL;
        Texture2D iconTextureLitR;
        Texture2D iconTextureLitC;

        public MenuSelection(string title, Texture2D iconTexture, int x, int y)
        {
            this.title = title;
            this.iconTexture = iconTexture;
            this.mouseSelectionBox = new Rectangle(x, y, iconTexture.Width, iconTexture.Height);
        }

        public MenuSelection(string title,
            Texture2D iconTextureDimL, Texture2D iconTextureDimR, Texture2D iconTextureDimC,
            Texture2D iconTextureLitL, Texture2D iconTextureLitR, Texture2D iconTextureLitC,
            int x, int y, int centerTextureWidth, SpriteFont textFont)
        {
            this.title = title;
            this.iconTextureDimL = iconTextureDimL;
            this.iconTextureDimR = iconTextureDimR;
            this.iconTextureDimC = iconTextureDimC;
            this.iconTextureLitL = iconTextureLitL;
            this.iconTextureLitR = iconTextureLitR;
            this.iconTextureLitC = iconTextureLitC;
            this.mouseSelectionBox = new Rectangle(x, y, 
                iconTextureDimL.Width + iconTextureDimR.Width + centerTextureWidth, 
                iconTextureDimC.Height);
            this.position = new Vector2(x, y);
            this.centerTextureWidth = centerTextureWidth;
            this.textFont = textFont;
        }

        public string GetTitle()
        {
            return title;
        }

        public Texture2D GetTexture()
        {
            return iconTexture;
        }

        public Rectangle GetMouseSelectionArea()
        {
            return mouseSelectionBox;
        }

        

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, bool isSelected)
        {
            if (isSelected)
            {
                spriteBatch.Draw(iconTextureLitL, new Rectangle((int)position.X, (int)position.Y, iconTextureLitL.Width, iconTextureLitL.Height), Color.White);
                spriteBatch.Draw(iconTextureLitC, new Rectangle((int)position.X + iconTextureLitL.Width, (int)position.Y, centerTextureWidth, iconTextureLitC.Height), Color.White);
                spriteBatch.Draw(iconTextureLitR, new Rectangle((int)position.X + iconTextureLitL.Width + centerTextureWidth, (int)position.Y, iconTextureLitR.Width, iconTextureLitR.Height), Color.White);
                spriteBatch.DrawString(textFont, title, 
                    new Vector2((int)position.X + + iconTextureLitL.Width +(centerTextureWidth / 2) - textFont.MeasureString(title).X /2, 
                        (int)position.Y + (iconTextureLitC.Height/10)), 
                        Color.Black);
            }
            else
            {
                spriteBatch.Draw(iconTextureDimL, new Rectangle((int)position.X, (int)position.Y, iconTextureDimL.Width, iconTextureDimL.Height), Color.White);
                spriteBatch.Draw(iconTextureDimC, new Rectangle((int)position.X + iconTextureDimL.Width, (int)position.Y, centerTextureWidth, iconTextureDimC.Height), Color.White);
                spriteBatch.Draw(iconTextureDimR, new Rectangle((int)position.X + iconTextureDimL.Width + centerTextureWidth, (int)position.Y, iconTextureLitR.Width, iconTextureDimR.Height), Color.White);
                spriteBatch.DrawString(textFont, title, 
                    new Vector2((int)position.X + iconTextureDimL.Width + (centerTextureWidth / 2) - textFont.MeasureString(title).X / 2, 
                        (int)position.Y + (iconTextureLitC.Height / 10)), 
                    Color.Black);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, int alphaValue)
        {
            spriteBatch.Draw(iconTextureDimL, new Rectangle((int)position.X, (int)position.Y, iconTextureLitL.Width, iconTextureLitL.Height), new Color(255, 255, 255, (byte)MathHelper.Clamp(alphaValue, 0, 255)));
            spriteBatch.Draw(iconTextureDimC, new Rectangle((int)position.X + iconTextureLitL.Width, (int)position.Y, centerTextureWidth, iconTextureLitC.Height), new Color(255, 255, 255, (byte)MathHelper.Clamp(alphaValue, 0, 255)));
            spriteBatch.Draw(iconTextureDimR, new Rectangle((int)position.X + iconTextureLitL.Width + centerTextureWidth, (int)position.Y, iconTextureLitR.Width, iconTextureLitR.Height), new Color(255, 255, 255, (byte)MathHelper.Clamp(alphaValue, 0, 255)));
            spriteBatch.DrawString(textFont, title,
                new Vector2((int)position.X + +iconTextureLitL.Width + (centerTextureWidth / 2) - textFont.MeasureString(title).X / 2,
                    (int)position.Y + (iconTextureLitC.Height / 10)),
                    new Color(0, 0, 0, (byte)MathHelper.Clamp(alphaValue, 0, 255)));
        }
    }
}
