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
    class IntroScroll
    {
        SpriteFont textFont;
        SpriteFont buttonFont;
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

        Vector2 iconPosition1;
        Rectangle mouseSelectionBox1;
        bool isMouseSelected1;
        public bool IsMouseSelectedAndClicked1
        {
            get { return isMouseSelectedAndClicked1; }
        }
        bool isMouseSelectedAndClicked1;

        Vector2 textPosition;

        public IntroScroll(ContentManager content, UserInput input)
        {
            this.input = input;
            LoadContent(content);
        }

        public void LoadContent(ContentManager content)
        {
            scrollBackground = content.Load<Texture2D>("Menus/long-scroll");
            scrollPosition = new Vector2(512 - (scrollBackground.Width / 2), 0);

            textFont = content.Load<SpriteFont>("UI/Bradley Hand ITC TT");
            buttonFont = content.Load<SpriteFont>("UI/Lithos Pro Regular");

            iconDimL = content.Load<Texture2D>("UI/button-dim-left");
            iconDimR = content.Load<Texture2D>("UI/button-dim-right");
            iconDimC = content.Load<Texture2D>("UI/button-dim-middle");
            iconLitL = content.Load<Texture2D>("UI/button-lit-left");
            iconLitR = content.Load<Texture2D>("UI/button-lit-right");
            iconLitC = content.Load<Texture2D>("UI/button-lit-middle");

            textPosition = new Vector2(scrollBackground.Bounds.Left + 225, 125);

            centerIconWidth = 100;
            iconPosition1 = new Vector2((1024 / 2) - ((iconDimL.Width + iconLitR.Width + centerIconWidth) / 2), scrollBackground.Bounds.Bottom - 250);
            this.mouseSelectionBox1 = new Rectangle((int)iconPosition1.X, (int)iconPosition1.Y,
                iconDimL.Width + iconDimR.Width + centerIconWidth,
                iconDimC.Height);
            isMouseSelected1 = false;
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

            if (input.ButtonJustPressed(UserInput.MouseButton.Left) || SageGame.PressedAnything())
            {
                if (isMouseSelected1)
                    isMouseSelectedAndClicked1 = true;
            }                      
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(scrollBackground, new Rectangle(
                            (int)scrollPosition.X, (int)scrollPosition.Y,
                            scrollBackground.Width, scrollBackground.Height), Color.White);
            spriteBatch.DrawString(textFont,
@"There is a saying, ages old and lost with time, that 'He who masters the 
scroll, will illuminate the soul of the world'. In those times, scholars and 
sages from across the land sought to harness the power of the magic 
inscribed on the parchment. Many of them found glory and praise for 
their skill, but the magic eventually corrupted them. Most, if not 
all, were consumed by their own twisted enchantments.

Today the world is darkened and bleak, as a shadow falls over the 
citizens who were once betrayed by those who were chosen to protect and 
guide them. Now that ancient magic has been lost to the world, and there 
is no one left with the ability to master it...  

The discovery of an aged parchment has presented you with an 
opportunity, the chance to right the wrongs of a time forgotten, and 
to rekindle the flame in the hearts of mankind. Help us kind Sage! 
Only your wisdom, sharpened with age, can shield and guide the 
people of this land.", textPosition, Color.Black);

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
            spriteBatch.DrawString(buttonFont, "Begin",
                    new Vector2((int)iconPosition1.X + iconDimL.Width + (centerIconWidth / 2) - buttonFont.MeasureString("Begin").X / 2,
                        (int)iconPosition1.Y + (iconDimC.Height / 4)),
                        Color.Black);

            spriteBatch.End();
        }

        public void ResetPosition()
        {
            scrollPosition.Y = -scrollBackground.Height;
            isMouseSelected1 = false;
            isMouseSelectedAndClicked1 = false;
        }
    }
}
