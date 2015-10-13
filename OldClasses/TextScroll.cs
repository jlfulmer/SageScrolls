using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace sagescroll
{
    class TextScroll
    {
        SpriteFont textFont;
        Texture2D scrollBackground;
        Vector2 scrollPosition;
        UserInput input;

        Texture2D iconDimL;
        Texture2D iconDimR;
        Texture2D iconDimC;
        Texture2D iconLitL;
        Texture2D iconLitR;
        Texture2D iconLitC;
        int centerIconWidth;
        Vector2 iconPosition;
        Rectangle mouseSelectionBox;
        bool isMouseSelected;

        public bool IsMouseSelectedAndClicked
        {
            get { return isMouseSelectedAndClicked; }
        }
        bool isMouseSelectedAndClicked;

        public TextScroll(ContentManager content, UserInput input, String texture)
        {
            this.input = input;
            LoadContent(content, texture);
        }

        public void LoadContent(ContentManager content, String texture)
        {
            scrollBackground = content.Load<Texture2D>(texture);
            scrollPosition = new Vector2(512 - (scrollBackground.Width / 2),
                -scrollBackground.Height);

            textFont = content.Load<SpriteFont>("UI/Lithos Pro Regular Menu");

            iconDimL = content.Load<Texture2D>("UI/button-dim-left");
            iconDimR = content.Load<Texture2D>("UI/button-dim-right");
            iconDimC = content.Load<Texture2D>("UI/button-dim-middle");
            iconLitL = content.Load<Texture2D>("UI/button-lit-left");
            iconLitR = content.Load<Texture2D>("UI/button-lit-right");
            iconLitC = content.Load<Texture2D>("UI/button-lit-middle");

            centerIconWidth = 150;
            iconPosition = new Vector2(745, 670);
            this.mouseSelectionBox = new Rectangle((int)iconPosition.X, (int)iconPosition.Y,
                iconDimL.Width + iconDimR.Width + centerIconWidth,
                iconDimC.Height);
            isMouseSelected = false;
            isMouseSelectedAndClicked = false;
        }

        public void Update(GameTime gameTime)
        {
            // Scrolling the page down.
            if (input.ButtonJustPressed(UserInput.MouseButton.Left) || SageGame.PressedAnything())
                scrollPosition.Y = 0;

            if (scrollPosition.Y < -20)
                scrollPosition += new Vector2(0, 20);
            else
                scrollPosition.Y = 0;

            if (Mouse.GetState().X < mouseSelectionBox.Right &&
                Mouse.GetState().X > mouseSelectionBox.Left &&
                Mouse.GetState().Y < mouseSelectionBox.Bottom &&
                Mouse.GetState().Y > mouseSelectionBox.Top)
            {
                isMouseSelected = true;
            }
            else
            {
                isMouseSelected = false;
            }

            if ((input.ButtonJustPressed(UserInput.MouseButton.Left) || SageGame.PressedAnything()) && scrollPosition.Y == 0)
            {
                if (isMouseSelected)
                    isMouseSelectedAndClicked = true;
            }
           
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(scrollBackground, new Rectangle(
                            (int)scrollPosition.X, (int)scrollPosition.Y,
                            scrollBackground.Width, scrollBackground.Height), Color.White);
#if WINDOWS
            if (isMouseSelected)
            {
                spriteBatch.Draw(iconLitL, new Rectangle((int)iconPosition.X, (int)iconPosition.Y, iconLitL.Width, iconLitL.Height), Color.White);
                spriteBatch.Draw(iconLitC, new Rectangle((int)iconPosition.X + iconLitL.Width, (int)iconPosition.Y, centerIconWidth, iconLitC.Height), Color.White);
                spriteBatch.Draw(iconLitR, new Rectangle((int)iconPosition.X + iconLitL.Width + centerIconWidth, (int)iconPosition.Y, iconLitR.Width, iconLitR.Height), Color.White);
            }
            else
            {
                spriteBatch.Draw(iconDimL, new Rectangle((int)iconPosition.X, (int)iconPosition.Y, iconDimL.Width, iconDimL.Height), Color.White);
                spriteBatch.Draw(iconDimC, new Rectangle((int)iconPosition.X + iconDimL.Width, (int)iconPosition.Y, centerIconWidth, iconDimC.Height), Color.White);
                spriteBatch.Draw(iconDimR, new Rectangle((int)iconPosition.X + iconDimL.Width + centerIconWidth, (int)iconPosition.Y, iconDimR.Width, iconDimR.Height), Color.White);
            }
            spriteBatch.DrawString(textFont, "Done",
                    new Vector2((int)iconPosition.X + iconDimL.Width + (centerIconWidth / 2) - textFont.MeasureString("Done").X / 2,
                        (int)iconPosition.Y + (iconDimC.Height / 10)),
                        Color.Black);
#endif
            spriteBatch.End();
        }

        public void ResetPosition()
        {
            scrollPosition.Y = -scrollBackground.Height;
            isMouseSelected = false;
            isMouseSelectedAndClicked = false;
        }
    }
}
