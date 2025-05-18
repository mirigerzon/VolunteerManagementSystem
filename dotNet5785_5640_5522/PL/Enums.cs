using System;
using System.Collections;
using System.Collections.Generic;
using BO;

namespace PL;

internal class VolunteerSortFieldCollection : IEnumerable
{
    static readonly IEnumerable<VolunteerSortField> s_enums =
        (Enum.GetValues(typeof(VolunteerSortField)) as IEnumerable<VolunteerSortField>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
