# Simple Usage

The simplest way to use the expressions in your own code and components is to add a field of the Expression type you need.

For example, the following code snippet will use an IntExpression.

```csharp
using Project.Expressions;

public class TestComponent : MonoBehaviour
{
    [SerializeField] private IntExpression myIntExpression;
    
    void Start()
    {
        Debug.Log(myIntExpression.Evaluate());
    }
}
```

This allows configuration in the Unity Inspector.
Game Designers can set it to any expression that returns an int. For example 2 + 3, or Random.Range(0, 10) + 3.
