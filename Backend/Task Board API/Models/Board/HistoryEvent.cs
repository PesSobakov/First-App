namespace Task_Board_API.Models.Board
{
    public enum HistoryEvent
    {
        Create,
        ChangeName,
        ChangeDescription,
        ChangeDueDate,
        ChangePriority,
        Move,
        Delete
    }
}
