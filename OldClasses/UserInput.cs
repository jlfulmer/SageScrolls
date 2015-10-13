using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace sagescroll
{
    public class UserInput
    {
        public delegate void UserInputEvent();
        public static UserInputEvent OnDoubleClick;

        public enum MouseButton
        {
            Right, Left, Middle
        }
        public enum DoubleClickStage
        {
            FirstClick,FirstRelease,SecondClick,None
        }
        MouseState PrevMouseState;
        KeyboardState PrevKeyboardState;
        static UserInput singleton;
        const int MILLIS_BETWEEN_DOUBLE_CICKS = 700;
        int m_nTimeSinceLastClick;
        DoubleClickStage m_DoubleClickStage;

        public static UserInput GetUserInput()
        {
            if (singleton == null)
            {
                singleton = new UserInput();
            }
            return singleton;
        }

        private UserInput()
        {
            PrevKeyboardState = Keyboard.GetState();
            PrevMouseState = Mouse.GetState();
            m_nTimeSinceLastClick = MILLIS_BETWEEN_DOUBLE_CICKS + 1;
            m_DoubleClickStage = DoubleClickStage.None;
        }

        private void HandleDoubleClick(GameTime gameTime)
        {
            if (MouseMovement() != Vector2.Zero)
            {
                m_DoubleClickStage = DoubleClickStage.None;
                m_nTimeSinceLastClick = MILLIS_BETWEEN_DOUBLE_CICKS + 1;
                return;
            }
            if (m_nTimeSinceLastClick <= MILLIS_BETWEEN_DOUBLE_CICKS)
                m_nTimeSinceLastClick += gameTime.ElapsedGameTime.Milliseconds;
            if (m_nTimeSinceLastClick > MILLIS_BETWEEN_DOUBLE_CICKS)
            {
                m_DoubleClickStage = DoubleClickStage.None;   
            }
            switch (m_DoubleClickStage)
            {
                case DoubleClickStage.None:
                    if (ButtonJustPressed(MouseButton.Left))
                    {
                        m_nTimeSinceLastClick = 0;
                        m_DoubleClickStage = DoubleClickStage.FirstClick;   
                    }
                    break;
                case DoubleClickStage.FirstClick:
                    if (ButtonJustReleased(MouseButton.Left))
                    {
                        m_DoubleClickStage = DoubleClickStage.FirstRelease;
                    }
                    break;
                case DoubleClickStage.FirstRelease:
                    if (ButtonJustPressed(MouseButton.Left))
                    {
                        m_DoubleClickStage = DoubleClickStage.SecondClick;
                    }
                    break;
                case DoubleClickStage.SecondClick:
                    if (ButtonJustReleased(MouseButton.Left))
                    {
                        m_DoubleClickStage = DoubleClickStage.None;
                        m_nTimeSinceLastClick = MILLIS_BETWEEN_DOUBLE_CICKS + 1;
                        if (OnDoubleClick != null)
                            OnDoubleClick();   
                    }
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            HandleDoubleClick(gameTime);
            PrevMouseState = Mouse.GetState();
            PrevKeyboardState = Keyboard.GetState();
        }

        /// <summary>
        /// returns true if the user just double-clicked the left mouse button
        /// </summary>
        /// <returns></returns>
        public bool JustDoubleClicked()
        {
            return (m_nTimeSinceLastClick <= MILLIS_BETWEEN_DOUBLE_CICKS && (ButtonJustPressed(MouseButton.Left) || SageGame.ButtonJustPressed(Buttons.A)));
        }

        public bool KeyJustPressed(Keys a_key)
        {
            return PrevKeyboardState.IsKeyUp(a_key) && Keyboard.GetState().IsKeyDown(a_key);
        }

        public bool KeyJustReleased(Keys a_key)
        {
            return PrevKeyboardState.IsKeyDown(a_key) && Keyboard.GetState().IsKeyUp(a_key);
        }

        public bool ButtonJustPressed(MouseButton a_button)
        {
            switch (a_button)
            {
                case MouseButton.Middle:
                    return PrevMouseState.MiddleButton == ButtonState.Released
                        && Mouse.GetState().MiddleButton == ButtonState.Pressed;
                case MouseButton.Left:
                    return PrevMouseState.LeftButton == ButtonState.Released
                        && Mouse.GetState().LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return PrevMouseState.RightButton == ButtonState.Released
                        && Mouse.GetState().RightButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        public bool ButtonJustReleased(MouseButton a_button)
        {
            switch (a_button)
            {
                case MouseButton.Middle:
                    return PrevMouseState.MiddleButton == ButtonState.Pressed
                        && Mouse.GetState().MiddleButton == ButtonState.Released;
                case MouseButton.Left:
                    return PrevMouseState.LeftButton == ButtonState.Pressed
                        && Mouse.GetState().LeftButton == ButtonState.Released;
                case MouseButton.Right:
                    return PrevMouseState.RightButton == ButtonState.Pressed
                        && Mouse.GetState().RightButton == ButtonState.Released;
                default:
                    return false;
            }
        }

        public Vector2 MouseMovement()
        {
            return new Vector2(Mouse.GetState().X - PrevMouseState.X, Mouse.GetState().Y - PrevMouseState.Y);
        }
    }
}
