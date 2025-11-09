namespace Project.Expressions
{
    /// <summary>
    /// Interface that allows easy wrapping of expressions.
    /// E.g. "wrap 3+5 with an Unary Int Expression ->  -(3+5)"
    /// </summary>
    public interface IWrapable<T>
    {
        T Wrap();
    }
   
    /// <summary>
    /// Interface that allows easy unwrapping of expressions.
    /// E.g. "unwrap -(3+5) with an Unary Int Expression ->  3+5"
    /// E.g. "unwrap 3+5 with first child: -> 3 or with second child: -> 5"
    /// </summary>
    public interface IUnwrapable<T>
    {
        int ChildCount { get; }
        T Unwrap(int childIndex);
    }
    // TODO: Implement IUnwrapable where needed (e.g. Unary and Binary Expressions)
    
}
// TODO: class ScalarRelationalExpression : BoolExpression
