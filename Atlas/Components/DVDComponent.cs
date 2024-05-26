
using Atlas.Core;
using Atlas.Extensions;
using Atlas.Primitives;
using Atlas.Utils;
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

        //private void UpdateLogo(object? sender, ElapsedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        private void UpdateLogo(object? sender, ElapsedEventArgs e)
        {
            x += dx;
            y += dy;

            if (x <= 1 || x >= 74)
            {
                Random random = new Random();
                double randomValue = random.NextDouble();
                (byte r, byte g, byte b) = ColorUtils.HSL2RGB(randomValue, 1, 0.5);
                var test = new Types.Color(r, g, b);
                Logo.StyleProperties.Color = new Core.Styles.StyleProperty<Types.Color>(test);
                dx = -dx;
            }
            if (y <= 1 || y >= 22)
            {
                Random random = new Random();
                double randomValue = random.NextDouble();
                (byte r, byte g, byte b) = ColorUtils.HSL2RGB(randomValue, 1, 0.5);
                var test = new Types.Color(r, g, b);
                Logo.StyleProperties.Color = new Core.Styles.StyleProperty<Types.Color>(test);
                dy = -dy;
            }

            Logo.Rect = Logo.Rect.Move(dx, dy);


        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(Logo);
            //UpdateLogo();
        }

        
    }
}
