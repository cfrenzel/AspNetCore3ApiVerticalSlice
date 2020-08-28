using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Banking.Core.Entities;

namespace Banking.Api
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            /***************** Institutions **********/
            CreateMap<Institution, Institutions.List.InstitutionModel>();
            CreateMap<Institution, Institutions.Find.InstitutionModel>();
            CreateMap<Institution, Institutions.Create.InstitutionModel>();
            

            /*************** Members ******************/
            CreateMap<Member,  Members.List.MemberModel>();
            CreateMap<Account, Members.List.AccountModel>();
            
            CreateMap<Member,  Members.Find.MemberModel>();
            CreateMap<Account, Members.Find.AccountModel>();

            CreateMap<Member, Members.Create.MemberModel>();

            CreateMap<Member, Members.Update.MemberModel>();
            CreateMap<Account, Members.Update.AccountModel>();

        }
    }
}
