using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using ReportService.Data;
using System.Text;
using Newtonsoft.Json;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConnection _connection;

    public Worker(IServiceScopeFactory scopeFactory, IConnection connection)
    {
        _scopeFactory = scopeFactory;
        _connection = connection;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: "report-queue", durable: true, exclusive: false, autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (model, eventArgs) =>
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var reportRequest = JsonConvert.DeserializeObject<dynamic>(message);

            Guid reportId = Guid.Parse((string)reportRequest.reportId);
            string location = reportRequest.location;

            var report = await dbContext.Reports.FindAsync(reportId);
            if (report != null)
            {
                // Raporu işleme
                report.HotelCount = dbContext.Hotels.Count(h => h.Location == location);
                //report.ContactCount = dbContext.Hotels
                //    .Where(h => h.Location == location)
                //    .SelectMany(h => h.Contacts)
                //    .Count();
                report.Status = "Completed";

                dbContext.Reports.Update(report);
                await dbContext.SaveChangesAsync();
            }

            channel.BasicAck(eventArgs.DeliveryTag, false);
        };

        channel.BasicConsume(queue: "report-queue", autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }
}
