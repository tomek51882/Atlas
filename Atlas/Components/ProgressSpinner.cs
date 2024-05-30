using Atlas.Core;
using Atlas.Core.Styles;
using Atlas.Interfaces.Renderables;
using Atlas.Primitives;
using Atlas.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Atlas.Components
{
    internal class ProgressSpinner : IPrimitive, IPrimitiveText, IDisposable  // ComponentBase, IDisposable
    {
        private System.Timers.Timer Timer;
        //private Text Spinner;
        private char[] frameMap = { '⣰', '⢸', '⠹', '⠛', '⠏', '⡇', '⣆', '⣤' };
        private byte frameIndex = 0;

        private Rect _rect = new Rect(0,0,1,1);
        public Rect Rect { 
            get => _rect;
            set => _rect = value;
        }

        public StyleProperties StyleProperties { get; } = new StyleProperties();

        public string? Value => frameMap[frameIndex].ToString();

        internal ProgressSpinner()
        {
            Timer = new System.Timers.Timer(50);
            Timer.Elapsed += UpdateSpinner;
            Timer.Enabled = true;
        }

        //public override void OnInitialized()
        //{
        //    Spinner = new Text();
        //    Timer = new System.Timers.Timer(100);
        //    Timer.Elapsed += UpdateSpinner;
        //    Timer.Enabled = true;
        //}

        private void UpdateSpinner(object? sender, ElapsedEventArgs e)
        {
            frameIndex++;
            if (frameIndex >= frameMap.Length)
            {
                frameIndex = 0;
            }
            //Spinner.Value = frameMap[frameIndex].ToString();
        }

        //public override void BuildRenderTree(RenderTreeBuilder builder)
        //{
        //    builder.AddContent(Spinner);
        //}

        public void Dispose()
        {
            Timer?.Dispose();
        }
    }
}
