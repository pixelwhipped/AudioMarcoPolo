namespace AudioMarcoPolo
{
    public delegate void Setter<in TValue>(TValue value);

    public delegate TValue Getter<out TValue>();

    public delegate void Procedure<in TValue>(TValue value);

    public delegate void Operation<in TValue>(TValue a, TValue b);

    public delegate void Routine();


}
