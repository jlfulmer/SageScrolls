using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sagescroll
{
    class Tutorial : IDisplayableItem
    {
        public bool actedOn { get; set; }
        public enum TutorialNumber
        {
            Tutorial_1 = 0, Tutorial_2 = 3
        }
        int m_nStage;
        Level m_Level;
        Texture2D m_tScroll1_1;
        Vector2 m_pScroll1_1 = new Vector2(81, 283);
        Texture2D m_tScroll1_2;
        Vector2 m_pScroll1_2 = new Vector2(81, 283);
        Texture2D m_tScroll2_1;
        Vector2 m_pScroll2_1 = new Vector2(81, 283);
        Texture2D m_tScroll2_2;
        Vector2 m_pScroll2_2 = new Vector2(31, 616);
        Element m_TempElement;
        bool m_bDisposed;

        public Tutorial(Level a_Level)
        {
            m_Level = a_Level;
            m_nStage = -1;
            m_bDisposed = false;
            LoadContent(m_Level);
            
            Element.ElementBuilder.OnBuild += Element_OnBuild;
            Element.OnSpamDrop += Element_OnDrop;
        }

        private void LoadContent(Level a_Level)
        {
            m_tScroll1_1 = a_Level.Content.Load<Texture2D>("UI/Tutorial1_1");
            m_tScroll1_2 = a_Level.Content.Load<Texture2D>("UI/Tutorial1_2");
            m_tScroll2_1 = a_Level.Content.Load<Texture2D>("UI/Tutorial2_1");
            m_tScroll2_2 = a_Level.Content.Load<Texture2D>("UI/Tutorial2_2");
        }

        public bool GetTask(Task a_Task, int a_nPrestige, out int ao_nPoints, out Element.Type[] ao_arrElements)
        {
            if (a_Task != null)
            {
                switch (m_nStage)
                {
                    case (int)TutorialNumber.Tutorial_1:
                        ao_nPoints = 0;
                        ao_arrElements = new Element.Type[] { Element.Type.Fire };
                        break;
                    case (int)TutorialNumber.Tutorial_2:
                        ao_nPoints = 0;
                        ao_arrElements = new Element.Type[] { Element.Type.Atmosphere };
                        break;
                    default:
                        Task temp = new Task(a_nPrestige);
                        ao_nPoints = temp.Points;
                        ao_arrElements = temp.Goals;
                        return false;
                }
            }
            else
            {
                ao_nPoints = 0;
                ao_arrElements = null;
                return false;
            }
            return true;
        }

        public void Element_OnCast(Element source)
        {
            if (m_nStage == (int)TutorialNumber.Tutorial_1 + 1)
                m_nStage++;
        }

        public void Element_OnBuild(Element source)
        {
            source.OnDrop += Element_OnDrop;
            source.OnFizzled += Element_OnFizzled;
            if (m_nStage == (int)TutorialNumber.Tutorial_2 + 1 && source.GetElementTier() > 1)
                m_nStage++;
        }

        public void Element_OnFizzled(Element source)
        {
            m_TempElement.OnFizzled -= Element_OnFizzled;
            m_TempElement = null;
        }

        public void Element_OnDrop(DraggableItem source)
        {
            switch (m_nStage)
            {
                case (int)TutorialNumber.Tutorial_1:
                case (int)TutorialNumber.Tutorial_2:
                    m_TempElement = source as Element;
                    m_TempElement.OnFizzled += Element_OnFizzled;
                    break;
            }
        }

        public void StartTutorial(TutorialNumber a_eTutorialNum)
        {
            switch (a_eTutorialNum)
            {
                case TutorialNumber.Tutorial_1:
                    break;
                case TutorialNumber.Tutorial_2:
                    break;
                default:
                    m_nStage = -1;
                    return;
            }
            m_nStage = (int)a_eTutorialNum;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            switch (m_nStage)
            {
                case (int)TutorialNumber.Tutorial_1:
                    spriteBatch.Draw(m_tScroll1_1, m_pScroll1_1, Color.White);
                    break;
                case (int)TutorialNumber.Tutorial_1 + 1:
                    spriteBatch.Draw(m_tScroll1_2, m_pScroll1_2, Color.White);
                    break;
                case (int)TutorialNumber.Tutorial_2:
                    spriteBatch.Draw(m_tScroll2_1, m_pScroll2_1, Color.White);
                    break;
                case (int)TutorialNumber.Tutorial_2 + 1:
                    spriteBatch.Draw(m_tScroll2_2, m_pScroll2_2, Color.White);
                    break;
                case (int)TutorialNumber.Tutorial_2 + 2:
                    break;
            }
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            switch (m_nStage)
            {
                case (int)TutorialNumber.Tutorial_1:
                    if (m_TempElement != null)
                    {
                        m_nStage++;
                        m_TempElement = null;
                        Element.OnCast += Element_OnCast;
                    }
                    break;
                case (int)TutorialNumber.Tutorial_1 + 1:
                    break;
                case (int)TutorialNumber.Tutorial_1 + 2:
                    m_Level.Remove(this);
                    break;
                case (int)TutorialNumber.Tutorial_2: 
                    if (m_TempElement != null)
                    {
                        m_nStage++;
                        m_TempElement = null;
                    }
                    break;
                case (int)TutorialNumber.Tutorial_2 + 1:
                    break;
                case (int)TutorialNumber.Tutorial_2 + 2:
                    m_Level.Remove(this);
                    break;
                case (int)TutorialNumber.Tutorial_2 + 3:
                    break;
            }
        }

        public void Dispose()
        {
            if (!m_bDisposed)
            {
                m_Level = null;
                Element.ElementBuilder.OnBuild -= Element_OnBuild;
                Element.OnSpamDrop -= Element_OnDrop;
                if (m_TempElement != null)
                {
                    m_TempElement.OnFizzled -= Element_OnFizzled;
                    m_TempElement = null;
                }
                m_bDisposed = true;
            }
        }
    }
}
