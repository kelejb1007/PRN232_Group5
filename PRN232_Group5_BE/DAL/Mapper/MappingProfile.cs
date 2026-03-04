using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DAL.DTOs.UserAccount;
using DAL.Models;
using AutoMapper;

namespace DAL.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserAccount, AuthDTO.AuthResponseDTO>();
        }
    }
}
