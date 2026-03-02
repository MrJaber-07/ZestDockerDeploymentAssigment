using Microsoft.AspNetCore.Mvc.Filters;
using System;

public class LogActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        Console.WriteLine($"[Filter] Starting {context.ActionDescriptor.DisplayName}");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        Console.WriteLine($"[Filter] Finished {context.ActionDescriptor.DisplayName}");
    }
}