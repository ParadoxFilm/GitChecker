using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using xNet;
namespace GitChecker
{
    /*
    Sorry for bullshit. Once I rewrite this code normally.
    Now these are just crutches that violate all the concepts of normal programming.
    */
    class Site
    {
        public string Url = null;
        public bool GitExist = false;
        public bool Git403 = false;
        public Site(string _Url)
        {
            Url = _Url;
        }
        public void GitCheck()
        {
            HttpRequest Request = new HttpRequest();
            Request.IgnoreProtocolErrors = true;
            Request.UserAgent = Http.ChromeUserAgent();
            Request.ConnectTimeout = 10000;
            try
            {
                HttpResponse Responce = Request.Get(Url + "/.git/config");
                string Result = Responce.ToString();
                GitExist = Result.Contains("[core]");
                Git403 = (Responce.StatusCode == HttpStatusCode.Forbidden);
                Request.Close();
                Request.Dispose();
            }
            catch
            {
                GitExist = false;
            } 
        }
    }
    class Checker
    {
        public List<Site> Sites = new List<Site>();
        int CheckCount = 0;
        int CheckGood = 0;
        private object Locker = new object();
        public void CheckStart()
        {
            CheckCount = 0;
            CheckGood = 0;
            Parallel.For(0, Sites.Count, new ParallelOptions { MaxDegreeOfParallelism = 500 },
                     i =>
                     {
                         int x = i;
                         Sites[i].GitCheck();
                         if (Sites[i].Git403)
                         {
                             lock (Locker)
                             {
                                 ++CheckCount;

                                 Console.ForegroundColor = ConsoleColor.Yellow;
                                 Console.WriteLine(Sites[i].Url + " -> Git Forbidden. [{0}/{1}/{2}]",CheckGood,CheckCount,Sites.Count);
                                 Console.ForegroundColor = ConsoleColor.Gray;
                             }
                         }
                         else
                         {
                             if (Sites[i].GitExist)
                             {
                                 lock (Locker)
                                 {
                                     ++CheckCount;
                                     ++CheckGood;
                                 Console.ForegroundColor = ConsoleColor.Green;
                                 Console.WriteLine(Sites[i].Url + " -> Git exist! [{0}/{1}/{2}]",CheckGood,CheckCount,Sites.Count);
                                 Console.ForegroundColor = ConsoleColor.Gray;
                                 }
                             }
                             else
                             {
                                 lock (Locker)
                                 {
                                     ++CheckCount;
                                     Console.WriteLine(Sites[i].Url + " -> Git don't exist :( [{0}/{1}/{2}]",CheckGood,CheckCount,Sites.Count);
                                 }
                             }
                         }
                     });
        }
        public void SiteAdd(string Url)
        {
            Sites.Add(new Site(Url));
        }
        public void AddFromDump()
        {
            string dump = File.ReadAllLines("in.txt");
            for(int i=0; i<dump.Length;++i)
                SiteAdd(dump[i]);
        }
        public void SaveResult()
        {
            string _out = string.Empty;
            for (int i = 0; i < Sites.Count;++i ) 
                if (Sites[i].GitExist)
                    _out += Sites[i].Url+"\r\n";
            File.WriteAllText("out.txt", _out);
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press Enter to start...");
            Console.ReadKey();
            Checker Now = new Checker();
            Now.AddFromDump();
            Console.WriteLine("Sites count: "+Now.Sites.Count);
            Now.CheckStart();
            Now.SaveResult();
            Console.WriteLine("Done.");
            Console.ReadKey();
            
        }
    }
}
