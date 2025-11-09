using System.Collections.Generic;
using UnityEngine.Pool;

public interface IExpression<out T>
{
    public T Evaluate(Dictionary<int, object> ctx);
    public T Evaluate();

    public T DefaultEvaluate()
    {
        var ctx = DictionaryPool<int, object>.Get();
        T result = Evaluate(ctx);
        DictionaryPool<int, object>.Release(ctx);
        return result;
    }
}
