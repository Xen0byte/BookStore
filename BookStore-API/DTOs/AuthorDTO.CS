﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class AuthorDTO
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public virtual IList<BookDTO> Books { get; set; }
    }

    public class AuthorCreateDTO
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        public string Bio { get; set; }
    }

    public class AuthorUpdateDTO
    {
        [Required] public int ID { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        public string Bio { get; set; }
    }
}