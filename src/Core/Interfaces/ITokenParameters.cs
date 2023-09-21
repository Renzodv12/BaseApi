using System;
namespace Core.Interfaces
{
	public interface ITokenParameters
    {
        string UserName { get; set; }
        string PasswordHash { get; set; }
        string Id { get; set; }

    }
}

