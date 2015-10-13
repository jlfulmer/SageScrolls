using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sagescroll
{
    class Animate
    {
        public enum Style
        {
            Linear, Fall, Parabolic
        }
        int m_nTotalTime;
        int m_nElapsedTime;

        public int TotalTimeMillis
        {
            get { return m_nTotalTime; }
            set { m_nTotalTime = value; }
        }

        public Animate(Style a_eStyle, int a_nTimeMillis, params double[] a_narrPoints)
        {
        }
    }
}
