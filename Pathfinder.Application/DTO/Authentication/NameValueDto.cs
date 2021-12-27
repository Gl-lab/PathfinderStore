using Newtonsoft.Json;

namespace Pathfinder.Application.DTO.Authentication
{
    public class NameValueDto
    {
        public NameValueDto(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}