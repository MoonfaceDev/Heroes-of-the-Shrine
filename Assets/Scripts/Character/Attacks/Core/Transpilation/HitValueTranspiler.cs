using System.Collections.Generic;
using System.Linq;

public class HitValueTranspiler<T>
{
    public delegate T TranspileCallable(BaseAttack attack, IHittable hittable, T value);

    private readonly List<TranspileCallable> transpilers;

    public HitValueTranspiler()
    {
        transpilers = new List<TranspileCallable>();
    }

    public T Transpile(BaseAttack attack, IHittable hittable, T value)
    {
        return transpilers.Aggregate(value, (current, callable) => callable(attack, hittable, current));
    }

    public void Add(TranspileCallable callable)
    {
        transpilers.Add(callable);
    }
    
    
    public void Remove(TranspileCallable callable)
    {
        transpilers.Remove(callable);
    }
}