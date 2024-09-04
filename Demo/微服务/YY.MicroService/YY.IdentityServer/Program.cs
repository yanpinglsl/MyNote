using YY.IdentityServer;

var builder = WebApplication.CreateBuilder(args);


//ע��IdentityServer��������ע��һ�������ڴ�洢������ʱ״̬
builder.Services.AddIdentityServer(options =>
{
    //options.Events.RaiseErrorEvents = true;
    //options.Events.RaiseInformationEvents = true;
    //options.Events.RaiseFailureEvents = true;
    //options.Events.RaiseSuccessEvents = true;
})
    //����ģʽ�µ�ǩ��֤�飬�����������ü���
    .AddDeveloperSigningCredential()
    //OpenID Connect�����֤��Ϣ����
    //.AddInMemoryIdentityResources(Config.GetIdentityResources())
    //�����Դ����
    .AddInMemoryApiResources(Config.GetApis())//���ܱ�����Api��Դ��ӵ��ڴ���                                                         
    .AddInMemoryApiScopes(Config.GetApiScopes()) //���巶Χ
    .AddInMemoryClients(Config.GetClients())//�ͻ���������ӵ��ڴ�
    .AddTestUsers(Config.GetUsers()); 

var app = builder.Build();

app.UseIdentityServer();

app.Run();

