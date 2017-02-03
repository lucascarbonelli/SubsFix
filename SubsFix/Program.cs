using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubsFix
{
    class Program
    {
        private static readonly string MSG_InvalidArgumentList = string.Format("{0}\n{1}\n{2}", "Lista de parametros invalida", 
            "SubsFix.exe [archivo.srt] [corrimiento en segundos del primer subtitulo] [corrimiento en segundos del ultimo subtitulo] [opcional: codifición]",
            "Opciones de codificación del archivo:\tansi utf-8 unicode unicodeBE (unicode big endian)");

        static void Main(string[] args)
        {
            // Parametros de entrada:
            // [archivo.srt] 
            // [corrimiento en segundos del primer subtitulo] 
            // [corrimiento en segundos del ultimo subtitulo]
            // [opcional: encoding]

            bool validArgumentList = true;
            string srtFile;
            double timeBeginSubCorrection = 0.0;
            double timeEndSubCorrection = 0.0;
            Encoding fileEncoding = Encoding.GetEncoding(0);

            if(args.Length != 3 && args.Length != 4)
            {
                Console.WriteLine(MSG_InvalidArgumentList);
                return;
            }

            srtFile = args[0];
            validArgumentList &= double.TryParse(args[1], out timeBeginSubCorrection);
            validArgumentList &= double.TryParse(args[2], out timeEndSubCorrection);
            if(args.Length == 4)
            {
                validArgumentList &= TryParseEncoding(args[3], out fileEncoding);
            }

            if (!validArgumentList)
            {
                Console.WriteLine(MSG_InvalidArgumentList);
                return;
            }

            // Pasaje del corrimiento de segundos a milisegundos
            timeBeginSubCorrection *= 1000;
            timeEndSubCorrection *= 1000;

            SubsProcessor subsProc = new SubsProcessor(srtFile, fileEncoding);
            
            subsProc.fixTimes(timeBeginSubCorrection, timeEndSubCorrection);

            StringReader reader = subsProc.getOutput();

            Console.WriteLine(reader.ReadToEnd());
        }


        private static bool TryParseEncoding(string arg, out Encoding encoding)
        {
            switch (arg)
            {
                case "ansi":
                    encoding = Encoding.GetEncoding(1252);
                    return true;
                case "utf-8":
                    encoding = Encoding.GetEncoding(65001);
                    return true;
                case "unicode":
                    encoding = Encoding.GetEncoding(1200);
                    return true;
                case "unicodeBE":
                    encoding = Encoding.GetEncoding(1201);
                    return true;
                default:
                    encoding = Encoding.GetEncoding(0);
                    return false;
            }
        }
    }
}
