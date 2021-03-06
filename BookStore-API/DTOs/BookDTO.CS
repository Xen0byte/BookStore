﻿using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class BookDTO
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public string ISBN { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }
        public decimal? Price { get; set; }
        public int? AuthorID { get; set; }
        public virtual AuthorDTO Author { get; set; }
    }

    public class BookCreateDTO
    {
        [Required] public string Title { get; set; }
        public int? Year { get; set; }
        [Required] public string ISBN { get; set; }
        [StringLength(500)] public string Summary { get; set; }
        public string Image { get; set; }
        public decimal? Price { get; set; }
        [Required] public int AuthorID { get; set; }
    }

    public class BookUpdateDTO
    {
        public int ID { get; set; }
        [Required] public string Title { get; set; }
        public int? Year { get; set; }
        [StringLength(500)] public string Summary { get; set; }
        public string Image { get; set; }
        public decimal? Price { get; set; }
        public int? AuthorID { get; set; }
    }
}