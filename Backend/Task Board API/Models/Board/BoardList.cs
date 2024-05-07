namespace Task_Board_API.Models.Board
{
    public class BoardList
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public List<Card> Cards { get; set; } = new List<Card>();
    }
}
