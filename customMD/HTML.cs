using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;


namespace customMD{
    public class HTML{

        private Mark html_root_mark;
        private Mark head_mark;
        private Mark body_mark;

        public Mark initialize(){
            this.html_root_mark = Mark.createHtmlRootMark();
            this.head_mark = this.html_root_mark.addSChildMark(null, MarkType.head);
            this.body_mark = this.html_root_mark.addSChildMark(null, MarkType.body);
            return this.html_root_mark;
        }
        
        public override string ToString(){
            return "<!doctype html>\n" + this.html_root_mark.ToString();
        }

        
        public Mark addBody(string pre_content, string post_content, MarkType mt) =>
            this.body_mark.addChildMark(pre_content, post_content, mt);
        public Mark addHead(string pre_content, string post_content, MarkType mt) =>
            this.head_mark.addChildMark(pre_content, post_content, mt);

        public Mark addBody(Mark mark) => this.body_mark.addChildMark(mark);

        public Mark addHead(Mark mark) => this.head_mark.addChildMark(mark);

        public Mark addSBody(string content, MarkType mt) =>
            this.body_mark.addSChildMark(content, mt);
        public Mark addSHead(string content, MarkType mt) =>
            this.head_mark.addSChildMark(content, mt);
        

        /*public static void Main(string[] args){
            HTML html = new HTML();
            html.initialize();

            Mark parentMark = new Mark(0,"parentContent is here.", null, MarkType.h1);
            parentMark.addSChildMark("childContent is here.", MarkType.a);
            parentMark.addProperty(PropertyType.href, "https://www.baidu.com/");
            html.addBody(parentMark);
            Console.WriteLine(html.ToString());
            
            Console.WriteLine(parentMark.ToString());
        }*/
        
        
    }
    public enum MarkType{
        html,head,body,
        a, p, img, b, i,
        h1,h2,h3,h4,h5,h6,
        div, span, code, blockquote,
        ul, ol, li,
            
    }
    public enum PropertyType{
        href, src, _class,
    }

    class Property{
        private PropertyType pt;
        private string value;

        public Property(PropertyType pt, string value){
            this.pt = pt;
            this.value = value;
        }

        public override string ToString(){
            return $"{pt}=\"{value}\"";
        }

        public static string PropertyTypeConvert(PropertyType propertyType){
            switch (propertyType){
                case PropertyType._class: return "class";
                default: return propertyType.ToString();
            }
        }
    }

    public class Mark{
        
        
        internal int generation;
        internal List<Mark> childMarks;
        private List<Property> Properties;
        private string pre_content;
        private string post_content;
        private MarkType mt;

        public Mark(int generation, string pre_content, string post_content, MarkType mt){
            this.generation = generation;
            this.pre_content = pre_content;
            this.post_content = post_content;
            this.mt = mt;
            this.childMarks = new List<Mark>();
            this.Properties = new List<Property>();
        }
        public Mark(string pre_content, string post_content, MarkType mt){
            this.generation = 0;
            this.pre_content = pre_content;
            this.post_content = post_content;
            this.mt = mt;
            this.childMarks = new List<Mark>();
            this.Properties = new List<Property>();
        }

        public Mark AddTo(Mark mark){
            mark.addChildMark(this);
            return this;
        }

        public Mark addChildMark(string pre_content, string post_content, MarkType mt){
            childMarks.Add(new Mark(this.generation + 1, pre_content, post_content, mt));
            return childMarks.Last();
        }

        public Mark addChildMark(Mark mark){
            childMarks.Add(mark.generationInc(this.generation + 1));
            return this.childMarks.Last();
        }

        public Mark addSChildMark(string content, MarkType mt){
            childMarks.Add((new Mark(this.generation + 1, content, null, mt)));
            return childMarks.Last();
        }

        public static Mark createHtmlRootMark(){
            return new Mark(0, null, null, MarkType.html);
        }

        public static Mark createRootMark(MarkType mt){
            return new Mark(0, null, null, mt);
        }

        public string getIndentation(){
            return new string(' ', this.generation * 4);
        }

        public string getCIndentation(){
            return new string(' ', (this.generation + 1) * 4);
        }

        public Mark addProperty(PropertyType pt, string value){
            this.Properties.Add(new Property(pt, value));
            return this;
        }

        public Mark generationInc(){
            this.generation += 1;
            if (this.childMarks.Count <= 0) return this;
            foreach (var childMark in this.childMarks){
                childMark.generationInc();
            }

            return this;
        }
        
        public Mark generationInc(int gener_num){
            this.generation += gener_num;
            if (this.childMarks.Count <= 0) return this;
            foreach (var childMark in this.childMarks){
                childMark.generationInc(gener_num);
            }

            return this;
        }

        public override string ToString(){
            /*
             *<parentMark>
             *     preContent
             *     <childMark>
             *         childContent
             *     <childMark/>
             *     postContent
             *<parentMark/>
             *
             * 
             */
            
            string properties = "";
            foreach (var prop in Properties){
                properties += $" {prop}";
            }
            String mark_string = this.getIndentation() + $"<{this.mt}{properties}>\n";
            

            if (this.pre_content != null){
                mark_string += this.getCIndentation() + this.pre_content + "\n";
            }
            if (childMarks.Count > 0){
                
                foreach (var childMark in childMarks){
                    mark_string += childMark.ToString();
                }
                
            }

            if (this.post_content != null){
                mark_string += this.getCIndentation() + this.post_content + "\n";
            }
            mark_string += this.getIndentation() + $"</{this.mt}>\n";
            return mark_string;
        }

        
    }
}