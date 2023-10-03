using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NOTEA.Models
{
    public class ConspectString// : string, IDisplayable<string>
    {
        private string data = "";
        public string Display()
        {
            return data;
        }
    }
}
