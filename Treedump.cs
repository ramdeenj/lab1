//Treedump.cs
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

public class Treedump {

    private class Node{
        public string label;
        public string unique;
        public bool isNull;
        public List<Node> children = new();
        private static int counter=0;
        public Node(string lbl, bool isNull){
            this.label=lbl;
            this.unique = $"n{counter++}";
            this.isNull=isNull;
        }
    }

    public static void textTree(Object obj, TextWriter outs){
        reflect(outs,obj, null, new List<bool>());
    }

    public static void jsonTree(Object obj, TextWriter outs){
        var sw = new StringWriter();
        var root = reflect(sw,obj, null, new List<bool>());
        var opts = new System.Text.Json.JsonSerializerOptions();
        opts.IncludeFields=true;
        opts.WriteIndented=true;
        opts.MaxDepth=1000000;
        string J = System.Text.Json.JsonSerializer.Serialize(root,opts);
        outs.WriteLine(J);
    }

    public static void dotTree(Object obj, TextWriter outs){
        var sw = new StringWriter();
        var root = reflect(sw,obj, null, new List<bool>());
        outs.WriteLine("graph d {");
        outs.WriteLine("node [ shape=rectangle, fontfamily=helvetica]");
        walk(root, (Node n) => {
            outs.Write($"{n.unique} [label=\"{escape(n.label)}\"");
            if( n.isNull )
                outs.Write(" style=filled fillcolor=grey");
            outs.WriteLine("];");
        });
        walk(root, (Node n) => {
            foreach(var c in n.children){
                outs.WriteLine($"{n.unique} -- {c.unique};");
            }
        });
        outs.WriteLine("}");
    }
    
    private static string escape(string s){
        string o="";
        foreach(var c in s){
            switch(c){
                case '\n':
                    o+="\\n";
                    break;
                case '\"':
                    o += "\\\"";
                    break;
                default:
                    o += c;
                    break;
            }
        }
        return o;
    }
    private static void walk(Node n, Action<Node> f){
        f(n);
        foreach(var c in n.children){
            walk(c,f);
        }
    }

    private static void printWithIndentation(TextWriter outs, List<bool> hasNextSibling, string label){

        for(int i=0;i<hasNextSibling.Count-1;++i){
            if( hasNextSibling[i]){
                outs.Write("│ ");
            } else {
                outs.Write("  ");
            }
        }
        if( hasNextSibling.Count > 0 ){
            if( hasNextSibling[^1] == true )
                outs.Write("├─");
            else
                outs.Write("└─");
        }
        outs.Write(label);
        outs.WriteLine();
    }

    private static bool isAggregate(Object? obj){
        return isList(obj) || isArray(obj) || isEnumerable(obj);
    }

    private static bool isList(Object? obj){
        if( obj == null )
            return false;
        var f = obj.GetType();
        if( f.IsGenericType && f.GetGenericTypeDefinition() == typeof(List<>) ){
            return true;
        } else {
            return false;
        }
    }

    private static bool isEnumerable(Object? obj){
        if( obj == null )
            return false;
        if( obj as String != null )
            return false;       //don't count strings as enumerable
        return null != obj as IEnumerable;
    } 

    private static bool isTuple(Object? obj){
        if( obj == null )
            return false;
        var tmp = obj as ITuple;
        return tmp != null;
    }
    private static bool isArray(Object? obj){ 
        if( obj == null )
            return false;
        var f = obj.GetType();
        return f.IsArray;
    }

    private static bool isUserDefinedType(Object? obj) { //}, FieldInfo f){
        if( obj == null )
            return false;
        string? ns = obj.GetType().Namespace;
        if( ns == null )
            return true;
        if( ns == "System")
            return false;
        if( ns.StartsWith("System."))
            return false;
        if( obj.GetType().IsArray )
            return false;
        return true;
    }
 

