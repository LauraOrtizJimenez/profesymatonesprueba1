namespace Proyecto1.DTOs.Moves
{
    public class ProfesorQuestionDto
    {
        public string Profesor { get; set; } = string.Empty;
        public string Equation { get; set; } = string.Empty;
        public Dictionary<string, string> Options { get; set; } = new(); // A, B, C
    }
}