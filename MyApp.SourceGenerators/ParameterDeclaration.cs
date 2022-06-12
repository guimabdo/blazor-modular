using MyApp.SourceGenerators.Enums;

namespace MyApp.SourceGenerators
{
    public class ParameterDeclaration
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string MethodDeclaration { get; set; }
        public ParameterFromType From { get; set; }
    }
}