using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSPA.Shared.Models
{
    public class GenericGetResult<TModel> : GenericResult
    {
        public TModel Value { get; set; }
    }
}
