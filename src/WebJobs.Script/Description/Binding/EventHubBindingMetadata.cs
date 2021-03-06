﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    public class EventHubBindingMetadata : BindingMetadata
    {
        [AutoResolve]
        public string Path { get; set; }

        // Optional Consumer group
        public string ConsumerGroup { get; set; }

        public override void ApplyToConfig(JobHostConfigurationBuilder configBuilder)
        {
            if (configBuilder == null)
            {
                throw new ArgumentNullException("configBuilder");
            }
            EventHubConfiguration eventHubConfig = configBuilder.EventHubConfiguration;

            string connectionString = null;
            if (!string.IsNullOrEmpty(Connection))
            {
                connectionString = Utility.GetAppSettingOrEnvironmentValue(Connection);
            }

            if (this.IsTrigger)
            {
                string eventProcessorHostName = Guid.NewGuid().ToString();

                string storageConnectionString = configBuilder.Config.StorageConnectionString;

                string consumerGroup = this.ConsumerGroup;
                if (consumerGroup == null)
                {
                    consumerGroup = Microsoft.ServiceBus.Messaging.EventHubConsumerGroup.DefaultGroupName;
                }

                var eventProcessorHost = new Microsoft.ServiceBus.Messaging.EventProcessorHost(
                     eventProcessorHostName,
                     this.Path,
                     consumerGroup,
                     connectionString,
                     storageConnectionString);

                eventHubConfig.AddEventProcessorHost(this.Path, eventProcessorHost);
            }
            else
            {                
                var client = Microsoft.ServiceBus.Messaging.EventHubClient.CreateFromConnectionString(
                    connectionString, this.Path);

                eventHubConfig.AddEventHubClient(this.Path, client);
            }
        }        
    }
}
