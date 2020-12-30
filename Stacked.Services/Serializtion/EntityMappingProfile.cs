using AutoMapper;
using Stacked.Data.Models;
using Stacked.Models;

namespace Stacked.Services.Serialization
{
    public class EntityMappingProfile : Profile
    {
        public EntityMappingProfile()
        {
            CreateMap<Article, ArticleDto>().ReverseMap();
            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}