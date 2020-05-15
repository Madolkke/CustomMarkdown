using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace customMD{
    public class ConfigConverter{
        public static void Main(string[] args){
            var mddom = new MDReader("C:\\Users\\madol\\Desktop\\test.md").Read();
            var converter = new ConfigConverter(mddom);
            converter.Convert();
            
        }
        
        
        private MDDOM mddom;
        public List<Mark> head_marks;
        public List<Mark> header_marks;
        public List<Mark> footer_marks;
        private bool header_flag = false;
        private bool head_flag = false;
        private bool footer_flag = false;
        
        
        public ConfigConverter(MDDOM mddom){
            this.mddom = mddom;
            this.head_marks = new List<Mark>();
            this.header_marks = new List<Mark>();
            this.footer_marks = new List<Mark>();
        }

        public void Convert(){
            foreach (var baseComponent in mddom.BaseComponents){
                switch (baseComponent){
                    case MDC_Title title:
                        switch (title.level){
                            case MDC_Title.LevelType.LEVEL1:{
                                var t = this.MidConvert(title.Components.First());
                                this.head_marks.Add(t);
                                break;   
                            }
                            case MDC_Title.LevelType.LEVEL2:{
                                this.head_flag = true;
                                break;
                            }
                            case MDC_Title.LevelType.LEVEL3:{
                                this.header_flag = true;
                                break;
                            }
                            case MDC_Title.LevelType.LEVEL4:{
                                this.footer_flag = true;
                                break;
                            }
                        }
                        break;


                    case MDC_CodeBlock codeBlock:{
                        if (head_flag){
                            switch (codeBlock.languageType){
                                case "javascript":{
                                    this.head_marks.Add(new Mark(codeBlock.full_contents, null, MarkType.script));
                                    break;   
                                }
                                case "css":{
                                    this.head_marks.Add(new Mark(codeBlock.full_contents, null,MarkType.style));
                                    break;
                                }
                            }
                        }

                        if (header_flag){
                            switch (codeBlock.languageType){
                                case "html":{
                                    this.header_marks.Add(new Mark(codeBlock.full_contents, null, MarkType.div));
                                    break;
                                }
                                case "css":{
                                    this.header_marks.Add(new Mark(codeBlock.full_contents, null, MarkType.style));
                                    break;
                                }
                                case "javascript":{
                                    this.header_marks.Add(new Mark(codeBlock.full_contents, null, MarkType.script));
                                    break;
                                }
                            }
                        }

                        if (footer_flag){
                            switch (codeBlock.languageType){
                                case "html":{
                                    this.footer_marks.Add(new Mark(codeBlock.full_contents, null, MarkType.div));
                                    break;
                                }
                                case "css":{
                                    this.footer_marks.Add(new Mark(codeBlock.full_contents, null, MarkType.style));
                                    break;
                                }
                                case "javascript":{
                                    this.footer_marks.Add(new Mark(codeBlock.full_contents, null, MarkType.script));
                                    break;
                                }
                            }
                        }
                        this.ClearFlags();
                        break;
                    }
                }
            }

        }

        public Mark MidConvert(MDC_MidComponent midComponent){
            switch (midComponent){
                case MDC_StyleComponent styleComponent:{
                    return this.SingleConvert(styleComponent.contents.First());
                }
            }

            return new Mark("", null, MarkType.div);
        }

        public Mark SingleConvert(MDC_SingleComponent singleComponent){
            
            switch (singleComponent){
                case MDC_Hyperlink hyperlink:{
                    Mark mark = new Mark("", null, MarkType.script);
                    mark.addProperty(PropertyType.src, hyperlink.content);
                    return mark;
                    break;
                }
                case MDC_Image image:{
                    Mark mark = new Mark("", null, MarkType.link);
                    mark.addProperty(PropertyType.rel, image.name);
                    mark.addProperty(PropertyType.href, image.content);
                    return mark;
                    break;
                }
                case MDC_CustomSingleComponent csc:{
                    Console.WriteLine(csc.contents);
                    break;
                }

            }

            return new Mark("", null, MarkType.script);
        }

        public void ClearFlags(){
            this.head_flag = false;
            this.header_flag = false;
            this.footer_flag = false;
        }
    }
}