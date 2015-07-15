using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadFileData.DAL.Models
{
    public class Error
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public Guid? FileId { get; set; }
        public string FileName { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Date { get; set; }
    }
}
