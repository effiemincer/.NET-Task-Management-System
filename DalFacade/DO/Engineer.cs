using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO;


/*
Engineer:
ID - A unique identifier
Full name
Email address
Experience level
Cost per hour
 */
public record Engineer
(
    int ID,
    string FullName,
    string EmailAddress,
    string? ExperienceLevel = null,
    double? CostPerHour = null
)
{
    public Engineer() : this (0, "", ""){ } //empty ctor for stage 3


}

