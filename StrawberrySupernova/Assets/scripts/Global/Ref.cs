using System;

public class Ref<T>
{
    private T store;

    public T Value{
        get { return store; }
        set { store = value; }
    }

    public Ref(T reference)
    {
        store = reference;
    }
}
