using System;

namespace Delight.Component.Controls
{
    public interface ISetter : IDisposable
    {
        object Value { get; set; }

        bool IsStable { get; }
    }
}
