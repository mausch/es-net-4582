using System;
using System.IO;
using System.Linq;
using System.Text;
using Nest;

namespace es_bug_repro
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = @"
{
  ""hits"": {
  },
  ""aggregations"":
  {
    ""some_agg"" : {
      ""buckets"" : {
        ""value1"" : {
          ""doc_count"" : 0
        },
        ""value2"" : {
          ""doc_count"" : 0
        },
        ""value3#something else"" : {
          ""doc_count"" : 0
        }
      }
    }
  }
}
";
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var response = new ElasticClient().RequestResponseSerializer
              .Deserialize<SearchResponse<object>>(ms);
            var filters = response.Aggregations
              .Filters("some_agg")
              .Select(x => x.Key)
              .ToList();

            const string expected = "value3#something else";
            var actual = filters[2];
            if (actual != expected)
            { 
                throw new Exception($"Expected '{expected}', actual '{actual}'");
            }
        }            
    }
}
