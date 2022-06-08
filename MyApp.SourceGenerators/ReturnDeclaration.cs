namespace MyApp.SourceGenerators
{
    public class ReturnDeclaration
    {
        public string TypeName { get; set; } = string.Empty;
        public string MethodDeclaration { get; set; } = string.Empty;
        public string ResponseDeclaration { get; set; } = string.Empty;
        public bool HasVoid { get; set; } = false;
        public bool HasAsync { get; set; } = false;
    }
}
