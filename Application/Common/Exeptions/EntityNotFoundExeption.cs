using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common.Exeptions
{
    public sealed class EntityNotFoundExeption(string message) : Exception(message)
    {
    }
}
