using System.Collections.Generic;

namespace Domain.Primitives
{
    public class JsonValidationResult
    {
        public bool IsValid { get; set; }
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }
}
