using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace sagescroll
{
    public class UIButton : IDisplayableItem
    {
        public bool actedOn { get; set; }
        public delegate void UIButtonEvent(UIButton source);
        public UIButtonEvent OnClick;

        Texture2D litTexture;
        Texture2D dimTexture;
        Texture2D currentTexture;
        Texture2D xboxTexture;
        Rectangle mouseSelectionArea;
        Vector2 position;
        UserInput input;
        SageGame sageGame;
        bool m_bPaused;
        static SpriteFont m_Font;

        //Add more types to the Enum as necessary.
        public enum Button { pauseButton, castButton, clearButton, castallButton, clearallButton }
        public Button ButtonType
        {
            get { return buttonType; }
        }
        Button buttonType;
        int slot;

        public UIButton(Vector2 position, ContentManager content, Button button, bool bDisableOnPause, int index)
        {
            this.position = position;
            this.input = UserInput.GetUserInput();
            this.buttonType = button;
            this.slot = index;
            m_bPaused = false;
            SageGame.OnPause += Game_OnPause;
            SageGame.OnUnpause += Game_OnUnpause;
            LoadContent(content);

            //the offsets make up for drop-shaddow on most buttons
            mouseSelectionArea = new Rectangle((int)position.X + 11, (int)position.Y, currentTexture.Width - 11, currentTexture.Height - 12);
        }

        public void Game_OnPause()
        {
            m_bPaused = true;
        }

        public void Game_OnUnpause()
        {
            m_bPaused = false;
        }

        public void LoadContent(ContentManager content)
        {
            switch (buttonType)
            {
                case Button.pauseButton:
                    litTexture = content.Load<Texture2D>("UI/pause-lit");
                    dimTexture = content.Load<Texture2D>("UI/pause-dim");
                    xboxTexture = content.Load<Texture2D>("UI/XBox/xboxControllerStart");
                    break;
                case Button.clearButton:
                    litTexture = content.Load<Texture2D>("UI/reset-single-lit");
                    dimTexture = content.Load<Texture2D>("UI/reset-single-dim");
                    xboxTexture = content.Load<Texture2D>("UI/XBox/xboxControllerButtonB");
                    break;
                case Button.castButton:
                    litTexture = content.Load<Texture2D>("UI/cast-single-lit");
                    dimTexture = content.Load<Texture2D>("UI/cast-single-dim");
                    xboxTexture = content.Load<Texture2D>("UI/XBox/xboxControllerButtonX");
                    break;
                case Button.clearallButton:
                    litTexture = content.Load<Texture2D>("UI/reset-all-lit");
                    dimTexture = content.Load<Texture2D>("UI/reset-all-dim");
                    xboxTexture = content.Load<Texture2D>("UI/XBox/xboxControllerLeftTrigger");
                    if (m_Font == null)
                        m_Font = content.Load<SpriteFont>("UI/Lithos Pro Regular Score");
                    break;
                case Button.castallButton:
                    litTexture = content.Load<Texture2D>("UI/cast-all-lit");
                    dimTexture = content.Load<Texture2D>("UI/cast-all-dim");
                    xboxTexture = content.Load<Texture2D>("UI/XBox/xboxControllerRightTrigger");
                    if (m_Font == null)
                        m_Font = content.Load<SpriteFont>("UI/Lithos Pro Regular Score");
                    break;
            }
            currentTexture = dimTexture;
        }

        public void Dispose()
        {
            SageGame.OnPause -= Game_OnPause;
            SageGame.OnUnpause -= Game_OnUnpause;
            sageGame = null;
        }

        public void Update(GameTime gametime)
        {
            Point mouseCoord = new Point(Mouse.GetState().X,Mouse.GetState().Y);
            Rectangle mouseRec = new Rectangle(mouseCoord.X, mouseCoord.Y, 1, 1);

#if !WINDOWS
            if (SageGame.ButtonJustPressed(Buttons.X) && buttonType == Button.castButton
                || SageGame.ButtonJustPressed(Buttons.B) && buttonType == Button.clearButton) {
                if (slot == SageGame.gameSelectedIndex) {
                    ButtonPressResponse();
                }
            }

#else
            if (!m_bPaused && mouseRec.Intersects(mouseSelectionArea))
            {
                currentTexture = litTexture;
                if (input.ButtonJustPressed(UserInput.MouseButton.Left))
                {
                    ButtonPressResponse();
                    //System.Diagnostics.Debug.WriteLine("Button Clicked");       
                }
            }
            else
            {
                currentTexture = dimTexture;
            }
#endif

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(currentTexture, new Rectangle((int)position.X, (int)position.Y, currentTexture.Width, currentTexture.Height), Color.White);
            if (buttonType == Button.clearallButton || buttonType == Button.castallButton)
            {
                string sText = (buttonType == Button.castallButton ? "Cast All" : "Reset All");
                Color cText = (buttonType == Button.castallButton ? new Color(55, 64, 7) : new Color(60, 31, 4));
                spriteBatch.DrawString(m_Font, sText, new Vector2(mouseSelectionArea.X, mouseSelectionArea.Y) + (new Vector2(mouseSelectionArea.Width, mouseSelectionArea.Height) - m_Font.MeasureString(sText)) / 2, cText);
            }
#if !WINDOWS
            if (buttonType == Button.castallButton) {
                spriteBatch.Draw(xboxTexture, new Rectangle((int)position.X + 240, (int)position.Y - 8, 40, 80), Color.White);
            } else if (buttonType == Button.clearallButton) {
                spriteBatch.Draw(xboxTexture, new Rectangle((int)position.X - 50, (int)position.Y - 8, 40, 80), Color.White);
            } else if (buttonType == Button.pauseButton) {
                spriteBatch.Draw(xboxTexture, new Rectangle((int)position.X - 60, (int)position.Y - 8, 64, 58), Color.White);
            } else if (buttonType == Button.clearButton) {
                spriteBatch.Draw(xboxTexture, new Rectangle((int)position.X - 42, (int)position.Y, 40, 40), Color.White);
            } else if (buttonType == Button.castButton) {
                spriteBatch.Draw(xboxTexture, new Rectangle((int)position.X - 42, (int)position.Y, 40, 40), Color.White);
            }
#endif

            spriteBatch.End();
        }

        private void ButtonPressResponse( )
        {
            if (OnClick != null)
                OnClick(this);
        }
    }
}
