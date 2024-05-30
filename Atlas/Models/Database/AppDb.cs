using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Models.Database
{
    internal class AppDb
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
