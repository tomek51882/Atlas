using Atlas.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Models.Database
{
    internal class SavedAppsDb
    {
        [Required]
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public AppType AppType { get; set; }
        public string AppData { get; set; }
    }
}
