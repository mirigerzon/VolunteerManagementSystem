using System.Data;

namespace DalApi;
public interface IConfig
{
    DateTime Clock { get; set; }
    int GetNextCallId();
    int GetNextAssignmentId();
    void reset();
}