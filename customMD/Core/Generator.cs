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
        public string config_path;

        private MDDOM Mddom;
        private MDDOM config_Mddom;
        private Converter converter;
        private ConfigConverter config_converter;
        
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

        public Generator(string markdown_source_path, CSS css, string generate_dir = null, string config_path = null){
            this.generating_dir = generate_dir;
            this.src_path = markdown_source_path;
            this.Mddom = new MDReader(markdown_source_path).Read();
            this.converter = new Converter(Mddom);
            this.converter.InitialConvert();
            this.converter.Convert();
            this.converter.html.AddCSS(css);
            
            this.config_path = config_path;
            if (config_path != null){
                this.config_Mddom = new MDReader(config_path).Read();
                this.config_converter = new ConfigConverter(config_Mddom);
                config_converter.Convert();
                this.converter.html.UseConfig(config_converter);
            }

            if (generate_dir != null){
                this.generating_dir = generate_dir;
            }
            else{
                this.generating_dir = this.src_dir;
            }
            

        }

        public Generator(string markdown_source_path, CSS css, string generate_dir){
            this.generating_dir = generate_dir;
            this.src_path = markdown_source_path;
            this.Mddom = new MDReader(markdown_source_path).Read();
            this.converter = new Converter(Mddom);
            this.converter.InitialConvert();
            this.converter.Convert();
            this.converter.html.AddCSS(css);
        }

        public Generator(string sp, CSS css){
            this.src_path = sp;
            this.generating_dir = this.src_dir;
            this.Mddom = new MDReader(sp).Read();
            this.converter = new Converter(Mddom);
            this.converter.InitialConvert();
            this.converter.Convert();
            this.converter.html.AddCSS(css);
        }

        /*public static void Main(string[] args){
            //new Generator("C:\\Users\\madol\\Desktop\\test1.md").Generate();
        }*/
        
    }
}