using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;


namespace AWS.DynamoDBAttributes
{
    [DynamoDBTable("CubeWalkC1")]
    public class Rooms
    {
        [DynamoDBHashKey] public string PartitionKey { get; set; } = "Rooms";
        [DynamoDBRangeKey] public string SortKey { get; set; } = "XXX#XXX#XXXXXXX";
        [DynamoDBProperty("Players")] public List<string> DatePlayerIDs { get; set; } = new List<string>();
    }

    [DynamoDBTable("CubeWalkC1")]
    public class Players
    {
        [DynamoDBHashKey] public string PartitionKey { get; set; } = "Players";
        [DynamoDBRangeKey] public string SortKey { get; set; } = "20XX-XX-XXTXX:XX:XX#xxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        [DynamoDBProperty] public string PlayerName { get; set; }
    }
}

