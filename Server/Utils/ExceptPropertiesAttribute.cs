using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSPA.Server.Utils
{
    public class ExceptPropertiesAttribute : ActionFilterAttribute
    {
        private string[] _properties;

        public ExceptPropertiesAttribute(params string[] properties)
        {
            _properties = properties;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_properties != null)
            {
                foreach (var property in _properties)
                {
                    if (context.ModelState.ContainsKey(property))
                        context.ModelState.Remove(property);
                }
            }
        }
    }
}
