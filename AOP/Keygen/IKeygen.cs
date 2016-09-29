using System;

namespace Keygen
{
    public interface IKeygen
    {
        string GenerateKey(DateTime date);
    }
}
