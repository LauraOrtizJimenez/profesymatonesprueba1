namespace Proyecto1.DTOs.Moves
{
    public class ProfesorAnswerRequest
    {
        public int GameId { get; set; }
        public string Answer { get; set; } = string.Empty; // "A", "B", o "C"
    }
}