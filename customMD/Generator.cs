using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace customMD{
    public class Generator{
        public string generating_dir;
        public string src_path{
            set{
                Match match = new Regex(@"^(.*)\\(.*).md$").Match(value);
                if (match.Success){
                    this.src_filename = match.Groups[2].Value;
                    this.src_dir = match.Groups[1].Value;
                }
                else{
                    Console.WriteLine(value);
                    throw new Exception("Not supported source path.");
                }
            }
        }
        public string src_filename;
        public string src_dir;

        private MDDOM Mddom;
        private Converter converter;
        
        public void Generate(){
            string root = $"{generating_dir}/{src_filename}";
            if (!Directory.Exists(root)){
                Directory.CreateDirectory(root);
            }

            string assets = $"{root}//assets";
            if (!Directory.Exists(assets)){
                Directory.CreateDirectory(assets);
            }

            string index = $"{root}//index.html";
            if (!File.Exists(index)){
                File.Create(index).Close();
            }
            File.WriteAllText(index, this.converter.html.ToString());

        }

        public Generator(string gd, string sp){
            this.generating_dir = gd;
            this.src_path = sp;
            this.Mddom = new MDReader(sp).Read();
            this.converter = new Converter(Mddom);
            converter.InitialConvert();
            converter.Convert();
        }

        public Generator(string sp){
            this.src_path = sp;
            this.generating_dir = this.src_dir;
            this.Mddom = new MDReader(sp).Read();
            this.converter = new Converter(Mddom);
            converter.InitialConvert();
            converter.Convert();
        }

        public static void Main(string[] args){
            new Generator("C:\\Users\\madol\\Desktop\\test1.md").Generate();
        }
        
    }
}