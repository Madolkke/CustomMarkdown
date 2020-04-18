using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace customMD{
    public class CSS{
        
        private List<Selector> Selectors;
        private int extra_indentation;
        CSS(){
            Selectors = new List<Selector>();
            extra_indentation = 0;
        }

        public CSS addSelector(Selector selector){
            Selectors.Add(selector);
            return this;
        }

        public CSS updateIndentation(int new_value){
            this.extra_indentation = new_value;
            return this;
        }

        public CSS syncIndentation(){
            foreach (var selector in this.Selectors){
                selector.updateIndentation(this.extra_indentation);
            }
            return this;
        }

        public static string parse(string style){
            return style.ToString().Replace('_', '-');
        }

        public override string ToString(){
            this.syncIndentation();
            string result = "";
            foreach (var selector in Selectors){
                result += selector;
            }

            return result;
        }

        /*public static void Main(string[] args){
            CSS css = new CSS();
            Selector selector = new IDSelector("hello");
            selector.addStyle(Style.text_align, "center");
            css.addSelector(selector);
            var selector1 = new ElementSelector(MarkType.a);
            selector1.addTarget(MarkType.body).addTarget(MarkType.head);
            css.addSelector(selector1);
            Console.WriteLine(css.ToString());
        }*/
    }

    

    public class Selector{
        protected Dictionary<string, string> Styles;
        protected int extra_indentation;
        
        protected Selector(){
            Styles = new Dictionary<string, string>();
            extra_indentation = 0;
        }

        protected internal Selector updateIndentation(int new_value){
            this.extra_indentation = new_value;
            return this;
        }

        public Selector addStyle(string style, string style_value){
            Styles.Add(style, style_value);
            return this;
        }

        public static string getIndentation(int gener_num){
            return new string(' ', gener_num * 4);
        }

        
    }

    class IDSelector: Selector{
        private string ID;

        public IDSelector(string id){
            ID = id;
        }

        
        public override string ToString(){
            string result = getIndentation(extra_indentation) + $"#{ID}" + " {\n";
            foreach (var style in Styles){
                result += getIndentation(extra_indentation + 1) + $"{CSS.parse(style.Key)}: {style.Value};\n";
            }

            result += getIndentation(extra_indentation) + "}\n";
            return result;
        }
    }

    class ClassSelector: Selector{
        private string class_name;
        public ClassSelector(string class_name){
            this.class_name = class_name;
        }

        public override string ToString(){
            string result = getIndentation(extra_indentation) + $".{class_name}" + " {\n";
            foreach (var style in Styles){
                result += getIndentation(extra_indentation + 1) + $"{CSS.parse(style.Key)}: {style.Value};\n";
            }

            result += getIndentation(extra_indentation) + "}\n";
            return result;
        }
    }

    class ElementSelector: Selector{
        private List<MarkType> select_targets;

        public ElementSelector(MarkType selector_target){
            select_targets = new List<MarkType>();
            select_targets.Add(selector_target);
        }

        public ElementSelector addTarget(MarkType mt){
            this.select_targets.Add(mt);
            return this;
        }

        public override string ToString(){
            string targets_string = "";
            foreach (var target in select_targets){
                if (target != select_targets[^1]){
                    targets_string += $"{target}, ";
                }
                else{
                    targets_string += target;
                }
                
            }
            
            string result = getIndentation(extra_indentation) + targets_string + " {\n";
            foreach (var style in Styles){
                result += getIndentation(extra_indentation + 1) + $"{CSS.parse(style.Key)}: {style.Value};\n";
            }

            result += getIndentation(extra_indentation) + "}\n";
            return result;
        }
    }
}