using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace sagescroll
{
    /// <summary>
    /// The object responsible for displaying and fading the task text on the scroll.
    /// </summary>
    class TaskDescription : IDisplayableItem
    {
        private enum State
        {
            DISPLAY, FADE
        }
        private const int FADE_TIME_MILLIS = 700;
        private const int LEFT_PADDING = 60;
        private const int TOP_PADDING = 70;
        private const int LEFT_POINT_PADDING = 60;
        private const int TOP_POINT_PADDING = 275;
        private static Color TEXT_COLOR = new Color(107, 81, 48);
       
        private string m_sOldText;
        private string m_sCurrText;
        private int m_nOldPoints;
        private int m_nCurrPoints;
        private State m_CurrState;
        private Level m_level;
        Texture2D m_tScroll;
        SpriteFont m_Font;
        SpriteFont m_PointFont;
        private Vector2 m_Position;
        private int m_nMillisSinceFadeStart;

        public TaskDescription(Level level, Vector2 a_Position)
        {
            Task.OnCreate += NewTask;
            m_level = level;
            m_Position = a_Position;
            m_CurrState = State.DISPLAY;

            LoadContent();
        }

        public void NewTask(Task a_Task)
        {
            m_sOldText = m_sCurrText;
            m_sCurrText = a_Task.Text;
            m_nOldPoints = m_nCurrPoints;
            m_nCurrPoints = a_Task.Points;
            m_CurrState = State.FADE;
            m_nMillisSinceFadeStart = 0;
        }

        public void LoadContent()
        {
            m_tScroll = m_level.Content.Load<Texture2D>("UI/TaskScroll");
            m_Font = m_level.Content.Load<SpriteFont>("UI/Bradley Hand ITC TT");
            m_PointFont = m_level.Content.Load<SpriteFont>("UI/Lithos Pro Regular Points");
        }

        public void Dispose()
        {
            Task.OnCreate -= NewTask;
            m_level = null;
        }

        public int getTaskScore()
        {
            return m_nOldPoints;
        }

        public void Update(GameTime gameTime)
        {
            switch (m_CurrState)
            {
                case State.FADE:
                    m_nMillisSinceFadeStart += gameTime.ElapsedGameTime.Milliseconds;
                    if (m_nMillisSinceFadeStart >= FADE_TIME_MILLIS)
                        m_CurrState = State.DISPLAY;
                    break;
                case State.DISPLAY:
                    break;
                default:
                    break;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(m_tScroll, m_Position, Color.White);
            Vector2 stringPosition = m_Position + new Vector2(LEFT_PADDING, TOP_PADDING);
            Vector2 pointPosition = m_Position + new Vector2(LEFT_POINT_PADDING, TOP_POINT_PADDING);
            switch (m_CurrState)
            {
                case State.FADE:
                    float nNewTextAlpha = ((float)m_nMillisSinceFadeStart) / FADE_TIME_MILLIS;
                    if (!string.IsNullOrEmpty(m_sOldText))
                    {
                        spriteBatch.DrawString(m_Font, m_sOldText, stringPosition, TEXT_COLOR * (1 - nNewTextAlpha));
                        spriteBatch.DrawString(m_PointFont, "Value: " + m_nCurrPoints.ToString(), pointPosition, TEXT_COLOR * (1 - nNewTextAlpha));
                    }
                    if (!string.IsNullOrEmpty(m_sCurrText))
                    {
                        spriteBatch.DrawString(m_Font, m_sCurrText, stringPosition, TEXT_COLOR * nNewTextAlpha);
                        spriteBatch.DrawString(m_PointFont, "Value: " + m_nCurrPoints.ToString(), pointPosition, TEXT_COLOR * nNewTextAlpha);
                    }
                    break;
                case State.DISPLAY:
                    if (!string.IsNullOrEmpty(m_sCurrText))
                    {
                        spriteBatch.DrawString(m_Font, m_sCurrText, stringPosition, TEXT_COLOR);
                        spriteBatch.DrawString(m_PointFont, "Value: " + m_nCurrPoints.ToString(), pointPosition, TEXT_COLOR);
                    }
                    break;
                default:
                    break;
            }
            spriteBatch.End();
        }
    }
}
