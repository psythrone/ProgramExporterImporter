using System.ComponentModel.DataAnnotations;

namespace ProgramExporterImporter.Model
{
    public class Product
    {
        public Product(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public Product()
        {

        }



        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        
    }
}
