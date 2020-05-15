using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace customMD{
    public class Pattern{
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

        public static char[] ESCAPE_CHAR_COLLECTION = new[]{'_', '*'};

        public static readonly Regex LINK = new Regex(@"([^!]|^)\[(.*?)\]\((.*?)\)");

        public static readonly Regex IMAGE = new Regex(@"!\[(.*?)\]\((.*?)\)");
        
        public static bool isEndOfCodeBlock(string line){
            return Pattern.CODEBLOCK_END.Match(line).Success;
        }

        public static string isStartOfCodeBlock(string line){
            //检查语言类型合法性
            Match match = Pattern.CODEBLOCK_START.Match(line);
            //未紧跟在```后的语言类型或不合规定的语言类型会被视为空类型
            return match.Success ? match.Value : null;

        }
        
        public static bool isEscapeChar(char c){
            foreach (var ec in Pattern.ESCAPE_CHAR_COLLECTION){
                if (ec == c){
                    return true;
                }
            }
            return false;
        }
    }

    public class SinglePattern{
        public Regex MatchRegex;
        public int[] CaughtGroupIndex;
        public int identifier;
        public static int g_identifier;

        public SinglePattern(Regex matchRegex, int[] caughtGroupIndex){
            this.MatchRegex = matchRegex;
            this.CaughtGroupIndex = caughtGroupIndex;
            this.identifier = g_identifier;
            g_identifier++;
        }

        public SinglePattern(Regex matchRegex, List<int> caughtGroupIndex){
            this.MatchRegex = matchRegex;
            this.CaughtGroupIndex = caughtGroupIndex.ToArray();
            this.identifier = g_identifier;
            g_identifier++;
        }
    }
}