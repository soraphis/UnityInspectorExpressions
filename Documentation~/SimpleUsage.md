# Simple Usage

Use `NumericExpression<TCtx>` for numeric calculations. It evaluates as a float, so integer context values can be combined directly with fractional values such as `0.25`.

Use `IntExpression<TCtx>` when a consumer requires an integer. It wraps the same numeric expression and rounds the final result to the nearest integer.

```csharp
using UnityInspectorExpressions.Expressions;

[System.Serializable]
public class MyContext { }

public class TestComponent : MonoBehaviour
{
    [SerializeField] private NumericExpression<MyContext> m_Damage;
    [SerializeField] private IntExpression<MyContext> m_WholeDamage;
    
    void Apply(MyContext context)
    {
        Debug.Log(m_Damage.Evaluate(context));
        Debug.Log(m_WholeDamage.Evaluate(context));
    }
}
```

Both fields expose the same numeric expression choices in the Unity Inspector. For example, `m_WholeDamage` can evaluate an integer context value multiplied by `0.25`; rounding happens only when `IntExpression` returns the result, using `Mathf.RoundToInt` semantics.
