using System.IO;
using System.Collections.Generic;

namespace PRPG {
    public static class CSVReader {

        public static List<List<string>> ReadFile(string filename) {
            var reader = new StreamReader(filename);

            string line = string.Empty;
            List<List<string>> result = new List<List<string>>();
            while ((line = reader.ReadLine()) != null) {
                var splitLine = line.Split(',');
                List<string> trimmedLine = new List<string>(splitLine.Length);
                foreach (var e in splitLine) {
                    trimmedLine.Add(e.Trim());
                }
                result.Add(trimmedLine);
            }
            return result;
        }
    }
}
