using Lind.Example.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.Example.Data
{
    public class RepositoryResultSet<TEntity>
        where TEntity : ExampleEntity, new()
    {
        public IEnumerable<TEntity> Entities { get; set; } = null!;
        public int Count { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }


    }
    public struct Pager
    {
        public int Length { get; set; }
        public int Page { get; set; }
    }
}
