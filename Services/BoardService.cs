using Proyecto1.DTOs.Moves;
using Proyecto1.Models;
using Proyecto1.Services.Interfaces;

namespace Proyecto1.Services
{
    public class BoardService : IBoardService
    {
        // Profesores hardcodeados
        private readonly Dictionary<int, (string Profesor, int TailPosition, string Equation, Dictionary<string,string> Options, string CorrectOption)> Profesores
            = new()
        {
            { 23, ("Huanca", 4, "5x-3=12", new Dictionary<string,string>{{"A","x=4"},{"B","x=3"},{"C","x=2"}}, "B") },
            { 30, ("Nancy", 8, "2x + 7 = 19", new Dictionary<string,string>{{"A","x=5"},{"B","x=6"},{"C","x=7"}}, "B") },
            { 33, ("Guerra", 26, "4x - 5 = 15", new Dictionary<string,string>{{"A","x=3"},{"B","x=4"},{"C","x=5"}}, "C") },
            { 40, ("Vladimir", 22, "x/2 + 3 = 9", new Dictionary<string,string>{{"A","x=10"},{"B","x=12"},{"C","x=14"}}, "B") },
            { 64, ("Ulises", 58, "6x = 4x + 10", new Dictionary<string,string>{{"A","x=5"},{"B","x=4"},{"C","x=3"}}, "A") },
            { 56, ("Melisa", 36, "3(x - 2) = 15", new Dictionary<string,string>{{"A","x=6"},{"B","x=7"},{"C","x=8"}}, "B") },
            { 53, ("Infantas", 49, "10 - 2x = 4", new Dictionary<string,string>{{"A","x=2"},{"B","x=4"},{"C","x=3"}}, "C") },
            { 94, ("Bojanic", 74, "x + 11 = 3x - 1", new Dictionary<string,string>{{"A","x=6"},{"B","x=5"},{"C","x=4"}}, "A") },
            { 99, ("Enrique", 78, "x/4 + 2 = 6", new Dictionary<string,string>{{"A","x=12"},{"B","x=16"},{"C","x=20"}}, "B") },
            { 89, ("Sapag", 71, "8x + 1 = 41", new Dictionary<string,string>{{"A","x=4"},{"B","x=5"},{"C","x=6"}}, "B") }
        };

        public BoardService() {}

        public Board GenerateBoard(int gameId, int size = 100)
        {
            var board = new Board
            {
                GameId = gameId,
                Size = size,
                Snakes = new List<Snake>(),
                Ladders = new List<Ladder>()
            };

            // Profesores = serpientes
            foreach(var kv in Profesores)
            {
                board.Snakes.Add(new Snake
                {
                    HeadPosition = kv.Key,
                    TailPosition = kv.Value.TailPosition,
                    BoardId = board.Id
                });
            }

            // Matones = escaleras
            board.Ladders.Add(new Ladder { BottomPosition = 13, TopPosition = 28, BoardId = board.Id });
            board.Ladders.Add(new Ladder { BottomPosition = 42, TopPosition = 60, BoardId = board.Id });
            board.Ladders.Add(new Ladder { BottomPosition = 32, TopPosition = 50, BoardId = board.Id });
            board.Ladders.Add(new Ladder { BottomPosition = 65, TopPosition = 86, BoardId = board.Id });
            board.Ladders.Add(new Ladder { BottomPosition = 6,  TopPosition = 14, BoardId = board.Id });
            board.Ladders.Add(new Ladder { BottomPosition = 54, TopPosition = 69, BoardId = board.Id });
            board.Ladders.Add(new Ladder { BottomPosition = 84, TopPosition = 96, BoardId = board.Id });

            return board;
        }

        public bool ValidatePosition(int position, int boardSize) => position >= 0 && position <= boardSize;

        public int? GetSnakeDestination(Board board, int position) => board.Snakes.FirstOrDefault(s => s.HeadPosition == position)?.TailPosition;

        public int? GetLadderDestination(Board board, int position) => board.Ladders.FirstOrDefault(l => l.BottomPosition == position)?.TopPosition;

        // Método para obtener la pregunta del profesor si existe
        public ProfesorQuestionDto? GetProfesorQuestion(int position)
        {
            if (Profesores.ContainsKey(position))
            {
                var data = Profesores[position];
                return new ProfesorQuestionDto
                {
                    Profesor = data.Profesor,
                    Equation = data.Equation,
                    Options = data.Options
                };
            }
            return null;
        }

        // Método para validar la respuesta del profesor
        public bool ValidateProfesorAnswer(int position, string answer)
        {
            return Profesores.ContainsKey(position) && Profesores[position].CorrectOption == answer;
        }
    }
}


