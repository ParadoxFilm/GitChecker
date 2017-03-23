using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GitChecker
{
    class Checker
    {
        private const string InputFileName = "in.txt";
        public List<Site> Sites = new List<Site>();
        int _checkCount = 0;
        int _checkGood = 0;
        private object _lock = new object();

        public void CheckStart()
        {
            _checkCount = 0;
            _checkGood = 0;
            Parallel.For(0, Sites.Count, new ParallelOptions { MaxDegreeOfParallelism = 500 },
                i =>
                {
                    Sites[i].GitCheck();
                    if (Sites[i].Git403)
                    {
                        lock (_lock)
                        {
                            _checkCount++;

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(Sites[i].Url + " -> Git Forbidden. [{0}/{1}/{2}]", _checkGood, _checkCount, Sites.Count);
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                    }
                    else
                    {
                        if (Sites[i].GitExist)
                        {
                            lock (_lock)
                            {
                                _checkCount++;
                                _checkGood++;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(Sites[i].Url + " -> Git exist! [{0}/{1}/{2}]", _checkGood, _checkCount, Sites.Count);
                                Console.ForegroundColor = ConsoleColor.Gray;
                            }
                        }
                        else
                        {
                            lock (_lock)
                            {
                                ++_checkCount;
                                Console.WriteLine(Sites[i].Url + " -> Git doesn't exist :( [{0}/{1}/{2}]", _checkGood, _checkCount, Sites.Count);
                            }
                        }
                    }
                });
        }

        public void SiteAdd(string url)
        {
            Sites.Add(new Site(url));
        }

        public void AddFromDump()
        {
            if (!File.Exists(InputFileName))
            {
                throw new InvalidOperationException($"File {InputFileName} not exists!");
            }

            var dump = File.ReadAllLines(InputFileName);
            foreach (var url in dump)
            {
                SiteAdd(url);
            }
        }

        public void SaveResult()
        {
            var _out = string.Empty;
            foreach (Site site in Sites)
            {
                if (site.GitExist)
                {
                    _out += site.Url + Environment.NewLine;
                }
            }

            File.WriteAllText("out.txt", _out);
        }
    }
}
