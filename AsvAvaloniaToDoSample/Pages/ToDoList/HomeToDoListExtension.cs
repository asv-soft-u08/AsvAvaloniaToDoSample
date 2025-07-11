using System.Composition;
using Asv.Avalonia;
using Asv.Common;
using AsvAvaloniaToDoSample.Commands;
using Microsoft.Extensions.Logging;
using R3;

namespace AsvAvaloniaToDoSample.Pages.ToDoList;

[ExportExtensionFor<IHomePage>]
[method: ImportingConstructor]
public class HomeToDoListExtension(ILoggerFactory loggerFactory) : IExtensionFor<IHomePage>
{
    public void Extend(IHomePage context, CompositeDisposable contextDispose)
    {
        context.Tools.Add(
            OpenToDoListCommand
                .StaticInfo.CreateAction(loggerFactory)
                .DisposeItWith(contextDispose)
        );
    }
}