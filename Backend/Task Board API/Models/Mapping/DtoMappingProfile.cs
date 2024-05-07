using AutoMapper;
using Task_Board_API.Models.Board;
using Task_Board_API.Models.DTOs;

namespace Task_Board_API.Models.Mapping
{
    public class DtoMappingProfile : Profile
    {
        public DtoMappingProfile()
        {
            CreateMap<Card, CardDto>();
            CreateMap<CardEditDto, Card>();
            CreateMap<BoardList, BoardListDto>();
            CreateMap<BoardListEditDto, BoardList>();
            CreateMap<CardHistory, CardHistoryDto>();
        }
    }
}