using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public enum ExceptionCodes
    {
        Default,
        Validation,
        Database,
        Routing,
        Storage,
        Restriction,
        ItemNotFound,
        Operation,
        UnAuthorized
    }
}
