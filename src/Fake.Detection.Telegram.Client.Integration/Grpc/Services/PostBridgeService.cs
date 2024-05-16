using Fake.Detection.Post.Bridge.Api;
using Fake.Detection.Post.Bridge.Contracts;
using Fake.Detection.Telegram.Client.Bll.Services.interfaces;
using Grpc.Core;

namespace Fake.Detection.Telegram.Client.Integration.Grpc.Services;

public class PostBridgeService : IBridgeService
{
    private readonly Post.Bridge.Api.PostBridgeService.PostBridgeServiceClient _client;

    public PostBridgeService(Post.Bridge.Api.PostBridgeService.PostBridgeServiceClient client) => _client = client;

    public async Task<long> CreatePost(
        string authorId,
        string externalId,
        string token,
        CancellationToken cancellationToken)
    {
        var request = new CreatePostRequest
        {
            AuthorId = authorId,
            Source = DataSource.Author,
            ExternalId = externalId
        };

        var metadata = new Metadata { { "authorization", $"Bearer {token}" } };
        var response = await _client.CreatePostAsync(request, metadata, cancellationToken: cancellationToken);

        return response.PostId;
    }

    public async Task<long?> GetPost(
        string externalId,
        string token,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetPostRequest
            {
                ExternalId = externalId,
                UseExternalId = true
            };

            var metadata = new Metadata { { "authorization", $"Bearer {token}" } };
            var response = await _client.GetPostAsync(request, metadata, cancellationToken: cancellationToken);

            return response.Post.Id;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<string?> SendPostItem(
        long postId,
        string itemType,
        byte[] bytes,
        string token,
        CancellationToken cancellationToken)
    {
        try
        {
            var metadata = new Metadata { { "authorization", $"Bearer {token}" } };
            var call = _client.SendPostItem(metadata, cancellationToken: cancellationToken);

            const int bufferSize = 4096;

            for (var offset = 0; offset < bytes.Length; offset += bufferSize)
            {
                var size = Math.Min(bufferSize, bytes.Length - offset);

                var buffer = new byte[size];

                Array.Copy(bytes, offset, buffer, 0, size);

                await call.RequestStream.WriteAsync(new SendPostItemRequest
                {
                    Item = new ItemChunk
                    {
                        PostId = postId,
                        MetaData = new MetaData
                        {
                            Type = Enum.Parse<ItemType>(itemType),
                        },
                        Chunk = Google.Protobuf.ByteString.CopyFrom(buffer, 0, size)
                    }
                }, cancellationToken);
            }

            await call.RequestStream.CompleteAsync();

            var response = await call.ResponseAsync;
            return response.Result == RequestResult.Ok ? response.ItemId : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task ProcessItem(
        long postId,
        string itemId,
        string token,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new ProcessItemRequest
            {
                PostId = postId,
                ItemId = itemId
            };

            var metadata = new Metadata { { "authorization", $"Bearer {token}" } };
            await _client.ProcessItemAsync(request, metadata, cancellationToken: cancellationToken);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}