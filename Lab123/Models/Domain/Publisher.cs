using System.ComponentModel.DataAnnotations;

namespace Lab123.Models.Domain
{
    public class Publisher
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        //Navigation Properties - One publisher has many books
        public List<Book> Books { get; set; }
    }
}
