#if IsNetStandard
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Not null atribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class NotNullAttribute : Attribute
    {
    }
}
#endif
