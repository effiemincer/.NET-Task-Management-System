using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO;


//<param name="Id">A unique identifier (created automatically)</param>
//<param name="Full name"></param>
//<param name="Email address"></param>
//<param name="Experience level"></param>
//<param name="Cost per hour"></param>
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

