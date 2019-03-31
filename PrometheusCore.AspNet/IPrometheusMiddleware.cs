using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PrometheusCore.AspNet
{
    internal interface IPrometheusMiddleware
    {
        Task Handle(HttpContext context, Func<Task> next);
    }
}