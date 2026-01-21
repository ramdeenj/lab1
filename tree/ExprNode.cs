//ExprNode.cs
public class ExprNode : TreeNode {
    public Token NUM;
    public static ExprNode parse(Tokenizer T){
        var e = new ExprNode();
        e.NUM = T.expect("NUM");
        return e;
    }
}