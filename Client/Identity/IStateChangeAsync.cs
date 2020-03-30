using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSPA.Client.Identity
{
    public interface IStateChangeAsync
    {
        event Func<Task> OnStateChange;
        Task NotifyStateChange();
    }
}
