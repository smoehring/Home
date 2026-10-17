using Smoehring.Home.Domain.Common;

namespace Smoehring.Home.Domain.Assets;

public class Currency : Entity
{
    public string Code { get; set; } = "";
    public string Symbol { get; set; } = "";
    public string Name { get; set; } = "";
}

public class Brand : Entity
{
    public string Name { get; set; } = "";
}

public class AssetType : Entity
{
    public string Name { get; set; } = "";
    /// <summary>Items of this type should carry a serial number (insurance).</summary>
    public bool RequiresSerialNumber { get; set; }
}

public class AssetState : Entity
{
    public string Name { get; set; } = "";
    public bool Possession { get; set; }
    public bool Ownership { get; set; }
    public bool IsDefault { get; set; }

    public const int AvailableId = 1;
}
