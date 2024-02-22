using System.ComponentModel.DataAnnotations;

namespace projeto_polo.Model
{
    class Export
    {
        [Key]
        public required string Id { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
