namespace Znode.Engine.ERPConnector.Model
{
    public class ERPParameterModel
    {
        public string ERPName { get; set; }
        public string ControllerName { get; set; }
        public string MethodName { get; set; }
        public string AccessType { get; set; }
        public string FilePath { get; set; }
        public string Data { get; set; }
        public bool IsHeadersAvailable { get; set; }
        public string Headers { get; set; }
    }
}


