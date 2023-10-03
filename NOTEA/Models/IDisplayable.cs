using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NOTEA.Models
{
    public interface IDisplayable <DataType>
    {
        public DataType Display();
    }
}
