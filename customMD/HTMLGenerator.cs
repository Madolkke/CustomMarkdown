using System;
using System.Collections.Generic;
using System.Linq;

namespace customMD{
    public class Converter{
        public MDDOM Mddom;
        public HTML html;
        public Converter(MDDOM mddom){
            this.Mddom = mddom;
            this.html = new HTML();
        }

        public void InitialConvert(){
            this.html.initialize();
        }

        public void Convert(){
            Stack<Mark> ConvertStack = new Stack<Mark>();
            foreach (var baseComponent in Mddom.BaseComponents){
                if (baseComponent is MDC_Paragraph){
                    Mark p = this.html.addBody(null, null, MarkType.p);
                    foreach (var line in ((MDC_Paragraph)baseComponent).Lines){
                        Mark span = p.addChildMark(null, "<br>", MarkType.span);
                        foreach (var midComponent in line.Components){
                            span.addChildMark(MidConvert(midComponent));
                        }
                    }
                }else if (baseComponent is MDC_CodeBlock){
                    MDC_CodeBlock bc = (MDC_CodeBlock) baseComponent;
                    Mark code = this.html.addBody(null, null, MarkType.code);
                    foreach (var lineContent in bc.line_contents){
                        code.addChildMark(lineContent, "<br>", MarkType.span);
                    }
                }else if (baseComponent is MDC_Title){
                    MDC_Title t = (MDC_Title) baseComponent;
                    Mark h;
                    switch (t.level){
                        case MDC_Title.LevelType.LEVEL1:
                            h = this.html.addBody(null, null, MarkType.h1);
                            break;
                        case MDC_Title.LevelType.LEVEL2: 
                            h = this.html.addBody(null, null, MarkType.h2);
                            break;
                        case MDC_Title.LevelType.LEVEL3: 
                            h = this.html.addBody(null, null, MarkType.h3);
                            break;
                        case MDC_Title.LevelType.LEVEL4: 
                            h = this.html.addBody(null, null, MarkType.h4);
                            break;
                        case MDC_Title.LevelType.LEVEL5: 
                            h = this.html.addBody(null, null, MarkType.h5);
                            break;
                        case MDC_Title.LevelType.LEVEL6: 
                            h = this.html.addBody(null, null, MarkType.h6);
                            break;
                        default: 
                            h = this.html.addBody(null, null, MarkType.h6);
                            break;
                    }

                    foreach (var midComponent in t.Components){
                        h.addChildMark(MidConvert(midComponent));
                    }
                }else if (baseComponent is MDC_Splitter){
                    this.html.addBody("<hr>", null, MarkType.span);
                }else if (baseComponent is MDC_MultiComponentDiv){
                    MDC_MultiComponentDiv multiComponentDiv = (MDC_MultiComponentDiv) baseComponent;
                    this.html.addBody(MultiConvert(multiComponentDiv));
                }
                
            }
        }

        public static Mark MidConvert(MDC_MidComponent midComponent){
            if (midComponent is MDC_InlineCode){
                MDC_InlineCode mid = (MDC_InlineCode)midComponent;
                Mark mark = new Mark(mid.code, null, MarkType.code);
                return mark;
            }else if (midComponent is MDC_StyleComponent){
                MDC_StyleComponent mid = (MDC_StyleComponent) midComponent;
                Mark mark = new Mark(null, null, MarkType.span);
                Mark boldMark = mid.bold ? mark.addChildMark(null, null, MarkType.b) : mark;
                Mark italicMark = mid.italic ? boldMark.addChildMark(null, null, MarkType.i) : boldMark;
                foreach (MDC_SingleComponent singleComponent in mid.contents){
                    italicMark.addChildMark(SingleConvert(singleComponent));
                }
                return mark;
            }
            return null;
        }

        public static Mark SingleConvert(MDC_SingleComponent singleComponent){
            if (singleComponent is MDC_PlainText){
                MDC_PlainText mid = (MDC_PlainText) singleComponent;
                return new Mark(mid.content, null, MarkType.span);
            }else if (singleComponent is MDC_Hyperlink){
                MDC_Hyperlink mid = (MDC_Hyperlink) singleComponent;
                return new Mark(mid.name, null, MarkType.a).addProperty(PropertyType.href, mid.content);
            }else if (singleComponent is MDC_Image){
                MDC_Image mid = (MDC_Image) singleComponent;
                return new Mark(mid.name, null, MarkType.img).addProperty(PropertyType.src, mid.content);
            }
            return null;
        }

        public static Mark MultiConvert(MDC_MultiComponentDiv multiComponentDiv){
            if (multiComponentDiv is MDC_Block){
                MDC_Block block = (MDC_Block) multiComponentDiv;
                Mark blockMark = new Mark(null, null, MarkType.blockquote);
                foreach (var content in block.Contents){
                    if (content is MDC_BlockEle){
                        MDC_BlockEle ele = (MDC_BlockEle) content;
                        foreach (var eleComponent in ele.Components){
                            blockMark.addChildMark(MidConvert(eleComponent));
                        }
                        blockMark.addChildMark(null, "<br>", MarkType.span);
                    }else if (content is MDC_MultiComponentDiv){
                        blockMark.addChildMark(MultiConvert((MDC_MultiComponentDiv)content));
                    }
                }
                return blockMark;
            }else if (multiComponentDiv is MDC_OrderedList){
                MDC_OrderedList ol = (MDC_OrderedList) multiComponentDiv;
                Mark OLMark = new Mark(null, null, MarkType.ol);
                foreach (var content in ol.Contents){
                    if (content is MDC_ListEle){
                        var li = OLMark.addChildMark(null, null, MarkType.li);
                        foreach (var component in ((MDC_ListEle)content).Components){
                            li.addChildMark(MidConvert(component));
                        }
                    }else if (content is MDC_MultiComponentDiv){
                        OLMark.addChildMark(MultiConvert((MDC_MultiComponentDiv) content));
                    }
                }
                return OLMark;
            }else if (multiComponentDiv is MDC_UnorderedList){
                MDC_UnorderedList ul = (MDC_UnorderedList) multiComponentDiv;
                Mark ULMark = new Mark(null, null, MarkType.ul);
                foreach (var content in ul.Contents){
                    if (content is MDC_ListEle){
                        var li = ULMark.addChildMark(null, null, MarkType.li);
                        foreach (var component in ((MDC_ListEle)content).Components){
                            li.addChildMark(MidConvert(component));
                        }
                    }else if (content is MDC_MultiComponentDiv){
                        ULMark.addChildMark(MultiConvert((MDC_MultiComponentDiv) content));
                    }
                }
                return ULMark;
            }

            return null;
        }

        public static void Main(string[] args){
            //                          ↓此处填入md文件路径
            MDDOM mddom = new MDReader("").Read();
            Converter converter = new Converter(mddom);
            converter.InitialConvert();
            converter.Convert();
            Console.WriteLine(converter.html.ToString());
        }
    }
}