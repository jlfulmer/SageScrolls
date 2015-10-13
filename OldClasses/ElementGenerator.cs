using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sagescroll
{
    class ElementGenerator : IDisplayableItem
    {
        public bool actedOn { get; set; }
        Element.Type m_Type;
        Level m_Level;
        Vector2 m_Position;
        Element currElement;

        public ElementGenerator(Vector2 a_Position, Level a_Level, Element.Type a_Type)
        {
            m_Level = a_Level;
            m_Position = a_Position;
            m_Type = a_Type;
        }

        public void Dispose()
        {
            m_Level = null;
            currElement.OnDrop -= Element_OnDrop;
            currElement = null;
        }

        private void CreateElement(int slot)
        {
            currElement = Element.ElementBuilder.GetBaseElement(m_Type, m_Level, slot);
            currElement.Position = m_Position;
            //currElement.OnGrab += Element_OnGrab;
            currElement.OnDrop += Element_OnDrop;
        }

        public void Element_OnGrab(DraggableItem sender)
        {
        }

        public void Element_OnDrop(DraggableItem sender)
        {
            currElement.OnDrop -= Element_OnDrop;
            CreateElement(SageGame.gameSelectedIndex);//SLOT
        }

        public void Update(GameTime gameTime)
        {
            if (currElement == null) {
                if (m_Type == Element.Type.Fire) {
                    CreateElement(0);//SLOT
                } else if (m_Type == Element.Type.Water) {
                    CreateElement(3);//SLOT
                } else if (m_Type == Element.Type.Wind) {
                    CreateElement(4);//SLOT
                } else if (m_Type == Element.Type.Earth) {
                    CreateElement(7);//SLOT
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }
    }
}
