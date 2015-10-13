using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace sagescroll
{
    class NotificationScroll
    {
        SpriteFont textFont;
        Texture2D scrollBackground;
        Vector2 scrollPosition;
        UserInput input;
        String title;

        Texture2D iconDimL;
        Texture2D iconDimR;
        Texture2D iconDimC;
        Texture2D iconLitL;
        Texture2D iconLitR;
        Texture2D iconLitC;
        int centerIconWidth;

        Vector2 iconPosition1;
        Rectangle mouseSelectionBox1;
        bool isMouseSelected1;
        public bool IsMouseSelectedAndClicked1
        {
            get { return isMouseSelectedAndClicked1; }
        }
        bool isMouseSelectedAndClicked1;

        Vector2 iconPosition2;
        Rectangle mouseSelectionBox2;
        bool isMouseSelected2;
        public bool IsMouseSelectedAndClicked2
        {
            get { return isMouseSelectedAndClicked2; }
        }
        bool isMouseSelectedAndClicked2;

        Vector2 textPosition;

        public NotificationScroll(ContentManager content, UserInput input, String texture, string title)
        {
            this.input = input;
            this.title = title;
            LoadContent(content, texture);
        }

        public void LoadContent(ContentManager content, String texture)
        {
            scrollBackground = content.Load<Texture2D>(texture);
            scrollPosition = new Vector2(512 - (scrollBackground.Width / 2), 0);

            textFont = content.Load<SpriteFont>("UI/Lithos Pro Regular Menu");

            iconDimL = content.Load<Texture2D>("UI/button-dim-left");
            iconDimR = content.Load<Texture2D>("UI/button-dim-right");
            iconDimC = content.Load<Texture2D>("UI/button-dim-middle");
            iconLitL = content.Load<Texture2D>("UI/button-lit-left");
            iconLitR = content.Load<Texture2D>("UI/button-lit-right");
            iconLitC = content.Load<Texture2D>("UI/button-lit-middle");

            textPosition = new Vector2((1024 / 2) - (textFont.MeasureString(title).X/2), 200);

            centerIconWidth = 350;
            iconPosition1 = new Vector2((1024 / 2) - ((iconDimL.Width + iconLitR.Width + centerIconWidth) / 2), 285);
            this.mouseSelectionBox1 = new Rectangle((int)iconPosition1.X, (int)iconPosition1.Y,
                iconDimL.Width + iconDimR.Width + centerIconWidth,
                iconDimC.Height);
            isMouseSelected1 = false;
            isMouseSelectedAndClicked1 = false;

            iconPosition2 = new Vector2((1024 / 2) - ((iconDimL.Width + iconLitR.Width + centerIconWidth) / 2), 385);
            this.mouseSelectionBox2 = new Rectangle((int)iconPosition2.X, (int)iconPosition2.Y,
                iconDimL.Width + iconDimR.Width + centerIconWidth,
                iconDimC.Height);
            isMouseSelected2 = false;
            isMouseSelectedAndClicked1 = false;
        }

        public void Update(GameTime gameTime)
        {
            // Icon 1
            if (Mouse.GetState().X < mouseSelectionBox1.Right &&
                Mouse.GetState().X > mouseSelectionBox1.Left &&
                Mouse.GetState().Y < mouseSelectionBox1.Bottom &&
                Mouse.GetState().Y > mouseSelectionBox1.Top)
            {
                isMouseSelected1 = true;
            }
            else
            {
                isMouseSelected1 = false;
            }

            if ((input.ButtonJustPressed(UserInput.MouseButton.Left) || SageGame.PressedAnything()) && scrollPosition.Y == 0)
            {
                if (isMouseSelected1)
                    isMouseSelectedAndClicked1 = true;
            }
           
            // Icon 2
            if (Mouse.GetState().X < mouseSelectionBox2.Right &&
                Mouse.GetState().X > mouseSelectionBox2.Left &&
                Mouse.GetState().Y < mouseSelectionBox2.Bottom &&
                Mouse.GetState().Y > mouseSelectionBox2.Top)
            {
                isMouseSelected2 = true;
            }
            else
            {
                isMouseSelected2 = false;
            }

            if ((input.ButtonJustPressed(UserInput.MouseButton.Left) || SageGame.PressedAnything()) && scrollPosition.Y == 0)
            {
                if (isMouseSelected2)
                    isMouseSelectedAndClicked2 = true;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(scrollBackground, new Rectangle(
                            (int)scrollPosition.X, (int)scrollPosition.Y,
                            scrollBackground.Width, scrollBackground.Height), Color.White);
            spriteBatch.DrawString(textFont, title, textPosition, Color.Black);

            //Icon 1
            if (isMouseSelected1)
            {
                spriteBatch.Draw(iconLitL, new Rectangle((int)iconPosition1.X, (int)iconPosition1.Y, iconLitL.Width, iconLitL.Height), Color.White);
                spriteBatch.Draw(iconLitC, new Rectangle((int)iconPosition1.X + iconLitL.Width, (int)iconPosition1.Y, centerIconWidth, iconLitC.Height), Color.White);
                spriteBatch.Draw(iconLitR, new Rectangle((int)iconPosition1.X + iconLitL.Width + centerIconWidth, (int)iconPosition1.Y, iconLitR.Width, iconLitR.Height), Color.White);
            }
            else
            {
                spriteBatch.Draw(iconDimL, new Rectangle((int)iconPosition1.X, (int)iconPosition1.Y, iconDimL.Width, iconDimL.Height), Color.White);
                spriteBatch.Draw(iconDimC, new Rectangle((int)iconPosition1.X + iconDimL.Width, (int)iconPosition1.Y, centerIconWidth, iconDimC.Height), Color.White);
                spriteBatch.Draw(iconDimR, new Rectangle((int)iconPosition1.X + iconDimL.Width + centerIconWidth, (int)iconPosition1.Y, iconDimR.Width, iconDimR.Height), Color.White);
            }
            spriteBatch.DrawString(textFont, "Play Again",
                    new Vector2((int)iconPosition1.X + iconDimL.Width + (centerIconWidth / 2) - textFont.MeasureString("Play Again").X / 2,
                        (int)iconPosition1.Y + (iconDimC.Height / 10)),
                        Color.Black);

            //Icon 2
            if (isMouseSelected2)
            {
                spriteBatch.Draw(iconLitL, new Rectangle((int)iconPosition2.X, (int)iconPosition2.Y, iconLitL.Width, iconLitL.Height), Color.White);
                spriteBatch.Draw(iconLitC, new Rectangle((int)iconPosition2.X + iconLitL.Width, (int)iconPosition2.Y, centerIconWidth, iconLitC.Height), Color.White);
                spriteBatch.Draw(iconLitR, new Rectangle((int)iconPosition2.X + iconLitL.Width + centerIconWidth, (int)iconPosition2.Y, iconLitR.Width, iconLitR.Height), Color.White);
            }
            else
            {
                spriteBatch.Draw(iconDimL, new Rectangle((int)iconPosition2.X, (int)iconPosition2.Y, iconDimL.Width, iconDimL.Height), Color.White);
                spriteBatch.Draw(iconDimC, new Rectangle((int)iconPosition2.X + iconDimL.Width, (int)iconPosition2.Y, centerIconWidth, iconDimC.Height), Color.White);
                spriteBatch.Draw(iconDimR, new Rectangle((int)iconPosition2.X + iconDimL.Width + centerIconWidth, (int)iconPosition2.Y, iconDimR.Width, iconDimR.Height), Color.White);
            }
            spriteBatch.DrawString(textFont, "Return to Title",
                    new Vector2((int)iconPosition2.X + iconDimL.Width + (centerIconWidth / 2) - textFont.MeasureString("Return to Title").X / 2,
                        (int)iconPosition2.Y + (iconDimC.Height / 10)),
                        Color.Black);

            spriteBatch.End();
        }

        public void ResetPosition()
        {
            scrollPosition.Y = -scrollBackground.Height;
            isMouseSelected1 = false;
            isMouseSelectedAndClicked1 = false;
            isMouseSelected2 = false;
            isMouseSelectedAndClicked2 = false;
        }
    }
}
