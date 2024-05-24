using System.ComponentModel.DataAnnotations.Schema;

namespace YY.Docker.Http.API.Models
{
    public class Person
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
