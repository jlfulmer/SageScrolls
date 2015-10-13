using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace sagescroll
{
    public abstract class DraggableItem : IDisplayableItem
    {
        public delegate void DraggableEvent(DraggableItem sender);
        public DraggableEvent OnDrop;
        public DraggableEvent OnGrab;
        public bool actedOn { get; set; }
        
        public enum State
        {
            Dragging, Static
        }
        UserInput m_Input;
        protected Vector2 m_Position;
        protected int m_nWidth;
        protected int m_nHeight;
        public State currState;
        private Vector2 m_MouseOffset;
        private bool m_bDraggable = true;
        private bool m_bButtonPressedOnMe;
        int slot;

        public virtual Vector2 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public bool Draggable
        {
            get { return m_bDraggable; }
            set { m_bDraggable = value; }
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)m_Position.X, (int)m_Position.Y, m_nWidth, m_nHeight); }
        }

        public DraggableItem(Level a_Level, int newSlot)
        {
            m_Input = a_Level.Input;
            currState = State.Static;
            m_bButtonPressedOnMe = false;
            slot = newSlot;
        }

        protected bool MouseOverMe()
        {
#if !WINDOWS
            return new Rectangle((int)m_Position.X, (int)m_Position.Y, m_nWidth, m_nHeight).Contains((int)SageGame.GetGameLoc().X, (int)SageGame.GetGameLoc().Y);
#else
            return new Rectangle((int)m_Position.X, (int)m_Position.Y, m_nWidth, m_nHeight).Contains(Mouse.GetState().X, Mouse.GetState().Y);
#endif
        }


        public virtual void Update(GameTime a_GameTime)
        {
            
            if (!m_bDraggable)
                return;
            switch (currState)
            {
                case State.Dragging:

#if !WINDOWS
                    if (SageGame.ButtonJustPressed(Buttons.A)) {
                    m_bButtonPressedOnMe = false;
                        currState = State.Static;
                    System.Diagnostics.Debug.WriteLine("Drop");
            
                        HandleDrop();
                    }
#else
                    if (Mouse.GetState().LeftButton == ButtonState.Released) {
                        m_bButtonPressedOnMe = false;
                        currState = State.Static;
                        HandleDrop();
                    }
#endif
                    
                    else
                    {
#if !WINDOWS
                        Position = new Vector2(SageGame.GetGameLoc().X-45, SageGame.GetGameLoc().Y-55);
#else
                        Position = new Vector2(Mouse.GetState().X - m_MouseOffset.X, Mouse.GetState().Y - m_MouseOffset.Y);
#endif
                    }
                    break;
                case State.Static:
                    if (!Level.CannotGrab) {
                        if (!m_bButtonPressedOnMe && (m_Input.ButtonJustPressed(UserInput.MouseButton.Left) || SageGame.ButtonJustPressed(Buttons.A)) && MouseOverMe()) {
                            m_bButtonPressedOnMe = true;
                        }
                        if (m_bButtonPressedOnMe) {
#if !WINDOWS

                        currState = State.Dragging;
                                //m_MouseOffset = new Vector2(Mouse.GetState().X - m_Input.MouseMovement().X - m_Position.X, Mouse.GetState().Y - m_Input.MouseMovement().Y - m_Position.Y);
                        System.Diagnostics.Debug.WriteLine("Grab");        
                        HandleGrab();
#else
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed) {
                                if (m_Input.MouseMovement() != Vector2.Zero) {
                                    currState = State.Dragging;
                                    m_MouseOffset = new Vector2(Mouse.GetState().X - m_Input.MouseMovement().X - m_Position.X, Mouse.GetState().Y - m_Input.MouseMovement().Y - m_Position.Y);

                                    HandleGrab();
                                }
                            } else {
                                m_bButtonPressedOnMe = false;
                            }
#endif
                        }
                    }
                    break;
                default:
                    break;
            }
            
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        protected virtual void HandleGrab()
        {
            actedOn = true;
            if (OnGrab != null)
                OnGrab(this); 
        }

        protected virtual void HandleDrop()
        {
            actedOn = true;
            if (OnDrop != null)
                OnDrop(this);
        }

        public virtual void Dispose()
        {
        }
    }
}
