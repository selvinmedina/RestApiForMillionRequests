using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Movies.Contracts.Responses
{
    public abstract class HalResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public required List<Link> Links { get; set; }
    }

    public class Link
    {
        public required string Href { get; init; }
        public required string Rel { get; init; }
        public required string Type { get; init; }
    }
}
