using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DO;

/// <summary>
/// Dependency: limits the completion of tasks to certain dependiencies
/// </summary>

/// <param name = "Id">A unique identifier(automatic)</param>
/// <param name = "DependentTaskId"></param>
/// <param name = "RequisiteID">(DependsOn) </param>
/// <param name = "CustomerEmail"></param>
/// <param name = "ShippingAddress"></param>
/// <param name = "OrderCreationDate"></param>
/// <param name = "ShippingDate"></param>
/// <param name = "DeliveryDate"></param>
/// <param name="Inactive"> (T/F) </param>
public record Dependency
(
    int Id,
    int? DependentTaskId = null,
    int? RequisiteID = null,
    string? CustomerEmail = null,
    string? ShippingAddress = null,
    DateTime OrderCreationDate = new DateTime(),
    DateTime? ShippingDate = null,
    DateTime? DeliveryDate = null,
    bool Inactive = false
    )
{
    //empty ctor
    //public Dependency() : this(0) { }     //for stage 3
}
