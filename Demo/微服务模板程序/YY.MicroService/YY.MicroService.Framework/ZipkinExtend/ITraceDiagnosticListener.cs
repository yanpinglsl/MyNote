using System;

namespace YY.MicroService.Framework.ZipkinExtend
{
    public interface ITraceDiagnosticListener
    {
        string DiagnosticName { get; }
    }
}
