using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SubsFix
{
    class SubsProcessor
    {
        private List<SubsBlock> SubsBlockList = new List<SubsBlock>();

        private class SubsBlock
        {
            public int id;
            public int timeBeginSub;
            public int timeEndSub;
            public string text;

            public SubsBlock(int Id, int TimeBeginSub, int TimeEndSub, string Text)
            {
                id = Id;
                timeBeginSub = TimeBeginSub;
                timeEndSub = TimeEndSub;
                text = Text;
            }

            public string time()
            {
                int tBh, tBm, tBs, tBms, tEh, tEm, tEs, tEms;

                tBh = timeBeginSub / (60 * 60 * 1000);
                tBm = timeBeginSub / (60 * 1000) - tBh * 60;
                tBs = timeBeginSub / 1000 - tBh * 60 * 60 - tBm * 60;
                tBms = timeBeginSub - tBh * 60 * 60 * 1000 - tBm * 60 * 1000 - tBs * 1000;

                tEh = timeEndSub / (60 * 60 * 1000);
                tEm = timeEndSub / (60 * 1000) - tEh * 60;
                tEs = timeEndSub / 1000 - tEh * 60 * 60 - tEm * 60;
                tEms = timeEndSub - tEh * 60 * 60 * 1000 - tEm * 60 * 1000 - tEs * 1000;

                string time = String.Format("{0,2}:{1,2}:{2,2},{3,3} --> {4}:{5}:{6},{7}",
                    tBh.ToString("D2"), tBm.ToString("D2"), tBs.ToString("D2"), tBms.ToString("D3"),
                    tEh.ToString("D2"), tEm.ToString("D2"), tEs.ToString("D2"), tEms.ToString("D3"));

                return time;
            }
        }


        public SubsProcessor(string fileName, Encoding fileEncoding)
        {
            FileStream fsSource = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            using (StreamReader sr = new StreamReader(fsSource, fileEncoding))
            {
                while (!sr.EndOfStream)
                {
                    int id = 0;
                    int timeBeginSub = 0;
                    int timeEndSub = 0;
                    string textLine = "";
                    string text = "";

                    while (!sr.EndOfStream && !int.TryParse(sr.ReadLine(), out id)) ;

                    while (!sr.EndOfStream && !TryParseTime(sr.ReadLine(), out timeBeginSub, out timeEndSub)) ;

                    while (!sr.EndOfStream && TryParseText(sr.ReadLine(), out textLine))
                    {
                        text += textLine;
                        text += "\n";
                    }
                    SubsBlockList.Add(new SubsBlock(id, timeBeginSub, timeEndSub, text));
                }
            }
        }

        private bool TryParseText(string textToParse, out string text)
        {
            text = textToParse;
            if (text.Trim().Length == 0)
            {
                text = "";
                return false;
            }
            text = textToParse;
            return true;
        }

        private bool TryParseTime(string timeString, out int timeBeginSub, out int timeEndSub)
        {
            string[] separatingChars = { " ", ":", ",", "-->" };
            string[] times = timeString.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
            int tBh, tBm, tBs, tBms, tEh, tEm, tEs, tEms;
            if (times.Count() == 8 &&
                int.TryParse(times[0], out tBh) && int.TryParse(times[1], out tBm) &&
                int.TryParse(times[2], out tBs) && int.TryParse(times[3], out tBms) &&
                int.TryParse(times[4], out tEh) && int.TryParse(times[5], out tEm) &&
                int.TryParse(times[6], out tEs) && int.TryParse(times[7], out tEms))
            {
                tBs *= 1000;
                tBm *= 60 * 1000;
                tBh *= 60 * 60 * 1000;
                tEs *= 1000;
                tEm *= 60 * 1000;
                tEh *= 60 * 60 * 1000;
                timeBeginSub = tBh + tBm + tBs + tBms;
                timeEndSub = tEh + tEm + tEs + tEms;
                return true;
            }
            timeBeginSub = 0;
            timeEndSub = 0;
            return false;
        }
        
        public void fixTimes(double timeFirstSubCorrection, double timeLastSubCorrection)
        {
            double timeFirstSubOriginal = SubsBlockList.First().timeBeginSub;
            double timeLastSubOriginal = SubsBlockList.Last().timeBeginSub;
            double timeFirstSubFixed = timeFirstSubOriginal + timeFirstSubCorrection;
            double timeLastSubFixed = timeLastSubOriginal + timeLastSubCorrection;
            double m = (timeLastSubFixed - timeFirstSubFixed) / (timeLastSubOriginal - timeFirstSubOriginal);
            double b = timeFirstSubFixed - m * timeFirstSubOriginal;

            foreach (SubsBlock sblock in SubsBlockList)
            {
                sblock.timeBeginSub = (int)(m * (double)sblock.timeBeginSub + b);
                sblock.timeEndSub = (int)(m * sblock.timeEndSub + b);
            }
        }

        public StringReader getOutput()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            foreach (SubsBlock sblock in SubsBlockList)
            {
                writer.WriteLine(sblock.id);
                writer.WriteLine(sblock.time());
                writer.WriteLine(sblock.text);
            }

            writer.Flush();
            writer.Close();

            StringReader reader = new StringReader(sb.ToString());

            return reader;
        }
    }
}
