using AutoMapper;

using ServerProject.Models;
using ServerProject.Models.Response;

namespace ServerProject.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Product, ProductInPOSResponse>()
            .ForMember(pro => pro.CategoryName, act => act.MapFrom(src => src.Category!.CategoryName));


        }

        //public static Mapper InitializeAutomapper()
        //{

        //    //Provide all the Mapping Configuration
        //    var config = new MapperConfiguration(cfg =>
        //    {
        //        //Configuring Employee and EmployeeDTO
        //        cfg.CreateMap<Product, ProductInPOSResponse>();
        //        //Any Other Mapping Configuration ....
        //    });
        //    //Create an Instance of Mapper and return that Instance
        //    var mapper = new Mapper(config);
        //    return mapper;
        //}
    }
}
