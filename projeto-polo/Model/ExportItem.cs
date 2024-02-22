using System.ComponentModel.DataAnnotations;

namespace projeto_polo.Model
{
    class ExportItem : Item
    {
        [Key]
        public required string ItemId { get; set; }
        public required string ExportId { get; set; }
    }
}
