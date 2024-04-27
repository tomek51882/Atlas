namespace Atlas.Types.Windows
{
    internal ref struct WindowFrame
    {
        public static WindowFrame Window => new WindowFrame('┌', '┐', '└', '┘', '│', '─');
        public static WindowFrame WindowFocused => new WindowFrame('╔', '╗', '╚', '╝', '║', '═');

        public Dictionary<WindowFrameFragment, char> fragmentMap = new Dictionary<WindowFrameFragment, char>();
        WindowFrame(char topLeft, char topRight, char bottomLeft, char bottomRight, char vertical, char horizontal)
        {
            fragmentMap.Add(WindowFrameFragment.TopLeft, topLeft);
            fragmentMap.Add(WindowFrameFragment.TopRight, topRight);
            fragmentMap.Add(WindowFrameFragment.BottomLeft, bottomLeft);
            fragmentMap.Add(WindowFrameFragment.BottomRight, bottomRight);
            fragmentMap.Add(WindowFrameFragment.Vertical, vertical);
            fragmentMap.Add(WindowFrameFragment.Horizontal, horizontal);
        }
    }
}
