using API.Data.Models;
using API.DTOs;
using AutoMapper;

namespace API.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Author, AuthorCreateDTO>().ReverseMap();

            CreateMap<Book, BookDTO>().ReverseMap();
        }
    }
}