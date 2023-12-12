using Castle.DynamicProxy;
namespace NoteaAPI.Interceptor;

public class PasswordValidationInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        bool isPassword = invocation.Method.GetCustomAttributes(typeof(PasswordValidationAttribute), true).Length > 0;
        var value = invocation.Arguments[0];
        if (isPassword) 
        {
            string password = value.ToString();
            if (password.Length >= 8 && password.Any(char.IsDigit) && password.Any(char.IsUpper) && password.Any(char.IsLower))
            {
                invocation.Proceed();
            }
            else
            {
                throw new Exception("Invalid Password");
            }
        }
        invocation.Proceed();
    }
}
