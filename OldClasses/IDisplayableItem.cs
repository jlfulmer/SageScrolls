using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sagescroll
{

    public interface IDisplayableItem : IDisposable
    {
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        void Update(GameTime gameTime);
        bool actedOn { get; set; }
    }
}
