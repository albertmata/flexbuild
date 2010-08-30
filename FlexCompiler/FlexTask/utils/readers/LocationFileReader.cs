using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BuildTask.Flex.utils
{
    public class LocationFileReader : IDisposable
    {
        private const string uriMatch = "URI//file:/";
        private string path;

        private StreamReader reader;

        public LocationFileReader(string pathToLocationFile)
        {
            path = pathToLocationFile;
            reader = File.OpenText(path);
        }

        public string ReadProjectLocation()
        {
            string data = reader.ReadToEnd();
            int uriIndex = data.IndexOf(uriMatch);
            if (-1 == uriIndex) throw new Exception("URI not found");

            uriIndex += uriMatch.Length;

            bool read = false;

            StringBuilder sb = new StringBuilder();
            while (!read)
            {
                if (uriIndex == data.Length) break;

                char c = data[uriIndex++];

                if (Char.IsControl(c))
                {
                    read = true;
                }
                else
                {
                    sb.Append(c);
                }
            }

           return sb.ToString();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (null != reader)
            {
                reader.Dispose();
            }
        }

        #endregion
    }
}
