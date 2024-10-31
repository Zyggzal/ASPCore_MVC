using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ASPCore_MVC.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; } = 0;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;

        [StringLength(1024)]
        public string Description { get; set; } = "";
    }
}
