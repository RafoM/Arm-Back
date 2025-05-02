using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbito.Shared.Contracts.Identity
{
    public interface IGetUserRoleRequest
    {
        Guid UserId { get; }
    }
}
