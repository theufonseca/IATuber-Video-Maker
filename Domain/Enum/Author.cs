using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public static class Author
    {
        private const string DA_VINCI = "Leonardo da Vinci";
        private const string DALI = "Salvador Dalí";
        private const string MICHELANGELO = "Michelangelo";
        private const string MONET = "Claude Monet";
        private const string EDVARD = " Edvard Munch";
        private const string PICASSO = "Pablo Picasso";
        private const string RENOIR = "Renoir";
        private const string REMBRANDT = "Rembrandt";
        private const string VAN_GOGH = "Van Gogh";
        private const string PORTINARI = "Candido Portinari";
        private const string TARSILA_DO_AMARAL = "Tarsila do Amaral";

        private static List<string> Authors = new()
        {
            DA_VINCI, 
            DALI, 
            MICHELANGELO, 
            MONET, 
            EDVARD,
            PICASSO,
            RENOIR,
            REMBRANDT,
            VAN_GOGH,
            PORTINARI,
            TARSILA_DO_AMARAL
        };

        public static string GetRandomAuthor()
            => Authors[new Random().Next(0, Authors.Count - 1)];
        
    }
}
