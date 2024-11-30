using AutoMapper;
using HotelService.DTOs;
using Newtonsoft.Json;
using RabbitMQ.Client;
using shared.Messaging;
using System.Text;

public class ReportManagementService
{
    private readonly IMessageQueue _messageQueue;

    public ReportManagementService(IMessageQueue messageQueue)
    {
        _messageQueue = messageQueue;
    }

    // 1. Rapor Talebi Gönderme
    public void RequestReport(CreateReportRequestDTO reportRequestDTO)
    {
        var message = JsonConvert.SerializeObject(reportRequestDTO);
        _messageQueue.Publish("report-queue", message);
    }
}
