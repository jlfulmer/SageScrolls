using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sagescroll
{
    /// <summary>
    /// This is responsible for holding all the task information and requirements.
    /// </summary>
    class Task
    {

        private static int m_nLastTask = -1;

        #region TaskDescriptions
        PossibleTask[] PossibleTasks = new PossibleTask[] {
            new PossibleTask(new PossibleTask.TaskLevel[]{
                new PossibleTask.TaskLevel(getElements(1), 3, 4),
                new PossibleTask.TaskLevel(combine(getElements(1), getElements(2)), 3, 4),
                new PossibleTask.TaskLevel(combine(getElements(1), getElements(2)), 4, 12),
                new PossibleTask.TaskLevel(combine(getElements(2), getElements(3)), 3, 4),
                new PossibleTask.TaskLevel(combine(combine(getElements(1), getElements(2)), getElements(3)), 4, 12),
                new PossibleTask.TaskLevel(combine(combine(getElements(2), getElements(3)), getElements(4)), 3, 4),
                new PossibleTask.TaskLevel(combine(combine(getElements(2), getElements(3)), getElements(4)), 4, 12),
                new PossibleTask.TaskLevel(combine(combine(getElements(3), getElements(4)), getElements(5)), 4, 12),
                new PossibleTask.TaskLevel(combine(combine(getElements(4), getElements(5)), getElements(6)), 4, 12)})};

        #endregion

        private static Element.Type[] combine(Element.Type[] a, Element.Type[] b)
        {
            List<Element.Type> c = new List<Element.Type>();
            c.AddRange(a);
            c.AddRange(b);

            return c.ToArray();
        }

        private static Element.Type[] getElements(int tier)
        {
            Element.Type[] e;
            if (tier == 1)
            {
                e = new Element.Type[] { Element.Type.Wind, Element.Type.Water, Element.Type.Earth, Element.Type.Fire };
            }
            else if (tier == 2)
            {
                e = new Element.Type[] { Element.Type.Combustion, Element.Type.Atmosphere, Element.Type.Steam, Element.Type.Coal, 
                    Element.Type.Cyclone, Element.Type.Life, Element.Type.River, Element.Type.Clay, Element.Type.Stone, Element.Type.Dust };
            }
            else if (tier == 3)
            {
                e = new Element.Type[] { Element.Type.Magma, Element.Type.Creature, Element.Type.Energy, Element.Type.Sand, 
                    Element.Type.Lightning, Element.Type.Brick, Element.Type.Golem, Element.Type.Plant, Element.Type.Oil, Element.Type.Death};
            }
            else if (tier == 4)
            {
                e = new Element.Type[] { Element.Type.Metal, Element.Type.Torrent, Element.Type.Glass, Element.Type.Diamond, Element.Type.Corpse, 
                    Element.Type.Shelter, Element.Type.Fish, Element.Type.Bird, Element.Type.Beast, Element.Type.Tree, Element.Type.Blood,
                    Element.Type.Seed};
            }
            else if (tier == 5)
            {
                e = new Element.Type[] { Element.Type.Phoenix, Element.Type.Tool, Element.Type.Electricity, Element.Type.Undead, Element.Type.Ash, 
                    Element.Type.Machine, Element.Type.Hurricane, Element.Type.Ent, Element.Type.Human, Element.Type.Pottery };
            }
            else 
            {
                e = new Element.Type[] { Element.Type.Spirit, Element.Type.Robot, Element.Type.Weapon, Element.Type.Wood, Element.Type.Light,
                    Element.Type.Jewel, Element.Type.Industry, Element.Type.Vampire};
            }

            return e;
        }

        protected class PossibleTask
        {
            public TaskLevel[] Levels;
            public class TaskLevel
            {
                public Element.Type[] Elements;
                public int NumberElementsMin;
                public int NumberElementsMax;

                public TaskLevel(Element.Type[] Elements, int NumberElementsMin, int NumberElementsMax)
                {
                    this.Elements = Elements;
                    this.NumberElementsMin = NumberElementsMin;
                    this.NumberElementsMax = NumberElementsMax;
                }
            }

            public PossibleTask(TaskLevel[] Levels)
            {
                this.Levels = Levels;
            }
        }

        public delegate void TaskEvent(Task source);
        public static TaskEvent OnCreate;

        Element.Type[] m_arrGoals;
        int m_nPoints;

        public int Points
        {
            get { return m_nPoints; }
        }

        public Element.Type[] Goals
        {
            get 
            {
                Element.Type[] output = new Element.Type[m_arrGoals.Length];
                Array.Copy(m_arrGoals, output, m_arrGoals.Length);
                return output;
            }
        }

        public Task(Tutorial t, int a_nPrestige)
        {
            if (t.GetTask(this, a_nPrestige, out m_nPoints,  out m_arrGoals) && OnCreate != null)
                OnCreate(this);
        }

        public Task(int nLevel)
        {
            nLevel--;
            Random random = new Random();
            int nTaskNumber;

            do
            {
                nTaskNumber = random.Next(PossibleTasks.Length);
            }  while (nTaskNumber == -1);

            m_nLastTask = nTaskNumber;
            PossibleTask chosenTask = PossibleTasks[nTaskNumber];
            PossibleTask.TaskLevel chosenTaskLevel = chosenTask.Levels[nLevel];
            int nNumberElements;
            if (chosenTaskLevel.NumberElementsMax <= chosenTaskLevel.NumberElementsMin)
                nNumberElements = chosenTaskLevel.NumberElementsMin;
            else
                nNumberElements = random.Next(chosenTaskLevel.NumberElementsMin, chosenTaskLevel.NumberElementsMax);


            m_arrGoals = new Element.Type[nNumberElements];
            for (int i = 0; i < nNumberElements; i++)
            {
                Element.Type e = chosenTaskLevel.Elements[random.Next(chosenTaskLevel.Elements.Length)];
                m_arrGoals[i] = e;
            }

            if (OnCreate != null)
                OnCreate(this);
        }
    }
}
