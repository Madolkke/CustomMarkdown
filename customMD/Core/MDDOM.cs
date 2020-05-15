using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace customMD{
    public class MDDOM{
        public List<IBaseComponent> BaseComponents;
        public static MDDOM parseMarkdown(){
            return null;
        }
        public MDDOM(){
            this.BaseComponents = new List<IBaseComponent>();
        }
        public IBaseComponent AddBaseComponent(IBaseComponent baseComponent){
            this.BaseComponents.Add(baseComponent);
            return this.BaseComponents.Last();
        }
    }
    
    

    public class MDComponent{
        public enum CType{
            MultiDiv, MultiSpan, Paragraph, Title, PlainText, List
        }
    }

    public interface IMultiContentD{
        //Div多元素组件允许使用的组件
    }

    public interface IBaseComponent{
        //可放置在MDDOM最底层的组件
    }

    public class MDC_MultiComponentDiv : MDComponent{
        //Only for List and Block component
        //div表明其可嵌套
        public List<IMultiContentD> Contents;
        public int depth;//description of nesting

        public MDC_MultiComponentDiv(int depth){
            this.depth = depth;
            this.Contents = new List<IMultiContentD>();
        }

        public static MDC_MultiComponentDiv initialRootComponent(){
            return new MDC_MultiComponentDiv(1);
        }
        
        public MDC_Block AddSubBlock(){
            var mid = new MDC_Block(this.depth + 1);
            this.Contents.Add(mid);
            return mid;
        }

        public MDC_OrderedList AddSubOrderedList(){
            var mid = new MDC_OrderedList(this.depth + 1);
            this.Contents.Add(mid);
            return mid;
        }

        public MDC_UnorderedList AddSubUnorderedList(){
            var mid = new MDC_UnorderedList(this.depth + 1);
            this.Contents.Add(mid);
            return mid;
        }
        
    }

    public class MDC_MultiComponentSpan : MDComponent{
        public List<MDC_MidComponent> Components;

        public MDC_MultiComponentSpan(List<MDC_MidComponent> parseResult){
            this.Components = parseResult;
        }
    }

    public class MDC_UnorderedList : MDC_MultiComponentDiv, IMultiContentD, IBaseComponent{

        public MDC_UnorderedList(int depth) : base(depth){
        }

        public static MDC_UnorderedList initialRootMultiComponent(){
            return new MDC_UnorderedList(1);
        }

        public MDC_UnorderedList AddListEle(MDC_ListEle ele){
            this.Contents.Add(ele);
            return this;
        }

    }

    public class MDC_OrderedList : MDC_MultiComponentDiv, IMultiContentD, IBaseComponent{
        public MDC_OrderedList(int depth) : base(depth){
        }

        public static MDC_OrderedList initialRootMultiComponent(){
            return new MDC_OrderedList(1);
        }

        public MDC_OrderedList AddListEle(MDC_ListEle ele){
            this.Contents.Add(ele);
            return this;
        }
    }

    public class MDC_Block : MDC_MultiComponentDiv, IMultiContentD, IBaseComponent{
        public MDC_Block(int depth) : base(depth){
        }

        public static MDC_Block initialRootMultiComponent(){
            return new MDC_Block(1);
        }

        public MDC_Block AddBlockEle(MDC_BlockEle ele){
            this.Contents.Add(ele);
            return this;
        }
    }

    public class MDC_Paragraph : MDComponent, IBaseComponent{
        public List<MDC_Line> Lines;
        public MDC_Paragraph(){
            this.Lines = new List<MDC_Line>();
        }

        public MDC_Line AddLine(List<MDC_MidComponent> parseResult){
            MDC_Line line = new MDC_Line(parseResult);
            this.Lines.Add(line);
            return line;
        }
    }

    public class MDC_Line : MDC_MultiComponentSpan{
        public MDC_Line(List<MDC_MidComponent> parseResult) : base(parseResult){
        }
    }

    public class MDC_Title : MDC_MultiComponentSpan, IBaseComponent{
        public enum LevelType{
            LEVEL1,LEVEL2,LEVEL3,LEVEL4,LEVEL5,LEVEL6
        }
        public LevelType level;

        public MDC_Title(int level, List<MDC_MidComponent> parseResult) : base(parseResult){
            if (level >= 1 && level <= 6){
                switch (level){
                    case 1: 
                        this.level = LevelType.LEVEL1;
                        break;
                    case 2:
                        this.level = LevelType.LEVEL2;
                        break;
                    case 3:
                        this.level = LevelType.LEVEL3;
                        break;
                    case 4:
                        this.level = LevelType.LEVEL4;
                        break;
                    case 5:
                        this.level = LevelType.LEVEL5;
                        break;
                    case 6:
                        this.level = LevelType.LEVEL6;
                        break;
                }
            }
            else{
                throw new Exception("Illegal Title Level");
            }
        }
    }

    public class MDC_PlainText : MDC_SingleComponent{
        public string content;

        public MDC_PlainText(string content){
            this.content = content;
        }
    }

    public class MDC_ListEle : MDC_MultiComponentSpan, IMultiContentD{
        public int order;
        
        public MDC_ListEle(int order, List<MDC_MidComponent> parseResult) : base(parseResult){
            this.order = order;
        }
    }

    public class MDC_Splitter : MDComponent, IBaseComponent{
        public int length;
    }

    public class MDC_BlockEle : MDC_MultiComponentSpan, IMultiContentD{
        public MDC_BlockEle(List<MDC_MidComponent> parseResult) : base(parseResult){
        }
    }

    public class MDC_FootNote : MDC_SingleComponent{
        public string notation;
        public string content;

        public MDC_FootNote(string content, string notation){
            this.content = content;
            this.notation = notation;
        }
    }

    public class MDC_CodeBlock : MDComponent, IBaseComponent{
        public List<string> line_contents;
        public string languageType;

        public string full_contents{
            get{
                string result = "";
                foreach (var content in line_contents){
                    result += $"{content}\n";
                }
                return result;
            }
        }
        
        public MDC_CodeBlock(){
            this.line_contents = new List<string>();
        }

        public MDC_CodeBlock(string languageType){
            this.languageType = languageType;
            this.line_contents = new List<string>();
        }

        public MDC_CodeBlock AddLine(string line_content){
            this.line_contents.Add(line_content);
            return this;
        }
    }

    public class MDC_Hyperlink : MDC_SingleComponent{
        //advanced hyperlink is not supported
        public string name;
        public string content;

        public MDC_Hyperlink(string name, string content){
            this.name = name;
            this.content = content;
        }
    }

    public class MDC_Image : MDC_SingleComponent{
        public string content;
        public string name;

        public MDC_Image(string name, string content){
            this.content = content;
            this.name = name;
        }
    }

    public class MDC_SingleComponent : MDComponent{
        
    }

    public class MDC_MidComponent : MDComponent{
    }

    public class MDC_InlineCode : MDC_MidComponent{
        public string code;
        public MDC_InlineCode(string code){
            this.code = code;
        }
    }

    public class MDC_StyleComponent : MDC_MidComponent{
        public bool bold;
        public bool italic;
        public List<MDC_SingleComponent> contents;

        public MDC_StyleComponent(bool bold, bool italic){
            this.bold = bold;
            this.italic = italic;
            this.contents = new List<MDC_SingleComponent>();
        }

        public MDC_StyleComponent AddContent(MDC_SingleComponent content){
            this.contents.Add(content);
            return this;
        }
    }

    public class MDC_TempMidComponent : MDC_MidComponent{
        public string raw;
        public MDC_TempMidComponent(string raw){
            this.raw = raw;
        }
    }

    public class MDC_TempSingleComponent : MDC_SingleComponent{
        public string raw;

        public MDC_TempSingleComponent(string raw){
            this.raw = raw;
        }
    }

    public class MDC_CustomSingleComponent : MDC_SingleComponent{
        public int identifier;
        public List<string> contents;

        public MDC_CustomSingleComponent(int identifier, List<string> contents){
            this.identifier = identifier;
            this.contents = contents;
        }
        public void AddContent(string content){
            this.contents.Add(content);
        }
    }
    
    //TODO table didn't set
    
}