using Castle.DynamicProxy;
namespace NoteaAPI.Interceptor;

public class PasswordValidationInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        Console.WriteLine("interceptor on fireeeee");
        bool isPassword = invocation.Method.GetCustomAttributes(typeof(PasswordValidationAttribute), true).Length > 0;
        Console.WriteLine(isPassword.ToString());
        var value = invocation.Arguments[0];
        Console.WriteLine(value);
        if (isPassword) 
        {
            string password = value.ToString();
            if (password.Length >= 8 && password.Any(char.IsDigit) && password.Any(char.IsUpper) && password.Any(char.IsLower))
            {
                Console.WriteLine("i do slayyy");
                invocation.Proceed();
            }
            else
            {
                Console.WriteLine("no slay");
                throw new Exception("Invalid Password");
            }
        }
        invocation.Proceed();
    }
}
