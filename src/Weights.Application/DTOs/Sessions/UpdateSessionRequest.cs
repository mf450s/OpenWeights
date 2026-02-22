using System.ComponentModel.DataAnnotations;

namespace Weights.Application.DTOs.Sessions;

public class UpdateSessionRequest
{
    [StringLength(100)]
    public string? Name { get; set; }

    public DateTime? EndTime { get; set; }

    public string? Note { get; set; }
}
