using System.ComponentModel.DataAnnotations;
using Task_Board_API.Models.Board;

namespace Task_Board_API.Models.DTOs
{
    public class CardEditDto
    {
        public string? Name { get; set; } = "";

        public string? Description { get; set; } = "";

        [Range(0, 2)]
        public Priority? Priority { get; set; }

        public DateOnly? DueDate { get; set; }

        public int? BoardListId { get; set; }
    }
}
