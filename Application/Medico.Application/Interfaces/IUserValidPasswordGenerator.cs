namespace Medico.Application.Interfaces
{
    public interface IUserValidPasswordGenerator
    {
        //generates password that will be valid during app user creation (ASP.NET Identity)
        string Generate();
    }
}