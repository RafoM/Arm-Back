using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbito.Shared.Contracts.Transaction
{
    public interface IUpdateUserRoleRequest
    {
        Guid UserId { get; }
        int NewRoleId { get; }
    }
}
