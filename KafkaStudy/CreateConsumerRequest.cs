using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaStudy
{
    class CreateConsumerRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("auto.offset.reset")]
        public string AutoOffsetReset { get; set; }

        [JsonProperty("auto.commit.enable")]
        public bool AutoCommitEnable { get; set; }
    }
}
