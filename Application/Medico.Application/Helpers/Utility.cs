using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace Medico.Application.Helpers
{
    public static class Utility
    {
        public static string GetContentFromTemplate(Hashtable ht, string fileName)
        {
            string strContents = "";

            FileInfo file__1 = new System.IO.FileInfo(fileName);
            if (file__1.Exists)
            {
                try
                {
                    // Get HTML data
                    StreamReader sr = new StreamReader(fileName, System.Text.Encoding.Default);
                    File.OpenText(fileName);
                    //strContents += Server.HtmlEncode(sr.ReadToEnd());
                    strContents += sr.ReadToEnd();
                    sr.Close();
                }
                catch (System.IO.IOException ex)
                {
                    throw ex;
                }
            }

            ICollection ICol = ht.Keys;
            Array arr = new string[ht.Keys.Count];
            ICol.CopyTo(arr, 0);

            for (int i = 0; i <= arr.GetUpperBound(0); i++)
            {
                if (strContents.Length > 0)
                {
                    strContents = strContents.Replace(arr.GetValue(i).ToString(), ht[arr.GetValue(i).ToString()].ToString());
                }
            }
            int index = strContents.IndexOf("<html");
            if (index > 0)
            {
                try
                {
                    strContents = strContents.Substring(index);
                    strContents.Replace("''", "'");
                }
                catch
                {
                }
            }
            return strContents;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string chars_2 = "0123456789";
            var p1 = new string(Enumerable.Repeat(chars, length / 2)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            var p2 = new string(Enumerable.Repeat(chars_2, length / 2)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return string.Format("{0}{1}", p1, p2);
            //return new string(Enumerable.Repeat(chars, length)
            //  .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string AutoGenerateSerialNo(string prefix, int maxId)
        {

            if (Convert.ToString(maxId).Length == 1)
            {
                return string.Format("{0}0000{1}", prefix, maxId);
            }
            if (Convert.ToString(maxId).Length == 2)
            {
                return string.Format("{0}000{1}", prefix, maxId);
            }
            if (Convert.ToString(maxId).Length == 3)
            {
                return string.Format("{0}00{1}", prefix, maxId);
            }
            if (Convert.ToString(maxId).Length == 4)
            {
                return string.Format("{0}0{1}", prefix, maxId);
            }

            return string.Empty;
        }
    }
}
