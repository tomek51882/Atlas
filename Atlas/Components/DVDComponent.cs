
using Atlas.Core;
using Atlas.Extensions;
using Atlas.Primitives;
using System.Timers;

namespace Atlas.Components
{
    internal class DVDComponent : ComponentBase
    {
        private Text Logo;
        private System.Timers.Timer Timer;
        private int x = 29;
        private int y = 9;
        private int dx = 1;
        private int dy = 1;

        //58x18
        public override void OnInitialized()
        {
            Logo = new Text("Hello World");
            Logo.Rect = Logo.Rect.Move(x, y);

            Timer = new System.Timers.Timer(100);
            Timer.Elapsed += UpdateLogo;
            Timer.Enabled = true;
        }

        private void UpdateLogo(object? sender, ElapsedEventArgs e)
        {
            x += dx;
            y += dy;

            if (x <= 1 || x >= 70)
            {
                Random random = new Random();
                double randomValue = random.NextDouble();
                (byte r, byte g, byte b) = HSL2RGB(randomValue, 1, 0.5);
                var test = new Types.Color(r, g, b);
                Logo.StyleProperties.Color = new Core.Styles.ColorProperty(test);
                dx = -dx;
            }
            if (y <= 1 || y >= 19)
            {
                Random random = new Random();
                double randomValue = random.NextDouble();
                (byte r, byte g, byte b) = HSL2RGB(randomValue, 1, 0.5);
                var test = new Types.Color(r, g, b);
                Logo.StyleProperties.Color = new Core.Styles.ColorProperty(test);
                dy = -dy;
            }

            Logo.Rect = Logo.Rect.Move(dx, dy);


        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(Logo);
            //UpdateLogo();
        }

        public (byte r, byte g, byte b) HSL2RGB(double h, double sl, double l)
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
    }
}
