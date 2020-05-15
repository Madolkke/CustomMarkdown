using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace customMD{
    public class StaticManager{
        public List<StaticRequest> static_requests;
        public string assets_dir;
        public static readonly Regex LOCAL_PATH = new Regex(@"^(http|https)\:\/\/.+\/(.+)\.(jpg|png|jpeg|gif|bmp)$");

        public static readonly Regex WEB_PATH = new Regex(@"");

        public StaticManager(){
            this.static_requests = new List<StaticRequest>();
        }

        /*public static void Main(string[] args){
            var t = new StaticManager();
            string raw = "https://p.ananas.chaoxing.com/star3/1920_314/be44afd300d6fd4a00d666989698d255.png";
            Match match = StaticManager.WEB_PATH.Match(raw);
            Console.WriteLine(match.Value);
            t.AddRequest("https://p.ananas.chaoxing.com/star3/1920_314/be44afd300d6fd4a00d666989698d255.png");
            t.SetAssetsDir("C:\\Users\\madol\\Desktop");
            t.DownloadFromWeb(t.static_requests.First());
        }*/
        public struct StaticRequest{
            public string hash_name;
            public string path;
            public bool type;//local->true or web->false
            public string file_type;
        }

        public void SetAssetsDir(string assets_dir){
            this.assets_dir = assets_dir;
        }
        

        public string AddRequest(string raw){
            StaticRequest sr = new StaticRequest();
            sr.path = raw;
            Match webmatch = WEB_PATH.Match(raw);
            if (webmatch.Success){
                sr.type = false;
                sr.file_type = webmatch.Groups[3].Value;
            }

            Match localmatch = LOCAL_PATH.Match(raw);
            if (localmatch.Success){
                sr.type = true;
                sr.file_type = "";
            }

            Console.WriteLine(webmatch.Value);
            string hash_name = raw.GetHashCode().ToString() + "." + sr.file_type;
            sr.hash_name = hash_name;
            static_requests.Add(sr);
            return hash_name;
        }

        public void Copy(){
            
        }

        public void DownloadFromWeb(StaticRequest sr){
            WebRequest request = WebRequest.Create(sr.path);
            WebResponse response = request.GetResponse();
            Stream reader = response.GetResponseStream();
            FileStream writer = new FileStream(this.assets_dir + $"//{sr.hash_name}", FileMode.OpenOrCreate, FileAccess.Write);
            byte[] buff = new byte[512];
            int c = 0;
            while ((c=reader.Read(buff, 0, buff.Length)) > 0)
            {
                writer.Write(buff, 0, c);
            }
            writer.Close();
            writer.Dispose();
            reader.Close();
            reader.Dispose();
            response.Close();
        }
        
    }
}