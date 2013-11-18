namespace Medseek.Util.Ioc
{
    using System;

    /// <summary>
    /// Used to identify a method that should be invoked by the injection 
    /// container when the component is first created.
    /// </summary>
    public class OnCreateAttribute : Attribute
    {
    }
}