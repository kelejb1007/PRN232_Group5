using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTOs.BookDTO;
using DAL.DTOs.UserAccount;
using DAL.DTOs.CategoryDTOs;
using DAL.Models;
using AutoMapper;
namespace DAL.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            //add cate map qua model
            CreateMap<CategoryCreateDto, Category>();
            //lấy model map qua viewDTO
            CreateMap<Category, CategoryResponseDto>();
            //updateDTO qua model
            CreateMap<CategoryUpdate, Category>();
            //model qua BookDTO
            CreateMap<Book, BookResponeDTO>();
            //BookDTO qua model 
            CreateMap<BookResponeDTO, Book>();
            //CreateDTO qua model
            CreateMap<BookCreateDTO, Book>();

            CreateMap<UserAccount, AuthDTO.AuthResponseDTO>();
        }
    }
}
