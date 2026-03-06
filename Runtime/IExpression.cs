public interface IExpression<out TResult, in TCtx>
{
    TResult Evaluate(TCtx ctx);
}
