using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommProject.DataAccess.Repository.IRepository
{
    public interface ISP_CALL:IDisposable
    {
        void Execute(string procedureName, DynamicParameters param=null); //it works for Save,Update and Delete. Execute is name here we can name anything
        T Single<T>(string procedureName, DynamicParameters param=null); //Single is name here we can write anything here
        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null);
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null);// Used to return result of more than 1 query
        T OneRecord<T>(string procedureName, DynamicParameters param = null);
    }
}
