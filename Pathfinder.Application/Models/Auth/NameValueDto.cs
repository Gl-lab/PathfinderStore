using Newtonsoft.Json;

namespace Pathfinder.Application.Models.Auth
{
    public class NameValueDto
    {
        public NameValueDto(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
