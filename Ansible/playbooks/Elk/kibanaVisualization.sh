# Creating index pattern to display logs on Kibana page.
curl -XPOST "http://localhost:9200/.kibana/doc/index-pattern:application" -H 'Content-Type: application/json' -d'
{
  "type" : "index-pattern",
  "index-pattern" : {
    "title": "application*",
    "timeFieldName": "Timestamp"
  }
}'

# Creating histogram visualization for logs.
curl -XPOST 'http://localhost:5601/api/saved_objects/visualization/Top10SlowestProducts' -H 'Content-Type: application/json' -H "kbn-xsrf: true" -d '
{
	"attributes": {
      "title": "Top10SlowestProducts",
      "visState": "{\"title\":\"Top 10 Products With Slowest Average Response Time\",\"type\":\"histogram\",\"params\":{\"type\":\"histogram\",\"grid\":{\"categoryLines\":false,\"style\":{\"color\":\"#eee\"},\"valueAxis\":\"ValueAxis-1\"},\"categoryAxes\":[{\"id\":\"CategoryAxis-1\",\"type\":\"category\",\"position\":\"bottom\",\"show\":true,\"style\":{},\"scale\":{\"type\":\"linear\"},\"labels\":{\"show\":true,\"truncate\":100},\"title\":{}}],\"valueAxes\":[{\"id\":\"ValueAxis-1\",\"name\":\"LeftAxis-1\",\"type\":\"value\",\"position\":\"left\",\"show\":true,\"style\":{},\"scale\":{\"type\":\"linear\",\"mode\":\"normal\"},\"labels\":{\"show\":true,\"rotate\":0,\"filter\":false,\"truncate\":100},\"title\":{\"text\":\"Product Avg Response Time (ms)\"}}],\"seriesParams\":[{\"show\":\"true\",\"type\":\"histogram\",\"mode\":\"stacked\",\"data\":{\"label\":\"Product Avg Response Time (ms)\",\"id\":\"1\"},\"valueAxis\":\"ValueAxis-1\",\"drawLinesBetweenPoints\":true,\"showCircles\":true}],\"addTooltip\":true,\"addLegend\":true,\"legendPosition\":\"right\",\"times\":[],\"addTimeMarker\":false},\"aggs\":[{\"id\":\"1\",\"enabled\":true,\"type\":\"avg\",\"schema\":\"metric\",\"params\":{\"field\":\"AdditionalData.ResponseTime\",\"customLabel\":\"Product Avg Response Time (ms)\"}},{\"id\":\"2\",\"enabled\":true,\"type\":\"terms\",\"schema\":\"segment\",\"params\":{\"field\":\"ProductName.keyword\",\"size\":10,\"order\":\"desc\",\"orderBy\":\"1\",\"otherBucket\":false,\"otherBucketLabel\":\"Other\",\"missingBucket\":false,\"missingBucketLabel\":\"Missing\",\"customLabel\":\"Product Names\"}}]}",
      "uiStateJSON": "{}",
      "description": "",
      "version": 1,
      "kibanaSavedObjectMeta": {
        "searchSourceJSON": "{\"index\":\"application\",\"query\":{\"query\":\"\",\"language\":\"lucene\"},\"filter\":[]}"
      }
    }
}
'

echo ""
echo "Note that after creating data, you need to Refresh Kibana index fields on Management->Index Patterns before using the visualization."