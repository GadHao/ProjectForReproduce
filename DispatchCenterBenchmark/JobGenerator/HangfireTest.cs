using Hangfire.HttpJob.Client;
using System.Diagnostics;
using Xunit.Abstractions;

namespace JobGenerator;

public class HangfireTest
{
    private readonly ITestOutputHelper _output;
    private string _serverUrl = "http://localhost:5000/job";

    public HangfireTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact(DisplayName = "����1000��1��ִ��һ�ε�����������")]
    public void AddRecurringJob()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        Stopwatch createJobStopwatch = Stopwatch.StartNew();
        _output.WriteLine($"��ʼ��������:{DateTimeOffset.Now}");
        try
        {
            for (int i = 974; i <= 1000; i++)
            {
                createJobStopwatch.Restart();
                // �����õ���ͬ���ķ�ʽ��Ҳ����ʹ���첽�� await HangfireJobClient.AddRecurringJobAsync
                var result = HangfireJobClient.AddRecurringJob(_serverUrl, new RecurringJob()
                {
                    JobName = $"ÿ��ִ��һ��{i}",
                    Method = "Get",
                    Data = new { name = "aaa", age = 10 },
                    Url = "http://framework-dispatchcenterbenchmark/Job",
                    Cron = "* * * * * *"
                }, new HangfireServerPostOption
                {
                    BasicUserName = "admin",//������hangfire���õ�basicauth
                    BasicPassword = "test"//������hangfire���õ�basicauth
                });

                if (!result.IsSuccess)
                {
                    _output.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                }
                Assert.True(result.IsSuccess);

                _output.WriteLine($"������{i}���������񻨷�ʱ�䣺{createJobStopwatch.ElapsedMilliseconds}ms");
            }
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            _output.WriteLine($"������������:{DateTimeOffset.Now}������ʱ��{stopwatch.ElapsedMilliseconds}ms");
        }
    }

    [Fact(DisplayName = "ɾ��1000��1��ִ��һ�ε�����������")]
    public void RemoveRecurringJob()
    {
        for (int i = 1; i <= 1000; i++)
        {
            var result = HangfireJobClient.RemoveRecurringJob(_serverUrl, $"ÿ��ִ��һ��{i}", new HangfireServerPostOption
            {
                BasicUserName = "admin",//������hangfire���õ�basicauth
                BasicPassword = "test"//������hangfire���õ�basicauth
            });

            Assert.True(result.IsSuccess);
        }
    }
}