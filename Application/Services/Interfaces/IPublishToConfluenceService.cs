public interface IPublishToConfluenceService
{
    Task<bool> PublishToConfluence(string documentation);

}