    private static bool allFieldsArePrimitive(Object? obj){
        if( obj == null )
            return true;
        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance|BindingFlags.Public);
        foreach(var f in fields){
            Object? fdata = f.GetValue(obj);
            if( fdata != null ){
                if( isAggregate(fdata) || isUserDefinedType(fdata) )
                    return false;
            }
        }
        return true;
    }

    private static string getValues(Object? obj){
        if( obj == null )
            return "null";
        List<string> tmp = new();
        foreach( var finfo in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public) ){
            string v = $"{finfo.GetValue(obj)}";
            v = v.Replace("\n","↵").Replace("\r","␍").Replace("\t","↦");
            tmp.Add(finfo.Name+"="+v);
        }
        return String.Join(" ",tmp);
    }

    private static Node reflect(TextWriter outs,
            Object? obj, 
            string? name,        //may be empty string 
            List<bool> hasNextSibling
    ){

        string longName = "";
        string shortName = "";
        if( name != null && name.Length != 0 ){
            longName += $"{name} : ";
            shortName = $"{name}";
        }

        bool recurse;
        bool isNull=false;
        if( Object.ReferenceEquals(obj,null)){
            longName += "null";
            shortName += " (null)";
            recurse=false;
            isNull=true;
        } else if( isList(obj) ){
            IList L = (IList)obj;
            string plural = ( (L.Count == 1) ? "":"s");
            longName += $"List<{obj.GetType().GenericTypeArguments[0].Name}> with {L.Count} element{plural}";
            recurse=true;
            shortName += $" (List<{obj.GetType().GenericTypeArguments[0].Name}>)";
       } else if( isArray(obj) ){
            Array A = (Array)obj;
            string plural = ( (A.Length == 1) ? "":"s");
            longName += $"{obj.GetType().Name} with {A.Length} element{plural}";  //includes []'s
            shortName += $" ({obj.GetType().Name}[{A.Length}])";
            recurse=true;
        } else if (isEnumerable(obj) ){
            recurse = true;
        } else if( isTuple(obj)){
            recurse=true;
            ITuple? tmp = (ITuple?)obj;
            if( tmp == null )
                throw new Exception();
            List<string> L = new();
            for(int i=0;i<tmp.Length;++i){
                var item = tmp[i];
                if( item == null )
                    L.Add("null");
                else
                    L.Add(item.GetType().Name);
            }
            longName += "Tuple<"+String.Join(",",L)+">";
            shortName += " (Tuple<"+String.Join(",",L)+">)";

        } else if( !isUserDefinedType(obj)  ){
            longName += $"{obj}";
            shortName += $" ({obj})";
            recurse=false;
        } else {
            longName += $"{obj.GetType().Name}";
            shortName += $" ({obj.GetType().Name})";
            recurse=true;
        }


        if( obj != null && isUserDefinedType(obj) && allFieldsArePrimitive(obj)){
            var fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            if( fields.Length > 0 ){
                longName += "{";
                foreach(var f in fields){
                    string v = $"{f.GetValue(obj)}";
                    longName += $" {f.Name}={v}";
                    shortName += $"\\n{f.Name}={escape(v)}";
                }
                longName += " }";
            }
            recurse=false;
        }


        Node n = new Node(shortName,isNull);

        printWithIndentation(outs,hasNextSibling,longName);

        if(!recurse){
            return n;
        }

        if( obj == null ){
            return n;
        } else if( isList(obj) || isArray(obj) || isEnumerable(obj) || isTuple(obj) ){
            List<Object?> items = new();

            if( isList(obj) ){
                IList? L = (IList?)obj;
                if( L == null )
                    throw new Exception();      //checked for null previously
                foreach(var ob in L){
                    items.Add(ob);
                }
            } else if( isArray(obj) ){
                Array? A = (Array?)obj;
                if(A == null )
                    throw new Exception();
                foreach(var ob in A){
                    items.Add(ob);
                }
            } else if( isEnumerable(obj) ){
                IEnumerable? E = (IEnumerable?)obj;
                if( E == null )
                    throw new Exception();
                foreach(var ob in E){
                    items.Add(ob);
                }
            } else if( isTuple(obj) ){
                ITuple? T = (ITuple?)obj;
                if(T == null)
                    throw new Exception();
                for(int i=0;i<T.Length;++i){
                    items.Add(T[i]);
                }
            } else {
                throw new Exception();
            }
            hasNextSibling.Add(true);
            for(int i=0;i<items.Count;++i){
                if( i == items.Count-1)
                    hasNextSibling[^1] = false;
                Node c = reflect(outs,items[i],$"[{i}]",hasNextSibling);
                n.children.Add(c);
            }
            hasNextSibling.RemoveAt(hasNextSibling.Count-1);
        } else if( !isUserDefinedType(obj) ){
            return n;
        } else {
            if( obj == null )
                throw new Exception();
            var ty = obj.GetType();
            if( ty == null )
                throw new Exception();
            var fields = ty.GetFields(BindingFlags.Instance | BindingFlags.Public);
            hasNextSibling.Add(true);
            for(int j=0;j<fields.Length;++j){
                if( j == fields.Length-1 )
                    hasNextSibling[^1]=false;
                Node c = reflect(outs,fields[j].GetValue(obj),fields[j].Name,hasNextSibling);
                n.children.Add(c);
            }
            hasNextSibling.RemoveAt(hasNextSibling.Count-1);
        }
        return n;
    } //end reflect()
} //end Treedump