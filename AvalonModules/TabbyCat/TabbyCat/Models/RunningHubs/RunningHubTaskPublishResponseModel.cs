namespace TabbyCat.Models.RunningHubs;

public class NodeInfoListItem
{

    [JsonProperty("nodeId")]
    public string NodeId { get; set; }=string.Empty;

    [JsonProperty("fieldName")]
    public string FieldName { get; set; }=string.Empty;

    [JsonProperty("fieldValue")]
    public string FieldValue { get; set; }=string.Empty;
}

public class RunningHubTaskPublishResponseModel
{

    [JsonProperty("workflowId")]
    public long WorkflowId { get; set; }

    [JsonProperty("apiKey")]
    public string ApiKey { get; set; }=string.Empty;

    [JsonProperty("nodeInfoList")] public IEnumerable<NodeInfoListItem> NodeInfoList { get; set; } = [];
}
