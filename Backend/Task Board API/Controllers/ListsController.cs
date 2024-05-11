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
    public class ListsController : ControllerBase
    {
        private readonly BoardContext _boardContext;
        private readonly IMapper _mapper;

        public ListsController(BoardContext boardContext, IMapper mapper)
        {
            _boardContext = boardContext;
            _mapper = mapper;
        }

        // GET: api/<Controller>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<BoardList> boardLists = await _boardContext.BoardLists.ToListAsync();
            if (boardLists.Count == 0)
            {
                return NoContent();
            }

            List<BoardListDto> boardListDtos = _mapper.ProjectTo<BoardListDto>(boardLists.AsQueryable()).ToList();
            return Ok(boardListDtos);
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            BoardList? boardList = await _boardContext.BoardLists.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (boardList == null)
            {
                return NotFound();
            }

            BoardListDto boardListDto = _mapper.Map<BoardListDto>(boardList);
            return Ok(boardListDto);
        }

        // POST api/<Controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BoardListEditDto boardListDto)
        {
            BoardList boardList = _mapper.Map<BoardList>(boardListDto);
                        _boardContext.BoardLists.Add(boardList);

            await _boardContext.SaveChangesAsync();
            return Ok();
        }

        // PATCH api/<Controller>/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] BoardListEditDto boardListDto)
        {
            BoardList? boardList = await _boardContext.BoardLists.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (boardList == null)
            {
                return NotFound();
            }

            if (boardListDto.Name != null && boardListDto.Name != boardList.Name)
            {
                boardList.Name = boardListDto.Name;
            }

            await _boardContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<Controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            BoardList? boardList = await _boardContext.BoardLists.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (boardList == null)
            {
                return NotFound();
            }

            _boardContext.BoardLists.Remove(boardList);
            await _boardContext.SaveChangesAsync();
            return Ok();
        }
    }
}
