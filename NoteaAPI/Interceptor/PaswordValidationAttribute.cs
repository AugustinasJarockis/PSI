namespace NoteaAPI.Interceptor;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class PasswordValidationAttribute : Attribute { }
