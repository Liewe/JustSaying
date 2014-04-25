using System.Linq;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using JustSaying.Messaging;
using JustSaying.Messaging.MessageSerialisation;
using NLog;

namespace JustSaying.AwsTools
{
    public class SnsTopicByName : SnsTopicBase, IMessagePublisher
    {
        public string TopicName { get; private set; }
        private static readonly Logger Log = LogManager.GetLogger("JustSaying");

        public SnsTopicByName(string topicName, IAmazonSimpleNotificationService client, IMessageSerialisationRegister serialisationRegister)
            : base(serialisationRegister)
        {
            TopicName = topicName;
            Client = client;
            Exists();
        }

        public override bool Exists()
        {
            var topicCheck = Client.ListTopics(new ListTopicsRequest());
            if (topicCheck.Topics.Any())
            {
                var topic = topicCheck.Topics.FirstOrDefault(x => x.TopicArn.Contains(TopicName));

                while (topic == null && !string.IsNullOrEmpty(topicCheck.NextToken))
                {
                    topicCheck = Client.ListTopics(new ListTopicsRequest(topicCheck.NextToken));
                    topic = topicCheck.Topics.FirstOrDefault(x => x.TopicArn.Contains(TopicName));
                    
                    if (topic != null)
                    {
                        Arn = topic.TopicArn;
                        return true;
                    }
                }

                if (topic != null)
                {
                    Arn = topic.TopicArn;
                    return true;
                }
            }

            return false;
        }

        public bool Create()
        {
            var response = Client.CreateTopic(new CreateTopicRequest(TopicName));
            if (!string.IsNullOrEmpty(response.TopicArn))
            {    
                Arn = response.TopicArn;
                Log.Info(string.Format("Created Topic: {0} on Arn: {1}", TopicName, Arn));
                return true;
            }
            Log.Info(string.Format("Failed to create Topic: {0}", TopicName));
            return false;
        }

        public void Delete()
        {
            //No need to delete a topic ever.
        }
    }
}