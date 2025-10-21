using System;

namespace SMarket.Business.DTOs.Common
{
    public class BadRequestException(string message) : Exception(message)
    {
    }
}