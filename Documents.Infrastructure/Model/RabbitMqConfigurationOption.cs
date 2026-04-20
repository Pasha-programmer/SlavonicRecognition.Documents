using System.ComponentModel.DataAnnotations;

namespace Documents.Infrastructure.Model;

internal class RabbitMqConfigurationOption
{
    [Required(ErrorMessage = $"{nameof(HostName)} обязателен для заполнения.")]
    public required string HostName { get; set; }

    [Required(ErrorMessage = $"{nameof(Port)} обязателен для заполнения.")]
    public required int Port {get;set; }

    [Required(ErrorMessage = $"{nameof(UserName)} обязателен для заполнения.")]
    public required string UserName { get; set; }

    [Required(ErrorMessage = $"{nameof(Password)} обязателен для заполнения.")]
    public required string Password { get; set; }

    [Required(ErrorMessage = $"{nameof(VirtualHost)} обязателен для заполнения.")]
    public required string VirtualHost { get; set; }
}

