namespace Task_Board_API.Models.Board
{
    public class CardHistory
    {
        public int Id { get; set; }

        public int? CardId { get; set; }
      
        public Card? Card { get; set; }

        public string CurrentCardName { get; set; }

        public HistoryEvent HistoryEvent { get; set; }

        public DateTime DateTime { get; set; }

        //Create

        public string? CreatedAtListName { get; set; }

        //ChangeName

        public string? OldCardName { get; set; }

        public string? NewCardName { get; set; }

        //ChangeDescription

        public string? OldDescription { get; set; }

        public string? NewDescription { get; set; }

        //ChangeDueDate

        public DateOnly? OldDueDate { get; set; }

        public DateOnly? NewDueDate { get; set; }

        //ChangePriority

        public Priority? OldPriority { get; set; }

        public Priority? NewPriority { get; set; }

        //Move

        public string? OldListName { get; set; }

        public string? NewListName { get; set; }

        //Delete

        public string? DeletedFromListName { get; set; }

    }
}
