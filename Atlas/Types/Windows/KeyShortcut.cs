
namespace Atlas.Types.Windows
{
    internal struct KeyShortcut : IEquatable<KeyShortcut>
    {
        internal ConsoleKey Key { get; set; }
        internal ConsoleModifiers KeyModifier { get; set; }

        internal static KeyShortcut None => new KeyShortcut { Key = ConsoleKey.None, KeyModifier = ConsoleModifiers.None };

        public KeyShortcut(ConsoleKeyInfo key)
        {
            Key = key.Key;
            KeyModifier = key.Modifiers;
        }

        public bool Equals(KeyShortcut other)
        {
            return Key == other.Key && KeyModifier == other.KeyModifier;
        }

        public static bool operator ==(KeyShortcut left, KeyShortcut right) => left.Equals(right);
        public static bool operator !=(KeyShortcut left, KeyShortcut right) => !left.Equals(right);
    }
}
