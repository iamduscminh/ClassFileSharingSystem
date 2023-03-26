using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entities
{
    public class File
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FileId { get; set; }
        [Required]
        [StringLength(40)]
        public string FileName { get; set; } = null!;
        public string CloudId { get; set; } = null!;
        public DateTime CreateDate { get; set; }

        [ForeignKey("Resource")]
        public int ResourceId { get; set; }
        //[NotMapped]
        //public virtual Resource Resource { get; set; }

    }
}
