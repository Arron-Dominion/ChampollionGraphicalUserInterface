using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ChampollionGraphicalUserInterface.ViewModels;

namespace ChampollionGraphicalUserInterface;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    #region Methods

    /// <summary>
    /// Creates the view associated with the supplied view model.
    /// </summary>
    /// <param name="param">The view model for which to create a view.</param>
    /// <returns>The matching view, a not-found control, or <see langword="null"/> when no view model is supplied.</returns>
    public Control? Build(object? param)
    {
        if (param is null)
            return null;
        
        var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }
        
        return new TextBlock { Text = "Not Found: " + name };
    }

    /// <summary>
    /// Determines whether this template supports the supplied data object.
    /// </summary>
    /// <param name="data">The data object to evaluate.</param>
    /// <returns><see langword="true"/> when the data is a view model; otherwise, <see langword="false"/>.</returns>
    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }

    #endregion
}
