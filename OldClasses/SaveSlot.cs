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
    public class SaveSlot : IDisplayableItem
    {
        public bool actedOn { get; set; }
        public bool active;
        Vector2 m_Position;
        Vector2 m_center;
        int m_nWidth;
        int m_nHeight;
        static Texture2D m_tBackground;
        Element currElement;
        Level m_Level;
        UIButton m_btnClear;

        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)m_Position.X, (int)m_Position.Y, m_nWidth, m_nHeight); }
        }

        public SaveSlot(Vector2 a_Position, Level a_Level, int slot)
        {
            active = false;
            m_Level = a_Level;
            m_Position = a_Position;
            LoadContent(a_Level, slot);
            m_nWidth = m_tBackground.Width;
            m_nHeight = m_tBackground.Height;
            m_center = m_Position;
            m_center.X = this.m_Position.X + (m_nWidth / 4);
            m_center.Y = this.m_Position.Y + (m_nHeight / 4);
            Element.ElementBuilder.OnBuild += ElementBuilt;
        }

        public void LoadContent(Level a_Level, int slot)
        {
            if (m_tBackground == null)
            {
                m_tBackground = a_Level.Content.Load<Texture2D>("UI/SaveSlot");
            }

            m_btnClear = new UIButton(m_Position + new Vector2(70, -5), m_Level.Content, UIButton.Button.clearButton, true, slot);
        }

        public void Dispose()
        {
            Element.ElementBuilder.OnBuild -= ElementBuilt;
            m_btnClear.OnClick -= Button_OnClick;

            //if nobody else is connected, clean them up
            m_btnClear.Dispose();
            currElement = null;
            m_Level = null;
        }

        public void Button_OnClick(UIButton source)
        {
            if (currElement == null)
                return;

            switch (source.ButtonType)
            {
                case UIButton.Button.clearButton:
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


        public void ElementDropped(DraggableItem sender)
        {
            if (active)
            {
                if (currElement == sender)
                {
                    Element.Type temp = currElement.ElementType;
                    currElement = null;
                    Element savedElement = Element.ElementBuilder.GetElement(temp, m_Level, SageGame.gameSelectedIndex);//SLOT
                    DroppedIn(savedElement);
                    m_Level.remain(savedElement);
                }
                if (sender is Element && new Rectangle((int)m_Position.X, (int)m_Position.Y, m_nWidth, m_nHeight).Intersects(sender.BoundingBox))
                    ((Element)sender).RegisterSaveArea(this);
            }

        }

        public void DroppedIn(Element a_Element)
        {

            if (currElement != null)
            {
                currElement.currHolder = Element.Holder.None;
                currElement.Draggable = false;
            }
            currElement = a_Element;
            //else if (currElement != a_Element)
            //{
            //    currElement = Element.ElementBuilder.GetElement(currElement, a_Element, m_Level);
            //    if (currElement.ElementType == Element.Type.Fizzle)
            //    {
            //        m_Level.AddSFX(Level.SFXNames.fizzle);
            //        currElement.Position = m_Position + new Vector2(m_nWidth / 2, m_nHeight / 2);
            //        currElement = null;
            //    }
            //    else
            //    {
            //        m_Level.AddSFX(Level.SFXNames.combineElement);
            //    }
            //}
            if (currElement != null) {
                currElement.Position = m_Position + new Vector2(10, 10);
            }
            currElement.currHolder = Element.Holder.SaveSlot;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (active)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(m_tBackground, m_Position, Color.White);
                spriteBatch.End();
            }
        }

        public void activate()
        {
            active = true;
            m_btnClear.OnClick += Button_OnClick;
            m_Level.Add(m_btnClear);
        }
    }
}
