using System;
namespace Core.Interfaces
{
	public interface ITokenHandler
	{
       string GenerateJwtToken(ITokenParameters pars);
    }
}

