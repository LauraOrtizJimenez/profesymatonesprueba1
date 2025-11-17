namespace Proyecto1.DTOs.Moves
{
    public class MoveResultDto
    {
        public int DiceValue { get; set; }
        public int FromPosition { get; set; }
        public int ToPosition { get; set; }
        public int? FinalPosition { get; set; }
        public string? SpecialEvent { get; set; }
        public bool IsWinner { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool RequiresProfesorAnswer { get; set; } = false;
        public ProfesorQuestionDto? ProfesorQuestion { get; set; }
    }
}