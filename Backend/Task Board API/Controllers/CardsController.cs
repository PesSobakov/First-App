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
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Cors;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Task_Board_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("OpenCORSPolicy")]
    public class CardsController : ControllerBase
    {
        private readonly BoardContext _boardContext;
        private readonly IMapper _mapper;

        public CardsController(BoardContext boardContext, IMapper mapper)
        {
            _boardContext = boardContext;
            _mapper = mapper;
        }

        // GET: api/<Controller>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Card> cards = await _boardContext.Cards.ToListAsync();
            if (cards.Count == 0)
            {
                return NoContent();
            }

            List<CardDto> cardDtos = _mapper.ProjectTo<CardDto>(cards.AsQueryable()).ToList();
            return Ok(cardDtos);
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Card? card = await _boardContext.Cards.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (card == null)
            {
                return NotFound();
            }

            CardDto cardDto = _mapper.Map<CardDto>(card);
            return Ok(cardDto);
        }

        // POST api/<Controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CardEditDto cardDto)
        {
            BoardList? boardList = await _boardContext.BoardLists.Where(x => x.Id == cardDto.BoardListId).FirstOrDefaultAsync();
            if (boardList == null)
            {
                Dictionary<string, string> errors = new()
                {
                    { "BoardListId", "This list don't exist" }
                };
                return BadRequest(new ValidationError(errors));
            }

            Card card = _mapper.Map<Card>(cardDto);

            boardList.Cards.Add(card);

            CardHistory cardHistory = new CardHistory() { HistoryEvent = HistoryEvent.Create, DateTime = DateTime.UtcNow, Card = card, CurrentCardName = card.Name, CreatedAtListName = boardList.Name };
            _boardContext.CardHistories.Add(cardHistory);

            await _boardContext.SaveChangesAsync();
            return Ok();
        }

        // PATCH api/<Controller>/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] CardEditDto cardDto)
        {
            Card? card = await _boardContext.Cards.Where(x => x.Id == id).Include(x => x.BoardList).FirstOrDefaultAsync();
            if (card == null)
            {
                return NotFound();
            }

            if (cardDto.Name != null && cardDto.Name != card.Name)
            {
                CardHistory cardHistory = new CardHistory() { HistoryEvent = HistoryEvent.ChangeName, DateTime = DateTime.UtcNow, Card = card, CurrentCardName = card.Name, OldCardName = card.Name, NewCardName = cardDto.Name };
                _boardContext.CardHistories.Add(cardHistory);
                card.Name = cardDto.Name;
            }
            if (cardDto.Description != null && cardDto.Description != card.Description)
            {
                CardHistory cardHistory = new CardHistory() { HistoryEvent = HistoryEvent.ChangeDescription, DateTime = DateTime.UtcNow, Card = card, CurrentCardName = card.Name, OldDescription = card.Description, NewDescription = cardDto.Description };
                _boardContext.CardHistories.Add(cardHistory);
                card.Description = cardDto.Description;
            }
            if (cardDto.Priority != null && cardDto.Priority != card.Priority)
            {
                CardHistory cardHistory = new CardHistory() { HistoryEvent = HistoryEvent.ChangePriority, DateTime = DateTime.UtcNow, Card = card, CurrentCardName = card.Name, OldPriority = card.Priority, NewPriority = cardDto.Priority.Value };
                _boardContext.CardHistories.Add(cardHistory);
                card.Priority = cardDto.Priority.Value;
            }
            if (cardDto.DueDate != null && cardDto.DueDate != card.DueDate)
            {
                CardHistory cardHistory = new CardHistory() { HistoryEvent = HistoryEvent.ChangeDueDate, DateTime = DateTime.UtcNow, Card = card, CurrentCardName = card.Name, OldDueDate = card.DueDate, NewDueDate = cardDto.DueDate.Value };
                _boardContext.CardHistories.Add(cardHistory);
                card.DueDate = cardDto.DueDate.Value;
            }

            if (cardDto.BoardListId != null && cardDto.BoardListId != card.BoardListId)
            {
                BoardList? boardList = await _boardContext.BoardLists.Where(x => x.Id == cardDto.BoardListId).FirstOrDefaultAsync();
                if (boardList == null)
                {
                    Dictionary<string, string> errors = new()
                        {
                            { "BoardListId", "This list don't exist" }
                        };
                    return BadRequest(new ValidationError(errors));
                }

                CardHistory cardHistory = new CardHistory() { HistoryEvent = HistoryEvent.Move, DateTime = DateTime.UtcNow, Card = card, CurrentCardName = card.Name, OldListName = card.BoardList.Name, NewListName = boardList.Name };
                _boardContext.CardHistories.Add(cardHistory);

                card.BoardList.Cards.Remove(card);
                boardList.Cards.Add(card);
            }

            await _boardContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<Controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Card? card = await _boardContext.Cards.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (card == null)
            {
                return NotFound();
            }

            CardHistory cardHistory = new CardHistory() { HistoryEvent = HistoryEvent.Delete, DateTime = DateTime.UtcNow, CurrentCardName = card.Name, DeletedFromListName = card.BoardList?.Name };
            _boardContext.CardHistories.Add(cardHistory);

            _boardContext.Cards.Remove(card);
            await _boardContext.SaveChangesAsync();
            return Ok();
        }

        // put api/<Controller>
        [HttpPut]
        public async Task<IActionResult> Put()
        {
            _boardContext.Database.EnsureDeleted();
            _boardContext.Database.EnsureCreated();

            List<BoardList> boardLists = new();
            boardLists.Add(new() { Name = "To Do" });
            boardLists.Add(new() { Name = "Done" });
            boardLists[0].Cards.Add(new() { Name = "Task 6", Priority = Priority.Low, DueDate = new DateOnly(2024, 6, 6), Description = "sample description 1" });
            boardLists[0].Cards.Add(new() { Name = "Task 7", Priority = Priority.Low, DueDate = new DateOnly(2024, 6, 7), Description = "sample description 2" });
            boardLists[0].Cards.Add(new() { Name = "Task 8", Priority = Priority.Low, DueDate = new DateOnly(2024, 6, 8) });
            boardLists[0].Cards.Add(new() { Name = "Task 9", Priority = Priority.Medium, DueDate = new DateOnly(2024, 6, 9) });
            boardLists[0].Cards.Add(new() { Name = "Task 10", Priority = Priority.Medium, DueDate = new DateOnly(2024, 6, 10) });
            boardLists[1].Cards.Add(new() { Name = "Task 1", Priority = Priority.Medium, DueDate = new DateOnly(2024, 6, 1) });
            boardLists[1].Cards.Add(new() { Name = "Task 2", Priority = Priority.High, DueDate = new DateOnly(2024, 6, 2) });
            boardLists[1].Cards.Add(new() { Name = "Task 3", Priority = Priority.High, DueDate = new DateOnly(2024, 6, 3), Description = "sample description 3" });
            boardLists[1].Cards.Add(new() { Name = "Task 4", Priority = Priority.High, DueDate = new DateOnly(2024, 6, 4) });
            boardLists[1].Cards.Add(new() { Name = "Task 5", Priority = Priority.High, DueDate = new DateOnly(2024, 6, 5) });

            List<CardHistory> cardHistories = new();
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.Create, DateTime = new DateTime(2024, 05, 07, 12, 0, 0).ToUniversalTime(), Card = boardLists[0].Cards[0], CurrentCardName = boardLists[0].Cards[0].Name, CreatedAtListName = boardLists[0].Name });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.Create, DateTime = new DateTime(2024, 05, 07, 12, 0, 1).ToUniversalTime(), Card = boardLists[0].Cards[1], CurrentCardName = boardLists[0].Cards[1].Name, CreatedAtListName = boardLists[0].Name });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.Create, DateTime = new DateTime(2024, 05, 07, 12, 0, 2).ToUniversalTime(), Card = boardLists[0].Cards[2], CurrentCardName = boardLists[0].Cards[2].Name, CreatedAtListName = boardLists[0].Name });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.Create, DateTime = new DateTime(2024, 05, 07, 12, 0, 3).ToUniversalTime(), Card = boardLists[0].Cards[3], CurrentCardName = boardLists[0].Cards[3].Name, CreatedAtListName = boardLists[0].Name });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.Create, DateTime = new DateTime(2024, 05, 07, 12, 0, 4).ToUniversalTime(), Card = boardLists[0].Cards[4], CurrentCardName = boardLists[0].Cards[4].Name, CreatedAtListName = boardLists[0].Name });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.Create, DateTime = new DateTime(2024, 05, 07, 12, 0, 5).ToUniversalTime(), Card = boardLists[1].Cards[0], CurrentCardName = boardLists[1].Cards[0].Name, CreatedAtListName = boardLists[1].Name });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.Create, DateTime = new DateTime(2024, 05, 07, 12, 0, 6).ToUniversalTime(), Card = boardLists[1].Cards[1], CurrentCardName = boardLists[1].Cards[1].Name, CreatedAtListName = boardLists[1].Name });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.Create, DateTime = new DateTime(2024, 05, 07, 12, 0, 7).ToUniversalTime(), Card = boardLists[1].Cards[2], CurrentCardName = boardLists[1].Cards[2].Name, CreatedAtListName = boardLists[1].Name });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.Create, DateTime = new DateTime(2024, 05, 07, 12, 0, 8).ToUniversalTime(), Card = boardLists[1].Cards[3], CurrentCardName = boardLists[1].Cards[3].Name, CreatedAtListName = boardLists[1].Name });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.Create, DateTime = new DateTime(2024, 05, 07, 12, 0, 9).ToUniversalTime(), Card = boardLists[1].Cards[4], CurrentCardName = boardLists[1].Cards[4].Name, CreatedAtListName = boardLists[1].Name });

            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.ChangeName, DateTime = new DateTime(2024, 05, 07, 12, 0, 10).ToUniversalTime(), Card = boardLists[0].Cards[0], CurrentCardName = boardLists[0].Cards[0].Name, OldCardName = "old name", NewCardName = boardLists[0].Cards[0].Name });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.ChangeDescription, DateTime = new DateTime(2024, 05, 07, 12, 0, 11).ToUniversalTime(), Card = boardLists[0].Cards[0], CurrentCardName = boardLists[0].Cards[0].Name, OldDescription = "old description", NewDescription = boardLists[0].Cards[0].Description });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.ChangePriority, DateTime = new DateTime(2024, 05, 07, 12, 0, 12).ToUniversalTime(), Card = boardLists[0].Cards[0], CurrentCardName = boardLists[0].Cards[0].Name, OldPriority = Priority.High, NewPriority = boardLists[0].Cards[0].Priority });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.ChangeDueDate, DateTime = new DateTime(2024, 05, 07, 12, 0, 13).ToUniversalTime(), Card = boardLists[0].Cards[0], CurrentCardName = boardLists[0].Cards[0].Name, OldDueDate = new DateOnly(2024, 5, 2), NewDueDate = boardLists[0].Cards[0].DueDate });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.Move, DateTime = new DateTime(2024, 05, 07, 12, 0, 14).ToUniversalTime(), Card = boardLists[0].Cards[0], CurrentCardName = boardLists[0].Cards[0].Name, OldListName = "old list name", NewListName = boardLists[0].Name });
           
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.Create, DateTime = new DateTime(2024, 05, 07, 12, 0, 15).ToUniversalTime(), Card = null, CurrentCardName = "Task 11", CreatedAtListName = boardLists[1].Name });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.ChangePriority, DateTime = new DateTime(2024, 05, 07, 12, 0, 16).ToUniversalTime(), Card = null, CurrentCardName = "Task 11", OldPriority = Priority.High, NewPriority = Priority.Medium });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.ChangePriority, DateTime = new DateTime(2024, 05, 07, 12, 0, 17).ToUniversalTime(), Card = null, CurrentCardName = "Task 11", OldPriority = Priority.Medium, NewPriority = Priority.Low });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.ChangePriority, DateTime = new DateTime(2024, 05, 07, 12, 0, 18).ToUniversalTime(), Card = null, CurrentCardName = "Task 11", OldPriority = Priority.Low, NewPriority = Priority.High });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.ChangePriority, DateTime = new DateTime(2024, 05, 07, 12, 0, 19).ToUniversalTime(), Card = null, CurrentCardName = "Task 11", OldPriority = Priority.High, NewPriority = Priority.Medium });
            cardHistories.Add(new CardHistory() { HistoryEvent = HistoryEvent.Delete, DateTime = new DateTime(2024, 05, 07, 12, 0, 20).ToUniversalTime(), CurrentCardName = "Task 11", DeletedFromListName = boardLists[1].Name });

            await _boardContext.BoardLists.AddRangeAsync(boardLists);
            await _boardContext.CardHistories.AddRangeAsync(cardHistories);
            await _boardContext.SaveChangesAsync();
            return Ok();
        }
    }
}
