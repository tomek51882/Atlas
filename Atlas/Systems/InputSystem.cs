using Atlas.Attributes;
using Atlas.Components;
using Atlas.Interfaces;

namespace Atlas.Systems
{
    internal class InputSystem : IInputSystem
    {

        public event Action<ConsoleKeyInfo> OnKeyPress = delegate { };
        //public event Action<Command> OnCommandEntered = delegate { };

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
