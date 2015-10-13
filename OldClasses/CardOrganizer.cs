using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sagescroll
{
    class CardOrganizer : IDisplayableItem
    {
        public bool actedOn { get; set; }
        enum State
        {
            Dropping, Static, TaskTransition
        }

        static int[] m_narrXValues = new int[] { 455, 565, 345, 673 };
        static int[] m_narrYValues = new int[] { 328, 213, 101 };
        State m_State;
        const int DROP_TIME_MILLIS = 700;
        int m_nTimer;
        List<Card> m_arrCards;
        List<Card> m_arrNewCards;
        Level m_Level;

        public CardOrganizer(Level a_Level)
        {
            m_Level = a_Level;
            Element.OnCast += Element_OnCast;
            Task.OnCreate += NewTask;
            m_State = State.Static;
            m_arrNewCards = new List<Card>();
            m_arrCards = new List<Card>();
        }

        public void Dispose()
        {
            m_Level = null;
            Element.OnCast -= Element_OnCast;
            Task.OnCreate -= NewTask;
            foreach (Card card in m_arrCards)
            {
                card.Dispose();
            }
            m_arrCards = null;
        }

        public void NewTask(Task a_Task)
        {
            //create the new cards based on the task goal
            for(int i = 0; i<a_Task.Goals.Length; i++)
            {
                m_arrNewCards.Add(new Card(a_Task.Goals[i], new Vector2(m_narrXValues[i % 4], m_narrYValues[i / 4]), m_Level, 
                    (i < 4 ? Card.State.Available : (i < 8 ? Card.State.Disabled : Card.State.ReallyDisabled))));
            }
            //if we are at the beginning of the game, then just 
            //set the new cards as current cards and get started right away
            if (m_arrCards.Count == 0)
            {
                m_arrCards = m_arrNewCards;
                m_arrNewCards = new List<Card>();
            }
            else//otherwise tell the old cards to start clearing themselves out of the way
            {
                foreach (Card c in m_arrCards)
                {
                    c.ClearCard();
                }
                m_State = State.TaskTransition;
                m_nTimer = 0;
            }
        }

        public void Element_OnCast(Element source)
        {
            int nSatisfied = 0;
            bool bSatisfied = false;
            int nMin = (m_State == State.Dropping ? 4 : 0);
            int nMax = Math.Min(nMin + 4, m_arrCards.Count);
            //check against each card in the bottom row of the queue
            for (int i = nMin; i < nMax; i++)
            {
                if (m_arrCards[i].Satisfied)
                    nSatisfied++;
                else if (!bSatisfied && m_arrCards[i].Satisfy(source))
                {
                    m_Level.AddSFX(Level.SFXNames.cardActivated);
                    m_Level.updateTaskValue(m_arrCards[i].Value);
                    nSatisfied++;
                    bSatisfied = true;
                }
            }
            //if there are no cards left in the queue, mark task as completed
            if (nSatisfied == m_arrCards.Count)
            {
                m_Level.AddSFX(Level.SFXNames.taskComplete);
                m_Level.TaskFinished();
            }
            else if (nSatisfied == 4)//if we finished a row, tell it to disappear and move the other rows down
            {
                m_Level.updateTaskTime(5000);
                m_Level.AddSFX(Level.SFXNames.rowComplete);
                for (int i = 0; i < m_arrCards.Count; i++)
                {
                    if (i < 4)
                        m_arrCards[i].ClearCard();
                    else
                        m_arrCards[i].Enable();
                }
                m_State = State.Dropping;
                m_nTimer = 0;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Card c in m_arrCards)
            {
                c.Draw(gameTime, spriteBatch);
            }
            foreach (Card c in m_arrNewCards)
            {
                c.Draw(gameTime, spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            float nPercentTransition;
            //if we are transitioning to a new task
            //keep updating both card lists while the old cards fade out
            //once the old cards are done fading out, remove them
            if (m_State == State.TaskTransition)
            {
                m_nTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (m_nTimer >= Card.DISAPPEAR_TIME_MILLIS)
                {
                    m_arrCards = m_arrNewCards;
                    m_arrNewCards = new List<Card>();
                    m_State = State.Static;
                }
                //we want to give the player time to recognize the old row disappearing
                //hence the odd condition on 1/2 disappear time
                else if (m_nTimer > Card.DISAPPEAR_TIME_MILLIS / 2)
                {
                    foreach (Card c in m_arrNewCards)
                    {
                        c.Update(gameTime);
                    }
                }
                foreach (Card c in m_arrCards)
                {
                    c.Update(gameTime);
                }
            }
            else
            {
                if (m_State == State.Dropping)
                {
                    m_nTimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (m_nTimer < DROP_TIME_MILLIS)
                        nPercentTransition = (float)m_nTimer / DROP_TIME_MILLIS;
                    else
                    {
                        nPercentTransition = 1;
                        if (m_nTimer >= Card.DISAPPEAR_TIME_MILLIS)
                        {
                            m_State = State.Static;
                            m_arrCards.RemoveRange(0, 4);
                        }
                    }
                }
                else
                    nPercentTransition = 1;
                //update the positions of cards in the upper rows
                for (int i = 0; i < m_arrCards.Count; i++)
                {
                    if (m_State == State.Dropping && i >= 4)
                    {
                        m_arrCards[i].Position = new Vector2(m_narrXValues[i % 4], (m_narrYValues[i / 4] * (1 - nPercentTransition) + (m_narrYValues[i / 4 - 1] * nPercentTransition)));
                    }
                    m_arrCards[i].Update(gameTime);
                }
            }
        }
    }
}
