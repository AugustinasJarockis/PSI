using Microsoft.AspNetCore.Mvc;

namespace NoteaAPI.Interceptor
{
    public class Validator
    {
        [PasswordValidation]
        public virtual void ValidatePassword (string password) { }
    }
}
