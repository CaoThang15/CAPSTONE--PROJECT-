using System;

namespace SMarket.Business.DTOs.Common
{
    public class NotFoundException(string message) : Exception(message)
    {
    }
}