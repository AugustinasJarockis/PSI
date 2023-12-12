using Castle.DynamicProxy;
using System.Reflection;

public class PasswordValidationInterceptor : IInterceptor
{
    //public void Intercept(IInvocation invocation)
    //{
    //    // Check if the method is marked with the PasswordValidationAttribute
    //    if (invocation.Method.GetCustomAttributes(typeof(PasswordValidationAttribute), true).Length > 0)
    //    {
    //        // Validate password based on the specified criteria
    //        object[] arguments = invocation.Arguments;
    //        int passwordIndex = Array.FindIndex(invocation.Method.GetParameters(), p => p.Name == "password");
    //        string password = arguments[passwordIndex] as string;

    //        if (!IsValidPassword(password))
    //        {
    //            Console.WriteLine("Password validation failed");
    //            // You can throw an exception or handle the validation failure in another way
    //            return;
    //        }
    //    }

    //    // Proceed with the original method execution
    //    invocation.Proceed();
    //}

    //private bool IsValidPassword(string password)
    //{
    //    // Example password validation criteria: at least one number, one capital letter, and minimum length of 8
    //    return !string.IsNullOrEmpty(password) && password.Any(char.IsDigit) && password.Any(char.IsUpper) && password.Length >= 8;
    //}
    public void Intercept(IInvocation invocation)
    {
        // Validate user.Password directly
        int userIndex = Array.FindIndex(invocation.Method.GetParameters(), p => p.Name == "user");
        if (userIndex >= 0)
        {
            object[] arguments = invocation.Arguments;
            object userObject = arguments[userIndex];

            // Get the value of the "Password" property from the "user" object
            string password = userObject.GetType().GetProperty("Password")?.GetValue(userObject) as string;

            if (!string.IsNullOrEmpty(password) && !IsValidPassword(password))
            {
                Console.WriteLine("Password validation failed");
                // You can throw an exception or handle the validation failure in another way
                return ;
            }
        }

        // Proceed with the original method execution
        invocation.Proceed();
    }

    private bool IsValidPassword(string password)
    {
        // Example password validation criteria: at least one number, one capital letter, and minimum length of 8
        return !string.IsNullOrEmpty(password) && password.Any(char.IsDigit) && password.Any(char.IsUpper) && password.Length >= 8;
    }
}