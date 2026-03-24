using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DAL.DTOs.UserAccount;
using DAL.DTOs.CategoryDTOs;
using DAL.Models;
using AutoMapper;

namespace DAL.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //add cate map qua model
            CreateMap<CategoryCreateDto, Category>();
            //lấy model map qua viewDTO
            CreateMap<Category, CategoryResponseDto>();
            //updateDTO qua model
            CreateMap<CategoryUpdate, Category>();
            CreateMap<UserAccount, AuthDTO.AuthResponseDTO>();

        }
    }
}
