﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbito.Shared.Contracts.Transaction
{
    public interface ICreateUserInfoRequest
    {
        Guid UserId { get; }
        string Email { get; }
        string? PromoCode { get; }
        Guid? ReferrerId { get; }
    }
}
