using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using Task_Board_API.Models.Board;
using Task_Board_API.Models.DTOs;
using Task_Board_API.Models.ErrorResponses;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using System.Security.Policy;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Task_Board_API.Controllers
{
   [Route("api/[controller]")]
    [ApiController]
    [EnableCors("OpenCORSPolicy")]
    public class HistoryController : ControllerBase
    {
        private readonly BoardContext _boardContext;
        private readonly IMapper _mapper;

        public HistoryController(BoardContext boardContext, IMapper mapper)
        {
            _boardContext = boardContext;
            _mapper = mapper;
        }

        // GET: api/<Controller>
        [HttpGet("{page}")]
        public async Task<IActionResult> Get(int page)
        {
            List<CardHistory> cardHistory = await _boardContext.CardHistories.OrderByDescending(x => x.DateTime).Skip(page * 20).Take(20).ToListAsync();
            if (cardHistory.Count == 0)
            {
                return NoContent();
            }

            List<CardHistoryDto> cardHistoryDtos = _mapper.ProjectTo<CardHistoryDto>(cardHistory.AsQueryable()).ToList();
            return Ok(cardHistoryDtos);
        }

        // GET api/<Controller>/5/Cards/5
        [HttpGet("{page}/Cards/{id}")]
        public async Task<IActionResult> Get(int id, int page)
        {
            if (!await _boardContext.Cards.Where(x => x.Id == id).AnyAsync())
            {
                return NotFound();
            }

            List<CardHistory> cardHistory = await _boardContext.Cards.Where(x => x.Id == id).SelectMany(x => x.CardHistories).OrderByDescending(x => x.DateTime).Skip(page * 20).Take(20).ToListAsync();
            if (cardHistory.Count == 0)
            {
                return NoContent();
            }

            List<CardHistoryDto> cardHistoryDtos = _mapper.ProjectTo<CardHistoryDto>(cardHistory.AsQueryable()).ToList();
            return Ok(cardHistoryDtos);
        }
    }
}
