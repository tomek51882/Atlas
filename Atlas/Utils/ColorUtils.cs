using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Utils
{
    internal class ColorUtils
    {
        internal static (byte r, byte g, byte b) HSL2RGB(double h, double sl, double l)
        {
            double v;
            double r, g, b;

            r = l;
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);

            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;

                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;

                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;

                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;

                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;

                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;

                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;

                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;

                }

            }

            byte rr = Convert.ToByte(r * 255.0f);
            byte rg = Convert.ToByte(g * 255.0f);
            byte rb = Convert.ToByte(b * 255.0f);

            return (rr, rg, rb);

        }
        internal static (double h, double s, double l) RGB2HSL(byte r, byte g, byte b)
        {
            double _r = (r / 255f);
            double _g = (g / 255f);
            double _b = (b / 255f);
            double v;

            double m;

            double vm;

            double r2, g2, b2;



            double h = 0; // default to black

            double s = 0;

            double l = 0;

            v = Math.Max(_r, _g);

            v = Math.Max(v, _b);

            m = Math.Min(_r, _g);

            m = Math.Min(m, _b);

            l = (m + v) / 2.0;

            if (l <= 0.0)

            {

                return (0,0,0);

            }

            vm = v - m;

            s = vm;

            if (s > 0.0)

            {

                s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);

            }

            else

            {

                return (0, 0, 0);

            }

            r2 = (v - _r) / vm;

            g2 = (v - _g) / vm;

            b2 = (v - _b) / vm;

            if (_r == v)

            {

                h = (_g == m ? 5.0 + b2 : 1.0 - g2);

            }

            else if (_g == v)

            {

                h = (_b == m ? 1.0 + r2 : 3.0 - b2);

            }

            else

            {

                h = (_r == m ? 3.0 + g2 : 5.0 - r2);

            }

            h /= 6.0;
            return (h, s, l);
        }
    }
}
