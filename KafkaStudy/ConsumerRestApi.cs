using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaStudy
{
    static class ConsumerRestApi
    {
        public static void CreateConsumerInstance(string groupName, string consumerName)
        {
            var client = new RestClient("http://pkc-4nmjv.francecentral.azure.confluent.cloud:9092")
            {
                Authenticator = new HttpBasicAuthenticator("MWCBUUIKZ6LCF7QM", "7BQ6cOLuc7fT2iaUGJfEGkOP3sWzYPwZ8o2CaArIopW0XyqroHc5HlItWvPYxMmt")
            };

            var request = new RestRequest("consumers");
            request.AddParameter("group_name ", groupName, ParameterType.QueryString);
            request.AddJsonBody(new CreateConsumerRequest
            {
                Name = consumerName,
                Format = "json",
                AutoOffsetReset = "earliest",
                AutoCommitEnable = false
            });

            var response = client.Post(request);
        }
    }
}
