using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace customMD{
    public class Parser{
        
        public List<SinglePattern> SinglePatterns;
        public MDDOM mddom;
        Stack<MDComponent> ComponentStack = new Stack<MDComponent>();

        public Parser(MDDOM mddom){
            this.SinglePatterns = new List<SinglePattern>();
            this.mddom = mddom;
        }

        public Parser(){
            this.SinglePatterns = new List<SinglePattern>();
        }

        public MDDOM Initialize(){
            this.mddom = new MDDOM();
            SinglePattern.g_identifier = 0;
            this.SinglePatterns.Add(new SinglePattern(Pattern.LINK, new []{2,3}));
            this.SinglePatterns.Add(new SinglePattern(Pattern.IMAGE, new []{1,2}));
            return this.mddom;
        }

        /*public static void Main(string[] args){
            Parser parser = new Parser();
            parser.Initialize();
            parser.ParseLine("dddddd__kkk![link](www.baidu.com)__pppppp");
        }*/

        public bool Parse(string current_line, string next_line){
            if (CodeBlockParse((current_line))){
                return false;
            }
            if (EmptyCheck(current_line)){
                return false;
            }
            if (UnorderedListParse(current_line)){
                return false;
            }
            if (OrderedListParse(current_line)){
                return false;
            }
            if (BlockParse(current_line)){
                return false;
            }
            if (TitleParse(current_line)){
                return false;
            }
            if (SplitterParse(current_line)){
                return false;
            }
            if (TLL1Parse(current_line, next_line)){
                return true;
            }
            if (TLL2Parse(current_line, next_line)){
                return true;
            }
            if (ParagraphParse(current_line)){
                return false;
            }
            return false;
        }
        public List<MDC_MidComponent> ParseLine(string raw){
            List<MDC_MidComponent> result = new List<MDC_MidComponent>();
            foreach (var midComponent in Parser.ParseInlineCode(raw)){
                if (midComponent is MDC_TempMidComponent){
                    var mid = ParseStyle(((MDC_TempMidComponent) midComponent).raw);
                    foreach (var styleComponent in mid){
                        this.ParseToSingleComponent(styleComponent);
                        result.Add(styleComponent);
                    }
                }
                else{
                    result.Add(midComponent);
                }
            }

            foreach (var res in result){
                if (res is MDC_InlineCode){
                    Console.WriteLine($"InlineCode: {((MDC_InlineCode) res).code}");
                }
                else if (res is MDC_StyleComponent){
                    MDC_StyleComponent mid = (MDC_StyleComponent) res;
                    Console.WriteLine($"Style: bold->{mid.bold}, italic->{mid.italic}");
                    foreach (var single in mid.contents){
                        if (single is MDC_Hyperlink){
                            MDC_Hyperlink s = (MDC_Hyperlink) single;
                            Console.WriteLine($"Link: name->{s.name}, content->{s.content}");
                        }
                        else if (single is MDC_Image){
                            MDC_Image s = (MDC_Image) single;
                            Console.WriteLine($"Image: name->{s.name}, content->{s.content}");
                        }
                        else if (single is MDC_PlainText){
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
                }
                else{
                    if (escape){
                        sb.Append(c);
                        escape = false;
                    }
                    else{
                        if (Pattern.isEscapeChar(c)){
                            if (!styling){
                                result.Add(PackToStyleComponent(typeCounter, sb.ToString()));
                                sb.Clear();
                            }

                            styling = true;
                            if (stackinEnd){
                                if (CharStack.Peek() == c){
                                    CharStack.Pop();
                                    if (CharStack.Count == 0){
                                        stackinEnd = false;
                                        styling = false;
                                        result.Add(PackToStyleComponent(typeCounter, sb.ToString()));
                                        typeCounter = 0;
                                        sb.Clear();
                                    }
                                }
                                else{
                                    stackinEnd = false;
                                    styling = false;
                                    CharStack.Clear();
                                    typeCounter = 0;
                                    result.Add(PackToStyleComponent(typeCounter, sb.ToString()));
                                    sb.Clear();
                                }
                            }
                            else{
                                CharStack.Push(c);
                                typeCounter++;
                            }
                        }
                        else{
                            if (styling){
                                if (stackinEnd){
                                    if (CharStack.Count < typeCounter){
                                        styling = false;
                                        stackinEnd = false;
                                        CharStack.Clear();
                                        result.Add(PackToStyleComponent(typeCounter, sb.ToString()));
                                        sb.Clear();
                                    }

                                }
                                else{
                                    stackinEnd = true;
                                }
                            }

                            sb.Append(c);
                        }
                    }
                }
            }

            result.Add(PackToStyleComponent(typeCounter, sb.ToString()));
            return result;
        }

        public static List<MDC_MidComponent> ParseInlineCode(string raw){
            List<(int, int)> mid = new List<(int, int)>();
            foreach (Match match in Pattern.INLINE_CODE.Matches(raw)){
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

        public static MDC_StyleComponent PackToStyleComponent(int style, string content){
            MDC_StyleComponent result;
            if (style == 1){
                result = new MDC_StyleComponent(false, true);
            }
            else if (style == 2){
                result = new MDC_StyleComponent(true, false);
            }
            else if (style == 0){
                result = new MDC_StyleComponent(false, false);
            }
            else{
                result = new MDC_StyleComponent(true, true);
            }
            return result.AddContent(new MDC_TempSingleComponent(content));
        }

        public void ParseToSingleComponent(MDC_StyleComponent sc){
            string raw = ((MDC_TempSingleComponent) sc.contents.Last()).raw;
            sc.contents = this.SingleSplit(raw);
        }

        public List<MDC_SingleComponent> SingleSplit(string raw){
            //location-> (start_index:int, length:int, contents:List<string>, identifier:int)
            List<(int, int, List<string>, int)> location = new List<(int, int, List<string>, int)>();

            foreach (var singlePattern in this.SinglePatterns){
                foreach (Match match in singlePattern.MatchRegex.Matches(raw)){
                    List<string> mid = new List<string>();
                    foreach (var index in singlePattern.CaughtGroupIndex){
                        mid.Add(match.Groups[index].Value);
                    }
                    location.Add((match.Index, match.Length, mid, singlePattern.identifier));
                }
            }

            location.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            List<MDC_SingleComponent> result = new List<MDC_SingleComponent>();
            int pointer = 0;
            foreach (var loc in location){
                if (loc.Item1 > pointer){
                    result.Add(new MDC_PlainText(raw.Substring(pointer, loc.Item1 - pointer)));
                }
                pointer = loc.Item1;
                switch (loc.Item4){
                    case 0:{
                        result.Add(new MDC_Hyperlink(loc.Item3[0], loc.Item3[1]));
                        break;
                    }
                    case 1:{
                        result.Add(new MDC_Image(loc.Item3[0], loc.Item3[1]));
                        break;
                    }
                    default:{
                        result.Add(new MDC_CustomSingleComponent(loc.Item4, loc.Item3));
                        break;
                    }
                }
                pointer = loc.Item1 + loc.Item2;
            }

            if (pointer < raw.Length){
                result.Add(new MDC_PlainText(raw.Substring(pointer, raw.Length - pointer)));
            }
            return result;
        }
        
        
        
        
        
        //MainParse split
        public bool CodeBlockParse(string current_line){
            if (ComponentStack.Count > 0 && ComponentStack.Peek() is MDC_CodeBlock){//if stack is null?
                if (!Pattern.isEndOfCodeBlock(current_line)){
                    ((MDC_CodeBlock)ComponentStack.Peek()).AddLine(current_line);
                    return true;
                }else{
                    ComponentStack.Pop();
                    return true;
                }
            }
            else{
                Match CBMatchResult = Pattern.CODEBLOCK_START.Match(current_line);
                if (CBMatchResult.Success){
                    ComponentStack.Clear();
                    ComponentStack.Push((MDComponent) mddom.AddBaseComponent(new MDC_CodeBlock(CBMatchResult.Groups[0].Value)));
                    return true;
                }
            }
            return false;
        }

        public bool EmptyCheck(string current_line){
            if (current_line == String.Empty){
                ComponentStack.Clear();
                return true;
            }
            return false;
        }

        public bool UnorderedListParse(string current_line){
            Match ListMatchResult = Pattern.UNORDERED_LIST.Match(current_line);
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
                    .AddListEle(new MDC_ListEle(0, this.ParseLine(ListMatchResult.Groups[3].Value)));
                return true;
            }
            return false;
        }

        public bool OrderedListParse(string current_line){
            Match OListMatchResult = Pattern.ORDERED_LIST.Match(current_line);
            if (OListMatchResult.Success){
                int listLevel = OListMatchResult.Groups[1].Value.Length;
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
                            
                ((MDC_OrderedList) ComponentStack.Peek()).AddListEle(new MDC_ListEle(Convert.ToInt32(OListMatchResult.Groups[2].Value), this.ParseLine(OListMatchResult.Groups[4].Value)));
                return true;
            }

            return false;
        }

        public bool BlockParse(string current_line){
            Match BlockMatchResult = Pattern.BLOCK.Match(current_line);
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
                ((MDC_Block) ComponentStack.Peek()).AddBlockEle(new MDC_BlockEle(this.ParseLine(BlockMatchResult.Groups[2].Value)));
                return true;
            }
            return false;
        }

        public bool TitleParse(string current_line){
            Match TitleMatchResult = Pattern.TITLE.Match(current_line);
            if (TitleMatchResult.Success){
                ComponentStack.Clear();
                mddom.AddBaseComponent(new MDC_Title(TitleMatchResult.Groups[1].Value.Length, this.ParseLine(TitleMatchResult.Groups[2].Value)));
                return true;
            }
            return false;
        }

        public bool SplitterParse(string current_line){
            Match SpliterMatchResult = Pattern.SPLITTER.Match(current_line);
            if (SpliterMatchResult.Success){
                ComponentStack.Clear();
                mddom.AddBaseComponent(new MDC_Splitter());
                return true;
            }
            return false;
        }

        public bool TLL1Parse(string current_line, string next_line){
            Match TLL1MatchResult = Pattern.TITLE_LUS_LEVEL1.Match(next_line);
            if (TLL1MatchResult.Success){
                ComponentStack.Clear();
                mddom.AddBaseComponent(new MDC_Title(1, this.ParseLine(current_line)));
                return true;
            }
            return false;
        }

        public bool TLL2Parse(string current_line, string next_line){
            Match TLL2MatchResult = Pattern.TITLE_LUS_LEVEL2.Match(next_line);
            if (TLL2MatchResult.Success){
                ComponentStack.Clear();
                mddom.AddBaseComponent(new MDC_Title(2, this.ParseLine(current_line)));
                return true;
            }
            return false;
        }

        public bool ParagraphParse(string current_line){
            if (ComponentStack.Count > 0 && ComponentStack.Peek() is MDC_Paragraph){
                ((MDC_Paragraph) ComponentStack.Peek()).AddLine(this.ParseLine(current_line));
            }else{
                ComponentStack.Clear();
                MDC_Paragraph p = new MDC_Paragraph();
                mddom.AddBaseComponent(p);
                ComponentStack.Push(p);
                ((MDC_Paragraph) ComponentStack.Peek()).AddLine(this.ParseLine(current_line));
            }
            return false;
        }
    }
}