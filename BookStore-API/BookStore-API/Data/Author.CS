using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data
{
    [Table("Authors")]
    public class Author
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }

        public virtual IList<Book> Books { get; set; }
    }
}