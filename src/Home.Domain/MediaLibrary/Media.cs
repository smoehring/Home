using Smoehring.Home.Domain.Common;

namespace Smoehring.Home.Domain.MediaLibrary;

public enum MediaKind { Other = 0, Game = 1, Film = 2, Book = 3, Music = 4, Software = 5 }

public class Language : Entity
{
    public string CultureName { get; set; } = "";
    public string DisplayName { get; set; } = "";
}

public class MediaGroup : Entity
{
    public string Name { get; set; } = "";
}

public class Media : Entity
{
    public int AssetId { get; set; }
    public MediaKind Kind { get; set; }
    public int? GroupId { get; set; }
    public MediaGroup? Group { get; set; }
    public int? GroupOrder { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public List<MediaName> Names { get; set; } = [];
    public List<MediaExternalId> ExternalIds { get; set; } = [];
}

public class MediaName : Entity
{
    public int MediaId { get; set; }
    public string Name { get; set; } = "";
    public int LanguageId { get; set; }
    public Language? Language { get; set; }
}

public class MediaExternalId : Entity
{
    public int MediaId { get; set; }
    public string System { get; set; } = "";
    public string Value { get; set; } = "";
}
