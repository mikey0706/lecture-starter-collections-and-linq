using CollectionsAndLinq.BL.Models.Users;

namespace CollectionsAndLinq.BL.Models.Tasks;

public record TaskWithPerformerDto(
    int Id,
    string Name,
    string Description,
    string State,
    DateTime CreatedAt,
    DateTime? FinishedAt,
    UserDto Performer)
{

}
