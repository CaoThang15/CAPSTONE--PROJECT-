using System;

namespace SMarket.Business.DTOs.Common
{
    public class UnauthorizedException(string message) : Exception(message)
    {
    }
}