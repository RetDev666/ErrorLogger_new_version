using AutoMapper;

namespace ErrSendApplication.Interfaces.Mapping
{
    public interface IMapWith<T>
    {
        void Mapping(Profile profile)
        {
            profile.CreateMap(typeof(T), GetType());
        }
    }
}


