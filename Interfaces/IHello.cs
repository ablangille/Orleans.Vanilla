namespace Interfaces;

public interface IHello : IGrainWithGuidKey
{
    ValueTask<string> SayHello(string greeting);
    ValueTask<IList<string>> GetGreetings();
}