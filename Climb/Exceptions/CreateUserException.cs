using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace Climb
{
    [Serializable]
    public class CreateUserException : Exception
    {
        public override string Message { get; }

        public CreateUserException(IdentityResult result)
        {
            Message = $"Couldn't create user. Errors\n\t{string.Join("\n\t", result.Errors.Select(e => $"{e.Code}: {e.Description}"))}";
        }
    }
}