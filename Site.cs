using xNet;

namespace GitChecker
{
    class Site
    {
        public string Url = null;
        public bool GitExist = false;
        public bool Git403 = false;
        public Site(string url)
        {
            Url = url;
        }

        public void GitCheck()
        {
            var request = new HttpRequest
            {
                IgnoreProtocolErrors = true,
                UserAgent = Http.ChromeUserAgent(),
                ConnectTimeout = 10000
            };
            try
            {
                var responce = request.Get(Url + "/.git/config");
                var result = responce.ToString();
                GitExist = result.Contains("[core]");
                Git403 = (responce.StatusCode == HttpStatusCode.Forbidden);
                request.Close();
                request.Dispose();
            }
            catch
            {
                GitExist = false;
            }
        }
    }
}