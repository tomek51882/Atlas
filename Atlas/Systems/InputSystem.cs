using Atlas.Components;
using Atlas.Interfaces;

namespace Atlas.Systems
{
    internal class InputSystem : IInputSystem
    {
        private readonly IWindowService _windowService;
        private readonly InputSystemDebugLine inputSystemDebugLine;

        public event Action<ConsoleKeyInfo> OnKeyPress = delegate { };
        //public event Action<Command> OnCommandEntered = delegate { };

        public InputSystem()
        {
            //_windowService = windowService;
            //(_, inputSystemDebugLine) = _windowService.CreateWindow<InputSystemDebugLine>(new Types.Rect(0, 24, 126, 3), "Input Debug");
        }

        public void Update()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                OnKeyPress?.Invoke(key);
            }
        }
    }
}
