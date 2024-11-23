using System.Data;

namespace DalApi;
public interface IConfig
{
    DateTime Clock { get; set; }
    void reset();

}
