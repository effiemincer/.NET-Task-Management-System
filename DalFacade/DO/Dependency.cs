using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DO;

/// <summary>
/// Dependency
/// </summary>
/*
ID - A unique identifier(automatic)
Dependent task ID
Requisite(DependsOn) ID
Customer’s email
Shipping address
Order creation date
Shipping date
Delivery date
*/
public record Dependency
(
    int Id,
    int? DependentTaskID = null,
    int? RequisiteID = null,
    string? CustomerEmail = null,
    string? ShippingAddress = null,
    DateTime OrderCreationDate = new DateTime(),
    DateTime? ShippingDate = null,
    DateTime? DeliveryDate = null,
    bool? inactive = false
    )
{
    //empty ctor
    public Dependency() : this(0) { }
}
