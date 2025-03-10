namespace TabbyCat.Models.RunningHubs;

public class NodeInfoListItem
{

    [JsonProperty("nodeId")]
    public string NodeId { get; set; }

    [JsonProperty("fieldName")]
    public string FieldName { get; set; }

    [JsonProperty("fieldValue")]
    public string FieldValue { get; set; }
}

public class RunningHubTaskPublishResponseModel
{

    [JsonProperty("workflowId")]
    public long WorkflowId { get; set; }

    [JsonProperty("apiKey")]
    public string ApiKey { get; set; }

    [JsonProperty("nodeInfoList")] public IEnumerable<NodeInfoListItem> NodeInfoList { get; set; } 
}
