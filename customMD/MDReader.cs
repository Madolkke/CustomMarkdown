using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace customMD{
    public class MDReader{
        //TODO read markdown files and generate HTML object
        //markdown file path setting 
        
        private string MarkdownFilePath;
        public string currentLine;
        public MDC_Paragraph currentParagraph;

        public MDDOM Read(){
            
            MDDOM mddom = new MDDOM();
            try 
            {
                using (StreamReader sr = new StreamReader(this.MarkdownFilePath)) 
                {
                    string current_line;
                    string next_line = sr.ReadLine();
                    Stack<MDComponent> ComponentStack = new Stack<MDComponent>();
                    
                    while (((current_line = next_line) == next_line) && ((next_line = sr.ReadLine()) != null)){
                        Console.WriteLine("");
                        Console.WriteLine($"current: {current_line}");
                        Console.WriteLine($"next: {next_line}");
                        // Code Block is processed here. BEGIN
                        
                        if (ComponentStack.Count > 0 && ComponentStack.Peek() is MDC_CodeBlock){//if stack is null?
                            if (!Parser.isEndOfCodeBlock(current_line)){
                                ((MDC_CodeBlock)ComponentStack.Peek()).AddLine(current_line);
                                continue;
                            }else{
                                ComponentStack.Pop();
                            }
                        }
                        else{
                            Match CBMatchResult = Parser.CODEBLOCK_START.Match(current_line);
                            Console.WriteLine(CBMatchResult.Success);
                            if (CBMatchResult.Success){
                                ComponentStack.Clear();
                                ComponentStack.Push((MDComponent) mddom.AddBaseComponent(new MDC_CodeBlock(CBMatchResult.Groups[0].Value)));
                                continue;
                            }
                        }
                        // Code Block is processed here. END
                        if (currentLine == String.Empty){
                            ComponentStack.Clear();
                            continue;
                        }
                        Match ListMatchResult = Parser.UNORDERED_LIST.Match(current_line);
                        if (ListMatchResult.Success){
                            int listLevel = ListMatchResult.Groups[1].Value.Length;
                            while (ComponentStack.Count == 0 || !(ComponentStack.Peek() is MDC_UnorderedList) && ComponentStack.Count <= listLevel){
                                ComponentStack.Pop();
                            }
                            if (ComponentStack.Count == 0){
                                MDC_UnorderedList newUnorderedList = new MDC_UnorderedList(1);
                                mddom.AddBaseComponent(newUnorderedList);
                                ComponentStack.Push(newUnorderedList);
                            }
                            
                            while (ComponentStack.Count < listLevel){
                                MDC_UnorderedList newUnorderedList = ((MDC_UnorderedList)ComponentStack.Peek()).AddSubUnorderedList();
                                ComponentStack.Push(newUnorderedList);
                            }
                            ((MDC_UnorderedList) ComponentStack.Peek())
                                .AddListEle(new MDC_ListEle(0, Parser.Parse(ListMatchResult.Groups[3].Value)));
                            continue;
                        }

                        
                        Match OListMatchResult = Parser.ORDERED_LIST.Match(current_line);
                        
                        if (OListMatchResult.Success){
                            int listLevel = ListMatchResult.Groups[1].Value.Length;
                            while (ComponentStack.Count != 0 && !(ComponentStack.Peek() is MDC_OrderedList) && ComponentStack.Count <= listLevel){
                                ComponentStack.Pop();
                            }
                            if (ComponentStack.Count == 0){
                                MDC_OrderedList newOrderedList = new MDC_OrderedList(1);
                                mddom.AddBaseComponent(newOrderedList);
                                ComponentStack.Push(newOrderedList);
                            }
                            
                            while (ComponentStack.Count < listLevel){
                                MDC_OrderedList newOrderedList =
                                    ((MDC_OrderedList) ComponentStack.Peek()).AddSubOrderedList();
                                ComponentStack.Push(newOrderedList);
                            }
                            
                            ((MDC_OrderedList) ComponentStack.Peek()).AddListEle(new MDC_ListEle(Convert.ToInt32(OListMatchResult.Groups[2].Value), Parser.Parse(OListMatchResult.Groups[4].Value)));
                            continue;
                        }
                        
                        Match BlockMatchResult = Parser.BLOCK.Match(current_line);
                        if (BlockMatchResult.Success){
                            int blockLevel = BlockMatchResult.Groups[1].Value.Length / 2;
                            while (!(ComponentStack.Peek() is MDC_Block) && ComponentStack.Count <= blockLevel){
                                ComponentStack.Pop();
                            }
                            if (ComponentStack.Count == 0){
                                MDC_Block newUnorderedList = new MDC_Block(1);
                                mddom.AddBaseComponent(newUnorderedList);
                                ComponentStack.Push(newUnorderedList);
                            }
                            
                            while (ComponentStack.Count < blockLevel){
                                MDC_Block newUnorderedList = ((MDC_Block)ComponentStack.Peek()).AddSubBlock();
                                ComponentStack.Push(newUnorderedList);
                            }
                            ((MDC_Block) ComponentStack.Peek())
                                .AddBlockEle(new MDC_BlockEle(Parser.Parse(ListMatchResult.Groups[2].Value)));
                            continue;
                        }

                        Match TitleMatchResult = Parser.TITLE.Match(current_line);
                        if (TitleMatchResult.Success){
                            ComponentStack.Clear();
                            mddom.AddBaseComponent(new MDC_Title(TitleMatchResult.Groups[1].Value.Length, Parser.Parse(TitleMatchResult.Groups[2].Value)));
                            continue;
                        }

                        Match SpliterMatchResult = Parser.SPLITTER.Match(current_line);
                        if (SpliterMatchResult.Success){
                            ComponentStack.Clear();
                            mddom.AddBaseComponent(new MDC_Splitter());
                            continue;
                        }

                        Match TLL1MatchResult = Parser.TITLE_LUS_LEVEL1.Match(next_line);
                        if (TLL1MatchResult.Success){
                            ComponentStack.Clear();
                            mddom.AddBaseComponent(new MDC_Title(1, Parser.Parse(current_line)));
                            current_line = next_line;
                            next_line = sr.ReadLine();
                            continue;
                        }

                        Match TLL2MatchResult = Parser.TITLE_LUS_LEVEL2.Match(next_line);
                        if (TLL2MatchResult.Success){
                            ComponentStack.Clear();
                            mddom.AddBaseComponent(new MDC_Title(2, Parser.Parse(current_line)));
                            current_line = next_line;
                            next_line = sr.ReadLine();
                            continue;
                        }
                        
                        if (ComponentStack.Count > 0 && ComponentStack.Peek() is MDC_Paragraph){
                            ((MDC_Paragraph) ComponentStack.Peek()).AddLine(Parser.Parse(current_line));
                        }else{
                            ComponentStack.Clear();
                            MDC_Paragraph p = new MDC_Paragraph();
                            mddom.AddBaseComponent(p);
                            ComponentStack.Push(p);
                            ((MDC_Paragraph) ComponentStack.Peek()).AddLine(Parser.Parse(current_line));
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

        public MDReader(string MarkdownFilePath){
            this.MarkdownFilePath = MarkdownFilePath;
        }
        
        
        /*public static void Main(string[] args){
            string test = "[telegram](https://t.me/joinchat/MHU8Gg2fP3Q51HLY2wqmQA)";
            Console.WriteLine(Parser.LINK.Match(test).Success);

        }*/
        
    }


    public class Parser{

        public static readonly Regex CODEBLOCK_START = new Regex(@"(?<=```)([a-z]|[A-Z])*");

        public static readonly Regex CODEBLOCK_END = new Regex(@"^```");

        public static readonly Regex BLOCK = new Regex(@"^((?:>\s)+)(.+)");

        public static readonly Regex UNORDERED_LIST = new Regex(@"^(\t*)(\*|\-)\s(.+)");

        public static readonly Regex ORDERED_LIST = new Regex(@"^(\t*)(\d+)(\.|\))\s(.+)");

        public static readonly Regex SPLITTER = new Regex(@"^((\-+)|(\*+))$");
        
        public static readonly Regex TITLE = new Regex(@"(#+)\s(.+)");

        public static readonly Regex TITLE_LUS_LEVEL1 = new Regex(@"^\=+$");

        public static readonly Regex TITLE_LUS_LEVEL2 = new Regex(@"^\-+$");

        public static readonly Regex INLINE_CODE = new Regex(@"`(.+?)`");

        public static char[] ESCAPE_CHAR_COLLECTION = new[]{'_','*' };

        public static readonly Regex LINK = new Regex(@"([^!]|^)\[(.*?)\]\((.*?)\)");

        public static readonly Regex IMAGE = new Regex(@"!\[(.*?)\]\((.*?)\)");
        
        public static bool isEndOfCodeBlock(string line){
            return CODEBLOCK_END.Match(line).Success;
        }

        public static string isStartOfCodeBlock(string line){
            //检查语言类型合法性
            Match match = CODEBLOCK_START.Match(line);
            //未紧跟在```后的语言类型或不合规定的语言类型会被视为空类型
            return match.Success ? match.Value : null;
            
        }

        public static bool isEscapeChar(char c){
            foreach (var ec in ESCAPE_CHAR_COLLECTION){
                if (ec == c){
                    return true;
                }
            }
            return false;
        }

        public static List<MDC_MidComponent> Parse(string raw){
            List<MDC_MidComponent> result = new List<MDC_MidComponent>();
            foreach (var midComponent in Parser.ParseInlineCode(raw)){
                if (midComponent is MDC_TempMidComponent){
                    var mid = ParseStyle(((MDC_TempMidComponent) midComponent).raw);
                    foreach (var styleComponent in mid){
                        ParseToSingleComponent(styleComponent);
                        result.Add(styleComponent);
                    }
                }
                else{
                    result.Add(midComponent);
                }
            }

            foreach (var res in result){
                if (res is MDC_InlineCode){
                    Console.WriteLine($"InlineCode: {((MDC_InlineCode)res).code}");
                }
                else if(res is MDC_StyleComponent){
                    MDC_StyleComponent mid = (MDC_StyleComponent) res;
                    Console.WriteLine($"Style: bold->{mid.bold}, italic->{mid.italic}");
                    foreach (var single in mid.contents){
                        if (single is MDC_Hyperlink){
                            MDC_Hyperlink s = (MDC_Hyperlink) single;
                            Console.WriteLine($"Link: name->{s.name}, content->{s.content}");
                        }else if (single is MDC_Image){
                            MDC_Image s = (MDC_Image) single;
                            Console.WriteLine($"Image: name->{s.name}, content->{s.content}");
                        }else if (single is MDC_PlainText){
                            MDC_PlainText s = (MDC_PlainText) single;
                            Console.WriteLine($"PlainText: content->{s.content}");
                        }
                    }
                    Console.WriteLine("Style End");
                }
            }
            return result;
        }

        public static List<MDC_StyleComponent> ParseStyle(string raw){
            bool escape = false;
            bool stackinEnd = false;
            bool styling = false;
            StringBuilder sb = new StringBuilder();
            Stack<char> CharStack = new Stack<char>();
            List<MDC_StyleComponent> result = new List<MDC_StyleComponent>();
            int typeCounter = 0;
            foreach (var c in raw){
                if (c == '\\'){
                    escape = true;
                    continue;
                }else{
                    if (escape){
                        sb.Append(c);
                        escape = false;
                    }else{
                        if (isEscapeChar(c)){
                            if (!styling){
                                result.Add(ParseToStyleComponent(typeCounter, sb.ToString()));
                                sb.Clear();
                            }
                            styling = true;
                            if (stackinEnd){
                                if (CharStack.Peek() == c){
                                    CharStack.Pop();
                                    if (CharStack.Count == 0){
                                        stackinEnd = false;
                                        styling = false;
                                        result.Add(ParseToStyleComponent(typeCounter, sb.ToString()));
                                        typeCounter = 0;
                                        sb.Clear();
                                    }
                                }else{
                                    stackinEnd = false;
                                    styling = false;
                                    CharStack.Clear();
                                    typeCounter = 0;
                                    result.Add(ParseToStyleComponent(typeCounter, sb.ToString()));
                                    sb.Clear();
                                }
                            }else{
                                CharStack.Push(c);
                                typeCounter++;
                            }
                        }else{
                            if (styling){
                                if (stackinEnd){
                                    if (CharStack.Count < typeCounter){
                                        styling = false;
                                        stackinEnd = false;
                                        CharStack.Clear();
                                        result.Add(ParseToStyleComponent(typeCounter, sb.ToString()));
                                        sb.Clear();
                                    }
                                    
                                }else{
                                    stackinEnd = true;
                                }
                            }
                            sb.Append(c);
                        }
                    }
                }
            }
            result.Add(ParseToStyleComponent(typeCounter, sb.ToString()));
            return result;
        }

        public static List<MDC_MidComponent> ParseInlineCode(string raw){
            List<(int, int)> mid = new List<(int, int)>();
            foreach (Match match in Parser.INLINE_CODE.Matches(raw)){
                mid.Add((match.Index, match.Value.Length));
            }
            List<MDC_MidComponent> result = new List<MDC_MidComponent>();
            int pointer = 0;
            foreach (var location in mid){
                if (location.Item1 > pointer){
                    result.Add(new MDC_TempMidComponent(raw.Substring(pointer, location.Item1 - pointer)));
                }
                result.Add(new MDC_InlineCode(raw.Substring(location.Item1 + 1, location.Item2 - 2)));
                pointer = location.Item1 + location.Item2;
            }
            if (raw.Length > pointer){
                result.Add(new MDC_TempMidComponent(raw.Substring(pointer, raw.Length - pointer)));
            }
            return result;
        }

        public static MDC_StyleComponent ParseToStyleComponent(int style, string content){
            MDC_StyleComponent result;
            if (style == 1){
                result = new MDC_StyleComponent(false, true);
            }else if (style == 2){
                result = new MDC_StyleComponent(true, false);
            }else if (style == 0){
                result = new MDC_StyleComponent(false, false);
            }else{
                result = new MDC_StyleComponent(true, true);
            }
            return result.AddContent(new MDC_TempSingleComponent(content));
        }

        public static void ParseToSingleComponent(MDC_StyleComponent sc){
            string raw = ((MDC_TempSingleComponent) sc.contents.Last()).raw;
            sc.contents = LinkImageSplit(raw);
        }

        public static List<MDC_SingleComponent> LinkImageSplit(string raw){
            //Link -> true, Image -> false
            List<(int, int, string, string, bool)> location = new List<(int, int, string, string, bool)>();
            foreach (Match LinkMC in Parser.LINK.Matches(raw)){
                location.Add((LinkMC.Index, LinkMC.Value.Length, LinkMC.Groups[2].Value, LinkMC.Groups[3].Value, true));
            }

            foreach (Match ImageMC in Parser.IMAGE.Matches(raw)){
                location.Add((ImageMC.Index, ImageMC.Value.Length, ImageMC.Groups[1].Value, ImageMC.Groups[2].Value, false));
            }
            location.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            List<MDC_SingleComponent> result = new List<MDC_SingleComponent>();
            int pointer = 0;
            foreach (var loc in location){
                if (loc.Item1 > pointer){
                    result.Add(new MDC_PlainText(raw.Substring(pointer, loc.Item1 - pointer)));
                }
                pointer = loc.Item1;
                if (loc.Item5){
                    result.Add(new MDC_Hyperlink(loc.Item3, loc.Item4));
                }
                else{
                    result.Add(new MDC_Image(loc.Item3, loc.Item4));
                }

                pointer = loc.Item1 + loc.Item2;
            }

            if (pointer < raw.Length){
                result.Add(new MDC_PlainText(raw.Substring(pointer, raw.Length - pointer)));
            }
            return result;
        }


    }
}