using EchoServer;

var mainServer = new MainServer();
mainServer.InitConfig(3000, "127.0.0.1", 10, "EchoServer");
mainServer.CreateServer();

var startRes = mainServer.Start();

if (startRes)
{
    mainServer.Logger.Info("서버 네트워크 시작");
}
else
{
    Console.WriteLine("서버 네트워크 시작 실패");
    return;
}

Console.WriteLine("키 입력시 종료");
Console.ReadKey();

mainServer.CloseServer();