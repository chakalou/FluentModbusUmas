using System.ComponentModel;

// workaround class for records and "init" keyword
namespace FluentModbusUmas.Assets
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}