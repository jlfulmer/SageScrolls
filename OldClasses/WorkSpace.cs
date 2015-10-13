using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace sagescroll
{
    public class WorkSpace : IDisplayableItem
    {
        public bool actedOn { get; set; }
        Vector2 m_Position;
        Vector2 m_center;
        int m_nWidth;
        int m_nHeight;
        static Texture2D m_tBackground;
        Element currElement;
        Level m_Level;
        UIButton m_btnCast;
        UIButton m_btnClear;
        static UIButton m_btnClearAll;
        static UIButton m_btnCastAll;

        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)m_Position.X, (int)m_Position.Y, m_nWidth, m_nHeight); }
        }

        public WorkSpace(Vector2 a_Position, Level a_Level, int slot)
        {
            m_Level = a_Level;
            m_Position = a_Position;
            LoadContent(a_Level, slot);
            m_nWidth = m_tBackground.Width;
            m_nHeight = m_tBackground.Height;
            m_center = m_Position;
            m_center.X = this.m_Position.X + (m_nWidth / 4);
            m_center.Y = this.m_Position.Y + (m_nHeight / 4);
            Element.ElementBuilder.OnBuild += ElementBuilt;
            Element.OnSpamDrop += Element_OnSpamDrop;
        }

        public void LoadContent(Level a_Level, int slot)
        {
            if (m_tBackground == null)
            {
                m_tBackground = a_Level.Content.Load<Texture2D>("UI/mat");
            }
            m_btnCast = new UIButton(m_Position + new Vector2(220, 24), m_Level.Content, UIButton.Button.castButton, true, slot);
            m_btnCast.OnClick += Button_OnClick;
            m_btnClear = new UIButton(m_Position + new Vector2(220, 72), m_Level.Content, UIButton.Button.clearButton, true, slot);
            m_btnClear.OnClick += Button_OnClick;
            a_Level.Add(m_btnCast);
            a_Level.Add(m_btnClear);
            if (m_btnCastAll == null)
            {
                m_btnCastAll = new UIButton(new Vector2(516, 695), m_Level.Content, UIButton.Button.castallButton, true, -1);
                m_btnClearAll = new UIButton(new Vector2(280, 695), m_Level.Content, UIButton.Button.clearallButton, true, -1);
                a_Level.Add(m_btnCastAll);
                a_Level.Add(m_btnClearAll);
            }
            m_btnCastAll.OnClick += Button_OnClick;
            m_btnClearAll.OnClick += Button_OnClick;
        }

        public void Dispose()
        {
            Element.ElementBuilder.OnBuild -= ElementBuilt;
            m_btnCast.OnClick -= Button_OnClick;
            m_btnClear.OnClick -= Button_OnClick;
            Element.OnSpamDrop -= Element_OnSpamDrop;
            m_btnCastAll.OnClick -= Button_OnClick;
            m_btnClearAll.OnClick -= Button_OnClick;
            //if nobody else is connected, clean them up
            if (m_btnClearAll.OnClick == null)
            {
                m_btnClearAll = null;
                m_btnCastAll = null;
            }
            m_btnClear.Dispose();
            m_btnCast.Dispose();
            currElement = null;
            m_Level = null;
        }

        public void Button_OnClick(UIButton source)
        {
            if (currElement == null)
                return;

            switch (source.ButtonType)
            {
                case UIButton.Button.castButton:
                case UIButton.Button.castallButton:
                    currElement.Cast();
                    break;
                case UIButton.Button.clearButton:
                case UIButton.Button.clearallButton:
                    m_Level.AddSFX(Level.SFXNames.resetElement);
                    currElement.Destroy();
                    break;
                default:
                    return;
            }
            currElement = null;
        }

        public void ElementBuilt(Element created)
        {
            created.OnDrop += ElementDropped;
        }

        public void Element_OnSpamDrop(Element sender)
        {
            if (currElement == null)
                sender.RegisterDropArea(this);
        }

        public void ElementDropped(DraggableItem sender)
        {
            if (currElement == sender)
                currElement = null;
            if (sender is Element && new Rectangle((int)m_Position.X, (int)m_Position.Y, m_nWidth, m_nHeight).Intersects(sender.BoundingBox))
                ((Element)sender).RegisterDropArea(this);
        }

        public void DroppedIn(Element a_Element, int slot)
        {
            if (currElement == null)
            {
                currElement = a_Element;
            } 
            else if (currElement != a_Element)
            {
                currElement = Element.ElementBuilder.GetElement(currElement, a_Element, m_Level, slot);
                if (currElement.ElementType == Element.Type.Fizzle)
                {
                    m_Level.AddSFX(Level.SFXNames.fizzle);
                    currElement.Position = m_Position + new Vector2(m_nWidth / 2, m_nHeight / 2);
                    currElement = null;
                }
                else
                {
                    m_Level.AddSFX(Level.SFXNames.combineElement);
                }
            }
            if (currElement != null)
                currElement.Position = m_center;
        }

        public void Update(GameTime gameTime)
        {
            if (m_Level.Input.KeyJustPressed(Microsoft.Xna.Framework.Input.Keys.Space)) {
                Button_OnClick(m_btnCast);
            }
#if !WINDOWS
            if (SageGame.ButtonJustPressed(Microsoft.Xna.Framework.Input.Buttons.LeftTrigger)) {
                Button_OnClick(m_btnClearAll);
            } else if (SageGame.ButtonJustPressed(Microsoft.Xna.Framework.Input.Buttons.RightTrigger)) {
                Button_OnClick(m_btnCastAll);
            }
#endif
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(m_tBackground, m_Position, Color.White);
            spriteBatch.End();
        }
    }
}
