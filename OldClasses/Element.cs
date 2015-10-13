    using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace sagescroll
{
    public class Element : DraggableItem
    {
        public static class ElementBuilder
        {
            public delegate void ElementBuilderEvent(Element generated);
            public static ElementBuilderEvent OnBuild;

            

            private static Element.Type[,] s_map;

            private static Element.Type[,] Map
            {
                get
                {
                    if (s_map == null)
                    {
                        int nNumTypes = 56;
                        s_map = new Element.Type[nNumTypes, nNumTypes];
                        //Tier 2
                        AddToMap(Element.Type.Fire, Element.Type.Fire, Element.Type.Combustion, s_map);
                        AddToMap(Element.Type.Fire, Element.Type.Wind, Element.Type.Atmosphere, s_map);
                        AddToMap(Element.Type.Fire, Element.Type.Water, Element.Type.Steam, s_map);
                        AddToMap(Element.Type.Fire, Element.Type.Earth, Element.Type.Coal, s_map);
                        AddToMap(Element.Type.Wind, Element.Type.Wind, Element.Type.Cyclone, s_map);
                        AddToMap(Element.Type.Wind, Element.Type.Water, Element.Type.Life, s_map);
                        AddToMap(Element.Type.Wind, Element.Type.Earth, Element.Type.Dust, s_map);
                        AddToMap(Element.Type.Water, Element.Type.Water, Element.Type.River, s_map);
                        AddToMap(Element.Type.Water, Element.Type.Earth, Element.Type.Clay, s_map);
                        AddToMap(Element.Type.Water, Element.Type.Dust, Element.Type.Clay, s_map);
                        AddToMap(Element.Type.Earth, Element.Type.Earth, Element.Type.Stone, s_map);
                        //Tier 3
                        AddToMap(Element.Type.Fire, Element.Type.Stone, Element.Type.Magma, s_map);
                        AddToMap(Element.Type.Life, Element.Type.Earth, Element.Type.Creature, s_map);
                        AddToMap(Element.Type.Fire, Element.Type.Coal, Element.Type.Energy, s_map);
                        AddToMap(Element.Type.Wind, Element.Type.Stone, Element.Type.Sand, s_map);
                        AddToMap(Element.Type.Water, Element.Type.Stone, Element.Type.Sand, s_map);
                        AddToMap(Element.Type.Combustion, Element.Type.Atmosphere, Element.Type.Lightning, s_map);
                        AddToMap(Element.Type.Clay, Element.Type.Fire, Element.Type.Brick, s_map);
                        AddToMap(Element.Type.Stone, Element.Type.Life, Element.Type.Golem, s_map);
                        AddToMap(Element.Type.Clay, Element.Type.Life, Element.Type.Golem, s_map);
                        AddToMap(Element.Type.Life, Element.Type.Water, Element.Type.Plant, s_map);
                        AddToMap(Element.Type.Dust, Element.Type.Life, Element.Type.Creature, s_map);
                        AddToMap(Element.Type.Water, Element.Type.Coal, Element.Type.Oil, s_map);
                        AddToMap(Element.Type.Life, Element.Type.Fire, Element.Type.Death, s_map);
                        //Tier 4
                        AddToMap(Element.Type.Life, Element.Type.Sand, Element.Type.Seed, s_map);
                        AddToMap(Element.Type.Fire, Element.Type.Creature, Element.Type.Blood, s_map);
                        AddToMap(Element.Type.Magma, Element.Type.Water, Element.Type.Metal, s_map);
                        AddToMap(Element.Type.Energy, Element.Type.River, Element.Type.Torrent, s_map);
                        AddToMap(Element.Type.Fire, Element.Type.Sand, Element.Type.Glass, s_map);
                        AddToMap(Element.Type.Energy, Element.Type.Sand, Element.Type.Glass, s_map);
                        AddToMap(Element.Type.Energy, Element.Type.Coal, Element.Type.Diamond, s_map);
                        AddToMap(Element.Type.Death, Element.Type.Creature, Element.Type.Corpse, s_map);
                        AddToMap(Element.Type.Brick, Element.Type.Creature, Element.Type.Shelter, s_map);
                        AddToMap(Element.Type.Stone, Element.Type.Creature, Element.Type.Shelter, s_map);
                        AddToMap(Element.Type.Water, Element.Type.Creature, Element.Type.Fish, s_map);
                        AddToMap(Element.Type.Wind, Element.Type.Creature, Element.Type.Bird, s_map);
                        AddToMap(Element.Type.Earth, Element.Type.Creature, Element.Type.Beast, s_map);
                        AddToMap(Element.Type.Plant, Element.Type.Water, Element.Type.Tree, s_map);
                        AddToMap(Element.Type.Seed, Element.Type.Water, Element.Type.Tree, s_map);
                        AddToMap(Element.Type.Seed, Element.Type.Earth, Element.Type.Plant, s_map);
                        //Tier 5
                        AddToMap(Element.Type.Creature, Element.Type.Life, Element.Type.Human, s_map);
                        AddToMap(Element.Type.Human, Element.Type.Clay, Element.Type.Pottery, s_map);
                        AddToMap(Element.Type.Life, Element.Type.Tree, Element.Type.Ent, s_map);
                        AddToMap(Element.Type.Bird, Element.Type.Fire, Element.Type.Phoenix, s_map);
                        AddToMap(Element.Type.Creature, Element.Type.Metal, Element.Type.Tool, s_map);
                        AddToMap(Element.Type.Human, Element.Type.Metal, Element.Type.Tool, s_map);
                        AddToMap(Element.Type.Metal, Element.Type.Lightning, Element.Type.Electricity, s_map);
                        AddToMap(Element.Type.Corpse, Element.Type.Life, Element.Type.Undead, s_map);
                        AddToMap(Element.Type.Corpse, Element.Type.Fire, Element.Type.Ash, s_map);
                        AddToMap(Element.Type.Plant, Element.Type.Fire, Element.Type.Ash, s_map);
                        AddToMap(Element.Type.Metal, Element.Type.Oil, Element.Type.Machine, s_map);
                        AddToMap(Element.Type.Torrent, Element.Type.Wind, Element.Type.Hurricane, s_map);
                        AddToMap(Element.Type.Torrent, Element.Type.Cyclone, Element.Type.Hurricane, s_map);
                        //Tier 6
                        AddToMap(Element.Type.Blood, Element.Type.Undead, Element.Type.Vampire, s_map);
                        AddToMap(Element.Type.Human, Element.Type.Machine, Element.Type.Industry, s_map);
                        AddToMap(Element.Type.Human, Element.Type.Diamond, Element.Type.Jewel, s_map);
                        AddToMap(Element.Type.Life, Element.Type.Ash, Element.Type.Spirit, s_map);
                        AddToMap(Element.Type.Life, Element.Type.Machine, Element.Type.Robot, s_map);
                        AddToMap(Element.Type.Blood, Element.Type.Tool, Element.Type.Weapon, s_map);
                        AddToMap(Element.Type.Death, Element.Type.Tool, Element.Type.Weapon, s_map);
                        AddToMap(Element.Type.Tool, Element.Type.Tree, Element.Type.Wood, s_map);
                        AddToMap(Element.Type.Fire, Element.Type.Wood, Element.Type.Light, s_map);
                        AddToMap(Element.Type.Electricity, Element.Type.Metal, Element.Type.Light, s_map);
                    }
                    return s_map;
                }
            }

            /// <summary>
            /// Returns the appropriate Element object based on the two Elements provided to combine.
            /// the Elements provided will turn to Fizzle types since they should be discarded after
            /// they are used to create another Element.
            /// </summary>
            /// <param name="a_ElementOne"></param>
            /// <param name="a_ElementTwo"></param>
            /// <param name="a_level"></param>
            /// <returns>The resulting element. Fizzle if there is no valid combination.</returns>
            public static Element GetElement(Element a_ElementOne, Element a_ElementTwo, Level a_level, int slot)
            {
                Element.Type TypeA = a_ElementOne.m_Type;
                Element.Type TypeB = a_ElementTwo.m_Type;
                if (TypeA > TypeB)
                {
                    Element.Type temp = TypeB;
                    TypeB = TypeA;
                    TypeA = temp;
                }
                
                a_ElementOne.Destroy(true);
                a_ElementTwo.Destroy(true);
                Element element = new Element(Map[(int)TypeA, (int)TypeB], a_level, slot);
                element.currHolder = Holder.Board;
                if (GetElementPermission(a_level.Score.Prestige) < element.GetElementTier()) element.Destroy();
                if (OnBuild != null)
                    OnBuild(element);
                return element;
            }

            public static Element GetElement(Element.Type type, Level level, int slot)
            {
                Element element = new Element(type, level, slot);
                element.currHolder = Holder.Board;
                if (GetElementPermission(level.Score.Prestige) < element.GetElementTier()) element.Destroy();
                if (OnBuild != null)
                    OnBuild(element);
                return element;
            }

            private static void AddToMap(Element.Type a_ElementOne, Element.Type a_ElementTwo, Element.Type a_ElementResult, Element.Type[,] a_mapElements)
            {
                if (a_ElementOne > a_ElementTwo)
                {
                    Element.Type temp = a_ElementTwo;
                    a_ElementTwo = a_ElementOne;
                    a_ElementOne = temp;
                }
                a_mapElements[(int)a_ElementOne, (int)a_ElementTwo] = a_ElementResult;
            }

            /// <summary>
            /// This will only return Fire, Water, Wind, and Earth elements, everything else
            /// needs to go through GetElement(...) to ensure that proper combining is adhered to
            /// </summary>
            /// <param name="a_Type"></param>
            /// <param name="a_Level"></param>
            /// <returns></returns>
            public static Element GetBaseElement(Element.Type a_Type, Level a_Level, int slot)
            {
                switch (a_Type)
                {
                    case Element.Type.Water:
                    case Element.Type.Wind:
                    case Element.Type.Fire:
                    case Element.Type.Earth:
                        Element element = new Element(a_Type, a_Level, slot);
                        if (OnBuild != null)
                            OnBuild(element);
                        return element;
                    default:
                        return null;
                }
            }


            /// <summary>
            /// This function determines how high tier elements can be casted
            /// </summary>
            public static int GetElementPermission(int a_prestige) {
                if (a_prestige >= 9) {
                    return 6;
                } else if (a_prestige >= 8) {
                    return 5;
                } else if (a_prestige >= 6) {
                    return 4;
                } else if (a_prestige >= 4) {
                    return 3;
                } else if (a_prestige >= 2) {
                    return 2;
                } else {
                    return 1;
                }
            }
        }

        public delegate void ElementEvent(Element source);
        public static ElementEvent OnCast;
        public ElementEvent OnFizzled;
        public static ElementEvent OnSpamDrop;

        public enum Holder
        {
            Generator, Board, Mouse, SaveSlot, None
        }

        //*************READ ME BEFORE UPDATING*******************************
        //when adding to a tier, add to the end of that tier's row
        //when adding a new tier, make sure to update Element.GetElementTier()
        //*************READ ME BEFORE UPDATING*******************************
        public enum Type
        {
            Fizzle,
            Fire, Wind, Water, Earth,
            Combustion, Atmosphere, Steam, Coal, Cyclone, Mist, Life, River, Clay, Stone, Dust,
            Magma, Creature, Energy, Sand, Lightning, Brick, Golem, Plant, Oil, Death,
            Metal, Torrent, Glass, Diamond, Corpse, Shelter, Fish, Bird, Beast, Tree, Blood, Seed,
            Phoenix, Tool, Electricity, Undead, Ash, Machine, Hurricane, Ent, Human, Pottery,
            Spirit, Robot, Weapon, Wood, Light, Jewel, Industry, Vampire
        }


        public Element.Type[] getElements(int tier)
        {
            if (tier == 1)
            {
                m_tier = new Element.Type[] { Element.Type.Wind, Element.Type.Water, Element.Type.Earth, Element.Type.Fire };
            }
            else if (tier == 2)
            {
                m_tier = new Element.Type[] { Element.Type.Combustion, Element.Type.Atmosphere, Element.Type.Steam, Element.Type.Coal, 
                    Element.Type.Cyclone, Element.Type.Mist, Element.Type.Life, Element.Type.River, Element.Type.Clay, Element.Type.Stone };
            }
            else if (tier == 3)
            {
                m_tier = new Element.Type[] { Element.Type.Magma, Element.Type.Creature, Element.Type.Energy, Element.Type.Sand, 
                    Element.Type.Lightning, Element.Type.Brick, Element.Type.Golem, Element.Type.Plant, Element.Type.Oil, Element.Type.Death};
            }
            else if (tier == 4)
            {
                m_tier = new Element.Type[] { Element.Type.Metal, Element.Type.Torrent, Element.Type.Glass, Element.Type.Diamond, Element.Type.Corpse, 
                    Element.Type.Shelter, Element.Type.Fish, Element.Type.Bird, Element.Type.Beast, Element.Type.Tree };
            }
            else if (tier == 5)
            {
                m_tier = new Element.Type[] { Element.Type.Phoenix, Element.Type.Tool, Element.Type.Electricity, Element.Type.Undead, Element.Type.Ash, 
                    Element.Type.Machine, Element.Type.Hurricane };
            }
            else 
            {
                m_tier = new Element.Type[] { Element.Type.Spirit, Element.Type.Robot, Element.Type.Weapon, Element.Type.Wood, Element.Type.Light };
            }

            return m_tier;
        }

        const int FADE_OUT_TIME_MILLIS = 300;
        const int ENGLISH_PADDING_TOP = 60;
        const int ICON_OFFSET = -10;
        Element.Type[] m_tier;
        Type m_Type;
        Type m_FinalDisplay;
        static Texture2D m_tBackground;
        static Texture2D m_tShine;
        static SpriteFont m_Font;
        Texture2D m_tIcon;
        float m_Rotation;
        public Holder currHolder;
        /// <summary>
        /// texture offset from position
        /// </summary>
        Vector2 m_TextureOffset = new Vector2(-20, -11);
        List<WorkSpace> DropAreas;
        List<SaveSlot> SaveAreas;
        int m_nTimer;
        Level m_Level;
        static Random rand = new Random();
        double floatOffset;
        Vector2 m_PositionBase;

        public Type ElementType
        {
            get { return m_Type; }
        }

        public Type DisplayType {
            get {
                if (Draggable == false) {
                    return m_FinalDisplay;
                } else {
                    return m_Type;
                }
            }
        }

        /// <summary>
        /// the top left corner of the hit box
        /// </summary>
        public override Vector2 Position
        {
            get { return m_Position; }
            set 
            {
                m_PositionBase = value;
                base.Position = m_PositionBase + new Vector2(0.0f, 5 * (float)Math.Cos(floatOffset));
            }
        }
            
        /// <summary>
        /// This is a private constructor in order to force you to create Elements by using
        /// the Element.ElementBuilder static class.
        /// </summary>
        /// <param name="a_Type"></param>
        /// <param name="a_Level"></param>
        private Element(Type a_Type, Level a_Level, int newSlot) : base(a_Level, newSlot)
        {
            m_Type = a_Type;
            Draggable = (m_Type != Type.Fizzle);
            LoadContent(a_Level);
            m_Level = a_Level;
            m_nHeight = 90;
            m_nWidth = 90;
            floatOffset = rand.NextDouble()*2*Math.PI;
            UserInput.OnDoubleClick += Mouse_OnDoubleClick;
        }

        public void Mouse_OnDoubleClick()
        {
            System.Diagnostics.Debug.WriteLine(MouseOverMe() + "-" + currHolder);
            if (MouseOverMe() && OnSpamDrop != null && (currHolder == Holder.Generator || currHolder == Holder.SaveSlot))
            {
                DropAreas = new List<WorkSpace>();
                OnSpamDrop(this);
                if (DropAreas.Count > 0)
                {
                    if (OnDrop != null)
                        OnDrop(this);
                    DropAreas[0].DroppedIn(this, SageGame.gameSelectedIndex);//SLOT
                    //UserInput.OnDoubleClick -= Mouse_OnDoubleClick;
                }
            }
        }

        public int GetElementTier() {
            if (m_Type >= Type.Spirit) {
                return 6;
            } else if (m_Type >= Type.Phoenix) {
                return 5;
            } else if (m_Type >= Type.Metal) {
                return 4;
            } else if (m_Type >= Type.Magma) {
                return 3;
            } else if (m_Type >= Type.Combustion) {
                return 2;
            } else if (m_Type >= Type.Fire) {
                return 1;
            } else {
                return 0;
            }
        }

        public void LoadContent(Level a_Level)
        {
            if (m_tBackground == null)
            {
                m_tBackground = a_Level.Content.Load<Texture2D>("Elements/element-token-backdrop");
                m_tShine = a_Level.Content.Load<Texture2D>("Elements/element-token-shine");
                m_Font = a_Level.Content.Load<SpriteFont>("UI/Lithos Pro Regular Element");
            }
            if (ElementType != Type.Fizzle)
            {
               m_tIcon = a_Level.Content.Load<Texture2D>("Elements/symbols-token/" + ElementType.ToString().ToLower());
            }
        }

        public override void Dispose()
        {
            m_Level = null;
            UserInput.OnDoubleClick -= Mouse_OnDoubleClick;
            DropAreas = null;
            base.Dispose();
        }

        protected override void HandleDrop()
        {
            DropAreas = new List<WorkSpace>();
            SaveAreas = new List<SaveSlot>();
            base.HandleDrop();
            if (DropAreas.Count == 0 && SaveAreas.Count == 0)
            {

                currHolder = Holder.None;
                Draggable = false;

            }
            else
            {
                SaveSlot bestSaveArea = null;
                WorkSpace bestDropArea = null;
                int sBiggestArea = 0;
                int nBiggestArea = 0;
                int nCurrArea;
                Rectangle currRectangle;

                foreach (WorkSpace dropArea in DropAreas)
                {
                    currRectangle = Rectangle.Intersect(dropArea.BoundingBox, BoundingBox);
                    nCurrArea = currRectangle.Height * currRectangle.Width;
                    if (nCurrArea > nBiggestArea)
                    {
                        nBiggestArea = nCurrArea;
                        bestDropArea = dropArea;
                    }
                }

                foreach (SaveSlot saveArea in SaveAreas)
                {
                    currRectangle = Rectangle.Intersect(saveArea.BoundingBox, BoundingBox);
                    nCurrArea = currRectangle.Height * currRectangle.Width;
                    if (nCurrArea > nBiggestArea)
                    {
                        sBiggestArea = nCurrArea;
                        bestSaveArea = saveArea;
                    }
                }
                if (bestDropArea != null)
                {
                    bestDropArea.DroppedIn(this, SageGame.gameSelectedIndex);//SLOT
                    currHolder = Holder.Board;
                    //UserInput.OnDoubleClick -= Mouse_OnDoubleClick;
                }
                else if (bestSaveArea != null)
                {
                    bestSaveArea.DroppedIn(this);
                    currHolder = Holder.SaveSlot;
                    //UserInput.OnDoubleClick -= Mouse_OnDoubleClick;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Draggable == false)
            {
                m_nTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (m_nTimer >= FADE_OUT_TIME_MILLIS)
                {
                    m_Level.Remove(this);
                    m_nTimer = 0;
                }
            }
            floatOffset = (floatOffset + (double)gameTime.ElapsedGameTime.Milliseconds / 1000.0) % (2 * Math.PI);
            base.Position = m_PositionBase + new Vector2(0.0f, 5 * (float)Math.Cos(floatOffset));
            if (SageGame.ButtonJustPressed(Buttons.A)) System.Diagnostics.Debug.WriteLine("Update " + this.ToString());
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float layer = 0.5f;
            if (currState == State.Dragging) {
                layer = 0;
            }
            Vector2 texturePosition = m_Position + m_TextureOffset;
            spriteBatch.Begin();
            float nTransparancy = (Draggable == false ? 1 - (float)m_nTimer / FADE_OUT_TIME_MILLIS : 1);
            spriteBatch.Draw(m_tBackground, texturePosition, null, Color.White * nTransparancy, m_Rotation, Vector2.Zero, 1, SpriteEffects.None, layer);
            if(m_tIcon != null)
                spriteBatch.Draw(m_tIcon, m_Position + new Vector2(m_nWidth/2, m_nHeight/2 + ICON_OFFSET), null, Color.White * nTransparancy, 0, new Vector2(m_tIcon.Width/2, m_tIcon.Height/2), 1, SpriteEffects.None, layer);
            spriteBatch.DrawString(m_Font, DisplayType.ToString(), m_Position + new Vector2((m_nWidth - m_Font.MeasureString(DisplayType.ToString()).X) / 2, ENGLISH_PADDING_TOP), new Color(48, 48, 48) * nTransparancy);
            spriteBatch.Draw(m_tShine, texturePosition, null, Color.White * nTransparancy, m_Rotation, Vector2.Zero, 1, SpriteEffects.None, layer);
            spriteBatch.End();
        }

        public void RegisterDropArea(WorkSpace a_WorkSpace)
        {
            DropAreas.Add(a_WorkSpace);
        }

        public void RegisterSaveArea(SaveSlot a_SaveSlot)
        {
            SaveAreas.Add(a_SaveSlot);
        }

        public void Cast()
        {
            m_Level.AddSFX(Level.SFXNames.cast);
            if (OnCast != null)
                OnCast(this);
            Destroy(true);
        }

        public void Destroy(bool keepDisplay) {
            if (keepDisplay) {
                m_FinalDisplay = m_Type;
            }
            Destroy();
        }

        public void Destroy()
        {
            m_Type = Type.Fizzle;
            Draggable = false;
            LoadContent(m_Level);
        }
    }
}
