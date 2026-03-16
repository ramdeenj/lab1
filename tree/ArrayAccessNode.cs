using System.Collections.Generic;

public class ArrayAccessNode : ExprNode
{
    public ExprNode array;
    public ExprNode index;

    public ArrayAccessNode(ExprNode array, ExprNode index)
        : base(new Token("ARRAY_ACCESS", 0, "array-access"))
    {
        this.array = array;
        this.index = index;
    }

    public override List<TreeNode> getChildNodes()
    {
        return new List<TreeNode> { array, index };
    }
}
