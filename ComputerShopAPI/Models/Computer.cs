using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerShopAPI.Models
{
    public class Computer
    {
        [Key]
        [MaxLength(36)]
        public Guid Id { get; set; }
        [Column(TypeName = "varchar(30)")]
        public string Brand { get; set; }
        [Column(TypeName = "varchar(40)")]
        public string Type { get; set; }
        [Column(TypeName = "float")]
        public double Display { get; set; }
        public int Memory { get; set; }
        public DateTime CreatedTime { get; set; }
        public Osystem Osys { get; set; }
    }
}
