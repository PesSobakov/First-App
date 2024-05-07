namespace Task_Board_API.Models.Board
{
    public class Card
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public Priority Priority { get; set; }

        public DateOnly DueDate { get; set; }

        public int BoardListId { get; set; }

        public BoardList BoardList { get; set; }

        public List<CardHistory> CardHistories { get; set; } = new List<CardHistory>();
    }
}
