namespace YY.Zhihu.Application
{
    public record UserRegisterRequest(string Username, string Password);

    public record UserLoginRequest(string Username, string Password);
}
