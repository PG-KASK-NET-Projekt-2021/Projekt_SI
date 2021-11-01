namespace si.Services
{
    public interface IQueueService
    {
        public void Publish(object data, string topic);
    }
}