using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace customMD{
    public class MDReader{
        //TODO read markdown files and generate HTML object
        //markdown file path setting 
        
        private string MarkdownFilePath;

        public MDReader(string MarkdownFilePath){
            this.MarkdownFilePath = MarkdownFilePath;
        }

        public MDDOM Read(){
            Parser parser = new Parser();
            MDDOM mddom = parser.Initialize();
            try 
            {
                using (StreamReader sr = new StreamReader(this.MarkdownFilePath)) 
                {
                    string current_line;
                    string next_line = sr.ReadLine();

                    while (((current_line = next_line) == next_line) && ((next_line = sr.ReadLine()) != null)){
                        Console.WriteLine("");
                        Console.WriteLine($"current: {current_line}");
                        Console.WriteLine($"next: {next_line}");
                        if (parser.Parse(current_line, next_line)){
                            current_line = next_line;
                            next_line = sr.ReadLine();
                        }
                    }
                }
            }
            catch (Exception e){
                Console.WriteLine(e);
            }
            Console.WriteLine("");
            return mddom;
        }

        
    }
}