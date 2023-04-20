using System.Collections.Generic;

namespace Gopas.Entities;

public class TestPropertyBag
{
    private Dictionary<string, object> _properties;

    public int Id { get; set; }

    public object this[string propertyName]
    {
        get { return _properties[propertyName]; }
        set { _properties[propertyName] = value; }
    }
}
