using System.ComponentModel.DataAnnotations;

namespace Documents.Infrastructure.Model;

internal class RabbitMqConfigurationOption
{
    [Required(ErrorMessage = $"{nameof(HostName)} обязателен для заполнения.")]
    public required string HostName { get; set; }

    [Required(ErrorMessage = $"{nameof(Port)} обязателен для заполнения.")]
    public required int Port {get;set; }
}

