
namespace Atlas.Models.DTOs
{
    internal class SimpleFileInfo
    {
        public string Name { get; set; } = string.Empty;
        public bool IsDirectory { get; set; }
        public long Size { get; set; }
    }
}
