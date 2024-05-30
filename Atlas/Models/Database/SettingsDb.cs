using System.ComponentModel.DataAnnotations;

namespace Atlas.Models.Database
{
    internal class SettingsDb
    {
        [Required]
        [Key]
        public Guid Id { get; set; }

    }
}
