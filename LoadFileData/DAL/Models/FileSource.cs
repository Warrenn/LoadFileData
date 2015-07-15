using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadFileData.DAL.Models
{
    public class FileSource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime DateEdit { get; set; }
        public string FileHash { get; set; }
        public string OriginalFileName { get; set; }
        public string CurrentFileName { get; set; }
        public int TotalRows { get; set; }
        public int CurrentRow { get; set; }
        public FileStatus Status { get; set; }
        public string SettingsName { get; set; }
    }
}
