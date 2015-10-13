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
    class Card : IDisplayableItem
    {
        public bool actedOn { get; set; }
        public enum State
        {
            ReallyDisabled,//card is way far away
            Disabled,//card cannot accept elements
            Activated,//card has been activated by an element
            Available,//card may accept elements
            Disappear//fade to gone
        }
        const int AVAILABLE_TIME_MILLIS = 300;
        public const int DISAPPEAR_TIME_MILLIS = 1000;
        public const int APPEAR_TIME_MILLIS = 1000;
        const int ACTIVATE_TIME_MILLIS = 300;
        const int ENGLISH_PADDING_TOP = 20;
        const int ICON_OFFSET = -10;
        Element.Type m_Type;
        int value;
        static Texture2D[] m_tarrBackgrounds;
        Texture2D m_tIcon;
        State m_State;
        Vector2 m_Position;
        static SpriteFont m_Font;
        int m_nTimer;
        int m_nFadeInTimer;
        Level m_Level;

        public Vector2 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public bool Satisfied
        {
            get { return m_State == State.Activated; }
        }

        public Card(Element.Type a_Type, Vector2 a_Position, Level a_Level, State a_State)
        {
            m_Type = a_Type;
            m_Position = a_Position;
            m_State = a_State;
            m_Level = a_Level;
            LoadContent(a_Level);
            m_nFadeInTimer = 0;

            if (a_Type <= Element.Type.Earth)
                value =  25;
            else if (a_Type <= Element.Type.Stone)
                value = 50;
            else if (a_Type <= Element.Type.Death)
                value = 75;
            else if (a_Type <= Element.Type.Tree)
                value = 100;
            else if (a_Type <= Element.Type.Hurricane)
                value = 125;
            else
                value = 150;
        }

        public int Value
        {
            get { return value; }
        }

        public void Enable()
        {
            if (m_State == State.Disabled)
                m_State = State.Available;
            else if (m_State == State.ReallyDisabled)
                m_State = State.Disabled;
        }

        public Boolean checkifNeededCard(Element e)
        {
            return false;
        }

        public bool Satisfy(Element a_Element)
        {
            if (m_State == State.Available && a_Element.ElementType == m_Type)
            {
                m_State = State.Activated;
                return true;
            }
            return false;
        }

        public void ClearCard()
        {
            m_State = State.Disappear;
            m_nTimer = 0;
        }

        public void LoadContent(Level a_Level)
        {
            if (m_tarrBackgrounds == null)
            {
                m_tarrBackgrounds = new Texture2D[5];
                m_tarrBackgrounds[(int)State.ReallyDisabled] = a_Level.Content.Load<Texture2D>("Elements/element-card-dim2");
                m_tarrBackgrounds[(int)State.Activated] = a_Level.Content.Load<Texture2D>("Elements/element-card-lit");
                m_tarrBackgrounds[(int)State.Available] = a_Level.Content.Load<Texture2D>("Elements/element-card-normal");
                m_tarrBackgrounds[(int)State.Disabled] = a_Level.Content.Load<Texture2D>("Elements/element-card-dim1");
                m_tarrBackgrounds[(int)State.Disappear] = m_tarrBackgrounds[(int)State.Activated];
                m_Font = a_Level.Content.Load<SpriteFont>("UI/Lithos Pro Regular Element");
            }
            if (m_Type != Element.Type.Fizzle)
            {
                try
                {
                    m_tIcon = a_Level.Content.Load<Texture2D>("Elements/symbols-card/" + m_Type.ToString().ToLower());
                }
                catch (ContentLoadException e)
                {
                    m_tIcon = null;
                }
            }
        }

        public void Dispose()
        {
            m_Level = null;
        }

        public void Update(GameTime a_GameTime)
        {
            if (m_nFadeInTimer < APPEAR_TIME_MILLIS)
            {
                m_nFadeInTimer += a_GameTime.ElapsedGameTime.Milliseconds;
                if (m_nFadeInTimer > APPEAR_TIME_MILLIS)
                    m_nFadeInTimer = APPEAR_TIME_MILLIS;
            }
            switch (m_State)
            {
                case State.Available:
                case State.Activated:
                case State.Disappear:
                    int nMaxTime = (m_State == State.Available 
                                    ? AVAILABLE_TIME_MILLIS 
                                    : (m_State == State.Activated 
                                        ? ACTIVATE_TIME_MILLIS 
                                        : DISAPPEAR_TIME_MILLIS));
                    if (m_nTimer < nMaxTime)
                    {
                        m_nTimer += a_GameTime.ElapsedGameTime.Milliseconds;
                        if (m_nTimer > nMaxTime)
                            m_nTimer = nMaxTime;
                    }
                    break;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            float nBackgroundAlpha = (m_State == State.Disappear ? 1 - ((float)m_nTimer / DISAPPEAR_TIME_MILLIS) : ((float)m_nFadeInTimer / APPEAR_TIME_MILLIS));
            float nTextAlpha = 0;
            switch (m_State)
            {
                case State.Disappear:
                    nTextAlpha = nBackgroundAlpha;
                    break;
                case State.Available:
                case State.Activated:
                    nTextAlpha = ((float)m_nTimer / (m_State == State.Available ? AVAILABLE_TIME_MILLIS : ACTIVATE_TIME_MILLIS));
                    break;
            }
            spriteBatch.Draw(m_tarrBackgrounds[(int)m_State], m_Position, null, Color.White * nBackgroundAlpha, 0, new Vector2(m_tarrBackgrounds[(int)m_State].Width/2, m_tarrBackgrounds[(int)m_State].Height/2), 1, SpriteEffects.None, (float)0.5);
            if (m_State != State.ReallyDisabled && m_State != State.Disabled)
            {
                if(m_tIcon != null)
                    spriteBatch.Draw(m_tIcon, m_Position + new Vector2(0, ICON_OFFSET), null, Color.White * nTextAlpha, 0, new Vector2(m_tIcon.Width / 2, m_tIcon.Height / 2), 1, SpriteEffects.None, (float)0.5);
                spriteBatch.DrawString(m_Font, m_Type.ToString(), m_Position + new Vector2(-(m_Font.MeasureString(m_Type.ToString()).X / 2), ENGLISH_PADDING_TOP), new Color(124, 84, 19) * nTextAlpha);
            }
            spriteBatch.End();
        }
    }
}
